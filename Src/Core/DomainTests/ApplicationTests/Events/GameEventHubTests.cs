using Application.Events;
using Application.Interfaces;
using Domain.Entities.Events;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Events;

[TestClass]
public class GameEventHubTests
{
    private Mock<IGameEventPresenter> presenterMock;
    private GameEventStatistics statistics;
    private GameEventHub hub;

    [TestInitialize]
    public void Setup()
    {
        presenterMock = new Mock<IGameEventPresenter>();
        statistics = new GameEventStatistics();
        hub = new GameEventHub(statistics, presenterMock.Object);
    }

    [TestMethod]
    public void HandleEvent_PresentsEventsThatMeetTheImportanceThreshold()
    {
        hub.SetMinImportance(EventImportance.Notable);

        var special = new SpecialEvent("PlayerGesjaakt", "gesjaakt!");
        hub.HandleEvent(this, special);

        presenterMock.Verify(p => p.Present(special), Times.Once);
    }

    [TestMethod]
    public void HandleEvent_DoesNotPresentEventsBelowTheImportanceThreshold()
    {
        hub.SetMinImportance(EventImportance.Special);

        hub.HandleEvent(this, new OrdinaryEvent("CardDrawnFromDeck", "card 12"));

        presenterMock.Verify(p => p.Present(It.IsAny<GameEvent>()), Times.Never);
    }

    // The whole point of separating recording from display: a quiet 10.000-game run
    // still has to be able to say how often the ordinary things happened.
    [TestMethod]
    public void HandleEvent_RecordsEventsEvenWhenTheyAreTooOrdinaryToDisplay()
    {
        hub.SetMinImportance(EventImportance.GameChanging);

        hub.HandleEvent(this, new OrdinaryEvent("CardDrawnFromDeck", "card 12", value: 12));
        hub.HandleEvent(this, new OrdinaryEvent("CardDrawnFromDeck", "card 20", value: 20));

        var drawn = statistics.Report().Statistics.Single();
        drawn.Count.Should().Be(2);
        drawn.ValueMean.Should().Be(16);
    }

    [TestMethod]
    public void ShowSummary_HandsThePresenterTheAggregateOverEverythingRecorded()
    {
        hub.SetMinImportance(EventImportance.GameChanging);
        hub.HandleEvent(this, new OrdinaryEvent("CardDrawnFromDeck", "card 12"));

        hub.ShowSummary();

        presenterMock.Verify(p => p.PresentSummary(It.Is<EventStatisticsReport>(r => r.TotalEvents == 1)), Times.Once);
    }

    // Ordinary is the floor of the enum, so an unconfigured hub narrates everything -
    // the safe default for a manual game.
    [TestMethod]
    public void HandleEvent_PresentsEverythingWhenNoThresholdWasSet()
    {
        hub.HandleEvent(this, new OrdinaryEvent("CardDrawnFromDeck", "card 12"));

        presenterMock.Verify(p => p.Present(It.IsAny<GameEvent>()), Times.Once);
    }
}
