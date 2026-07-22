using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxGameDealerTests.
// Dice rolling uses inline randomness rather than an injected roller, matching how Deck<TCard>
// shuffles inline instead of taking an injected IRandom - consistent with the rest of this
// codebase. Turn-by-turn logic (rolling, offering the white/colored marks, validating a
// thinker's answer against Row.CanMark/CanLock and GameState.IsColorLocked, applying penalties)
// is intentionally left undesigned here - it's the actual game logic for this class to implement.
public class QwixxGameDealer : IQwixxGameDealer
{
    public QwixxGameDealer(IQwixxGameState gameState)
    {
        throw new NotImplementedException();
    }

    public void Add(IEnumerable<IQwixxPlayer> players)
    {
        throw new NotImplementedException();
    }

    // Qwixx has no deck or starting hand to prepare - score sheets start empty.
    public void Prepare()
    {
        throw new NotImplementedException();
    }

    // QX-007..QX-027
    public void Play()
    {
        throw new NotImplementedException();
    }

    // QX-032: highest score first. QX-033: ties are both kept, never dropped.
    public IOrderedEnumerable<IQwixxPlayer> GetPlayerResults()
    {
        throw new NotImplementedException();
    }
}
