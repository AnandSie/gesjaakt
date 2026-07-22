using Domain.Entities.Game.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.Qwixx;

[TestClass]
public class QwixxRowTests
{
    // QX-003/QX-004: ascending rows order low-to-high, descending rows order high-to-low.
    [TestMethod]
    public void QX003_AscendingRow_OrdersNumbersLowToHigh()
    {
        var row = new QwixxRow(QwixxColor.Red);

        row.Mark(6);

        row.CanMark(5).Should().BeFalse(); // 5 comes before 6 in ascending order
        row.CanMark(7).Should().BeTrue();  // 7 comes after 6
    }

    [TestMethod]
    public void QX004_DescendingRow_OrdersNumbersHighToLow()
    {
        var row = new QwixxRow(QwixxColor.Green);

        row.Mark(6);

        row.CanMark(7).Should().BeFalse(); // 7 comes before 6 in descending order
        row.CanMark(5).Should().BeTrue();  // 5 comes after 6 in descending order
    }

    // QX-016: once a number is marked, nothing earlier in the row's order can be marked afterward.
    [TestMethod]
    public void QX016_MarkingANumber_MakesEarlierNumbersUnmarkable()
    {
        var row = new QwixxRow(QwixxColor.Red);

        row.Mark(6);

        row.CanMark(2).Should().BeFalse();
        row.CanMark(5).Should().BeFalse();
    }

    // QX-017: a player may skip numbers; skipped numbers become permanently unavailable,
    // but marking further ahead afterward is still allowed.
    [TestMethod]
    public void QX017_SkippingANumber_MakesItPermanentlyUnavailable()
    {
        var row = new QwixxRow(QwixxColor.Red);

        row.Mark(6); // skips 2, 3, 4, 5

        row.CanMark(4).Should().BeFalse();
    }

    [TestMethod]
    public void QX017_MarkingANumberAfterASkip_IsStillAllowed()
    {
        var row = new QwixxRow(QwixxColor.Red);

        row.Mark(6); // skips 2, 3, 4, 5

        row.CanMark(9).Should().BeTrue();
    }

    // QX-021: the row's last number has no 5-mark prerequisite — it's markable like any other number.
    [TestMethod]
    [DataRow(QwixxColor.Red, 12)]
    [DataRow(QwixxColor.Yellow, 12)]
    [DataRow(QwixxColor.Green, 2)]
    [DataRow(QwixxColor.Blue, 2)]
    public void QX021_LastNumber_CanBeMarkedAsTheFirstMark(QwixxColor color, int lastNumber)
    {
        var row = new QwixxRow(color);

        row.CanMark(lastNumber).Should().BeTrue();
    }

    // QX-022: locking requires the last number marked AND at least 5 total marks in the row.
    // Ascending (Red/Yellow: 2,3,4,5..12) and descending (Green/Blue: 12,11,10,9..2) rows use a
    // different, literal sequence of numbers here — spelled out per row rather than computed,
    // so the test data doesn't silently depend on the same direction logic being tested.
    [TestMethod]
    [DataRow(QwixxColor.Red, 2, 3, 4, 5)]
    [DataRow(QwixxColor.Yellow, 2, 3, 4, 5)]
    [DataRow(QwixxColor.Green, 12, 11, 10, 9)]
    [DataRow(QwixxColor.Blue, 12, 11, 10, 9)]
    public void QX022_CanLock_IsFalse_WhenLastNumberNotYetMarked(QwixxColor color, int m1, int m2, int m3, int m4)
    {
        var row = new QwixxRow(color);
        row.Mark(m1);
        row.Mark(m2);
        row.Mark(m3);
        row.Mark(m4);

        row.CanLock().Should().BeFalse();
    }

    [TestMethod]
    [DataRow(QwixxColor.Red, 2, 3, 12)]
    [DataRow(QwixxColor.Yellow, 2, 3, 12)]
    [DataRow(QwixxColor.Green, 12, 11, 2)]
    [DataRow(QwixxColor.Blue, 12, 11, 2)]
    public void QX022_CanLock_IsFalse_WhenLastNumberMarkedWithFewerThanFiveTotalMarks(QwixxColor color, int m1, int m2, int lastNumber)
    {
        var row = new QwixxRow(color);
        row.Mark(m1);
        row.Mark(m2);
        row.Mark(lastNumber); // 3rd mark overall, but it's the last number

        row.CanLock().Should().BeFalse();
    }

