using Domain.Entities.Events;

namespace UserInterface;

// Maps the two event axes onto colour and glyph.
//
// Importance picks the colour ramp - the further an event is from routine, the
// warmer and brighter it reads - and category overrides it for faults, which
// should always be the red thing on screen no matter how important they are.
public static class EventTheme
{
    private static readonly (int R, int G, int B) Slate = (122, 132, 148);
    private static readonly (int R, int G, int B) Teal = (86, 182, 194);
    private static readonly (int R, int G, int B) Amber = (229, 176, 82);
    private static readonly (int R, int G, int B) Violet = (188, 140, 245);
    private static readonly (int R, int G, int B) Red = (224, 96, 96);

    public static string Colored(string text, GameEvent gameEvent)
    {
        var (r, g, b) = ColorFor(gameEvent);
        return Ansi.Rgb(text, r, g, b);
    }

    public static string Colored(string text, EventImportance importance, EventCategory category)
    {
        var (r, g, b) = ColorFor(importance, category);
        return Ansi.Rgb(text, r, g, b);
    }

    // Short, fixed-width tag shown in the gutter so scanning a game log is a
    // matter of shape rather than reading every line.
    public static string GlyphFor(GameEvent gameEvent) => gameEvent.Category switch
    {
        EventCategory.Fault => "!",
        EventCategory.Progress => ">",
        EventCategory.Result => "=",
        _ => gameEvent.Importance switch
        {
            EventImportance.Ordinary => Ansi.Glyph("·", "."),
            EventImportance.Notable => Ansi.Glyph("○", "o"),
            EventImportance.Special => Ansi.Glyph("◆", "*"),
            _ => Ansi.Glyph("★", "@"),
        },
    };

    private static (int, int, int) ColorFor(GameEvent gameEvent) => ColorFor(gameEvent.Importance, gameEvent.Category);

    private static (int, int, int) ColorFor(EventImportance importance, EventCategory category)
    {
        if (category == EventCategory.Fault) return Red;

        return importance switch
        {
            EventImportance.Ordinary => Slate,
            EventImportance.Notable => Teal,
            EventImportance.Special => Amber,
            _ => Violet,
        };
    }
}
