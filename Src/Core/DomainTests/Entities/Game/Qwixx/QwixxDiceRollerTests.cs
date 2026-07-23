using Domain.Entities.Game.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.Qwixx;

[TestClass]
public class QwixxDiceRollerTests
{
    private QwixxDiceRoller roller;

    [TestInitialize]
    public void Setup()
    {
        roller = new QwixxDiceRoller();
    }

    // QX-008: a roll produces values for all 6 dice.
    [TestMethod]
    public void Roll_ReturnsAQwixxDiceRoll()
    {
        var result = roller.Roll();

        result.Should().NotBeNull();
    }

    // QX-001: white dice are standard six-sided dice, so their values stay within 1-6
    // across many rolls, not just by chance on a single roll.
    [TestMethod]
    public void Roll_WhiteDiceValues_AreAlwaysWithinValidRange()
    {
        for (var i = 0; i < 500; i++)
        {
            var result = roller.Roll();

            result.White1.Should().BeInRange(QwixxRules.MinDieValue, QwixxRules.MaxDieValue);
            result.White2.Should().BeInRange(QwixxRules.MinDieValue, QwixxRules.MaxDieValue);
        }
    }

    // QX-001: same as above, for each colored die. QwixxDiceRoll only exposes colored dice
    // through ColoredSums, so the individual die value is recovered as sum-minus-white1.
    [TestMethod]
    [DataRow(QwixxColor.Red)]
    [DataRow(QwixxColor.Yellow)]
    [DataRow(QwixxColor.Green)]
    [DataRow(QwixxColor.Blue)]
    public void Roll_ColoredDieValue_IsAlwaysWithinValidRange(QwixxColor color)
    {
        for (var i = 0; i < 500; i++)
        {
            var result = roller.Roll();
            var coloredDieValue = result.ColoredSums(color)[0] - result.White1;

            coloredDieValue.Should().BeInRange(QwixxRules.MinDieValue, QwixxRules.MaxDieValue);
        }
    }

    // Sanity check that rolls are actually randomized, not a fixed/rigged sequence.
    [TestMethod]
    public void Roll_ConsecutiveRolls_AreUsuallyDifferent()
    {
        var first = roller.Roll();
        var second = roller.Roll();

        Fingerprint(first).Should().NotBe(Fingerprint(second), "two consecutive rolls of all 6 dice being identical is vanishingly unlikely");
    }

    private static string Fingerprint(QwixxDiceRoll roll)
    {
        var colorDice = new[] { QwixxColor.Red, QwixxColor.Yellow, QwixxColor.Green, QwixxColor.Blue }
            .Select(color => roll.ColoredSums(color)[0] - roll.White1);

        return string.Join(",", new[] { roll.White1, roll.White2 }.Concat(colorDice));
    }
}
