using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class BarryThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        if (isStreet(gameState))
        {
            return TurnAction.TAKECARD;
        }

        if ((gameState.PlayerOnTurn.Cards.Count == 0) && (gameState.PlayerOnTurn.CoinsAmount <= 1))
        {
            return TurnAction.TAKECARD;
        }
        else
            {
                return TurnAction.SKIPWITHCOIN;
            }
    }
    private bool isStreet(IGameStateReader gameState)
    {
        int openCard = gameState.OpenCardValue;
        foreach (var card in gameState.PlayerOnTurn.Cards)
            {
                if ((openCard == card.Value - 1) || (openCard == card.Value + 1))
                    {
                        return true;
                    }
            }
        return false;
    }
}