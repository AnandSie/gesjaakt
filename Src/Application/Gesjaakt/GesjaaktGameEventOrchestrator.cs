using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktGameEventOrchestrator
{
    private readonly GameStateEventListener _stateListener;
    private readonly GameDealerEventListener _gamedealerListner;

    public GesjaaktGameEventOrchestrator(
        GameStateEventListener gameStateListner,
        GameDealerEventListener gamedealerListner)
    {
        // TODO: simplify.., just log everything here. no sepearte listners. complex
        _stateListener = gameStateListner;
        _gamedealerListner = gamedealerListner;
    }

    public void Attach(IGesjaaktGameState state, IGesjaaktGameDealer dealer)
    {
        _stateListener.Subscribe(state);
        _gamedealerListner.Subscribe(dealer);
    }
}
