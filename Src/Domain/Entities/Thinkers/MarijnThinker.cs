using Domain.Entities.Cards;
using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class MarijnThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        //var random = new Random();
        //if (random.NextDouble() < 0.90)
        //{
        //    return TurnAction.SKIPWITHCOIN;
        //}
        //else
        //{
        //    return TurnAction.TAKECARD;
        //}

        //var perceivedCoinValue = gameState.AmountOfCoinsOnTable * 3;
        //card.Value - gameState.OpenCardValue
        var myCoins = gameState.PlayerOnTurn.CoinsAmount;
        var tableCoins = gameState.AmountOfCoinsOnTable;
        var dealStrength = gameState.OpenCardValue - tableCoins;
        //perceivedCoinValue 
        //myCoins +
        var thresholdMyCoins = 10.0; //coins
        var minTableThreshold = 5;
        var maxTableThreshold = 15;
        var minDealTreshold = 15;

        var wMyCoins = 0.34;
        var wCoinsOnTable = 0.33;
        var wDealStrength = 0.33;

        var pMyCoins = myCoins < thresholdMyCoins ? (1 / Math.Pow(thresholdMyCoins, 2)) * Math.Pow(myCoins - thresholdMyCoins, 2) : 0;
        var pCoinsOnTable = tableCoins < minTableThreshold ? 0 : tableCoins > maxTableThreshold ? 1 : tableCoins - 5;
        var pDealStrength = dealStrength <= 0 ? 1 : dealStrength > minDealTreshold ? 0 : (-1 / minDealTreshold) * dealStrength + 1;

        var pTotal = Math.Pow((wMyCoins * pMyCoins) * (wCoinsOnTable* pCoinsOnTable) * (wDealStrength* pDealStrength), 1 / 3.0);

        var random = new Random();
        if (random.NextDouble() < pTotal)
        {
            return TurnAction.SKIPWITHCOIN;
        }
        else
        {
            return TurnAction.TAKECARD;
        }
    }
}