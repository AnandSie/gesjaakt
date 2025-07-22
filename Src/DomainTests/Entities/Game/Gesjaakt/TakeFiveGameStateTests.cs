using Domain.Entities.Components;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.TakeFive;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.Gesjaakt;

[TestClass]
public class TakeFiveGameStateTests
{
    private TakeFiveGameState _state;

    [TestInitialize]
    public void Setup()
    {
        _state = new TakeFiveGameState();
    }

    [TestMethod]
    public void AddPlayers_GetPlayersTest()
    {
        // Arrange
        var player = new TakeFivePlayer();

        // Act
        _state.AddPlayer(player);

        // Assert
        _state.Players.Single().Should().Be(player);
    }

    [TestMethod]
    public void InitializeRowsFromDeckTest()
    {
        // Arrange
        var cardsAmount = 104; // Default TakeFive

        // Act
        _state.InitializeRowsFromDeck();

        // Assert
        var expectedRows = 4;
        _state.CardRows.Should().HaveCount(expectedRows);

        _state.CardRows.ElementAt(0).Should().HaveCount(1);
        _state.CardRows.ElementAt(1).Should().HaveCount(1);
        _state.CardRows.ElementAt(2).Should().HaveCount(1);
        _state.CardRows.ElementAt(3).Should().HaveCount(1);

        _state.Deck.AmountOfCardsLeft().Should().Be(cardsAmount - expectedRows);
    }

    [TestMethod]
    public void GameCanOnlyBeInitializedOnceTest()
    {
        // Arrange
        var cardsAmount = 104; // Default TakeFive
        _state.InitializeRowsFromDeck();

        // Act
        _state.InitializeRowsFromDeck(); // Initialize again

        // Assert
        var expectedRows = 4;
        _state.CardRows.Should().HaveCount(expectedRows);

        _state.CardRows.ElementAt(0).Should().HaveCount(1);
        _state.CardRows.ElementAt(1).Should().HaveCount(1);
        _state.CardRows.ElementAt(2).Should().HaveCount(1);
        _state.CardRows.ElementAt(3).Should().HaveCount(1);

        _state.Deck.AmountOfCardsLeft().Should().Be(cardsAmount - expectedRows);
    }


    [TestMethod]
    public void PlaceCardInRowTest()
    {
        // Arrange
        _state.InitializeRowsFromDeck();

        var card = new Card(10);
        var rowNumber = 3;

        // Act
        _state.PlaceCard(card, rowNumber);

        // Assert
        _state.CardRows.ElementAt(rowNumber).Should().HaveCount(2);
        _state.CardRows.ElementAt(rowNumber).Last().Should().Be(card);

        // Assert extra
        _state.CardRows.ElementAt(0).Should().HaveCount(1);
        _state.CardRows.ElementAt(1).Should().HaveCount(1);
        _state.CardRows.ElementAt(2).Should().HaveCount(1);
    }

    [TestMethod]
    public void GetCardsFromEmptyRowTest()
    {
        _state.GetCards(0).Should().BeEmpty();
        _state.GetCards(1).Should().BeEmpty();
        _state.GetCards(2).Should().BeEmpty();
        _state.GetCards(3).Should().BeEmpty();
    }

    [TestMethod]
    public void GetCardsShouldEmptyRowTest()
    {
        // Arrange
        var firstRow = 0;
        _state.InitializeRowsFromDeck();
        _state.CardRows.ElementAt(firstRow).Should().NotBeEmpty();

        // Act
        var result = _state.GetCards(firstRow);

        // Assert
        result.Should().NotBeEmpty();
        _state.CardRows.ElementAt(firstRow).Should().BeEmpty();
        _state.CardRows.ElementAt(1).Should().NotBeEmpty();
        _state.CardRows.ElementAt(2).Should().NotBeEmpty();
        _state.CardRows.ElementAt(3).Should().NotBeEmpty();
    }
}
