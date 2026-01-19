using Domain.Entities.Game.TakeFive;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTests.Entities.Game.TakeFive;

[TestClass]
public class TakeFiveDeckFactoryTests
{
    private TakeFiveDeckFactory factory;


    [TestInitialize]
    public void Setup()
    {
        var cardFactory = new TakeFiveCardFactory();
        factory = new TakeFiveDeckFactory(cardFactory);
    }

    [TestMethod]
    public void CreateDeckOfExpectedSizeTest()
    {
        // Arrange

        // Act
        var result = factory.Create();

        // Assert
        var expectedSize = 104;
        result.AmountOfCardsLeft().Should().Be(expectedSize);
    }


    [TestMethod]
    public void CardTypeIsCorrectTest()
    {
        // Arrange

        // Act
        var result = factory.Create().DrawCard();

        // Assert
        var expectedType = typeof(TakeFiveCard);
        result.GetType().Should().Be(expectedType);
    }


    [TestMethod]
    public void MinMaxTest()
    {
        // Arrange
        var deck = factory.Create();
        var cardValues = new HashSet<int>();
        
        // Act
        while (!deck.IsEmpty())
        {
            cardValues.Add(deck.DrawCard().Value);
        }

        // Assert
        cardValues.Max().Should().Be(104);
        cardValues.Min().Should().Be(1);
    }
}
