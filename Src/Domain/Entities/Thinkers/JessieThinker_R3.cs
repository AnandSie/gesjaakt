using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class JessieThinker_R3 : IThinker
{
    private GesjaaktTurnOption previousTurnAction;
    private double shift = 0;
    private static readonly double comfortableCoins = 15;
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        shift = gameState.PlayerOnTurn.CoinsAmount - comfortableCoins;
        var tippingPoint = 7 - shift;

        int nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
        if (IsStraatje(gameState) && otherHasAlmostStraat(gameState))
        {
            previousTurnAction = GesjaaktTurnOption.TAKECARD;
        }
        else if (nettoPenaltyPoints < tippingPoint)
        {
            previousTurnAction = GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            previousTurnAction = GesjaaktTurnOption.SKIPWITHCOIN;
        }

        return previousTurnAction;
    }

    private bool IsStraatje(IGameStateReader gameState)
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

    private bool otherHasAlmostStraat(IGameStateReader gameState)
    {
        foreach (var player in gameState.Players)
        {
            if (player != gameState.PlayerOnTurn)
            {
                foreach (var card in player.Cards)
                {
                    if (card.Value == gameState.OpenCardValue - 1 || card.Value == gameState.OpenCardValue + 1)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}