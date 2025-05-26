using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

namespace Domain.Entities.Thinkers;

public class HansThinker_R3 : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        var me = gameState.PlayerOnTurn;
        var openCardValue = gameState.OpenCardValue;
        if (BenIkDeSjaak(gameState, me))
        {
            if (SchadePuntenVoorDezeSpeler(me, gameState.OpenCardValue) < 6)
            {
                return TurnAction.TAKECARD;
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
                return TurnAction.TAKECARD;
            }
        }

        return TurnAction.SKIPWITHCOIN;
    }

    private bool BenIkDeSjaak(IGameStateReader gameState, IPlayerState me)
    {
        var coinsLow = gameState.Players.Min(i => i.CoinsAmount);
        return (me.CoinsAmount == coinsLow);
    }

    private int SchadePuntenVoorDezeSpeler(IPlayerState player, int openCardValue)
    {
        if (player.Cards.Where(i => ((i.Value - 1) == openCardValue)).Any()) return -1;
        if (player.Cards.Where(i => ((i.Value + 1) == openCardValue)).Any()) return 0;
        return openCardValue;
    }
}
