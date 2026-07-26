namespace Domain.Entities.Events;

// How much a game event matters to someone watching the game. This is the
// granularity knob: a simulation of 10.000 games only shows GameChanging,
// a manual game shows everything down to Ordinary.
//
// Deliberately NOT log levels. "A player was gesjaakt" is not a warning and
// "a player combination finished" is not a critical failure - they were only
// ever modelled that way to reuse the logger's filtering. Whether something
// went *wrong* is a separate axis, see EventCategory.
public enum EventImportance
{
    // Routine play; the texture of a game. A card is drawn, a coin is paid.
    Ordinary,

    // Worth pointing out, but the game keeps its shape. A row is taken,
    // a penalty is scored.
    Notable,

    // Rare and consequential for the player it happens to.
    Special,

    // Changes the course of the game or the run: a colour locks, a game ends,
    // a simulation reports its results.
    GameChanging,
}
