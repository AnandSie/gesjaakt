using Domain.Model;
using Domain.Cards;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.CardsTests;

[TestClass]
public class DeckTests
{
    private readonly IDeck sut;

    public DeckTests()
    {
        sut = new Deck();
    }

    [TestMethod]
    public void IsEmpty()
    {
        sut.IsEmpty();
    }
}