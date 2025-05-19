using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class Jeremy2Thinker :IThinker
{
    private TurnAction previousTurnAction;
    private double shift = 0;
    private static double comfortableCoins = 4.5;
    public TurnAction Decide(IGameStateReader gameState)
    {
        shift = gameState.PlayerOnTurn.CoinsAmount - comfortableCoins;
        var tippingPoint = 15 - shift;

        if (IsGameAlmostEnding(gameState))
        {
            HandleGameEnd(gameState);
        }

        double nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable * 2.5;
        if (IsStraatje(gameState))
        {
            previousTurnAction = TurnAction.TAKECARD;
        }
        else if (nettoPenaltyPoints < tippingPoint)
        {
            previousTurnAction = TurnAction.TAKECARD;
        }
        else
        {
            previousTurnAction = TurnAction.SKIPWITHCOIN;
        }

        return previousTurnAction;
    }

    private bool IsStraatje(IGameStateReader gameState)
    {
        return gameState.PlayerOnTurn.Cards.Any(c => Math.Abs(c.Value - gameState.OpenCardValue) == 1);
    }

    private bool IsGameAlmostEnding(IGameStateReader gameState)
    {
        return gameState.Deck.AmountOfCardsLeft() == 2;
    }

    private void HandleGameEnd(IGameStateReader gameState)
    {
        gameNumber += 1;
        results[^1] += gameState.PlayerOnTurn.Points() / gameState.Players.Sum(x => x.Points());
        if (gameNumber % 100 == 0)
        {
            results.Append(0);
            double scoreChange = results[^1] - results[^2];
            comfortableCoins += -scoreChange / 1000 * 5;
        }
    }

    private double[] results = [];
    private int gameNumber;
}



    

