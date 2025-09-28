using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt.Thinkers;

public class JorritThinker : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        const int TotalCards = 24;
        var perceivedCoinValue = gameState.AmountOfCoinsOnTable * 2.9875;
        if (IsStreetLower(gameState))
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else if (perceivedCoinValue * 100 > gameState.OpenCardValue * 100.0)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        //else if(IsStreetHigher(gameState) && ) { }
        else if (IsStreet(gameState) && gameState.AmountOfCoinsOnTable > 0)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
    private static bool IsStreet(IGesjaaktReadOnlyGameState gameState)
    {
        int openCard = gameState.OpenCardValue;
        foreach (var card in gameState.PlayerOnTurn.Cards)
        {
            if (openCard == card.Value - 1 || openCard == card.Value + 1)
            {
                return true;
            }
        }
        return false;
    }
    private static bool IsStreetLower(IGesjaaktReadOnlyGameState gameState)
    {
        int openCard = gameState.OpenCardValue;
        foreach (var card in gameState.PlayerOnTurn.Cards)
        {
            if (openCard == card.Value - 1)
            {
                return true;
            }
        }
        return false;
    }
    private static bool IsStreetHigher(IGesjaaktReadOnlyGameState gameState)
    {
        int openCard = gameState.OpenCardValue;
        foreach (var card in gameState.PlayerOnTurn.Cards)
        {
            if (openCard == card.Value + 1)
            {
                return true;
            }
        }
        return false;
    }
}





