using Domain.Entities.Game.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.Qwixx;

[TestClass]
public class QwixxDiceRollTests
{
    // QX-008: a roll holds the value of each of the 6 dice as thrown.
    [TestMethod]
    public void QX008_Roll_ExposesTheTwoWhiteDiceValues()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);

        roll.White1.Should().Be(3);
        roll.White2.Should().Be(5);
    }

    // QX-009: the white-dice sum is available to every player, regardless of who rolled.
    [TestMethod]
    public void QX009_WhiteSum_IsTheSumOfBothWhiteDice()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);

        roll.WhiteSum.Should().Be(8);
    }

    // QX-010: for each color, the active player has two candidate sums — one per white die
    // combined with that color's die. Expected values below are hand-computed from the roll
    // (white1=3, white2=5, red=2, yellow=4, green=6, blue=1), not derived through shared code.
    [TestMethod]
    public void QX010_ColoredSums_ForRed_CombinesEachWhiteDieWithTheRedDie()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);

        roll.ColoredSums(QwixxColor.Red).Should().BeEquivalentTo(new[] { 5, 7 });
    }

    [TestMethod]
    public void QX010_ColoredSums_ForYellow_CombinesEachWhiteDieWithTheYellowDie()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);

        roll.ColoredSums(QwixxColor.Yellow).Should().BeEquivalentTo(new[] { 7, 9 });
    }

    [TestMethod]
    public void QX010_ColoredSums_ForGreen_CombinesEachWhiteDieWithTheGreenDie()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);

        roll.ColoredSums(QwixxColor.Green).Should().BeEquivalentTo(new[] { 9, 11 });
    }

    [TestMethod]
    public void QX010_ColoredSums_ForBlue_CombinesEachWhiteDieWithTheBlueDie()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 5, red: 2, yellow: 4, green: 6, blue: 1);

        roll.ColoredSums(QwixxColor.Blue).Should().BeEquivalentTo(new[] { 4, 6 });
    }

    // QX-010: the two white dice can carry the same value, which legitimately produces a
    // duplicate candidate sum for a color rather than collapsing to one option.
    [TestMethod]
    public void QX010_ColoredSums_WhenBothWhiteDiceMatch_ReturnsTheDuplicateSumTwice()
    {
        var roll = new QwixxDiceRoll(white1: 4, white2: 4, red: 2, yellow: 1, green: 6, blue: 3);

        roll.ColoredSums(QwixxColor.Red).Should().BeEquivalentTo(new[] { 6, 6 });
    }
}
