using Application.Interfaces;
using Application.Qwixx;
using Application.Qwixx.Thinkers;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Qwixx;

[TestClass]
public class QwixxGameTests
{
    private Mock<IQwixxGameEventCollector> eventCollectorMock;
    private QwixxGame game;

    [TestInitialize]
    public void Setup()
    {
        eventCollectorMock = new Mock<IQwixxGameEventCollector>();
        // PlayWith chains the Attach calls, so the mock has to hand itself back.
        eventCollectorMock.Setup(c => c.Attach(It.IsAny<IQwixxGameDealer>())).Returns(() => eventCollectorMock.Object);
        eventCollectorMock.Setup(c => c.Attach(It.IsAny<IEnumerable<IQwixxPlayer>>())).Returns(() => eventCollectorMock.Object);

        game = new QwixxGame(eventCollectorMock.Object);
    }

    private static List<IQwixxPlayer> CreateGreedyPlayers(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => (IQwixxPlayer)new QwixxPlayer(new GreedyQwixxThinker($"Greedy{i}")))
            .ToList();
    }

    // Returns a QwixxColor value the enum never defined - the mistake a participant makes by
    // casting an out-of-bounds index, not by trying to break anything.
    private class UndefinedColorThinker(bool viaWhiteMark) : BaseQwixxThinker
    {
        public override string Name => "Rogue";

        public override QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum)
            => viaWhiteMark ? (QwixxColor)99 : null;

        public override QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
            => viaWhiteMark ? null : new QwixxMark((QwixxColor)99, roll.WhiteSum);

        public override bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color) => false;
    }

    // The whole-engine version of the dealer's own guard tests: with real QwixxPlayers behind
    // it, an undefined color reaches a per-color dictionary lookup and a switch that each throw.
    // One participant's bot must not be able to take down a whole tournament run.
    [TestMethod]
    [DataRow(true, DisplayName = "via DecideWhiteMark")]
    [DataRow(false, DisplayName = "via DecideColoredMark")]
    public void PlayWith_WhenAThinkerReturnsAnUndefinedColor_FinishesTheGameAnyway(bool viaWhiteMark)
    {
        var players = new List<IQwixxPlayer>
        {
            new QwixxPlayer(new UndefinedColorThinker(viaWhiteMark)),
            new QwixxPlayer(new GreedyQwixxThinker("Greedy")),
        };

        var act = () => game.PlayWith(players);

        act.Should().NotThrow();
        // QX-026: and it really ended, rather than being abandoned somewhere mid-round.
        var lockedColors = Enum.GetValues<QwixxColor>().Count(color => players.Any(p => p.Row(color).IsLocked));
        (lockedColors >= QwixxRules.RowsLockedToEndGame || players.Any(p => p.HasMaxPenalties)).Should().BeTrue();
    }

    [TestMethod]
    public void ExposesTheQwixxPlayerCountLimits()
    {
        game.MinNumberOfPlayers.Should().Be(QwixxRules.MinNumberOfPlayers);
        game.MaxNumberOfPlayers.Should().Be(QwixxRules.MaxNumberOfPlayers);
    }

    // The end-to-end wiring check: game state, dealer, dice roller and players all have to fit
    // together for a real game to run to completion, which no single unit test covers.
    [TestMethod]
    public void PlayWith_PlaysAGameThroughToItsEndCondition()
    {
        var players = CreateGreedyPlayers(3);

        game.PlayWith(players);

        // QX-026: a finished game has either two locked colors, or a player at the penalty limit.
        var lockedColors = Enum.GetValues<QwixxColor>().Count(color => players.Any(p => p.Row(color).IsLocked));
        var gameIsOver = lockedColors >= QwixxRules.RowsLockedToEndGame || players.Any(p => p.HasMaxPenalties);
        gameIsOver.Should().BeTrue();
    }

    // QX-032/QX-033: highest score first, ties kept.
    [TestMethod]
    public void Results_AreOrderedByScoreDescendingAndKeepEveryPlayer()
    {
        var players = CreateGreedyPlayers(3);
        game.PlayWith(players);

        var results = game.Results().ToList();

        results.Should().HaveCount(3);
        results.Select(p => p.Score).Should().BeInDescendingOrder();
    }

    [TestMethod]
    public void PlayWith_AttachesTheEventCollectorToTheDealerAndToThePlayers()
    {
        var players = CreateGreedyPlayers(2);

        game.PlayWith(players);

        eventCollectorMock.Verify(c => c.Attach(It.IsAny<IQwixxGameDealer>()), Times.Once);
        eventCollectorMock.Verify(c => c.Attach(It.IsAny<IEnumerable<IQwixxPlayer>>()), Times.Once);
    }

    // QX-006 is enforced by the dealer, but it has to survive the trip through QwixxGame -
    // a swallowed exception here would leave a one-player game silently running forever.
    [TestMethod]
    public void PlayWith_RefusesToStartWithFewerPlayersThanTheRulesAllow()
    {
        var act = () => game.PlayWith(CreateGreedyPlayers(1));

        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void PlayWith_RefusesToStartWithMorePlayersThanTheRulesAllow()
    {
        var act = () => game.PlayWith(CreateGreedyPlayers(QwixxRules.MaxNumberOfPlayers + 1));

        act.Should().Throw<InvalidOperationException>();
    }

    // Each PlayWith builds its own game state and dealer, so a second game must not inherit
    // the first one's locked colors or score sheets.
    [TestMethod]
    public void PlayWith_StartsAFreshGameEachTime()
    {
        game.PlayWith(CreateGreedyPlayers(2));
        var freshPlayers = CreateGreedyPlayers(2);

        game.PlayWith(freshPlayers);

        // Compared by reference on purpose: both rosters carry the same names, so a structural
        // comparison would pass even if the first game's players came back.
        game.Results().Should().OnlyContain(p => freshPlayers.Contains(p));
    }
}
