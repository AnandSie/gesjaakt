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
public class GesjaaktGameDealerTests
{
    [TestMethod]
    public void PlaceCoinAction()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GesjaaktGameDealer>>();
        var gameStateFactory = () => new GesjaaktGameState(new List<IGesjaaktPlayer>(), new Mock<ILogger<GesjaaktGameState>>().Object);
        var gamesState = gameStateFactory();

        var scaredPlayer = new GesjaaktPlayer(new ScaredThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object);
        scaredPlayer.AcceptCoins(new List<Coin> { new() });

        var sut = new GesjaaktGameDealer(new[] { scaredPlayer }, gamesState, loggerMock.Object);

        // Act
        //sut.PlayFirstCard();
        sut.PlayTurn();

        // Assert
        gamesState.AmountOfCoinsOnTable.Should().Be(1);
        scaredPlayer.CoinsAmount.Should().Be(0);
    }

    [TestMethod]
    public void GreedyPlayerTakesAllCards_LosesGameTest()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GesjaaktGameDealer>>();
        var gameStateFactory = () => new GesjaaktGameState(new List<IGesjaaktPlayer>(), new Mock<ILogger<GesjaaktGameState>>().Object);
        var gamesState = gameStateFactory();

        var scaredPlayer1 = new GesjaaktPlayer(new ScaredThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object, "ScaredPlayer One");
        var scaredPlayer2 = new GesjaaktPlayer(new ScaredThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object, "ScaredPlayer Two");
        var greedyPlayer = new GesjaaktPlayer(new GreedyThinker(), new Mock<ILogger<GesjaaktPlayer>>().Object, "Greedy");

        var sut = new GesjaaktGameDealer(new GesjaaktPlayer[] { scaredPlayer1, scaredPlayer2, greedyPlayer }, gamesState, loggerMock.Object);

        // Act
        sut.Prepare();
        sut.Play();
        var winner = sut.Winner();

        // Assert
        winner.Should().NotBe(greedyPlayer);
    }
}