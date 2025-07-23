using Domain.Entities.Components;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.TakeFive;

[TestClass]
public class TakeFiveGameDealerTests
{
    TakeFiveGameState gameState;
    TakeFiveGameDealer gameDealer;
    List<ITakeFivePlayer> players;

    [TestInitialize]
    public void Setup()
    {
        gameState = new TakeFiveGameState();
        var player1 = new TakeFivePlayer();
        gameState.AddPlayer(player1);
        var player2 = new TakeFivePlayer();
        gameState.AddPlayer(player2);

        players = new List<ITakeFivePlayer>();
        players.Add(player1);
        players.Add(player2);

        gameDealer = new TakeFiveGameDealer(gameState);
    }

    [TestMethod]
    public void PrepareSuchAllNPlayersHaveCardsTest()
    {
        // Arrange

        // Act
        gameDealer.Prepare();

        // Assert
        var cardsInGame = 104;
        var startCardsPerPlayer = 10;
        var deck = gameState.AsReadOnly().Deck;
        deck.AmountOfCardsLeft().Should().Be(cardsInGame - startCardsPerPlayer * players.Count);
    }

    // FIXME: meer werken met moq....

    [TestMethod]
    public void PrepareSuchThatOneCardIsPlacedInEachRowTest()
    {
        // Arrange

        // Act
        gameDealer.Prepare();

        // Assert
        foreach (var cardRow in gameState.CardRows)
        {
            cardRow.Should().HaveCount(1);
        }
    }

    // Rules
    // 1. Place in row with smallest difference
    // 2. More than 5, player takes all 5, and card is placed in row
    // 3. if smaller than all, choose row
    // 4. ?

    // Winner/
}
