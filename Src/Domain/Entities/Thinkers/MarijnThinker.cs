using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class MarijnThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        var myCoins = gameState.PlayerOnTurn.CoinsAmount;
        var tableCoins = gameState.AmountOfCoinsOnTable;
        var dealStrength = gameState.OpenCardValue - tableCoins;

        var thresholdMyCoins = 5; 
        var tableThreshold1 = 5;
        var tableThreshold2 = 10;
        var dealT1 = 0;
        var dealT2 = 15;

        var wMyCoins = 0.05; 
        var wCoinsOnTable = 0.90;
        var wDealStrength = 0.05;

        var pMyCoins = myCoins < thresholdMyCoins ? (1 / Math.Pow(thresholdMyCoins, 2)) * Math.Pow(myCoins - thresholdMyCoins, 2) : 0;
        var pCoinsOnTable = tableCoins < tableThreshold2 ? 0 : tableCoins > tableThreshold1 ? 1 : (1 / (tableThreshold2 - tableThreshold1)) * dealStrength - (tableThreshold1 / (tableThreshold2 - tableThreshold1));  //tableCoins - 5;
        var pDealStrength = dealStrength <= dealT1 ? 1 : dealStrength > dealT2 ? 0 : (1/(dealT1-dealT2))*dealStrength -(dealT2/(dealT1-dealT2));

        var pTotal = (wMyCoins * pMyCoins) + (wCoinsOnTable * pCoinsOnTable) + (wDealStrength * pDealStrength);

        var random = new Random();
        if (random.NextDouble() < pTotal)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }
    }
}