using Domain.Entities.Game.BaseGame;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.BaseGame;

[TestClass]
public class CardFactoryTests
{
    private CardFactory factory;

    [TestInitialize]
    public void Setup()
    {
        factory = new CardFactory();
    }


    [TestMethod]
    public void CreateCardReturnsCardTest()
    {
        // Arrange
        var value = 1;

        // Act
        var result = factory.Create(value);

        // Assert
        result.Value.Should().Be(value);
    }
}
