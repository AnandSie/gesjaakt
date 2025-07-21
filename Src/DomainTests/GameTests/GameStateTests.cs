using Domain.Entities.Components;
using Domain.Entities.Game.Gesjaakt;
using Domain.Entities.Thinkers;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.GameTests;

[TestClass]
public class GameStateTests
{
    private GesjaaktGameState sut;
    private GesjaaktPlayer greedyPlayer;
    private GesjaaktPlayer greedyPlayer2;

    [TestInitialize]
    public void Setup()
    {
        var mockLogger = new Mock<ILogger<GesjaaktGameState>>();

        greedyPlayer = new GesjaaktPlayer(new GreedyThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object);
        greedyPlayer2 = new GesjaaktPlayer(new GreedyThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object);

        sut = new GesjaaktGameState(new List<IGesjaaktPlayer>(), mockLogger.Object);

        sut.AddPlayer(greedyPlayer);
        sut.AddPlayer(greedyPlayer2);
    }

    [TestMethod]
    public void AddPlayer()
    {
        var expectedResult = new List<IGesjaaktPlayerActions> { greedyPlayer, greedyPlayer2 };
        ((IGesjaaktReadOnlyGameState)sut).Players.Should().BeEquivalentTo(expectedResult);
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

    [TestMethod]
    public void DivideCoins()
    {
        // Arrange
        var coinsPerPlayer = 2;

        // Act
        sut.DivideCoins(coinsPerPlayer);

        // Assert
        foreach (var player in ((IGesjaaktReadOnlyGameState)sut).Players)
        {
            player.CoinsAmount.Should().Be(coinsPerPlayer);
        }
    }
}