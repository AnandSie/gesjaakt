using Domain.Entities.Game.Gesjaakt;
using Domain.Entities.Thinkers;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        var scaredPlayer1 = new GesjaaktPlayer(new ScaredThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object, "ScaredPlayer One");
        var scaredPlayer2 = new GesjaaktPlayer(new ScaredThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object, "ScaredPlayer Two");
        var greedyPlayer = new GesjaaktPlayer(new GreedyThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object, "Greedy");

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