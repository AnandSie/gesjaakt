using System.Runtime.InteropServices;

namespace UserInterface;

// Minimal ANSI styling helpers. Deliberately hand-rolled rather than pulling in
// Spectre.Console: the whole surface we need is "colour a short string" plus a
// handful of box-drawing characters, and a hackathon repo participants clone
// benefits from having no extra package to restore.
public static class Ansi
{
    private const string Reset = "\u001b[0m";

    private static readonly bool _enabled = DetectSupport();
    private static readonly bool _unicode = DetectUnicode();

    // Picks the pretty character when the console can render it, and a plain
    // ASCII stand-in when it can't.
    public static string Glyph(string unicode, string ascii) => _unicode ? unicode : ascii;

    // Box-drawing set, with an ASCII fallback for consoles that would render
    // the pretty characters as mojibake.
    public static string TopLeft => _unicode ? "╭" : "+";
    public static string TopRight => _unicode ? "╮" : "+";
    public static string BottomLeft => _unicode ? "╰" : "+";
    public static string BottomRight => _unicode ? "╯" : "+";
    public static string Horizontal => _unicode ? "─" : "-";
    public static string Vertical => _unicode ? "│" : "|";
    public static string Gutter => _unicode ? "▎" : "|";
    public static string BarFull => _unicode ? "█" : "#";
    public static string BarEmpty => _unicode ? "░" : ".";

    // 24-bit colour. Every terminal that understands the cursor movement the
    // live display already relies on also understands truecolor; the ones that
    // don't get plain text, via DetectSupport below.
    public static string Rgb(string text, int r, int g, int b) =>
        _enabled ? $"\u001b[38;2;{r};{g};{b}m{text}{Reset}" : text;

    public static string Dim(string text) => _enabled ? $"\u001b[2m{text}{Reset}" : text;

    public static string Bold(string text) => _enabled ? $"\u001b[1m{text}{Reset}" : text;

    // Length of a string as the terminal will draw it: escape sequences occupy
    // no columns, so padding computed on the raw string is always too wide.
    public static int VisibleLength(string text)
    {
        int length = 0;
        bool inEscape = false;

        foreach (char c in text)
        {
            if (inEscape)
            {
                if (c == 'm') inEscape = false;
                continue;
            }

            if (c == '\u001b') { inEscape = true; continue; }

            length++;
        }

        return length;
    }

    public static string PadVisibleRight(string text, int width)
    {
        int padding = width - VisibleLength(text);
        return padding > 0 ? text + new string(' ', padding) : text;
    }

    public static string PadVisibleLeft(string text, int width)
    {
        int padding = width - VisibleLength(text);
        return padding > 0 ? new string(' ', padding) + text : text;
    }

    // Honours the NO_COLOR convention (https://no-color.org) and stands down
    // when output is piped to a file, where escape codes are just noise.
    private static bool DetectSupport()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR"))) return false;
        if (Console.IsOutputRedirected) return false;

        return EnableVirtualTerminal();
    }

    private static bool DetectUnicode()
    {
        try
        {
            return Console.OutputEncoding.CodePage is 65001 or 1200 or 1201;
        }
        catch (IOException)
        {
            return false;
        }
    }

    // Same reasoning as ConsoleDisplay.EnableAnsiProcessing: older Windows
    // console hosts drop escape sequences unless virtual terminal mode is set.
    private static bool EnableVirtualTerminal()
    {
        if (!OperatingSystem.IsWindows()) return true;

        const int StdOutputHandle = -11;
        const uint EnableVirtualTerminalProcessing = 0x0004;

        nint handle = GetStdHandle(StdOutputHandle);
        if (handle == nint.Zero || handle == new nint(-1)) return false;
        if (!GetConsoleMode(handle, out uint mode)) return false;

        return SetConsoleMode(handle, mode | EnableVirtualTerminalProcessing);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);
}
