using System.Runtime.InteropServices;
using System.Text;
using Application.Interfaces;

namespace UserInterface;

// Pins the latest simulation message (which may span several lines, e.g. a
// leaderboard) to the bottom of the console while regular (logged) output
// keeps scrolling above it. Cross-platform: only uses ANSI cursor-movement
// sequences, no OS-specific windowing.
public class ConsoleDisplay : IDisplay
{
    private readonly TextWriter _originalOut;
    private readonly bool _interactive;
    private int _linesPrinted;
    private string[] _lastLines = [];

    public ConsoleDisplay(bool useLiveDisplay = true)
    {
        _originalOut = Console.Out;
        _interactive = useLiveDisplay && !Console.IsOutputRedirected;

        if (_interactive)
        {
            EnableAnsiProcessing();
            Console.SetOut(new PinningTextWriter(_originalOut, ClearStatusBlock, RenderStatusBlockIfLineComplete));
        }
    }

    // Some Windows console hosts (older conhost/PowerShell sessions) don't have
    // ANSI escape sequence interpretation turned on by default, in which case our
    // cursor-movement codes are silently dropped instead of executed. Force it on
    // explicitly rather than relying on whatever the ambient mode happens to be.
    // No-op (and harmless) on Linux/macOS, where terminals support ANSI natively.
    private static void EnableAnsiProcessing()
    {
        if (!OperatingSystem.IsWindows()) return;

        const int StdOutputHandle = -11;
        const uint EnableVirtualTerminalProcessing = 0x0004;

        nint handle = GetStdHandle(StdOutputHandle);
        if (handle == nint.Zero || handle == new nint(-1)) return;
        if (!GetConsoleMode(handle, out uint mode)) return;

        SetConsoleMode(handle, mode | EnableVirtualTerminalProcessing);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    public void UpdateMessage(string message)
    {
        _lastLines = message.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');

        if (!_interactive)
        {
            _originalOut.WriteLine($"[status] {message}");
            return;
        }

        ClearStatusBlock();
        RenderStatusBlock();
    }

    // Moves the cursor up to the start of the block and erases everything from
    // there to the end of the screen, using only cursor movement RELATIVE to the
    // current position (ANSI "cursor previous line" + "erase to end of screen").
    // Absolute coordinates (Console.CursorTop/SetCursorPosition) break down once
    // the terminal viewport fills up and starts scrolling - there is no such thing
    // as a stable absolute row once that happens - so relative movement is the
    // only reliable option here, and it's what real terminal UI tools use.
    private void ClearStatusBlock()
    {
        if (_linesPrinted == 0) return;

        if (_linesPrinted > 1)
        {
            _originalOut.Write($"\u001b[{_linesPrinted - 1}F");
        }
        else
        {
            _originalOut.Write('\r');
        }
        _originalOut.Write("\u001b[0J");

        _linesPrinted = 0;
    }

    // Only safe to redraw once the cursor sits at column 0 - i.e. the writer just
    // completed a full line. If we redrew mid-line (e.g. after a Write() call with
    // no trailing newline yet), we'd insert a newline in the middle of that line.
    private void RenderStatusBlockIfLineComplete()
    {
        try
        {
            if (Console.CursorLeft != 0) return;
        }
        catch (IOException)
        {
            return;
        }

        RenderStatusBlock();
    }

    private void RenderStatusBlock()
    {
        if (_lastLines.Length == 0 || (_lastLines.Length == 1 && _lastLines[0].Length == 0)) return;

        try
        {
            if (Console.CursorLeft != 0)
            {
                _originalOut.WriteLine();
            }

            int maxWidth = Math.Max(Console.WindowWidth - 1, 0);

            // A visible boundary marker makes it obvious where the pinned block starts,
            // so it's easy to tell whether it's being redrawn in place or drifting down.
            _originalOut.Write(new string('-', Math.Min(maxWidth, 40)));
            _originalOut.Write(Environment.NewLine);

            for (int i = 0; i < _lastLines.Length; i++)
            {
                var line = _lastLines[i];
                var text = line.Length > maxWidth ? line[..maxWidth] : line;
                _originalOut.Write(text);
                if (i < _lastLines.Length - 1)
                {
                    _originalOut.Write(Environment.NewLine);
                }
            }
            _linesPrinted = _lastLines.Length + 1;
        }
        catch (IOException)
        {
        }
    }

    // Wraps Console.Out so any regular (e.g. logged) output clears the pinned
    // status block first, and the block is redrawn afterwards if the write left
    // the cursor at the start of a line (i.e. a full line was just completed).
    private sealed class PinningTextWriter(TextWriter inner, Action beforeWrite, Action afterWrite) : TextWriter
    {
        public override Encoding Encoding => inner.Encoding;

        public override void Write(char value)
        {
            beforeWrite();
            inner.Write(value);
            afterWrite();
        }

        public override void Write(string? value)
        {
            beforeWrite();
            inner.Write(value);
            afterWrite();
        }

        public override void WriteLine(string? value)
        {
            beforeWrite();
            inner.WriteLine(value);
            afterWrite();
        }

        public override void WriteLine()
        {
            beforeWrite();
            inner.WriteLine();
            afterWrite();
        }
    }
}
