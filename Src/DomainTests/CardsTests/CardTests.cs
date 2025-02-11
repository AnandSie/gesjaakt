using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Domain.Entities.Cards;

namespace DomainTests.CardsTests;

[TestClass]
public class GameTests
{
    [TestMethod]
    public void PositiveValueConstructsCard()
    {
        var value = 1;
        var result = new Card(value).Value;

        result.Should().Be(value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NonPositiveThrowsException()
    {
        var value = 0;
        new Card(value);
    }
}