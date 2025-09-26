using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.Entities.Game.TakeFive;

[TestClass]
public class TakeFiveGameDealerTests
{
    private Mock<ITakeFiveGameState> gameStateMock;
    private TakeFiveGameDealer gameDealer;

    [TestInitialize]
    public void Setup()
    {
        gameStateMock = new Mock<ITakeFiveGameState>();
        gameDealer = new TakeFiveGameDealer(gameStateMock.Object);
    }

    [TestMethod]
    public void PrepareTests()
    {
        // Arrange
        var startCardsPerPlayer = 10;

        // Act
        gameDealer.Prepare();

        // Assert
        gameStateMock.Verify(gs => gs.InitializeRowsFromDeck(), Times.Once);
        gameStateMock.Verify(gs => gs.DealStartingCards(startCardsPerPlayer), Times.Once);
    }

    [TestMethod]
    public void PlayerWithLowestPenaltyPointsWinsTest()
    {
        // Arrange
        var mockPlayerWithLessPenaltyPoints = new Mock<ITakeFivePlayer>();
        var mockPlayerWithMorePenaltyPoints = new Mock<ITakeFivePlayer>();

        List<TakeFiveCard> lessPoints = new()
        {
            new(1,1),
            new(2,1),
            new(3,1)
        };
        List<TakeFiveCard> morePoints = new()
        {
            new(4,1),
            new(5,5)
        };
        Assert.IsTrue(morePoints.Select(c => c.CowHeads).Sum() > lessPoints.Select(c => c.CowHeads).Sum());

        mockPlayerWithLessPenaltyPoints.Setup(p => p.PenaltyCards).Returns(lessPoints);
        mockPlayerWithMorePenaltyPoints.Setup(p => p.PenaltyCards).Returns(morePoints);

        var mockReadOnlyPlayer = new Mock<ITakeFiveReadOnlyPlayer>();
        mockPlayerWithLessPenaltyPoints.Setup(p => p.AsReadOnly()).Returns(mockReadOnlyPlayer.Object);

        gameStateMock.Setup(gs => gs.Players).Returns([mockPlayerWithLessPenaltyPoints.Object, mockPlayerWithMorePenaltyPoints.Object]);

        // Act
        var result = gameDealer.GetPlayerResults().First();

        // Assert
        result.Should().Be(mockReadOnlyPlayer.Object);
    }

    [TestMethod]
    public void LowestCardsArePlacedFirstTest()
    {
        // Arrange
        var mockPlayer1 = new Mock<ITakeFivePlayer>();
        var mockPlayer2 = new Mock<ITakeFivePlayer>(); ;
        var players = new List<ITakeFivePlayer> { mockPlayer1.Object, mockPlayer2.Object };

        mockPlayer1.SetupSequence(p => p.CardsCount)
            .Returns(1)
            .Returns(0);

        gameStateMock.Setup(gs => gs.Players).Returns(players);

        var card1 = new TakeFiveCard(20, 1);
        var card2 = new TakeFiveCard(60, 1);

        mockPlayer1.Setup(p => p.Decide(It.IsAny<ITakeFiveReadOnlyGameState>())).Returns(card2);
        mockPlayer2.Setup(p => p.Decide(It.IsAny<ITakeFiveReadOnlyGameState>())).Returns(card1);

        var cardRows = new List<List<TakeFiveCard>>
        {
            new(){ new TakeFiveCard(1,2) }, // Row0 - Too low for both cards
            new(){ new TakeFiveCard(5,2) }, // Row1 - Fits card1
            new(){ new TakeFiveCard(40,2) },// Row2 - Too high for card 1, fits card2
            new(){ new TakeFiveCard(80,2) } // Row3 - too high for both cards
        };

        gameStateMock.Setup(gs => gs.CardRows).Returns(cardRows);

        // Act
        gameDealer.Play();

        // Assert
        gameStateMock.Verify(gs => gs.PlaceCard(card1, 1));
        gameStateMock.Verify(gs => gs.PlaceCard(card2, 2));
    }

