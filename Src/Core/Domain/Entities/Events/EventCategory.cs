namespace Domain.Entities.Events;

// What kind of thing happened, independent of how much it matters
// (EventImportance). Drives how an event is presented: play events read as
// narration, faults read as something being broken.
public enum EventCategory
{
    // Something happened inside the rules of the game.
    Play,

    // The runner made progress through a simulation.
    Progress,

    // A game or a simulation produced a result.
    Result,

    // Something misbehaved - almost always a participant's thinker throwing or
    // returning an illegal move. Not fatal (the dealer substitutes a safe move)
    // but the participant wants to know.
    Fault,
}
