using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.Entities.Game.Qwixx;

[TestClass]
public class QwixxPlayerTests
{
    private Mock<IQwixxThinker> thinkerMock;
    private QwixxPlayer player;
    private Mock<IQwixxReadOnlyGameState> gameStateMock;

    [TestInitialize]
    public void Setup()
    {
        thinkerMock = new Mock<IQwixxThinker>();
        player = new QwixxPlayer(thinkerMock.Object);
        gameStateMock = new Mock<IQwixxReadOnlyGameState>();
    }

    // The player's name is whatever its thinker reports — the thinker is the bot
    // implementation hackathon participants write and compete with.
    [TestMethod]
    public void HasNameFromThinker()
    {
        thinkerMock.Setup(t => t.Name).Returns("Foo");

        var result = new QwixxPlayer(thinkerMock.Object);

        result.Name.Should().Be("Foo");
    }

    // QX-002: a player has one row per color.
    [TestMethod]
    [DataRow(QwixxColor.Red)]
    [DataRow(QwixxColor.Yellow)]
    [DataRow(QwixxColor.Green)]
    [DataRow(QwixxColor.Blue)]
    public void QX002_Row_ReturnsARowOfTheRequestedColor(QwixxColor color)
    {
        player.Row(color).Color.Should().Be(color);
    }

    // Row(color) must return the same instance every call, otherwise marks made through one
    // call would be invisible through the next.
    [TestMethod]
    public void QX002_Row_ReturnsTheSameInstanceOnEachCall()
    {
        player.Row(QwixxColor.Red).Mark(2);

        player.Row(QwixxColor.Red).MarkedCount.Should().Be(1);
    }

    // QX-018: penalties start at zero and accumulate one at a time.
    [TestMethod]
    public void QX018_NewPlayer_StartsWithZeroPenalties()
    {
        player.Penalties.Should().Be(0);
    }

    [TestMethod]
    public void QX018_AddPenalty_IncrementsPenaltiesByOne()
    {
        player.AddPenalty();

        player.Penalties.Should().Be(1);
    }

    [TestMethod]
    public void QX018_AddPenalty_AccumulatesAcrossMultipleCalls()
    {
        player.AddPenalty();
        player.AddPenalty();
        player.AddPenalty();

        player.Penalties.Should().Be(3);
    }

    // QX-020: the game-ending penalty threshold is 4.
    [TestMethod]
    public void QX020_HasMaxPenalties_IsFalseBelowFourPenalties()
    {
        player.AddPenalty();
        player.AddPenalty();
        player.AddPenalty();

        player.HasMaxPenalties.Should().BeFalse();
    }

    [TestMethod]
    public void QX020_HasMaxPenalties_IsTrueAtFourPenalties()
    {
        player.AddPenalty();
        player.AddPenalty();
        player.AddPenalty();
        player.AddPenalty();

        player.HasMaxPenalties.Should().BeTrue();
    }

    // QX-031: a fresh player (no marks, no penalties) scores zero.
    [TestMethod]
    public void QX031_NewPlayer_ScoresZero()
    {
        player.Score.Should().Be(0);
    }

    // QX-030: each penalty subtracts 5 points from the total score.
    [TestMethod]
    public void QX030_Penalties_SubtractFivePointsEach()
    {
        player.AddPenalty();
        player.AddPenalty();

        player.Score.Should().Be(-10);
    }

    // QX-031: score is the sum of all 4 row scores.
    [TestMethod]
    public void QX031_Score_SumsAllFourRowScores()
    {
        player.Row(QwixxColor.Red).Mark(2);
        player.Row(QwixxColor.Red).Mark(3);
        player.Row(QwixxColor.Red).Mark(4); // 3 marks -> 6 points

        player.Score.Should().Be(6);
    }

    // QX-031: row scores and the penalty deduction combine into one total.
    [TestMethod]
    public void QX031_Score_CombinesRowScoresAndPenaltyDeduction()
    {
        player.Row(QwixxColor.Red).Mark(2);
        player.Row(QwixxColor.Red).Mark(3);
        player.Row(QwixxColor.Red).Mark(4); // 6 points
        player.Row(QwixxColor.Yellow).Mark(2); // 1 point
        player.AddPenalty(); // -5 points

        player.Score.Should().Be(6 + 1 - 5);
    }

    // QX-009: DecideWhiteMark is a pure delegation to the injected thinker.
    [TestMethod]
    public void QX009_DecideWhiteMark_DelegatesToTheThinker()
    {
        thinkerMock.Setup(t => t.DecideWhiteMark(gameStateMock.Object, 8)).Returns(QwixxColor.Red);

        var result = player.DecideWhiteMark(gameStateMock.Object, 8);

        result.Should().Be(QwixxColor.Red);
    }

    [TestMethod]
    public void QX009_DecideWhiteMark_CanReturnNullToDecline()
    {
        thinkerMock.Setup(t => t.DecideWhiteMark(gameStateMock.Object, 8)).Returns((QwixxColor?)null);

        var result = player.DecideWhiteMark(gameStateMock.Object, 8);

        result.Should().BeNull();
    }

    // QX-010: DecideColoredMark is a pure delegation to the injected thinker.
    [TestMethod]
    public void QX010_DecideColoredMark_DelegatesToTheThinker()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);
        var expectedMark = new QwixxMark(QwixxColor.Red, 7);
        thinkerMock.Setup(t => t.DecideColoredMark(gameStateMock.Object, roll)).Returns(expectedMark);

        var result = player.DecideColoredMark(gameStateMock.Object, roll);

        result.Should().Be(expectedMark);
    }

    [TestMethod]
    public void QX010_DecideColoredMark_CanReturnNullToDecline()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);
        thinkerMock.Setup(t => t.DecideColoredMark(gameStateMock.Object, roll)).Returns((QwixxMark?)null);

        var result = player.DecideColoredMark(gameStateMock.Object, roll);

        result.Should().BeNull();
    }

    [TestMethod]
    public void ReturnsReadOnlyPlayer()
    {
        var result = player.AsReadOnly();

        result.GetType().Should().Be(typeof(QwixxReadOnlyPlayer));
    }
}
