using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class BarryRealThinker : IThinker
{
    double coinsToPoints = 3;

    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        Random rnd = new Random();

        if (isStreet(gameState) && (gameState.Players.Select(p => p.CoinsAmount).Min() <= 2))
        {
            return GesjaaktTurnOption.TAKECARD;
        }


        double avgRichness = gameState.Players.Select(p => p.CoinsAmount).Sum() / gameState.Players.Count();
        double perceivedRichness = coinsToPoints * (-1 + gameState.PlayerOnTurn.CoinsAmount / avgRichness);
        double perceivedCoinValue = coinsToPoints * gameState.AmountOfCoinsOnTable;
        double dice = coinsToPoints * (rnd.NextDouble() - 0.5);
        double perceivedCardPenalty = gameState.OpenCardValue - coinsToPoints * valueStreet(gameState) + dice;

        if (perceivedCoinValue - perceivedRichness > perceivedCardPenalty)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
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

    private double valueStreet(IGameStateReader gameState)
    {
        int openCard = gameState.OpenCardValue;
        foreach (var card in gameState.PlayerOnTurn.Cards)
        {
            if (openCard == card.Value - 1)
            {
                return coinsToPoints;
            }
            else if (openCard == card.Value + 1)
            {
                return coinsToPoints - 1;
            }
        }
        return 0;
    }
}


