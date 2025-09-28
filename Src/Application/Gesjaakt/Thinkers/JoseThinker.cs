using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using System;

namespace Application.Gesjaakt.Thinkers;

public class JoseThinker : IGesjaaktThinker
{
    private int[] _cardValues = { };
    private double limit = 10;
    private double max_coins = 6;
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        int card_value = gameState.OpenCardValue;
        int coins_on_table = gameState.AmountOfCoinsOnTable;
        double my_coins = gameState.PlayerOnTurn.CoinsAmount;
        int cards_left = gameState.Deck.AmountOfCardsLeft();
        if (gameState.PlayerOnTurn.Cards.Count == 0)
        {
            _cardValues = new int[0];
        }

        limit = (1 - (double)cards_left / 24) * (my_coins / max_coins);

        // YourThinker
        if (my_coins > 0 && EstimateCardValue(card_value, coins_on_table) < 20 * Math.Pow(limit, 0.1) - 10)
        {
            _cardValues.Append(card_value);
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }

    public int EstimateCardValue(int cardValue, int coinsOnTable)
    {
        // Card Value Evaluator
        if (Array.Find(_cardValues, x => x == cardValue - 1) != 0)
        {
            if (Array.Find(_cardValues, x => x == cardValue + 1) != 0)
            {
                return -coinsOnTable - cardValue - 1;
            }
            return -coinsOnTable;
        }
        else if (Array.Find(_cardValues, x => x == cardValue + 1) != 0)
        {
            return -coinsOnTable - 1;
        }


        return cardValue - coinsOnTable;
    }
}