    [TestMethod]
    public void PlayerTakesFiveTest()
    {
        // Arrange
        var mockPlayer1 = new Mock<ITakeFivePlayer>();
        var mockPlayer2 = new Mock<ITakeFivePlayer>(); ;
        var players = new List<ITakeFivePlayer> { mockPlayer1.Object, mockPlayer2.Object };

        mockPlayer1.SetupSequence(p => p.CardsCount)
            .Returns(1)
            .Returns(0);

        gameStateMock.Setup(gs => gs.Players).Returns(players);

        var card1 = new TakeFiveCard(6, 1);
        var card2 = new TakeFiveCard(7, 1);

        mockPlayer1.Setup(p => p.Decide(It.IsAny<ITakeFiveReadOnlyGameState>())).Returns(card1);
        mockPlayer2.Setup(p => p.Decide(It.IsAny<ITakeFiveReadOnlyGameState>())).Returns(card2);

        TakeFiveCard card1InRow = new(1, 2);
        TakeFiveCard card2InRow = new(2, 2);
        TakeFiveCard card3InRow = new(3, 2);
        TakeFiveCard card4InRow = new(4, 2);
        TakeFiveCard card5InRow = new(5, 2);
        List<TakeFiveCard> fullRow = new() { card1InRow, card2InRow, card3InRow, card4InRow, card5InRow };
        var fullRowIndex = 0;
        gameStateMock.Setup(gm => gm.GetCards(fullRowIndex)).Returns(fullRow);

        var cardRows = new List<List<TakeFiveCard>>
        {
            fullRow,
            new(){ new TakeFiveCard(70,2) },
            new(){ new TakeFiveCard(75,2) },
            new(){ new TakeFiveCard(80,2) }
        };

        var cardRowsAfterTakingRow = new List<List<TakeFiveCard>>
        {
            new(){card1},
            new(){ new TakeFiveCard(70,2) },
            new(){ new TakeFiveCard(75,2) },
            new(){ new TakeFiveCard(80,2) }
        };

        gameStateMock.SetupSequence(gs => gs.CardRows)
            .Returns(cardRows)
            .Returns(cardRows)
            .Returns(cardRowsAfterTakingRow)
            .Returns(cardRowsAfterTakingRow);

        // Act
        gameDealer.Play();

        // Assert
        gameStateMock.Verify(gs => gs.PlaceCard(card1, 0));
        gameStateMock.Verify(gs => gs.PlaceCard(card2, 0));

        mockPlayer1.Verify(p => p.AccecptPenaltyCards(fullRow), Times.Once());
    }

    [TestMethod]
    public void PlayerPlaysCardLowerThanRowsTakesARowTest()
    {
        // Arrange
        var mockPlayer1 = new Mock<ITakeFivePlayer>();
        var mockPlayer2 = new Mock<ITakeFivePlayer>(); ;
        var players = new List<ITakeFivePlayer> { mockPlayer1.Object, mockPlayer2.Object };

        mockPlayer1.SetupSequence(p => p.CardsCount)
            .Returns(1)
            .Returns(0);

        gameStateMock.Setup(gs => gs.Players).Returns(players);

        var card1 = new TakeFiveCard(1, 1);
        var card2 = new TakeFiveCard(60, 1);

        mockPlayer1.Setup(p => p.Decide(It.IsAny<ITakeFiveReadOnlyGameState>())).Returns(card1).Verifiable();
        mockPlayer2.Setup(p => p.Decide(It.IsAny<ITakeFiveReadOnlyGameState>())).Returns(card2).Verifiable();

        List<TakeFiveCard> firstRow = new() { new TakeFiveCard(10, 1) };
        var rowIndexToTake = 0;
        gameStateMock.Setup(gs => gs.GetCards(rowIndexToTake)).Returns(firstRow).Verifiable();

        var cardRows = new List<List<TakeFiveCard>>
        {
            new(){ new TakeFiveCard(30,2) }, // Row1 -
            firstRow, // Row0 - 
            new(){ new TakeFiveCard(40,2) }, // Row2 - fits card2
            new(){ new TakeFiveCard(80,2) }  // Row3 - 
        };
        mockPlayer1.Setup(p => p.Decide(cardRows)).Returns(rowIndexToTake);//Chooses first row

        gameStateMock.Setup(gs => gs.CardRows).Returns(cardRows).Verifiable();

        // Act
        gameDealer.Play();

        // Assert
        mockPlayer1.Verify(p => p.AccecptPenaltyCards(firstRow), Times.Once());
        gameStateMock.Verify(gs => gs.PlaceCard(card1, rowIndexToTake)); // because players decides to plays this row
        gameStateMock.Verify(gs => gs.PlaceCard(card2, 2)); // fits row2 easaly

        mockPlayer1.VerifyAll();
        mockPlayer2.VerifyAll();
        gameStateMock.VerifyAll();
    }
}
