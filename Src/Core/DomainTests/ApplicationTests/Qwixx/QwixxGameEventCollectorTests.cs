using Application.Interfaces;
using Application.Qwixx;
using Domain.Entities.Events;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Qwixx;

[TestClass]
public class QwixxGameEventCollectorTests
{
    private Mock<IGameEventHandler> eventHandlerMock;
    private QwixxGameEventCollector collector;

    [TestInitialize]
    public void Setup()
    {
        eventHandlerMock = new Mock<IGameEventHandler>();
        collector = new QwixxGameEventCollector(eventHandlerMock.Object);
    }

    [TestMethod]
    public void Attach_Dealer_ForwardsColorLockedToTheEventHandler()
    {
        var dealerMock = new Mock<IQwixxGameDealer>();
        collector.Attach(dealerMock.Object);

        dealerMock.Raise(d => d.ColorLocked += null, dealerMock.Object, new WarningEvent("a row was locked"));

        VerifyHandled("a row was locked");
    }

    [TestMethod]
    public void Attach_Dealer_ForwardsPenaltyTakenToTheEventHandler()
    {
        var dealerMock = new Mock<IQwixxGameDealer>();
        collector.Attach(dealerMock.Object);

        dealerMock.Raise(d => d.PenaltyTaken += null, dealerMock.Object, new WarningEvent("a penalty was taken"));

        VerifyHandled("a penalty was taken");
    }

    [TestMethod]
    public void Attach_Dealer_ForwardsMarkRejectedToTheEventHandler()
    {
        var dealerMock = new Mock<IQwixxGameDealer>();
        collector.Attach(dealerMock.Object);

        dealerMock.Raise(d => d.MarkRejected += null, dealerMock.Object, new ErrorEvent("a mark was rejected"));

        VerifyHandled("a mark was rejected");
    }

    // A thinker is hackathon-participant code, so its failures are the events most worth surfacing.
    [TestMethod]
    public void Attach_Players_ForwardsEveryPlayersDecideErrorToTheEventHandler()
    {
        var player1 = new Mock<IQwixxPlayer>();
        var player2 = new Mock<IQwixxPlayer>();
        collector.Attach(new[] { player1.Object, player2.Object });

        player1.Raise(p => p.DecideError += null, player1.Object, new ErrorEvent("player 1 broke"));
        player2.Raise(p => p.DecideError += null, player2.Object, new ErrorEvent("player 2 broke"));

        VerifyHandled("player 1 broke");
        VerifyHandled("player 2 broke");
    }

    // QwixxGame chains the Attach calls, so each one has to hand the collector back.
    [TestMethod]
    public void Attach_ReturnsTheCollectorSoCallsCanBeChained()
    {
        var dealerMock = new Mock<IQwixxGameDealer>();

        collector.Attach(dealerMock.Object).Should().BeSameAs(collector);
        collector.Attach(Array.Empty<IQwixxPlayer>()).Should().BeSameAs(collector);
    }

    // Events raised before Attach - or on a dealer/player that was never attached - simply
    // go nowhere, rather than reaching the handler.
    [TestMethod]
    public void EventsFromAnUnattachedDealerAreNotForwarded()
    {
        var dealerMock = new Mock<IQwixxGameDealer>();

        dealerMock.Raise(d => d.ColorLocked += null, dealerMock.Object, new WarningEvent("ignored"));

        eventHandlerMock.Verify(h => h.HandleEvent(It.IsAny<object>(), It.IsAny<BaseEvent>()), Times.Never);
    }

    private void VerifyHandled(string expectedMessage)
    {
        eventHandlerMock.Verify(h => h.HandleEvent(It.IsAny<object>(), It.Is<BaseEvent>(e => e.Message == expectedMessage)), Times.Once);
    }
}
