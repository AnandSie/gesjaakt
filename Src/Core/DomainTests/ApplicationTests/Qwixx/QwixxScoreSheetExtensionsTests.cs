using Application.Qwixx;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Qwixx;

[TestClass]
public class QwixxScoreSheetExtensionsTests
{
    // The exact escape sequences the render is expected to emit. Spelled out literally rather
    // than imported from the production constants, so a change to the palette has to be a
    // deliberate change to these tests too.
    private const string Reset = "\x1b[0m";
    private const string Bold = "\x1b[1m";
    private const string Dim = "\x1b[2m";
    private const string Strikethrough = "\x1b[9m";
    private const string Red = "\x1b[91m";

    private QwixxGameState gameState;
    private QwixxPlayer player;

    [TestInitialize]
    public void Setup()
    {
        gameState = new QwixxGameState();
        player = CreatePlayer("Alice");
        gameState.AddPlayer(player);
    }

    private static QwixxPlayer CreatePlayer(string name)
    {
        var thinkerMock = new Mock<IQwixxThinker>();
        thinkerMock.Setup(t => t.Name).Returns(name);
        return new QwixxPlayer(thinkerMock.Object);
    }

    private IQwixxReadOnlyGameState ReadOnlyState => gameState.AsReadOnly();

    private IQwixxReadOnlyPlayer Me => player.AsReadOnly();

    [TestMethod]
    public void AnsiColorCode_GivesEveryRowColorItsOwnNonEmptyCode()
    {
        var codes = Enum.GetValues<QwixxColor>().Select(color => color.AnsiColorCode()).ToList();

        codes.Should().OnlyHaveUniqueItems();
        codes.Should().NotContain(string.Empty);
    }

    // Every seat needs its own color, otherwise two players' names look the same in the console.
    [TestMethod]
    public void AnsiNameColorCode_GivesEverySeatItsOwnColorUpToTheMaximumPlayerCount()
    {
        var extraPlayers = Enumerable.Range(2, QwixxRules.MaxNumberOfPlayers - 1)
            .Select(seat => CreatePlayer($"Player{seat}"))
            .ToList();
        foreach (var extraPlayer in extraPlayers)
        {
            gameState.AddPlayer(extraPlayer);
        }

        var codes = gameState.Players.Select(p => p.AsReadOnly().AnsiNameColorCode(ReadOnlyState)).ToList();

        codes.Should().HaveCount(QwixxRules.MaxNumberOfPlayers);
        codes.Should().OnlyHaveUniqueItems();
    }

    // A player's color has to survive the whole game, or the console output stops being readable.
    [TestMethod]
    public void AnsiNameColorCode_IsTheSameOnEveryCallForTheSamePlayer()
    {
        var second = CreatePlayer("Bob");
        gameState.AddPlayer(second);

        var firstCall = second.AsReadOnly().AnsiNameColorCode(ReadOnlyState);
        var secondCall = second.AsReadOnly().AnsiNameColorCode(ReadOnlyState);

        secondCall.Should().Be(firstCall);
    }

    // Manual players type their own names, so nothing stops two of them entering the same one.
    // Seats still have to be told apart - colouring by name would hand them the same color and
    // quietly undo the one thing this method exists for.
    [TestMethod]
    public void AnsiNameColorCode_GivesTwoPlayersWithTheSameNameDifferentColors()
    {
        var namesake = CreatePlayer("Alice"); // the player created in Setup is also called Alice
        gameState.AddPlayer(namesake);

        var firstSeat = player.AsReadOnly().AnsiNameColorCode(ReadOnlyState);
        var secondSeat = namesake.AsReadOnly().AnsiNameColorCode(ReadOnlyState);

        secondSeat.Should().NotBe(firstSeat);
    }

    // A player who isn't seated in this game has no seat color to give. Falling back to the
    // first seat's color would silently disguise them as the first player.
    [TestMethod]
    public void AnsiNameColorCode_ForAPlayerNotInTheGame_IsNotAnySeatsColor()
    {
        var outsider = CreatePlayer("Nobody");

        var code = outsider.AsReadOnly().AnsiNameColorCode(ReadOnlyState);

        var seatColors = gameState.Players.Select(p => p.AsReadOnly().AnsiNameColorCode(ReadOnlyState));
        seatColors.Should().NotContain(code);
    }

