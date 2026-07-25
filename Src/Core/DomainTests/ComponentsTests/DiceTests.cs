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

    // An inverted range is a caller bug that would otherwise surface much later, as an
    // ArgumentOutOfRangeException from deep inside Roll() rather than at the mistake itself.
    [TestMethod]
    public void Constructor_ThrowsWhenTheRangeIsInverted()
    {
        var act = () => new Dice(6, 1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void Constructor_AcceptsASingleValueRange()
    {
        var dice = new Dice(3, 3);

        dice.Roll().Should().Be(3);
    }

    // Pins the guarantee Random.Shared gives, rather than reproducing a bug: Random's instance
    // methods are documented as not thread-safe, and a single Dice can be shared by anything
    // holding it (this suite alone runs at MethodLevel parallelism). Under .NET 8's Xoshiro
    // implementation a race degrades the quality of the sequence rather than pushing values out
    // of range, so this passes either way - it exists to catch a future regression to an
    // unshared Random, not to have demonstrated the switch to Random.Shared was needed.
    [TestMethod]
    public void Roll_StaysWithinRangeWhenRolledFromManyThreadsAtOnce()
    {
        var dice = new Dice(1, 6);
        var outOfRange = 0;

        Parallel.For(0, 16, _ =>
        {
            for (var i = 0; i < 20_000; i++)
            {
                var value = dice.Roll();
                if (value is < 1 or > 6)
                {
                    Interlocked.Increment(ref outOfRange);
                }
            }
        });

        outOfRange.Should().Be(0);
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
