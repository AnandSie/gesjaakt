using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.Entities.Game.Qwixx;

[TestClass]
public class QwixxGameDealerTests
{
    private Mock<IQwixxGameState> gameStateMock;
    private Mock<IQwixxDiceRoller> diceRollerMock;
    private QwixxGameDealer dealer;

    [TestInitialize]
    public void Setup()
    {
        gameStateMock = new Mock<IQwixxGameState>();
        diceRollerMock = new Mock<IQwixxDiceRoller>();
        dealer = new QwixxGameDealer(gameStateMock.Object, diceRollerMock.Object);

        // Play() calls AsReadOnly() every turn; a real pass-through wrapper over the same mock
        // means IsColorLocked/IsGameOver/etc. still resolve through gameStateMock's own setups.
        gameStateMock.Setup(gs => gs.AsReadOnly()).Returns(new QwixxReadOnlyGameState(gameStateMock.Object));
    }

    // Gives the mock a real QwixxRow per color, the same way the real QwixxPlayer does, so
    // TryMark's marking/locking logic runs for real against it during Play().
    private static Mock<IQwixxPlayer> CreatePlayerMock()
    {
        var player = new Mock<IQwixxPlayer>();
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            player.Setup(p => p.Row(color)).Returns(new QwixxRow(color));
        }
        return player;
    }

    // A player stubbed to always decline both marks - used purely to pad a test's player list
    // up to QwixxRules.MinNumberOfPlayers without affecting the behavior under test.
    private static Mock<IQwixxPlayer> CreateInertPlayerMock()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        return player;
    }

    // Runs Play() for exactly one round (playerCount turns), then stops. If fewer than
    // QwixxRules.MinNumberOfPlayers are given, inert players pad the list out to satisfy that
    // rule and become active (via PlayerOnTurn) for the round's extra turns, so the given
    // player(s) stay active for exactly one turn each - callers that already pass a legal
    // player count keep full control of PlayerOnTurn themselves, unaffected by this.
    private void SetupExactlyOneRound(params IQwixxPlayer[] players)
    {
        if (players.Length < QwixxRules.MinNumberOfPlayers)
        {
            var padding = Enumerable.Range(0, QwixxRules.MinNumberOfPlayers - players.Length)
                .Select(_ => CreateInertPlayerMock().Object);
            var allPlayers = players.Concat(padding).ToArray();

            gameStateMock.Setup(gs => gs.Players).Returns(allPlayers);
            var playerOnTurnSequence = gameStateMock.SetupSequence(gs => gs.PlayerOnTurn);
            foreach (var player in allPlayers)
            {
                playerOnTurnSequence = playerOnTurnSequence.Returns(player);
            }
        }
        else
        {
            gameStateMock.Setup(gs => gs.Players).Returns(players);
        }

        gameStateMock.SetupSequence(gs => gs.IsGameOver).Returns(false).Returns(true);
    }

    [TestMethod]
    public void Add_AddsEachPlayerToGameState()
    {
        var player1 = new Mock<IQwixxPlayer>().Object;
        var player2 = new Mock<IQwixxPlayer>().Object;

        dealer.Add([player1, player2]);

        gameStateMock.Verify(gs => gs.AddPlayer(player1), Times.Once);
        gameStateMock.Verify(gs => gs.AddPlayer(player2), Times.Once);
    }

    // QX-032: the player with the highest score comes first.
    [TestMethod]
    public void QX032_GetPlayerResults_OrdersPlayersByScoreDescending()
    {
        var lowScorer = new Mock<IQwixxPlayer>();
        lowScorer.Setup(p => p.Score).Returns(10);
        var highScorer = new Mock<IQwixxPlayer>();
        highScorer.Setup(p => p.Score).Returns(50);
        gameStateMock.Setup(gs => gs.Players).Returns([lowScorer.Object, highScorer.Object]);

        var result = dealer.GetPlayerResults();

        result.First().Should().Be(highScorer.Object);
        result.Last().Should().Be(lowScorer.Object);
    }

    // QX-033: tied players are both kept in the results, neither dropped.
    [TestMethod]
    public void QX033_GetPlayerResults_KeepsTiedPlayersInTheResults()
    {
        var player1 = new Mock<IQwixxPlayer>();
        player1.Setup(p => p.Score).Returns(30);
        var player2 = new Mock<IQwixxPlayer>();
        player2.Setup(p => p.Score).Returns(30);
        gameStateMock.Setup(gs => gs.Players).Returns([player1.Object, player2.Object]);

        var result = dealer.GetPlayerResults();

        result.Should().HaveCount(2);
    }

    // QX-026: if the game is already over, no turn should be played at all.
    [TestMethod]
    public void Play_DoesNothingIfGameIsAlreadyOver()
    {
        gameStateMock.Setup(gs => gs.Players).Returns([CreatePlayerMock().Object, CreatePlayerMock().Object]);
        gameStateMock.Setup(gs => gs.IsGameOver).Returns(true);

        dealer.Play();

        gameStateMock.Verify(gs => gs.NextPlayer(), Times.Never);
    }

    // QX-006: the game supports 2 to 5 players.
    [TestMethod]
    public void Play_ThrowsWhenThereAreFewerThanTheMinimumNumberOfPlayers()
    {
        gameStateMock.Setup(gs => gs.Players).Returns([CreatePlayerMock().Object]);

        var act = () => dealer.Play();

        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Play_ThrowsWhenThereAreMoreThanTheMaximumNumberOfPlayers()
    {
        var tooManyPlayers = Enumerable.Range(0, QwixxRules.MaxNumberOfPlayers + 1)
            .Select(_ => CreatePlayerMock().Object)
            .ToArray();
        gameStateMock.Setup(gs => gs.Players).Returns(tooManyPlayers);

        var act = () => dealer.Play();

        act.Should().Throw<InvalidOperationException>();
    }

    // QX-009: every player, including the active one, is offered the white-dice sum.
    [TestMethod]
    public void Play_OffersWhiteMarkToEveryPlayer()
    {
        var activePlayer = CreatePlayerMock();
        var otherPlayer = CreatePlayerMock();
        activePlayer.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns((QwixxColor?)null);
        otherPlayer.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns((QwixxColor?)null);
        activePlayer.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(activePlayer.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(activePlayer.Object, otherPlayer.Object);

        dealer.Play();

        // A round with 2 players is 2 turns, and the white mark is offered to everyone every turn.
        activePlayer.Verify(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5), Times.Exactly(2));
        otherPlayer.Verify(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5), Times.Exactly(2));
    }

    // QX-010: only the active player is offered the colored-dice combination.
    [TestMethod]
    public void Play_OffersColoredMarkOnlyToTheActivePlayer()
    {
        var activePlayer = CreatePlayerMock();
        var otherPlayer = CreatePlayerMock();
        activePlayer.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        otherPlayer.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        activePlayer.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(activePlayer.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(activePlayer.Object, otherPlayer.Object);

        dealer.Play();

        // activePlayer stays active for both turns of this 2-player round, so is offered the colored mark each time.
        activePlayer.Verify(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()), Times.Exactly(2));
        otherPlayer.Verify(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()), Times.Never);
    }

    [TestMethod]
    public void Play_WhiteMark_WhenChosenAndValid_MarksTheChosenRow()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Object.Row(QwixxColor.Red).MarkedCount.Should().Be(1);
    }

    [TestMethod]
    public void Play_WhiteMark_WhenDeclined_LeavesTheRowUnmarked()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Object.Row(QwixxColor.Red).MarkedCount.Should().Be(0);
    }

    [TestMethod]
    public void Play_ColoredMark_WhenChosenAndValid_MarksTheActivePlayersRow()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        // roll: white1=2, white2=3, red=1 -> red candidate sums are 3 and 4.
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()))
            .Returns(new QwixxMark(QwixxColor.Red, 4));
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Object.Row(QwixxColor.Red).MarkedCount.Should().Be(1);
    }

    // A thinker is hackathon-participant code; a colored mark whose number isn't actually one
    // of the roll's candidate sums for that color must be rejected rather than trusted.
    [TestMethod]
    public void Play_ColoredMark_WhenNumberIsNotAnActualCandidateSum_IsRejected()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        // roll: white1=2, white2=3, red=1 -> real red candidate sums are 3 and 4, not 99.
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()))
            .Returns(new QwixxMark(QwixxColor.Red, 99));
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Object.Row(QwixxColor.Red).MarkedCount.Should().Be(0);
    }

    [TestMethod]
    public void Play_Mark_WhenColorIsLocked_IsRejected()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        gameStateMock.Setup(gs => gs.IsColorLocked(QwixxColor.Red)).Returns(true);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Object.Row(QwixxColor.Red).MarkedCount.Should().Be(0);
    }

    // QX-022: locking a row on the same mark that reaches it, when the thinker agrees to lock.
    // QX-025: the LockColor call is what closes that color for every player, not just this one.
    [TestMethod]
    public void QX022_QX025_Play_Locking_WhenCanLockAndThinkerAgrees_LocksRowAndNotifiesGameState()
    {
        var player = CreatePlayerMock();
        var redRow = player.Object.Row(QwixxColor.Red);
        redRow.Mark(2);
        redRow.Mark(3);
        redRow.Mark(4);
        redRow.Mark(5); // 4 marks so far; marking the last number (12) as the 5th makes CanLock true.
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 12)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        player.Setup(p => p.DecideToLock(It.IsAny<IQwixxReadOnlyGameState>(), QwixxColor.Red)).Returns(true);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 6, white2: 6, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        redRow.IsLocked.Should().BeTrue();
        gameStateMock.Verify(gs => gs.LockColor(QwixxColor.Red), Times.Once);
    }

    [TestMethod]
    public void Play_Locking_WhenThinkerDeclines_RowStaysUnlocked()
    {
        var player = CreatePlayerMock();
        var redRow = player.Object.Row(QwixxColor.Red);
        redRow.Mark(2);
        redRow.Mark(3);
        redRow.Mark(4);
        redRow.Mark(5);
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 12)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        player.Setup(p => p.DecideToLock(It.IsAny<IQwixxReadOnlyGameState>(), QwixxColor.Red)).Returns(false);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 6, white2: 6, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        redRow.IsLocked.Should().BeFalse();
        gameStateMock.Verify(gs => gs.LockColor(QwixxColor.Red), Times.Never);
    }

    // QX-013: only the active player is penalized, and only if they marked nothing at all this turn.
    [TestMethod]
    public void QX013_Play_Penalty_WhenActivePlayerMarksNothing_IsPenalized()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Verify(p => p.AddPenalty(), Times.Once);
    }

    [TestMethod]
    public void Play_Penalty_WhenActivePlayerMarksSomething_IsNotPenalized()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Verify(p => p.AddPenalty(), Times.Never);
    }

    // QX-014: a non-active player is never penalized, even if they decline the white mark.
    [TestMethod]
    public void QX014_Play_Penalty_IsNeverAppliedToANonActivePlayer()
    {
        var activePlayer = CreatePlayerMock();
        var otherPlayer = CreatePlayerMock();
        activePlayer.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        otherPlayer.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        activePlayer.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(activePlayer.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(activePlayer.Object, otherPlayer.Object);

        dealer.Play();

        otherPlayer.Verify(p => p.AddPenalty(), Times.Never);
    }

    // QX-012 regression: the active player may mark the same row via both the white sum and
    // the colored combination in one turn - this used to be (incorrectly) blocked.
    [TestMethod]
    public void QX012_Play_ActivePlayer_CanMarkTheSameRowTwiceInOneTurn()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns(QwixxColor.Red);
        // roll: white1=2, white2=3, red=6 -> red candidate sums are 8 and 9, both above 5.
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()))
            .Returns(new QwixxMark(QwixxColor.Red, 8));
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 6, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);

        dealer.Play();

        player.Object.Row(QwixxColor.Red).MarkedCount.Should().Be(2);
    }

    // QX-027: the round in progress always finishes (every player gets a turn) even though
    // IsGameOver is only re-checked between rounds, not after each individual turn.
    [TestMethod]
    public void QX027_Play_CompletesTheFullRoundBeforeCheckingGameOverAgain()
    {
        var player1 = CreatePlayerMock();
        var player2 = CreatePlayerMock();
        var player3 = CreatePlayerMock();
        foreach (var player in new[] { player1, player2, player3 })
        {
            player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
            player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        }
        // A different active player for each of the 3 turns in this round.
        gameStateMock.SetupSequence(gs => gs.PlayerOnTurn)
            .Returns(player1.Object)
            .Returns(player2.Object)
            .Returns(player3.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player1.Object, player2.Object, player3.Object);

        dealer.Play();

        gameStateMock.Verify(gs => gs.NextPlayer(), Times.Exactly(3));
        // Every player is offered the white mark on every one of the 3 turns.
        player1.Verify(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>()), Times.Exactly(3));
        player2.Verify(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>()), Times.Exactly(3));
        player3.Verify(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>()), Times.Exactly(3));
        // Each player is active for exactly one of the 3 turns, so gets the colored mark exactly once.
        player1.Verify(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()), Times.Once);
        player2.Verify(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()), Times.Once);
        player3.Verify(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()), Times.Once);
    }

    // QX-024/QX-025: locking closes the color for everyone, which is exactly the kind of
    // table-wide moment the other two dealers report to the console via events.
    [TestMethod]
    public void Play_ColorLocked_IsRaisedWhenARowIsLocked()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.Name).Returns("Locker");
        var redRow = player.Object.Row(QwixxColor.Red);
        redRow.Mark(2);
        redRow.Mark(3);
        redRow.Mark(4);
        redRow.Mark(5); // marking 12 next is the 5th mark, so CanLock becomes true.
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 12)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        player.Setup(p => p.DecideToLock(It.IsAny<IQwixxReadOnlyGameState>(), QwixxColor.Red)).Returns(true);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 6, white2: 6, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);
        var messages = CaptureColorLocked();

        dealer.Play();

        messages.Should().ContainSingle().Which.Should().Contain("Locker").And.Contain("Red");
    }

    [TestMethod]
    public void Play_ColorLocked_IsNotRaisedWhenTheThinkerDeclinesToLock()
    {
        var player = CreatePlayerMock();
        var redRow = player.Object.Row(QwixxColor.Red);
        redRow.Mark(2);
        redRow.Mark(3);
        redRow.Mark(4);
        redRow.Mark(5);
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 12)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        player.Setup(p => p.DecideToLock(It.IsAny<IQwixxReadOnlyGameState>(), QwixxColor.Red)).Returns(false);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 6, white2: 6, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);
        var messages = CaptureColorLocked();

        dealer.Play();

        messages.Should().BeEmpty();
    }

    // QX-013: a penalty is a scoring-relevant surprise for the player, so it gets reported too.
    [TestMethod]
    public void Play_PenaltyTaken_IsRaisedWhenTheActivePlayerMarksNothing()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.Name).Returns("Unlucky");
        player.Setup(p => p.Penalties).Returns(1);
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);
        var messages = CapturePenaltyTaken();

        dealer.Play();

        // The inert padding player is active for this round's other turn and takes its own
        // penalty, so only the messages naming this player are this test's business.
        messages.Should().ContainSingle(m => m.Contains("Unlucky"));
    }

    [TestMethod]
    public void Play_PenaltyTaken_IsNotRaisedWhenTheActivePlayerMarksSomething()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.Name).Returns("Marker");
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);
        var messages = CapturePenaltyTaken();

        dealer.Play();

        messages.Should().NotContain(m => m.Contains("Marker"));
    }

    // Rejections used to be dropped silently, which is what made an unmarkable menu option so
    // hard to diagnose - a thinker gets no return value telling it the mark didn't land.
    [TestMethod]
    public void Play_MarkRejected_IsRaisedWhenTheColorIsLocked()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.Name).Returns("Hopeful");
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        gameStateMock.Setup(gs => gs.IsColorLocked(QwixxColor.Red)).Returns(true);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);
        var messages = CaptureMarkRejected();

        dealer.Play();

        // The white mark is offered to every player on every turn of the round, so this player's
        // doomed Red choice is rejected once per turn - all that matters is that none go silent.
        messages.Should().NotBeEmpty().And.OnlyContain(m => m.Contains("Hopeful") && m.Contains("locked"));
    }

    [TestMethod]
    public void Play_MarkRejected_IsRaisedWhenTheRowDoesNotAllowTheNumber()
    {
        var player = CreatePlayerMock();
        // Red is ascending and already marked at 6, so the white sum of 5 can never land there.
        player.Object.Row(QwixxColor.Red).Mark(6);
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), 5)).Returns(QwixxColor.Red);
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>())).Returns((QwixxMark?)null);
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);
        var messages = CaptureMarkRejected();

        dealer.Play();

        messages.Should().NotBeEmpty().And.OnlyContain(m => m.Contains("Red 5"));
    }

    [TestMethod]
    public void Play_MarkRejected_IsRaisedWhenTheNumberIsNotAnActualCandidateSum()
    {
        var player = CreatePlayerMock();
        player.Setup(p => p.DecideWhiteMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<int>())).Returns((QwixxColor?)null);
        // roll: white1=2, white2=3, red=1 -> real red candidate sums are 3 and 4, not 99.
        player.Setup(p => p.DecideColoredMark(It.IsAny<IQwixxReadOnlyGameState>(), It.IsAny<QwixxDiceRoll>()))
            .Returns(new QwixxMark(QwixxColor.Red, 99));
        gameStateMock.Setup(gs => gs.PlayerOnTurn).Returns(player.Object);
        diceRollerMock.Setup(dr => dr.Roll()).Returns(new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1));
        SetupExactlyOneRound(player.Object);
        var messages = CaptureMarkRejected();

        dealer.Play();

        messages.Should().ContainSingle().Which.Should().Contain("candidate sums");
    }

    private List<string> CaptureColorLocked()
    {
        var messages = new List<string>();
        dealer.ColorLocked += (_, e) => messages.Add(e.Message);
        return messages;
    }

    private List<string> CapturePenaltyTaken()
    {
        var messages = new List<string>();
        dealer.PenaltyTaken += (_, e) => messages.Add(e.Message);
        return messages;
    }

    private List<string> CaptureMarkRejected()
    {
        var messages = new List<string>();
        dealer.MarkRejected += (_, e) => messages.Add(e.Message);
        return messages;
    }
}
