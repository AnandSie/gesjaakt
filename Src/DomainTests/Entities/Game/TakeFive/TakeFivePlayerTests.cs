using Domain.Entities.Components;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.Entities.Game.TakeFive;

[TestClass]
public class TakeFivePlayerTests
{
    private TakeFivePlayer _player;
    private Mock<ITakeFiveThinker> _takeFiveThinkerMock;
    private Mock<ITakeFiveReadOnlyGameState> gameState;

    [TestInitialize]
    public void Setup()
    {
        _takeFiveThinkerMock = new Mock<ITakeFiveThinker>();
        _player = new TakeFivePlayer(_takeFiveThinkerMock.Object, "player");
        gameState = new Mock<ITakeFiveReadOnlyGameState>();
    }

    [TestMethod]
    public void HasNameTest()
    {
        // Arrange
        var name = "Foo";

        // Act
        var result = new TakeFivePlayer(_takeFiveThinkerMock.Object, name);

        // Assert
        result.Name.Should().Be(name);
    }

    // TODO: test for other methods

    [TestMethod]
    public void PlayerAccecptsEmptyCards()
    {

        var result = _player.CardsCount;

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public void PlayerAccecptsCards()
    {
        // Arrange
        var cards = new List<TakeFiveCard>()
        {
            new TakeFiveCard(1, 1),
            new TakeFiveCard(2, 2)
        };

        // Act
        _player.AccecptCards(cards);
        var result = _player.CardsCount;

        // Assert
        result.Should().Be(cards.Count);
    }

    [TestMethod]
    public void PlayerAccecptsEmptyPenaltyCards()
    {
        // Arrange
        var cards = Array.Empty<TakeFiveCard>();

        // Act
        _player.AccecptPenaltyCards(cards);
        var result = _player.PenaltyCards.Select(c => c.CowHeads).Sum();

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public void PlayerAccecptsPenaltyCards()
    {
        // Arrange
        var cards = new List<TakeFiveCard>()
        {
            new TakeFiveCard(1, 1),
            new TakeFiveCard(2, 2)
        };

        // Act
        _player.AccecptPenaltyCards(cards);
        var result = _player.PenaltyCards.Select(c => c.CowHeads).Sum();

        // Assert
        result.Should().Be(3);
    }

    [TestMethod]
    public void DecideGetsCardFromHandTest()
    {
        // Arrange
        var cardValue = 1;
        TakeFiveCard card = new(cardValue, 1);
        var cards = new List<TakeFiveCard>() { card };
        _player.AccecptCards(cards);

        _takeFiveThinkerMock.Setup(tft => tft.Decide(gameState.Object)).Returns(cardValue);

        // Act
        var result = _player.Decide(gameState.Object);

        // Assert
        _player.CardsCount.Should().Be(cards.Count - 1);
        result.Should().Be(card);
    }

    [TestMethod]
    public void DecideReturnsFirstCardIfCardIsNotInHandTest()
    {
        // Arrange
        TakeFiveCard card1 = new(1, 1);
        TakeFiveCard card2 = new(2, 1);
        TakeFiveCard card3 = new(3, 1);
        var cards = new List<TakeFiveCard>() { card1, card2, card3 };
        _player.AccecptCards(cards);

        var cardNotInHand = 4;
        _takeFiveThinkerMock.Setup(tft => tft.Decide(gameState.Object)).Returns(cardNotInHand);

        // Act
        var result = _player.Decide(gameState.Object);

        // Assert
        result.Should().Be(card1);
    }
}
