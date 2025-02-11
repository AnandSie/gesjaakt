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
        var player = new ScaredPlayer();
        var sut = new GameDealer(new[] { player });

        var expectedResult = new List<IPlayer> { player };
        sut.State.Players.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void GreedyPlayerTakesAllCards_LosesGameTest()
    {
        var greedyPlayer = new GreedyPlayer();
        var scaredPlayer = new ScaredPlayer();
        var sut = new GameDealer(new Player[] { greedyPlayer, scaredPlayer });

        sut.Play();

        var winner = sut.Winner();

        winner.Should().Be(scaredPlayer);
    }
}