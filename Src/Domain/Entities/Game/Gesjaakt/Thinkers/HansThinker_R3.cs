using Domain.Entities.Game;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

namespace Domain.Entities.Thinkers;

public class HansThinker_R3 : IThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var me = gameState.PlayerOnTurn;
        var openCardValue = gameState.OpenCardValue;
        if (BenIkDeSjaak(gameState, me))
        {
            if (SchadePuntenVoorDezeSpeler(me, gameState.OpenCardValue) < 6)
            {
                return GesjaaktTurnOption.TAKECARD;
            }
            //else
            //{
            //    return TurnAction.SKIPWITHCOIN;
            //}
        }

        //var minVal = gameState.Players.Min(i => SchadePuntenVoorDezeSpeler(i, openCardValue));
        //var maxVal = gameState.Players.Max(i => SchadePuntenVoorDezeSpeler(i, openCardValue));
        //if (SchadePuntenVoorDezeSpeler(me, openCardValue) == maxVal)
        //{
        //    return TurnAction.SKIPWITHCOIN;
        //}
        //else if (SchadePuntenVoorDezeSpeler(me, openCardValue) == minVal)
        //{
        //    return TurnAction.TAKECARD;
        //}

        if (gameState.AmountOfCoinsOnTable >= (gameState.Players.Count()))
        {
            if ((gameState.OpenCardValue - gameState.AmountOfCoinsOnTable) < 20)
            {
                return GesjaaktTurnOption.TAKECARD;
            }
        }

        return GesjaaktTurnOption.SKIPWITHCOIN;
    }

    private bool BenIkDeSjaak(IGesjaaktReadOnlyGameState gameState, IGesjaaktPlayerState me)
    {
        var coinsLow = gameState.Players.Min(i => i.CoinsAmount);
        return (me.CoinsAmount == coinsLow);
    }

    private int SchadePuntenVoorDezeSpeler(IGesjaaktPlayerState player, int openCardValue)
    {
        if (player.Cards.Where(i => ((i.Value - 1) == openCardValue)).Any()) return -1;
        if (player.Cards.Where(i => ((i.Value + 1) == openCardValue)).Any()) return 0;
        return openCardValue;
    }
}
