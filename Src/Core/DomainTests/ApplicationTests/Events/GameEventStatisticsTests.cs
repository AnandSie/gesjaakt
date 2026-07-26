using Application.Events;
using Domain.Entities.Events;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.ApplicationTests.Events;

[TestClass]
public class GameEventStatisticsTests
{
    private GameEventStatistics statistics;

    [TestInitialize]
    public void Setup()
    {
        statistics = new GameEventStatistics();
    }

    [TestMethod]
    public void Record_GroupsEventsByKindRatherThanByMessage()
    {
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "Anand takes card 12"));
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "Bot takes card 30"));
        statistics.Record(new OrdinaryEvent("SkippedWithCoin", "Anand pays a coin"));

        var report = statistics.Report();

        report.Statistics.Should().HaveCount(2);
        report.Statistics.Single(s => s.Kind == "PlayerGesjaakt").Count.Should().Be(2);
        report.Statistics.Single(s => s.Kind == "SkippedWithCoin").Count.Should().Be(1);
    }

    [TestMethod]
    public void Record_AggregatesTheNumericValueIntoMeanMinAndMax()
    {
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "took 10", value: 10));
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "took 20", value: 20));
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "took 30", value: 30));

        var gesjaakt = statistics.Report().Statistics.Single();

        gesjaakt.ValueMean.Should().Be(20);
        gesjaakt.Minimum.Should().Be(10);
        gesjaakt.Maximum.Should().Be(30);
    }

    // Not every event carries a number, and the ones that don't must not drag the
    // mean towards zero by being counted as a value of 0.
    [TestMethod]
    public void Record_IgnoresEventsWithoutAValueWhenComputingTheMean()
    {
        statistics.Record(new NotableEvent("RowIsTaken", "row 1 taken", value: 8));
        statistics.Record(new NotableEvent("RowIsTaken", "row 2 taken"));

        var rows = statistics.Report().Statistics.Single();

        rows.Count.Should().Be(2);
        rows.ValueMean.Should().Be(8);
    }

    [TestMethod]
    public void Report_LeavesTheMeanUnsetWhenNoEventOfThatKindCarriedAValue()
    {
        statistics.Record(new OrdinaryEvent("CoinsDivided", "everyone gets coins"));

        var coins = statistics.Report().Statistics.Single();

        coins.HasValues.Should().BeFalse();
        coins.ValueMean.Should().BeNull();
    }

    // GameRunner raises GameEnded once per finished game; that is what turns raw
    // totals into "per game" numbers.
    [TestMethod]
    public void Report_CountsGamesFromTheGameEndedEvent()
    {
        statistics.Record(new NotableEvent(GameEventStatistics.GameEndedKind, "game 1", EventCategory.Result));
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "a"));
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "b"));
        statistics.Record(new SpecialEvent("PlayerGesjaakt", "c"));
        statistics.Record(new NotableEvent(GameEventStatistics.GameEndedKind, "game 2", EventCategory.Result));

        var report = statistics.Report();

        report.GamesObserved.Should().Be(2);
        report.Statistics.Single(s => s.Kind == "PlayerGesjaakt").PerGame(report.GamesObserved).Should().Be(1.5);
    }

    // Faults are what a hackathon participant is actually looking for, so they sort
    // above the loud-but-routine events no matter how rare they are.
    [TestMethod]
    public void Report_OrdersFaultsFirstAndThenByHowOftenTheyHappened()
    {
        statistics.Record(new OrdinaryEvent("CardDrawnFromDeck", "a"));
        statistics.Record(new OrdinaryEvent("CardDrawnFromDeck", "b"));
        statistics.Record(new NotableEvent("RowIsTaken", "c"));
        statistics.Record(new FaultEvent("PlayerDecideError", "thinker threw"));

        var kinds = statistics.Report().Statistics.Select(s => s.Kind);

        kinds.Should().ContainInOrder("PlayerDecideError", "CardDrawnFromDeck", "RowIsTaken");
    }

    [TestMethod]
    public void Report_TotalsEveryEventThatWasRecorded()
    {
        statistics.Record(new OrdinaryEvent("CardDrawnFromDeck", "a"));
        statistics.Record(new OrdinaryEvent("CardDrawnFromDeck", "b"));
        statistics.Record(new FaultEvent("PlayerDecideError", "boom"));

        statistics.Report().TotalEvents.Should().Be(3);
    }

    [TestMethod]
    public void Reset_ClearsBothTheCountersAndTheGameCount()
    {
        statistics.Record(new NotableEvent(GameEventStatistics.GameEndedKind, "game 1", EventCategory.Result));
        statistics.Record(new OrdinaryEvent("CardDrawnFromDeck", "a"));

        statistics.Reset();

        var report = statistics.Report();
        report.IsEmpty.Should().BeTrue();
        report.GamesObserved.Should().Be(0);
    }

    // Nothing observed yet means "per game" has no denominator; falling back to the
    // raw count is more useful than dividing by zero.
    [TestMethod]
    public void PerGame_FallsBackToTheRawCountWhenNoGameHasFinished()
    {
        statistics.Record(new OrdinaryEvent("CardDrawnFromDeck", "a"));
        statistics.Record(new OrdinaryEvent("CardDrawnFromDeck", "b"));

        var report = statistics.Report();

        report.GamesObserved.Should().Be(0);
        report.Statistics.Single().PerGame(report.GamesObserved).Should().Be(2);
    }
}
