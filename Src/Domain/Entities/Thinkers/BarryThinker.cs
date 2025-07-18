using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class BarryThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        if (IsStreet(gameState))
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        if ((gameState.PlayerOnTurn.Cards.Count == 0) && (gameState.PlayerOnTurn.CoinsAmount <= 1))
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
    private static bool IsStreet(IGameStateReader gameState)
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