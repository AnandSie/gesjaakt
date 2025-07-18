using Domain.Entities.Components;
using Domain.Entities.Game.Gesjaakt;
using Domain.Entities.Players;
using Domain.Entities.Thinkers;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.GameTests;

[TestClass]
public class GameDealerTests
{
    [TestMethod]
    public void PlaceCoinAction()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GesjaaktGameDealer>>();
        var gameStateFactory = () => new GesjaaktGameState(new List<IPlayer>(), new Mock<ILogger<GesjaaktGameState>>().Object);
        var gamesState = gameStateFactory();

        var scaredPlayer = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object);
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
    public void TakeCardAction()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GesjaaktGameDealer>>();
        var gameStateFactory = () => new GesjaaktGameState(new List<IPlayer>(), new Mock<ILogger<GesjaaktGameState>>().Object);
        var gamesState = gameStateFactory();

        var scaredPlayer = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object, "Donald");
        scaredPlayer.AcceptCoins(new List<Coin> { new() });

        var greedyPlayer = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object, "Dagobert");
        greedyPlayer.AcceptCoins(new List<Coin> { new() });

        var greedyPlayer2 = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object, "Dagobert 2");
        greedyPlayer2.AcceptCoins(new List<Coin> { new() });

        var sut = new GesjaaktGameDealer(new[] { scaredPlayer, greedyPlayer, greedyPlayer2 }, gamesState, loggerMock.Object);

        // Act
        sut.Prepare();
        gamesState.PlayerOnTurn.Name.Should().Be("Donald");

        sut.PlayTurn();
        sut.NextPlayer();
        gamesState.PlayerOnTurn.Name.Should().Be("Dagobert");

        sut.PlayTurn();

        // Assert
        gamesState.AmountOfCoinsOnTable.Should().Be(1);
        scaredPlayer.CoinsAmount.Should().Be(11);
        greedyPlayer.CoinsAmount.Should().Be(12);
        scaredPlayer.Cards.Count.Should().Be(0);
        greedyPlayer.Cards.Count.Should().Be(1);
    }

    [TestMethod]
    public void GreedyPlayerTakesAllCards_LosesGameTest()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GesjaaktGameDealer>>();
        var gameStateFactory = () => new GesjaaktGameState(new List<IPlayer>(), new Mock<ILogger<GesjaaktGameState>>().Object);
        var gamesState = gameStateFactory();

        var scaredPlayer1 = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object);
        var greedyPlayer1 = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object);
        var greedyPlayer2 = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object);

        var sut = new GesjaaktGameDealer(new Player[] { scaredPlayer1, greedyPlayer1, greedyPlayer2 }, gamesState, loggerMock.Object);

        // Act
        sut.Prepare();
        sut.Play();
        var winner = sut.Winner();

        // Assert
        winner.Should().Be(scaredPlayer1);
    }
}