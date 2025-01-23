using Domain.Cards;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace DomainTests.CardsTests;

[TestClass]
public class DeckTests
{
    readonly int min = 3;
    readonly int max = 35;
    private Deck sut;

    [TestInitialize]
    public void Setup()
    {
        this.sut = new Deck(min, max);
    }

    [TestMethod]
    public void DeckSizeBasedOnMinMax()
    {
        var result = sut.AmountOfCardsLeft();
        result.Should().Be(DeckSize());
    }

    [TestMethod]
    public void DeckIsNotEmptyAfterInit()
    {
        sut.IsEmpty().Should().BeFalse();
    }

    [TestMethod]
    public void DeckIsEmptyAfterDrawingAllCards()
    {
        DrawAllCards();

        sut.IsEmpty().Should().BeTrue();
    }

    [TestMethod]
    public void DeckShrunkAfterDrawingCard()
    {
        sut.DrawCard();

        sut.AmountOfCardsLeft().Should().Be(DeckSize() - 1);
    }

    [TestMethod]
    public void CardsAreUnique()
    {
        var cardsDrawn = DrawAllCards();
        cardsDrawn.Distinct().Count().Should().Be(DeckSize());
    }

    [TestMethod]
    public void NotInOrderFromLowToHigh()
    {
        var minToMax = Enumerable.Range(min, DeckSize());
        var cardNumbers = DrawAllCards().Select(c => c.Value);
        Assert.AreEqual(cardNumbers.Count(), minToMax.Count());

        cardNumbers.SequenceEqual(minToMax).Should().BeFalse();
    }

    [TestMethod]
    public void NotInOrderFromHighToLow()
    {
        var maxToMin = Enumerable.Range(min, DeckSize()).Reverse();
        var cardNumbers = DrawAllCards().Select(c => c.Value);
        Assert.AreEqual(cardNumbers.Count(), maxToMin.Count());

        cardNumbers.SequenceEqual(maxToMin).Should().BeFalse();
    }

    [TestMethod]
    public void TakeOutNCards()
    {
        var amount = 5;

        sut.TakeOut(amount);

        sut.AmountOfCardsLeft().Should().Be(DeckSize() - amount);
    }

    private int DeckSize()
    {
        return max - min + 1;
    }

    private List<ICard> DrawAllCards()
    {
        var cardsDrawn = new List<ICard>();
        for (int i = 0; i < DeckSize(); i++)
        {
            cardsDrawn.Add(sut.DrawCard());
        }

        return cardsDrawn;
    }
}