    [TestMethod]
    [DataRow(QwixxColor.Red, 2, 3, 4, 5, 12)]
    [DataRow(QwixxColor.Yellow, 2, 3, 4, 5, 12)]
    [DataRow(QwixxColor.Green, 12, 11, 10, 9, 2)]
    [DataRow(QwixxColor.Blue, 12, 11, 10, 9, 2)]
    public void QX022_CanLock_IsTrue_WhenLastNumberMarkedAsTheFifthMark(QwixxColor color, int m1, int m2, int m3, int m4, int lastNumber)
    {
        var row = new QwixxRow(color);
        row.Mark(m1);
        row.Mark(m2);
        row.Mark(m3);
        row.Mark(m4);
        row.Mark(lastNumber); // 5th mark, and it's the last number

        row.CanLock().Should().BeTrue();
    }

    // QX-023: marking the last number without locking (fewer than 5 marks) forfeits the lock forever,
    // since no further numbers remain to raise the mark count.
    [TestMethod]
    [DataRow(QwixxColor.Red, 2, 12)]
    [DataRow(QwixxColor.Yellow, 2, 12)]
    [DataRow(QwixxColor.Green, 12, 2)]
    [DataRow(QwixxColor.Blue, 12, 2)]
    public void QX023_OnceLastNumberMarkedWithoutLocking_CanLockNeverBecomesTrueAfterward(QwixxColor color, int m1, int lastNumber)
    {
        var row = new QwixxRow(color);
        row.Mark(m1);
        row.Mark(lastNumber); // 2nd mark overall — below the threshold, lock not taken now

        row.CanLock().Should().BeFalse();
    }

    // QX-024: once locked, no further numbers can be marked in this row.
    [TestMethod]
    [DataRow(QwixxColor.Red, 2, 3, 4, 5, 12)]
    [DataRow(QwixxColor.Yellow, 2, 3, 4, 5, 12)]
    [DataRow(QwixxColor.Green, 12, 11, 10, 9, 2)]
    [DataRow(QwixxColor.Blue, 12, 11, 10, 9, 2)]
    public void QX024_OnceLocked_NoFurtherNumbersCanBeMarked(QwixxColor color, int m1, int m2, int m3, int m4, int lastNumber)
    {
        var row = new QwixxRow(color);
        row.Mark(m1);
        row.Mark(m2);
        row.Mark(m3);
        row.Mark(m4);
        row.Mark(lastNumber);
        row.Lock();

        row.IsLocked.Should().BeTrue();
        row.CanMark(6).Should().BeFalse(); // 6 was never marked, but the row is locked either way
    }

    // QX-028: row score follows the triangular sequence based on marked count.
    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(2, 3)]
    [DataRow(3, 6)]
    [DataRow(4, 10)]
    [DataRow(5, 15)]
    [DataRow(6, 21)]
    [DataRow(7, 28)]
    [DataRow(8, 36)]
    [DataRow(9, 45)]
    [DataRow(10, 55)]
    [DataRow(11, 66)]
    public void QX028_Score_FollowsTriangularSequence(int marksToMake, int expectedScore)
    {
        var row = new QwixxRow(QwixxColor.Red);
        for (var number = 2; number < 2 + marksToMake; number++)
        {
            row.Mark(number);
        }

        row.MarkedCount.Should().Be(marksToMake);
        row.Score.Should().Be(expectedScore);
    }

    // QX-028: locking counts as one more mark on top of the numbers marked, e.g. the minimum
    // 5 numbers plus the lock cell makes 6 total marks, worth 21 points.
    [TestMethod]
    [DataRow(QwixxColor.Red, 2, 3, 4, 5, 12)]
    [DataRow(QwixxColor.Yellow, 2, 3, 4, 5, 12)]
    [DataRow(QwixxColor.Green, 12, 11, 10, 9, 2)]
    [DataRow(QwixxColor.Blue, 12, 11, 10, 9, 2)]
    public void QX028_Score_WhenLockedWithMinimumFiveMarks_CountsLockAsSixthMark(QwixxColor color, int m1, int m2, int m3, int m4, int lastNumber)
    {
        var row = new QwixxRow(color);
        row.Mark(m1);
        row.Mark(m2);
        row.Mark(m3);
        row.Mark(m4);
        row.Mark(lastNumber);
        row.Lock();

        row.MarkedCount.Should().Be(6);
        row.Score.Should().Be(21);
    }

    // QX-029: a row with no marks scores zero.
    [TestMethod]
    public void QX029_UnmarkedRow_ScoresZero()
    {
        var row = new QwixxRow(QwixxColor.Red);

        row.Score.Should().Be(0);
    }
}
