using Application.Gesjaakt.Thinkers;
using Domain.Entities.Game.Gesjaakt;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.GameTests;

[TestClass]
public class GesjaaktGameDealerTests
{
    [TestMethod]
    public void GreedyPlayerTakesAllCards_LosesGameTest()
    {
        // Arrange
        var gameStateFactory = () => new GesjaaktGameState();
        var gamesState = gameStateFactory();

        // REFACTOR - Project DomainTests has access to application (which is wrong) => mock thinker dependencies instead of injecting application objects
        var scaredPlayer1 = new GesjaaktPlayer(new ScaredThinker(), "ScaredPlayer One");
        var scaredPlayer2 = new GesjaaktPlayer(new ScaredThinker(), "ScaredPlayer Two");
        var greedyPlayer = new GesjaaktPlayer(new GreedyThinker(), "Greedy");

        var sut = new GesjaaktGameDealer(gamesState);
        sut.Add([scaredPlayer1, scaredPlayer2, greedyPlayer]);

        // Act
        sut.Prepare();
        sut.Play();
        var winner = sut.GetPlayerResults().First();

        // Assert
        winner.Should().NotBe(greedyPlayer);
    }
}