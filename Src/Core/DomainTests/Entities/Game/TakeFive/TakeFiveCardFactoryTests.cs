
using Domain.Entities.Game.TakeFive;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.TakeFive;

[TestClass]
public class TakeFiveCardFactoryTests
{
    TakeFiveCardFactory factory;

    [TestInitialize]
    public void Setup()
    {
        factory = new TakeFiveCardFactory();
    }

    [TestMethod]
    public void OrdinaryValueReturnsOneCowHeadTest()
    {
        // Arrange
        const int cardValue = 1;

        // Act
        var result = factory.Create(cardValue);

        // Assert
        var expectedResult = 1;
        result.Value.Should().Be(cardValue);
        result.CowHeads.Should().Be(expectedResult);
    }

    [TestMethod]
    [DataRow(5)]
    [DataRow(15)]
    [DataRow(25)]
    [DataRow(35)]
    public void ValueEndingOnFiveReturnsTwoCardHeadsTest(int cardValue)
    {
        // Act
        var result = factory.Create(cardValue);

        // Assert
        var expectedResult = 2;
        result.Value.Should().Be(cardValue);
        result.CowHeads.Should().Be(expectedResult);
    }

    [TestMethod]
    [DataRow(10)]
    [DataRow(20)]
    [DataRow(30)]
    [DataRow(40)] 
    public void ValueWithEqualDigitsReturnsFiveCardHeadsTest(int cardValue)
    {
        // Act
        var result = factory.Create(cardValue);

        // Assert
        var expectedResult = 3;
        result.Value.Should().Be(cardValue);
        result.CowHeads.Should().Be(expectedResult);
    }

    [TestMethod]
    [DataRow(11)]
    [DataRow(22)]
    [DataRow(33)]
    [DataRow(44)]
    public void ValueEndingOnZeroReturnsThreeCardHeadsTest(int cardValue)
    {
        // Act
        var result = factory.Create(cardValue);

        // Assert
        var expectedResult = 5;
        result.Value.Should().Be(cardValue);
        result.CowHeads.Should().Be(expectedResult);
    }

    [TestMethod]
    public void SpecialCaseReturns7CowHeadTest()
    {
        // Arrange
        const int cardValue = 55;

        // Act
        var result = factory.Create(cardValue);

        // Assert
        var expectedResult = 7;
        result.Value.Should().Be(cardValue);
        result.CowHeads.Should().Be(expectedResult);
    }
}
