namespace UserInterface;

// Draws a framed block of text. Everything the app presents as a finished
// artifact - standings, the event summary, the attribution alarm - goes through
// here, so they all read as the same kind of object.
public static class ConsoleBox
{
    // A null row draws a full-width horizontal rule instead of text. Rows can't
    // compute that themselves: the width isn't known until every row has been
    // measured.
    // maxWidth 0 means "as wide as the console allows".
    public static void Draw(
        string? title,
        IEnumerable<string?> rows,
        Func<string, string>? frameStyle = null,
        int minWidth = 0,
        int maxWidth = 0,
        bool wrapLongRows = false)
    {
        int ceiling = maxWidth > 0 ? Math.Min(maxWidth, ConsoleWidth()) : ConsoleWidth();

        var lines = rows.ToList();
        int content = lines.Max(row => row is null ? 0 : Ansi.VisibleLength(row));
        int desired = Math.Max(content, TitleLength(title)) + 4;

        // Deliberately not Math.Clamp: minWidth is a table's *preference*, and on a
        // console narrower than that preference Clamp throws rather than picking a
        // side. A narrow terminal always wins.
        int width = Math.Min(Math.Max(desired, Math.Max(minWidth, 8)), Math.Max(ceiling, 12));
        int inner = width - 4;

        WriteTop(title, width, frameStyle);

        foreach (var row in lines)
        {
            if (row is null)
            {
                WriteContent(Ansi.Dim(string.Concat(Enumerable.Repeat(Ansi.Horizontal, inner))), inner, frameStyle);
                continue;
            }

            foreach (var piece in Fit(row, inner, wrapLongRows))
            {
                WriteContent(piece, inner, frameStyle);
            }
        }

        WriteBottom(width, frameStyle);
    }

    private static void WriteContent(string text, int inner, Func<string, string>? frameStyle) =>
        Console.WriteLine($"{Frame(Ansi.Vertical, frameStyle)} {Ansi.PadVisibleRight(text, inner)} {Frame(Ansi.Vertical, frameStyle)}");

    // Prose rows wrap rather than lose their tail - a manual game's card list is
    // content, not decoration. Table rows must NOT: they are built to a fixed
    // column layout, and wrapping one puts half its columns on the next line.
    // Only the caller knows which it is holding, hence the flag.
    //
    // A row carrying escape sequences is truncated either way, since splitting one
    // mid-sequence would bleed colour across the rest of the box. The truncation
    // goes through Ansi.TruncateVisible rather than a raw slice: cutting on the raw
    // index drops columns the caller paid for AND leaves the last colour unclosed,
    // which bleeds it over the box border and everything after it.
    private static IEnumerable<string> Fit(string row, int inner, bool wrapLongRows)
    {
        if (Ansi.VisibleLength(row) <= inner) return [row];
        if (!wrapLongRows || Ansi.VisibleLength(row) != row.Length) return [Ansi.TruncateVisible(row, inner)];

        return Wrap(row, inner);
    }

    private static IEnumerable<string> Wrap(string row, int inner)
    {
        var remaining = row;

        while (remaining.Length > inner)
        {
            int cut = remaining.LastIndexOf(' ', inner);
            if (cut <= 0) cut = inner;

            yield return remaining[..cut].TrimEnd();
            remaining = remaining[cut..].TrimStart();
        }

        if (remaining.Length > 0) yield return remaining;
    }

    private static int ConsoleWidth()
    {
        try
        {
            // Redirected output reports a width that means nothing; assume a roomy
            // terminal so tables aren't needlessly wrapped into a file.
            return Console.IsOutputRedirected ? 160 : Math.Max(Console.WindowWidth - 1, 24);
        }
        catch (IOException)
        {
            return 160;
        }
    }

    private static void WriteTop(string? title, int width, Func<string, string>? frameStyle)
    {
        int inner = width - 2;
        var label = title is null ? string.Empty : $" {title.Trim()} ";
        int labelLength = Math.Min(label.Length, inner - 1);
        label = label[..labelLength];

        var rule = string.Concat(Enumerable.Repeat(Ansi.Horizontal, Math.Max(inner - labelLength - 1, 0)));

        Console.WriteLine(Frame(Ansi.TopLeft + Ansi.Horizontal + label + rule + Ansi.TopRight, frameStyle));
    }

    private static void WriteBottom(int width, Func<string, string>? frameStyle)
    {
        var rule = string.Concat(Enumerable.Repeat(Ansi.Horizontal, width - 2));

        Console.WriteLine(Frame(Ansi.BottomLeft + rule + Ansi.BottomRight, frameStyle));
    }

    // Unstyled frames are bold so the box reads as chrome around the content;
    // a caller that passes a style (the attribution alarm) owns the whole look.
    private static string Frame(string text, Func<string, string>? frameStyle) =>
        frameStyle is null ? Ansi.Bold(text) : frameStyle(text);

    private static int TitleLength(string? title) => title is null ? 0 : title.Trim().Length + 2;

}
