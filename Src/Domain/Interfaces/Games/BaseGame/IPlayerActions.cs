namespace Domain.Interfaces.Games.BaseGame;

public interface IPlayerActions<in TGameState, out TDecision> where TGameState: IReadOnlyGameState<INamed>
{
    public TDecision Decide(TGameState gameState);
}
