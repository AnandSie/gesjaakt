using Application.Interfaces;
using Application.Qwixx.Thinkers;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Qwixx;

[TestClass]
public class ManualQwixxThinkerTests
{
    private Mock<IPlayerInputProvider> inputProviderMock;
    private QwixxGameState gameState;
    private QwixxPlayer player;
    private ManualQwixxThinker thinker;
    private StringWriter console;
    private TextWriter originalConsole;

    [TestInitialize]
    public void Setup()
    {
        inputProviderMock = new Mock<IPlayerInputProvider>();
        thinker = new ManualQwixxThinker(inputProviderMock.Object, "Alice");
        player = new QwixxPlayer(thinker);
        gameState = new QwixxGameState();
        gameState.AddPlayer(player);
        thinker.SetState(player.AsReadOnly());

        // The score sheet and menus go to Console.WriteLine directly (ILogger strips the ANSI
        // codes), so the tests capture the console instead of letting it reach the test output.
        originalConsole = Console.Out;
        console = new StringWriter();
        Console.SetOut(console);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetOut(originalConsole);
    }

    private IQwixxReadOnlyGameState ReadOnlyState => gameState.AsReadOnly();

    // Captures the choices actually offered to the human, which is what the menu-filtering
    // behavior is really about.
    private List<int> AnswerWith(int choice)
    {
        var offeredChoices = new List<int>();
        inputProviderMock
            .Setup(p => p.GetPlayerInputAsInt(It.IsAny<string>(), It.IsAny<IEnumerable<int>>()))
            .Callback<string, IEnumerable<int>>((_, choices) => offeredChoices.AddRange(choices))
            .Returns(choice);
        return offeredChoices;
    }

    [TestMethod]
    public void UsesTheNameItWasGiven()
    {
        thinker.Name.Should().Be("Alice");
    }

    [TestMethod]
    public void DecideWhiteMark_OffersEveryRowPlusSkipOnAnUntouchedSheet()
    {
        var offeredChoices = AnswerWith(1);

        thinker.DecideWhiteMark(ReadOnlyState, 5);

        offeredChoices.Should().HaveCount(Enum.GetValues<QwixxColor>().Length + 1);
    }

    [TestMethod]
    public void DecideWhiteMark_ReturnsTheChosenRow()
    {
        AnswerWith(1);

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        result.Should().Be(QwixxColor.Red);
    }

    // The bug this filtering was added for: an option that TryMark would silently reject must
    // never make it onto the menu in the first place.
    [TestMethod]
    public void DecideWhiteMark_DoesNotOfferAColorThatIsLockedForEveryone()
    {
        gameState.LockColor(QwixxColor.Red);
        var offeredChoices = AnswerWith(1);

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        offeredChoices.Should().HaveCount(Enum.GetValues<QwixxColor>().Length); // one row gone, skip still there
        result.Should().Be(QwixxColor.Yellow);
    }

    [TestMethod]
    public void DecideWhiteMark_DoesNotOfferAColorItsOwnRowCanNoLongerMark()
    {
        player.Row(QwixxColor.Red).Mark(6); // Red ascends, so 5 can never land there now.
        var offeredChoices = AnswerWith(1);

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        offeredChoices.Should().HaveCount(Enum.GetValues<QwixxColor>().Length);
        result.Should().Be(QwixxColor.Yellow);
    }

    [TestMethod]
    public void DecideWhiteMark_ReturnsNullWhenTheSkipOptionIsChosen()
    {
        var skipOption = Enum.GetValues<QwixxColor>().Length + 1;
        AnswerWith(skipOption);

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        result.Should().BeNull();
    }

    // With every row locked there is nothing to offer but Skip, and picking it is the only
    // possible answer - the menu must not collapse into an empty, unanswerable prompt.
    [TestMethod]
    public void DecideWhiteMark_StillOffersSkipWhenNothingCanBeMarked()
    {
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            gameState.LockColor(color);
        }
        var offeredChoices = AnswerWith(1);

        var result = thinker.DecideWhiteMark(ReadOnlyState, 5);