    // The name color palette must not collide with the row colors, or a name reads as a row.
    [TestMethod]
    public void AnsiNameColorCode_NeverReusesARowColor()
    {
        var rowColors = Enum.GetValues<QwixxColor>().Select(color => color.AnsiColorCode()).ToList();

        var nameColor = Me.AnsiNameColorCode(ReadOnlyState);

        rowColors.Should().NotContain(nameColor);
    }

    [TestMethod]
    public void CanReallyMark_IsTrueForANumberOnAnUntouchedRow()
    {
        Me.CanReallyMark(ReadOnlyState, QwixxColor.Red, 5).Should().BeTrue();
    }

    // The gap this method was written to close: only the locking player's own row is ever
    // Lock()ed, so another player's sheet can only learn about it from the global lock.
    [TestMethod]
    public void CanReallyMark_IsFalseWhenTheColorIsLockedByAnotherPlayer()
    {
        gameState.LockColor(QwixxColor.Red);

        Me.CanMark(QwixxColor.Red, 5).Should().BeTrue("this player's own row knows nothing about another player's lock");
        Me.CanReallyMark(ReadOnlyState, QwixxColor.Red, 5).Should().BeFalse();
    }

    [TestMethod]
    public void CanReallyMark_IsFalseWhenTheRowsOwnOrderingForbidsIt()
    {
        player.Row(QwixxColor.Red).Mark(6);

        Me.CanReallyMark(ReadOnlyState, QwixxColor.Red, 5).Should().BeFalse();
    }

    [TestMethod]
    public void ToScoreSheetString_ShowsThePlayersNameAndScore()
    {
        player.Row(QwixxColor.Red).Mark(5);

        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        sheet.Should().Contain("Alice");
        sheet.Should().Contain(player.Score.ToString());
    }

    [TestMethod]
    public void ToScoreSheetString_ShowsAllFourRows()
    {
        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            sheet.Should().Contain(color.ToString());
        }
    }

    [TestMethod]
    public void ToScoreSheetString_ShowsEveryNumberOfARow()
    {
        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        for (var number = QwixxRules.MinRowNumber; number <= QwixxRules.MaxRowNumber; number++)
        {
            sheet.Should().Contain(number.ToString());
        }
    }

    // The three renderable cell states (QX-017): marked, still available, permanently skipped.
    [TestMethod]
    public void ToScoreSheetString_RendersAMarkedNumberInItsRowColorAndStruckThrough()
    {
        player.Row(QwixxColor.Red).Mark(5);

        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        sheet.Should().Contain($"{Red}{Bold}{Strikethrough} 5{Reset}");
    }

    [TestMethod]
    public void ToScoreSheetString_RendersAStillAvailableNumberInItsRowColorWithoutStrikethrough()
    {
        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        sheet.Should().Contain($"{Red}{Bold} 5{Reset}");
    }

    [TestMethod]
    public void ToScoreSheetString_RendersASkippedNumberDimmedRatherThanInItsRowColor()
    {
        player.Row(QwixxColor.Red).Mark(5); // 2, 3 and 4 can never be marked now.

        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        sheet.Should().Contain($"{Dim}{Strikethrough} 4{Reset}");
    }

    [TestMethod]
    public void ToScoreSheetString_ShowsAnEmptyLockCellWhileTheRowIsOpen()
    {
        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        sheet.Should().Contain("[ ]");
    }

    [TestMethod]
    public void ToScoreSheetString_ShowsALockIconOnceTheRowIsLocked()
    {
        var redRow = player.Row(QwixxColor.Red);
        foreach (var number in new[] { 2, 3, 4, 5, 12 })
        {
            redRow.Mark(number);
        }
        redRow.Lock();

        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        sheet.Should().Contain("🔒");
    }

    [TestMethod]
    public void ToScoreSheetString_ShowsOneTickedPenaltyBoxPerPenalty()
    {
        player.AddPenalty();
        player.AddPenalty();

        var sheet = Me.ToScoreSheetString(ReadOnlyState);

        CountOccurrences(sheet, "[X]").Should().Be(2);
        CountOccurrences(sheet, "[ ]").Should().Be(QwixxRules.MaxPenalties - 2 + Enum.GetValues<QwixxColor>().Length);
    }

    private static int CountOccurrences(string text, string value)
    {
        return text.Split(value).Length - 1;
    }
}
