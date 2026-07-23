using Domain.Entities.Components;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.ComponentsTests;

[TestClass]
public class DiceTests
{
    [TestMethod]
    public void Roll_AlwaysReturnsAValueWithinTheGivenRange()
    {
        var dice = new Dice(1, 6);

        for (var i = 0; i < 500; i++)
        {
            dice.Roll().Should().BeInRange(1, 6);
        }
    }

    // Not hardcoded to a standard d6 - min/max are supplied by the caller, so a game could
    // use a different range without any change to this class.
    [TestMethod]
    public void Roll_RespectsACustomRange()
    {
        var dice = new Dice(10, 12);

        for (var i = 0; i < 500; i++)
        {
            dice.Roll().Should().BeInRange(10, 12);
        }
    }

    [TestMethod]
    public void Roll_ProducesMoreThanOneDistinctValueAcrossManyRolls()
    {
        var dice = new Dice(1, 6);
        var values = new HashSet<int>();

        for (var i = 0; i < 500; i++)
        {
            values.Add(dice.Roll());
        }

        values.Count.Should().BeGreaterThan(1, "500 rolls of a die should not all produce the same value");
    }
}
