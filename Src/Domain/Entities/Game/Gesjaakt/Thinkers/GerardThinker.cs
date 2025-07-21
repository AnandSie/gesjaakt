using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class GerardThinker : IThinker

{

    private GesjaaktTurnOption previousTurnAction;

    private double amountOfCoinsToComfortablySpend = 0;

    private static readonly Random random = new Random();

    private static readonly double comfortableCoins = 4;

    // private static readonly int numberOfplayers = 5;

    private static readonly int numberOfCoinsToStartWith = 11; // when there are 5 players


    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)

    {

        var defaultTippingPoint = 18;

        amountOfCoinsToComfortablySpend = gameState.PlayerOnTurn.CoinsAmount - comfortableCoins;

        var smartTippingPoint = defaultTippingPoint - amountOfCoinsToComfortablySpend;

        int negativePointsOnTable = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;

        if (IsStreet(gameState))

        {

            previousTurnAction = GesjaaktTurnOption.TAKECARD;

        }

        else if (negativePointsOnTable < smartTippingPoint)

        {

            previousTurnAction = GesjaaktTurnOption.TAKECARD;

        }

        else

        {

            previousTurnAction = GesjaaktTurnOption.SKIPWITHCOIN;

        }

        return previousTurnAction;

    }

    private bool IsStreet(IGesjaaktReadOnlyGameState gameState)

    {

        return gameState.PlayerOnTurn.Cards.Any(c => Math.Abs(c.Value - gameState.OpenCardValue) == 1);

    }

}

