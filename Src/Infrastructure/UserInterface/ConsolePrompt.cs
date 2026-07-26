namespace UserInterface;

// Questions asked of the person at the keyboard.
//
// These are not log records - they were only ever routed through the logger at
// Critical level so they would survive the log-level filter, which is why the
// menus used to be prefixed with "crit:". They are written straight to the
// console instead, styled to read as a prompt.
public static class ConsolePrompt
{
    private static readonly (int R, int G, int B) Question = (108, 176, 240);
    private static readonly (int R, int G, int B) Problem = (224, 96, 96);

    public static void Ask(string question)
    {
        Console.WriteLine();

        var lines = question.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
        var marker = Ansi.Rgb("?", Question.R, Question.G, Question.B);

        Console.WriteLine($" {marker} {Ansi.Bold(lines[0])}");

        foreach (var line in lines.Skip(1))
        {
            Console.WriteLine($"   {line}");
        }

        WriteInputMarker();
    }

    public static void Reject(string problem)
    {
        Console.WriteLine($" {Ansi.Rgb("!", Problem.R, Problem.G, Problem.B)} {problem}");
        WriteInputMarker();
    }

    // Left where the caret will sit, so it is obvious the program is waiting for
    // input rather than still working.
    private static void WriteInputMarker() =>
        Console.Write(" " + Ansi.Rgb(Ansi.Glyph("›", ">"), Question.R, Question.G, Question.B) + " ");
}
