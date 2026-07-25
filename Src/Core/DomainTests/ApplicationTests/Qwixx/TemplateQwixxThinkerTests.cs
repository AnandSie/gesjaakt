using Application.Qwixx.Thinkers;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Qwixx;

// The template is the starting point a hackathon participant copies. Throwing everywhere is its
// contract, not an oversight: a half-filled-in template must fail loudly rather than quietly
// play a game of "decline everything", which would look like a working but hopeless bot.
[TestClass]
public class TemplateQwixxThinkerTests
{
    private TemplateQwixxThinker thinker;
    private Mock<IQwixxReadOnlyGameState> gameStateMock;

    [TestInitialize]
    public void Setup()
    {
        thinker = new TemplateQwixxThinker();
        gameStateMock = new Mock<IQwixxReadOnlyGameState>();
    }

    [TestMethod]
    public void Name_IsNotImplemented()
    {
        var act = () => thinker.Name;

        act.Should().Throw<NotImplementedException>();
    }

    [TestMethod]
    public void DecideWhiteMark_IsNotImplemented()
    {
        var act = () => thinker.DecideWhiteMark(gameStateMock.Object, 5);

        act.Should().Throw<NotImplementedException>();
    }

    [TestMethod]
    public void DecideColoredMark_IsNotImplemented()
    {
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);

        var act = () => thinker.DecideColoredMark(gameStateMock.Object, roll);

        act.Should().Throw<NotImplementedException>();
    }

    [TestMethod]
    public void DecideToLock_IsNotImplemented()
    {
        var act = () => thinker.DecideToLock(gameStateMock.Object, QwixxColor.Red);

        act.Should().Throw<NotImplementedException>();
    }

    // The one piece a participant inherits rather than writes: BaseQwixxThinker.SetState is what
    // makes the protected `Me` property available inside the three Decide methods.
    [TestMethod]
    public void SetState_IsInheritedAndDoesNotThrow()
    {
        var stateMock = new Mock<IQwixxReadOnlyPlayer>();

        var act = () => thinker.SetState(stateMock.Object);

        act.Should().NotThrow();
    }
}
