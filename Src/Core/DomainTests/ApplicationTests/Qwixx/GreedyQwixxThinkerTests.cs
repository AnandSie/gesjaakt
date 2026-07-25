using Application.Qwixx.Thinkers;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Qwixx;

[TestClass]
public class GreedyQwixxThinkerTests
{
    private QwixxGameState gameState;
    private QwixxPlayer player;
    private GreedyQwixxThinker thinker;

    [TestInitialize]
    public void Setup()
    {
        thinker = new GreedyQwixxThinker("Greedy");
        player = new QwixxPlayer(thinker);
        gameState = new QwixxGameState();
        gameState.AddPlayer(player);

        // QwixxPlayer normally pushes this in before every decision; these tests call the
        // thinker directly, so they have to supply its own state themselves.
        thinker.SetState(player.AsReadOnly());
    }

    private IQwixxReadOnlyGameState ReadOnlyState => gameState.AsReadOnly();

    [TestMethod]
    public void UsesTheNameItWasGiven()
    {
        thinker.Name.Should().Be("Greedy");
    }

    [TestMethod]
    public void DecideWhiteMark_TakesTheFirstColorItCanMark()
    {
        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        result.Should().Be(QwixxColor.Red);
    }

    [TestMethod]
    public void DecideWhiteMark_SkipsAColorThatIsLockedForEveryone()
    {
        gameState.LockColor(QwixxColor.Red);

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        result.Should().Be(QwixxColor.Yellow);
    }

    [TestMethod]
    public void DecideWhiteMark_SkipsAColorItsOwnRowCanNoLongerMark()
    {
        player.Row(QwixxColor.Red).Mark(6); // Red ascends, so 5 can never land there now.

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        result.Should().Be(QwixxColor.Yellow);
    }

    [TestMethod]
    public void DecideWhiteMark_DeclinesWhenNoColorCanBeMarked()
    {
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            gameState.LockColor(color);
        }

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        result.Should().BeNull();
    }

    [TestMethod]
    public void DecideColoredMark_TakesTheFirstCandidateSumItCanMark()
    {
        // white1=2, white2=3, red=1 -> the red candidate sums are 3 and 4.
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);

        var result = thinker.DecideColoredMark(ReadOnlyState, roll);

        result.Should().Be(new QwixxMark(QwixxColor.Red, 3));
    }

    [TestMethod]
    public void DecideColoredMark_SkipsAColorThatIsLockedForEveryone()
    {
        gameState.LockColor(QwixxColor.Red);
        // yellow=1 gives the same candidate sums (3 and 4) as red does.
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);

        var result = thinker.DecideColoredMark(ReadOnlyState, roll);

        result.Should().Be(new QwixxMark(QwixxColor.Yellow, 3));
    }

    [TestMethod]
    public void DecideColoredMark_DeclinesWhenNoCandidateSumCanBeMarked()
    {
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            gameState.LockColor(color);
        }
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);

        var result = thinker.DecideColoredMark(ReadOnlyState, roll);

        result.Should().BeNull();
    }

    // Greedy by design: it never passes up a lock, which is what makes a simulated game
    // reliably reach the QX-026 end condition instead of running forever.
    [TestMethod]
    public void DecideToLock_AlwaysLocks()
    {
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            thinker.DecideToLock(ReadOnlyState, color).Should().BeTrue();
        }
    }
}
