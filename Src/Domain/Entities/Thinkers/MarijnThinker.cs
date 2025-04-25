using Domain.Entities.Cards;
using Domain.Entities.Game;
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

        var thresholdMyCoins = 5; //coins
        var minTableThreshold = 5;
        var maxTableThreshold = 15;
        var dealT1 = 5;
        var dealT2 = 10;

        var wMyCoins = 0.08; // 0.34;
        var wCoinsOnTable = 0.41;
        var wDealStrength = 0.59;

        var pMyCoins = myCoins < thresholdMyCoins ? (1 / Math.Pow(thresholdMyCoins, 2)) * Math.Pow(myCoins - thresholdMyCoins, 2) : 0;
        var pCoinsOnTable = tableCoins < minTableThreshold ? 0 : tableCoins > maxTableThreshold ? 1 : tableCoins - 5;
        var pDealStrength = dealStrength <= dealT1 ? 1 : dealStrength > dealT2 ? 0 : (1/(dealT1-dealT2))*dealStrength -(dealT2/(dealT1-dealT2));

        var pTotal = pCoinsOnTable;// Math.Pow((wMyCoins * pMyCoins) * (wCoinsOnTable * pCoinsOnTable) * (wDealStrength * pDealStrength), 1 / 3.0);

        //Wins from ananad    //Math.Pow((wMyCoins * pMyCoins) * (wCoinsOnTable * pCoinsOnTable) * (wDealStrength * pDealStrength), 1 / 3.0);

        //pMyCoins + pCoinsOnTable + pDealStrength; // wins from local maarten

        //Math.Pow((wMyCoins * pMyCoins) * (wCoinsOnTable* pCoinsOnTable) * (wDealStrength* pDealStrength), 1 / 3.0);

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