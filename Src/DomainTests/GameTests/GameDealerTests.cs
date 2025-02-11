using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;
using System.Numerics;

namespace DomainTests.GameTests;

[TestClass]
public class GameDealerTests
{
    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    public void AddPlayerTest()
    {
        var scaredPlayer = new Player(new ScaredThinker());

        var sut = new GameDealer(new[] { scaredPlayer });

        var expectedResult = new List<IPlayerActions> { scaredPlayer };
        sut.State.Players.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void GreedyPlayerTakesAllCards_LosesGameTest()
    {
        var greedyPlayer1 = new Player(new GreedyThinker());
        var greedyPlayer2 = new Player(new GreedyThinker());
        var scaredPlayer = new Player(new ScaredThinker());
        var sut = new GameDealer(new Player[] { greedyPlayer1, greedyPlayer2, scaredPlayer });

        sut.Play();

        var winner = sut.Winner();

        winner.Should().Be(scaredPlayer);
    }
}