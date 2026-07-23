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
    public void GetPlayerResults_OrdersPlayersByScoreDescending()
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
    public void GetPlayerResults_KeepsTiedPlayersInTheResults()
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
        gameStateMock.Setup(gs => gs.IsGameOver).Returns(true);

        dealer.Play();

        gameStateMock.Verify(gs => gs.NextPlayer(), Times.Never);
    }
}
