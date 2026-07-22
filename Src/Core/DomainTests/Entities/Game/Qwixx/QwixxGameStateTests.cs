using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.Entities.Game.Qwixx;

[TestClass]
public class QwixxGameStateTests
{
    private QwixxGameState state;

    [TestInitialize]
    public void Setup()
    {
        state = new QwixxGameState();
    }

    [TestMethod]
    public void AddPlayer_GetPlayers()
    {
        var player = new Mock<IQwixxPlayer>().Object;

        state.AddPlayer(player);

        state.Players.Single().Should().Be(player);
    }

    // QX-007: the first added player is on turn until NextPlayer() is called.
    [TestMethod]
    public void QX007_PlayerOnTurn_IsTheFirstAddedPlayerInitially()
    {
        var player1 = new Mock<IQwixxPlayer>().Object;
        var player2 = new Mock<IQwixxPlayer>().Object;
        state.AddPlayer(player1);
        state.AddPlayer(player2);

        state.PlayerOnTurn.Should().Be(player1);
    }

    [TestMethod]
    public void QX007_NextPlayer_AdvancesToTheNextAddedPlayer()
    {
        var player1 = new Mock<IQwixxPlayer>().Object;
        var player2 = new Mock<IQwixxPlayer>().Object;
        state.AddPlayer(player1);
        state.AddPlayer(player2);

        state.NextPlayer();

        state.PlayerOnTurn.Should().Be(player2);
    }

    [TestMethod]
    public void QX007_NextPlayer_WrapsAroundToTheFirstPlayerAfterTheLast()
    {
        var player1 = new Mock<IQwixxPlayer>().Object;
        var player2 = new Mock<IQwixxPlayer>().Object;
        state.AddPlayer(player1);
        state.AddPlayer(player2);

        state.NextPlayer(); // -> player2
        state.NextPlayer(); // -> wraps to player1

        state.PlayerOnTurn.Should().Be(player1);
    }

    // QX-024/QX-025: locking a color is global game state, not per-player.
    [TestMethod]
    public void IsColorLocked_IsFalseBeforeAnyLock()
    {
        state.IsColorLocked(QwixxColor.Red).Should().BeFalse();
    }

    [TestMethod]
    public void QX024_LockColor_MarksThatColorAsLocked()
    {
        state.LockColor(QwixxColor.Red);

        state.IsColorLocked(QwixxColor.Red).Should().BeTrue();
    }

    [TestMethod]
    public void QX024_LockColor_DoesNotAffectOtherColors()
    {
        state.LockColor(QwixxColor.Red);

        state.IsColorLocked(QwixxColor.Yellow).Should().BeFalse();
    }

    // QX-026: the game ends on 2+ locked colors, or any player hitting the penalty limit.
    [TestMethod]
    public void QX026_IsGameOver_IsFalseInitially()
    {
        var player = new Mock<IQwixxPlayer>();
        state.AddPlayer(player.Object);

        state.IsGameOver.Should().BeFalse();
    }

    [TestMethod]
    public void QX026_IsGameOver_IsFalseWhenOnlyOneColorIsLocked()
    {
        state.LockColor(QwixxColor.Red);

        state.IsGameOver.Should().BeFalse();
    }

    [TestMethod]
    public void QX026_IsGameOver_IsTrueWhenTwoColorsAreLocked()
    {
        state.LockColor(QwixxColor.Red);
        state.LockColor(QwixxColor.Green);

        state.IsGameOver.Should().BeTrue();
    }

    [TestMethod]
    public void QX026_IsGameOver_IsTrueWhenAnyPlayerHasMaxPenalties()
    {
        var player = new Mock<IQwixxPlayer>();
        player.Setup(p => p.HasMaxPenalties).Returns(true);
        state.AddPlayer(player.Object);

        state.IsGameOver.Should().BeTrue();
    }

    [TestMethod]
    public void ReturnsReadOnlyGameState()
    {
        var result = state.AsReadOnly();

        result.GetType().Should().Be(typeof(QwixxReadOnlyGameState));
    }
}
