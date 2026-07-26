namespace Domain.Entities.Events;

// A single thing that happened during a game or a simulation.
//
// Kind is the stable, machine-readable identity of the event - raise sites pass
// nameof(TheEvent), so it survives message rewording and can be used as an
// aggregation key. Message is the human sentence. Value is an optional number
// carried by the event (points taken, coins on the table, bull heads collected)
// so a simulation can report a mean and not just a count.
public abstract class GameEvent(
    string kind,
    string message,
    EventImportance importance,
    EventCategory category,
    double? value = null) : EventArgs
{
    public string Kind { get; } = kind;
    public string Message { get; } = message;
    public EventImportance Importance { get; } = importance;
    public EventCategory Category { get; } = category;
    public double? Value { get; } = value;
}

public class OrdinaryEvent(string kind, string message, EventCategory category = EventCategory.Play, double? value = null)
    : GameEvent(kind, message, EventImportance.Ordinary, category, value);

public class NotableEvent(string kind, string message, EventCategory category = EventCategory.Play, double? value = null)
    : GameEvent(kind, message, EventImportance.Notable, category, value);

public class SpecialEvent(string kind, string message, EventCategory category = EventCategory.Play, double? value = null)
    : GameEvent(kind, message, EventImportance.Special, category, value);

public class GameChangingEvent(string kind, string message, EventCategory category = EventCategory.Play, double? value = null)
    : GameEvent(kind, message, EventImportance.GameChanging, category, value);

// A thinker threw, or asked for a move it isn't allowed to make. Always shown
// at Special: a participant debugging their bot needs to see these even when
// the ordinary narration is filtered out.
public class FaultEvent(string kind, string message, double? value = null)
    : GameEvent(kind, message, EventImportance.Special, EventCategory.Fault, value);
