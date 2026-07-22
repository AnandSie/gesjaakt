namespace Domain.Entities.Game.Qwixx;

public class QwixxRules
{
    // QX-001
    public static readonly int NumberOfWhiteDice = 2;
    public static readonly int NumberOfColoredDice = 4;
    public static readonly int TotalDice = NumberOfWhiteDice + NumberOfColoredDice;

    // QX-003/QX-004
    public static readonly int MinRowNumber = 2;
    public static readonly int MaxRowNumber = 12;

    // QX-006
    public static readonly int MinNumberOfPlayers = 2;
    public static readonly int MaxNumberOfPlayers = 5;

    // QX-019/QX-030
    public static readonly int PenaltyPoints = 5;

    // QX-020
    public static readonly int MaxPenalties = 4;

    // QX-022
    public static readonly int MinMarksToLock = 5;

    // QX-026
    public static readonly int RowsLockedToEndGame = 2;

    // QX-028/QX-029: index = number of marks in a row (0..12), value = that row's score.
    public static readonly IReadOnlyList<int> RowScoreByMarkedCount =
        new[] { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66, 78 };
}
