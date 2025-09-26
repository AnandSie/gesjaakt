namespace Domain.Interfaces.Games.BaseGame;

public interface IDecide<in TGameState, out TDecision>
{
    public TDecision Decide(TGameState gameState);
}
