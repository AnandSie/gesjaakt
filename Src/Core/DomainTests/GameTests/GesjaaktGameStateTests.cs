using Application.Gesjaakt.Thinkers;
using Domain.Entities.Components;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.GameTests;

[TestClass]
public class GesjaaktGameStateTests
{
    private GesjaaktGameState sut;
    private GesjaaktPlayer greedyPlayer;
    private GesjaaktPlayer greedyPlayer2;

    [TestInitialize]
    public void Setup()
    {
        // REFACTOR - MOQ THINKQER & REMOVE PROJECT REFERENCE TO APPLICATION 
        greedyPlayer = new GesjaaktPlayer(new GreedyThinker());
        greedyPlayer2 = new GesjaaktPlayer(new GreedyThinker());

        sut = new GesjaaktGameState();

        sut.AddPlayer(greedyPlayer);
        sut.AddPlayer(greedyPlayer2);
    }

    [TestMethod]
    public void AddPlayer()
    {
        var expectedResult = new List<IGesjaaktPlayerActions> { greedyPlayer, greedyPlayer2 };
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

    [TestMethod]
    public void DivideCoins()
    {
        // Arrange
        var coinsPerPlayer = 2;

        // Act
        sut.DivideCoins(coinsPerPlayer);

        // Assert
        foreach (var player in sut.Players)
        {
            player.CoinsAmount.Should().Be(coinsPerPlayer);
        }
    }
}
