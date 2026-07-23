using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxGameStateTests.
// Dice rolling itself is deliberately not modeled here: QwixxDiceRoll is created and passed
// directly into Player.DecideWhiteMark/DecideColoredMark by whatever orchestrates a turn
// (QwixxGameDealer), so this class stays dice-agnostic.
public class QwixxGameState : IQwixxGameState
{
    private readonly List<IQwixxPlayer> _players = new();
    private readonly HashSet<QwixxColor> _lockedColors = new();
    private int _playerOnTurnIndex;

    public IEnumerable<IQwixxPlayer> Players => _players;

    public void AddPlayer(IQwixxPlayer newPlayer)
    {
        _players.Add(newPlayer);
    }

    // QX-007: the active roller; passes to the left via NextPlayer().
    public IQwixxPlayer PlayerOnTurn => _players[_playerOnTurnIndex];

    public void NextPlayer()
    {
        _playerOnTurnIndex = (_playerOnTurnIndex + 1) % _players.Count;
    }

    // QX-024/QX-025: once any player locks a row, that color is locked for everyone —
    // called by whoever orchestrates a turn when a player's Row(color).Lock() succeeds.
    public void LockColor(QwixxColor color)
    {
        _lockedColors.Add(color);
    }

    public bool IsColorLocked(QwixxColor color) => _lockedColors.Contains(color);

    // QX-026: 2+ locked colors, or any player at the penalty limit.
    public bool IsGameOver => _lockedColors.Count >= QwixxRules.RowsLockedToEndGame || _players.Any(p => p.HasMaxPenalties);

    public IQwixxReadOnlyGameState AsReadOnly()
    {
        return new QwixxReadOnlyGameState(this);
    }
}
