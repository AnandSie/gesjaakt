using Domain.Entities.Game.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.Qwixx;

// These pin QwixxRules' constants against docs/qwixx/rules.md so a future edit to either
// place drifts loudly instead of silently, rather than driving out new behavior.
[TestClass]
public class QwixxRulesTests
{
    // QX-001: 2 white dice + 4 colored dice = 6 dice total.
    [TestMethod]
    public void QX001_DiceCounts_MatchTheRulesSpec()
    {
        QwixxRules.NumberOfWhiteDice.Should().Be(2);
        QwixxRules.NumberOfColoredDice.Should().Be(4);
        QwixxRules.TotalDice.Should().Be(6);
    }

    // QX-003/QX-004: rows run between 2 and 12 (ascending or descending).
    [TestMethod]
    public void QX003_RowNumberRange_MatchesTheRulesSpec()
    {
        QwixxRules.MinRowNumber.Should().Be(2);
        QwixxRules.MaxRowNumber.Should().Be(12);
    }

    // QX-006: 2 to 5 players.
    [TestMethod]
    public void QX006_PlayerCountRange_MatchesTheRulesSpec()
    {
        QwixxRules.MinNumberOfPlayers.Should().Be(2);
        QwixxRules.MaxNumberOfPlayers.Should().Be(5);
    }

    // QX-019/QX-030: each penalty is worth 5 points.
    [TestMethod]
    public void QX019_PenaltyPoints_MatchesTheRulesSpec()
    {
        QwixxRules.PenaltyPoints.Should().Be(5);
    }

    // QX-020: 4 penalties ends the game.
    [TestMethod]
    public void QX020_MaxPenalties_MatchesTheRulesSpec()
    {
        QwixxRules.MaxPenalties.Should().Be(4);
    }

    // QX-022: at least 5 total marks (including the last number) are required to lock a row.
    [TestMethod]
    public void QX022_MinMarksToLock_MatchesTheRulesSpec()
    {
        QwixxRules.MinMarksToLock.Should().Be(5);
    }

    // QX-026: 2 locked rows ends the game.
    [TestMethod]
    public void QX026_RowsLockedToEndGame_MatchesTheRulesSpec()
    {
        QwixxRules.RowsLockedToEndGame.Should().Be(2);
    }

    // QX-028/QX-029: triangular score sequence, index = marked count (0..12).
    [TestMethod]
    public void QX028_RowScoreByMarkedCount_FollowsTheTriangularSequence()
    {
        QwixxRules.RowScoreByMarkedCount.Should().BeEquivalentTo(
            new[] { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66, 78 },
            options => options.WithStrictOrdering());
    }
}
