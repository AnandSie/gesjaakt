using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Entities.Thinkiers;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace DomainTests.GameTests;

[TestClass]
public class GameStateTests
{
    private GameState sut;
    private Player greedyPlayer;
    private Player greedyPlayer2;

    [TestInitialize]
    public void Setup()
    {
        var mockLogger = new Mock<ILogger<GameState>>();

        greedyPlayer = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object);
        greedyPlayer2 = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object);

        sut = new GameState(new List<IPlayer>(), mockLogger.Object);

        sut.AddPlayer(greedyPlayer);
        sut.AddPlayer(greedyPlayer2);
    }

    [TestMethod]
    public void AddPlayer()
    {
        var expectedResult = new List<IPlayerActions> { greedyPlayer, greedyPlayer2 };
        sut.Players.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void NoCoinsOnTable()
    {
        sut.AmountOfCoinsOnTable.Should().Be(0);
    }

    [TestMethod]
    public void FirstPlayerOnTurn()
    {
        sut.PlayerOnTurn.Should().Be(greedyPlayer);
    }

    [TestMethod]
    public void FirstCardIsDrawn()
    {
        sut.OpenNextCardFromDeck();
        sut.OpenCardValue.Should().BeInRange(3, 35);
    }

    [TestMethod]
    public void AddCoinToTable()
    {
        var coin = new Coin();
        sut.AddCoinToTable(coin);
        sut.AmountOfCoinsOnTable.Should().Be(1);
    }
}