using Domain.Entities.Game;
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
    private Mock<ILogger<GameDealer>> _loggerMock;
    private Func<IGameState> _gameStateFactory;

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<GameDealer>>();
        _gameStateFactory = () => new GameState(new List<IPlayer>(), new Mock<ILogger<GameState>>().Object);
    }

    [TestMethod]
    public void AddPlayerTest()
    {
        // Arrange
        var scaredPlayer = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object);

        var sut = new GameDealer(new[] { scaredPlayer }, _gameStateFactory, _loggerMock.Object);

        var expectedResult = new List<IPlayerActions> { scaredPlayer };

        // Act & Assert
        sut.State.Players.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void PlaceCoinAction()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GameDealer>>();
        var gameStateFactory = () => new GameState(new List<IPlayer>(), new Mock<ILogger<GameState>>().Object);

        var scaredPlayer = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object);
        scaredPlayer.AcceptCoins(new List<Coin> { new() });

        var sut = new GameDealer(new[] { scaredPlayer }, gameStateFactory, loggerMock.Object);

        // Act
        sut.PlayFirstCard();
        sut.PlayTurn();

        // Assert
        sut.State.AmountOfCoinsOnTable.Should().Be(1);
        scaredPlayer.CoinsAmount.Should().Be(0);
    }

    [TestMethod]
    public void TakeCardAction()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GameDealer>>();
        var gameStateFactory = () => new GameState(new List<IPlayer>(), new Mock<ILogger<GameState>>().Object);

        var scaredPlayer = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object, "Donald");
        scaredPlayer.AcceptCoins(new List<Coin> { new() });

        var greedyPlayer = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object, "Dagobert");
        greedyPlayer.AcceptCoins(new List<Coin> { new() });

        var sut = new GameDealer(new[] { scaredPlayer, greedyPlayer }, gameStateFactory, loggerMock.Object);

        // Act
        sut.PlayFirstCard();
        sut.State.PlayerOnTurn.Name.Should().Be("Donald");

        sut.PlayTurn();
        sut.NextPlayer();
        sut.State.PlayerOnTurn.Name.Should().Be("Dagobert");

        sut.PlayTurn();

        // Assert
        sut.State.AmountOfCoinsOnTable.Should().Be(1);
        scaredPlayer.CoinsAmount.Should().Be(0);
        greedyPlayer.CoinsAmount.Should().Be(1);
        scaredPlayer.Cards.Count.Should().Be(0);
        greedyPlayer.Cards.Count.Should().Be(1);
    }

    [TestMethod]
    public void GreedyPlayerTakesAllCards_LosesGameTest()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GameDealer>>();
        var gameStateFactory = () => new GameState(new List<IPlayer>(), new Mock<ILogger<GameState>>().Object);

        var scaredPlayer1 = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object);
        var greedyPlayer = new Player(new GreedyThinker(), new Mock<ILogger<Player>>().Object);
        var scaredPlayer2 = new Player(new ScaredThinker(), new Mock<ILogger<Player>>().Object);

        var sut = new GameDealer(new Player[] { scaredPlayer1, greedyPlayer, scaredPlayer2 }, gameStateFactory, loggerMock.Object);

        // Act
        sut.Play();
        var winner = sut.Winner();
        var ranking = sut.State.Players.OrderBy(p => p.Points()).ToList();

        // Assert
        winner.Should().Be(ranking.First());
        greedyPlayer.Should().Be(ranking.Last());
    }
}