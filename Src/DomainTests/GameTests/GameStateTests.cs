using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Entities.Thinkiers;
using Domain.Interfaces;
using FluentAssertions;
using Newtonsoft.Json.Bson;


namespace DomainTests.GameTests;

[TestClass]
public class GameStateTests
{
    GameState sut = new GameState();
    Player greedyPlayer = new (new GreedyThinker());

    [TestInitialize]
    public void Setup()
    {
        sut.AddPlayer(greedyPlayer);
    }

    [TestMethod]
    public void AddPlayer()
    {
        var expectedResult = new List<IPlayerActions> { greedyPlayer };
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