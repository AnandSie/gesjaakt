using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkers;

public class TomasThinker : IGesjaaktThinker
{
    private const int tippingPoint = 19;
    private const int rondjesTippingPoint = 1; 

    private bool isMelking = false;
    private int rondjes = 0;


    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var coinsStartAmount = gameState.Players.Select(p => p.CoinsAmount).Sum() / gameState.Players.Count();
        var myCoins = gameState.PlayerOnTurn.CoinsAmount;
        var perceivedCoinValue = 1.2 / Math.Sqrt(Math.Abs(coinsStartAmount - myCoins)) / coinsStartAmount;

        var perceivedCardValue = gameState.OpenCardValue - perceivedCoinValue * gameState.AmountOfCoinsOnTable;

        if (IsInStraatjeAndLangGenoegGemolken(gameState))
        {
            return GesjaaktTurnOption.TAKECARD;
        }

        if (perceivedCardValue < tippingPoint)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }

    private bool IsInStraatjeAndLangGenoegGemolken(IGesjaaktReadOnlyGameState gameState)
    {
        foreach (var card in gameState.PlayerOnTurn.Cards)
        {
            var diff = Math.Abs(card.Value - gameState.OpenCardValue);
            if (diff == 1)
            {
                if (isMelking && rondjes == rondjesTippingPoint)
                {
                    isMelking = false;
                    return true;
                }
                else
                {
                    rondjes += 1;
                    isMelking = true;
                }
            }
        }
        return false;
    }
}