        offeredChoices.Should().ContainSingle();
        result.Should().BeNull();
    }

    [TestMethod]
    public void DecideWhiteMark_ShowsTheScoreSheetBeforeAsking()
    {
        AnswerWith(1);

        thinker.DecideWhiteMark(ReadOnlyState, 5);

        console.ToString().Should().Contain("Alice").And.Contain("Score");
    }

    [TestMethod]
    public void DecideColoredMark_ReturnsTheChosenCandidate()
    {
        // white1=2, white2=3, red=1 -> the red candidate sums are 3 and 4.
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);
        AnswerWith(1);

        var result = thinker.DecideColoredMark(ReadOnlyState, roll);

        result.Should().Be(new QwixxMark(QwixxColor.Red, 3));
    }

    [TestMethod]
    public void DecideColoredMark_ReturnsNullWhenTheSkipOptionIsChosen()
    {
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);
        // 4 colors x 2 distinct candidate sums each, plus Skip.
        AnswerWith(9);

        var result = thinker.DecideColoredMark(ReadOnlyState, roll);

        result.Should().BeNull();
    }

    // QX-010: two identical white dice give each color the same candidate sum twice, which
    // would otherwise show up as two menu entries that look and behave identically.
    [TestMethod]
    public void DecideColoredMark_ListsADuplicatedCandidateSumOnlyOnce()
    {
        var roll = new QwixxDiceRoll(white1: 3, white2: 3, red: 2, yellow: 2, green: 2, blue: 2);
        var offeredChoices = AnswerWith(1);

        thinker.DecideColoredMark(ReadOnlyState, roll);

        // One candidate per color instead of two, plus Skip.
        offeredChoices.Should().HaveCount(Enum.GetValues<QwixxColor>().Length + 1);
    }

    [TestMethod]
    public void DecideColoredMark_DoesNotOfferACandidateInALockedColor()
    {
        gameState.LockColor(QwixxColor.Red);
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);
        var offeredChoices = AnswerWith(1);

        var result = thinker.DecideColoredMark(ReadOnlyState, roll);

        offeredChoices.Should().HaveCount(7); // 3 colors x 2 sums, plus Skip
        result!.Color.Should().Be(QwixxColor.Yellow);
    }

    [TestMethod]
    public void DecideToLock_ReturnsTrueWhenTheAnswerIsYes()
    {
        AnswerWith(1);

        thinker.DecideToLock(ReadOnlyState, QwixxColor.Red).Should().BeTrue();
    }

    [TestMethod]
    public void DecideToLock_ReturnsFalseWhenTheAnswerIsNo()
    {
        AnswerWith(2);

        thinker.DecideToLock(ReadOnlyState, QwixxColor.Red).Should().BeFalse();
    }

    [TestMethod]
    public void DecideToLock_NamesTheRowBeingLocked()
    {
        AnswerWith(2);

        thinker.DecideToLock(ReadOnlyState, QwixxColor.Green);

        console.ToString().Should().Contain("Green");
    }

    // ILogger strips ANSI codes from anything routed through IPlayerInputProvider, so the
    // colored parts have to reach the console directly and the prompt itself stays plain.
    [TestMethod]
    public void ThePromptPassedToTheInputProviderCarriesNoAnsiCodes()
    {
        var questions = new List<string>();
        inputProviderMock
            .Setup(p => p.GetPlayerInputAsInt(It.IsAny<string>(), It.IsAny<IEnumerable<int>>()))
            .Callback<string, IEnumerable<int>>((question, _) => questions.Add(question))
            .Returns(1);
        var roll = new QwixxDiceRoll(white1: 2, white2: 3, red: 1, yellow: 1, green: 1, blue: 1);

        thinker.DecideWhiteMark(ReadOnlyState, 5);
        thinker.DecideColoredMark(ReadOnlyState, roll);
        thinker.DecideToLock(ReadOnlyState, QwixxColor.Red);

        questions.Should().HaveCount(3);
        questions.Should().OnlyContain(q => !q.Contains('\x1b'));
        console.ToString().Should().Contain("\x1b", "the colored output has to bypass the input provider");
    }
}
