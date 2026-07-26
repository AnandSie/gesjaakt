using Domain.Entities.Events;
using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxGameDealerTests.
public class QwixxGameDealer : IQwixxGameDealer
{
    private readonly IQwixxGameState _gameState;
    private readonly IQwixxDiceRoller _diceRoller;

    public event EventHandler<NotableEvent>? ColorLocked;
    public event EventHandler<NotableEvent>? PenaltyTaken;
    public event EventHandler<FaultEvent>? MarkRejected;

    public QwixxGameDealer(IQwixxGameState gameState, IQwixxDiceRoller diceRoller)
    {
        _gameState = gameState;
        _diceRoller = diceRoller;
    }

    public void Add(IEnumerable<IQwixxPlayer> players)
    {
        foreach (var player in players)
        {
            _gameState.AddPlayer(player);
        }
    }

    // Qwixx has no deck or starting hand to prepare - score sheets start empty.
    public void Prepare()
    {
    }

    // QX-007..QX-027: plays full rounds (every player gets a turn) until the game-over condition
    // is met, only checking between rounds so a mid-round trigger still lets the round finish (QX-027).
    public void Play()
    {
        // QX-006
        var playerCount = _gameState.Players.Count();
        if (playerCount < QwixxRules.MinNumberOfPlayers || playerCount > QwixxRules.MaxNumberOfPlayers)
        {
            throw new InvalidOperationException(
                $"Qwixx requires between {QwixxRules.MinNumberOfPlayers} and {QwixxRules.MaxNumberOfPlayers} players, but {playerCount} were added.");
        }

        while (!_gameState.IsGameOver)
        {
            for (var i = 0; i < playerCount; i++)
            {
                PlayTurn();
                _gameState.NextPlayer();
            }
        }
    }

    // QX-032: highest score first. QX-033: ties are both kept, never dropped.
    public IOrderedEnumerable<IQwixxPlayer> GetPlayerResults()
    {
        return _gameState.Players.OrderByDescending(p => p.Score);
    }

    private void PlayTurn()
    {
        var readOnlyState = _gameState.AsReadOnly();
        var roll = _diceRoller.Roll();
        var activePlayer = _gameState.PlayerOnTurn;
        var activePlayerMarkedCountBeforeTurn = GetMarkedCount(activePlayer);

        OfferWhiteMarkToAllPlayers(readOnlyState, roll.WhiteSum);
        OfferColoredMark(activePlayer, readOnlyState, roll);

        // QX-013/QX-014: only the active player is penalized, and only if they marked nothing at all this turn.
        if (GetMarkedCount(activePlayer) == activePlayerMarkedCountBeforeTurn)
        {
            activePlayer.AddPenalty();
            var message = $"PENALTY - player {activePlayer.Name} marked nothing this turn and takes penalty {activePlayer.Penalties} of {QwixxRules.MaxPenalties}.";
            PenaltyTaken?.Invoke(this, new(nameof(PenaltyTaken), message, value: activePlayer.Penalties));
        }
    }

    // Total number of marks across all 4 of a player's rows.
    private static int GetMarkedCount(IQwixxPlayer player)
    {
        return Enum.GetValues<QwixxColor>().Sum(color => player.Row(color).MarkedCount);
    }

    // QX-009: offers the white-dice sum mark to every player.
    private void OfferWhiteMarkToAllPlayers(IQwixxReadOnlyGameState readOnlyState, int whiteSum)
    {
        foreach (var player in _gameState.Players)
        {
            OfferWhiteMark(player, readOnlyState, whiteSum);
        }
    }

    // QX-009: offers the white-dice sum mark to a single player.
    private void OfferWhiteMark(IQwixxPlayer player, IQwixxReadOnlyGameState readOnlyState, int whiteSum)
    {
        var chosenColor = player.DecideWhiteMark(readOnlyState, whiteSum);
        if (chosenColor is null)
        {
            return;
        }

        TryMark(player, readOnlyState, chosenColor.Value, whiteSum);
    }

    // QX-010: only the active player may mark one of the (up to 8) colored candidate sums.
    // QX-012: this is independent of the white step above - the active player may mark the
    // same row via both steps in one turn, e.g. white sum marks red 5, colored combination
    // marks a further-along red 8.
    private void OfferColoredMark(IQwixxPlayer activePlayer, IQwixxReadOnlyGameState readOnlyState, QwixxDiceRoll roll)
    {
        var mark = activePlayer.DecideColoredMark(readOnlyState, roll);
        if (mark is null)
        {
            return;
        }

        // Checked before the candidate-sum guard below, not after: that guard asks the roll for
        // this color's sums, and the roll has no answer for a color that doesn't exist - so
        // without this the validation meant to contain a rogue thinker is itself what throws.
        if (!Enum.IsDefined(mark.Color))
        {
            var unknownColorMessage = $"Mark Rejected - player {activePlayer.Name} chose {(int)mark.Color} {mark.Number}, which is not a Qwixx color.";
            MarkRejected?.Invoke(this, new(nameof(MarkRejected), unknownColorMessage));
            return;
        }

        // A thinker is hackathon-participant code - guard against a number that isn't actually one
        // of this roll's candidate sums for the color it claims.
        if (!roll.ColoredSums(mark.Color).Contains(mark.Number))
        {
            var message = $"Mark Rejected - player {activePlayer.Name} chose {mark.Color} {mark.Number}, which is not one of this roll's candidate sums for {mark.Color}.";
            MarkRejected?.Invoke(this, new(nameof(MarkRejected), message));
            return;
        }

        TryMark(activePlayer, readOnlyState, mark.Color, mark.Number);
    }

    // QX-015..QX-025: validates against both the row's own rules and the global lock state
    // before applying a mark, and offers the lock decision when the mark makes it possible.
    private bool TryMark(IQwixxPlayer player, IQwixxReadOnlyGameState readOnlyState, QwixxColor color, int number)
    {
        // A C# enum holds any value of its underlying type, so a color that came from a thinker
        // is not guaranteed to be one of the four that exist - and every per-color lookup below
        // (starting with the player's own rows) throws on one that isn't. Checked first, so an
        // undefined color is rejected like any other illegal mark instead of ending the game.
        if (!Enum.IsDefined(color))
        {
            var unknownColorMessage = $"Mark Rejected - player {player.Name} tried to mark {(int)color} {number}, which is not a Qwixx color.";
            MarkRejected?.Invoke(this, new(nameof(MarkRejected), unknownColorMessage));
            return false;
        }

        if (_gameState.IsColorLocked(color))
        {
            var lockedMessage = $"Mark Rejected - player {player.Name} tried to mark {color} {number}, but {color} is already locked for every player.";
            MarkRejected?.Invoke(this, new(nameof(MarkRejected), lockedMessage));
            return false;
        }

        var row = player.Row(color);
        if (!row.CanMark(number))
        {
            var rowMessage = $"Mark Rejected - player {player.Name} tried to mark {color} {number}, which that row does not allow (already marked, or skipped past).";
            MarkRejected?.Invoke(this, new(nameof(MarkRejected), rowMessage));
            return false;
        }

        row.Mark(number);

        if (row.CanLock() && player.DecideToLock(readOnlyState, color))
        {
            row.Lock();
            _gameState.LockColor(color);
            var lockMessage = $"ROW LOCKED - player {player.Name} locked {color}. That color is now closed for every player.";
            ColorLocked?.Invoke(this, new(nameof(ColorLocked), lockMessage));
        }

        return true;
    }
}
