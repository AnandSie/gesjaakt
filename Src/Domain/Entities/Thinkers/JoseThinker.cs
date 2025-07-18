using Domain.Entities.Players;
using Domain.Interfaces;
using System;

namespace Domain.Entities.Thinkers;

public class JoseThinker : IThinker
{
    private int[] _cardValues = { };
    private double limit = 10;
    private double max_coins = 6;
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        int card_value = gameState.OpenCardValue;
        int coins_on_table = gameState.AmountOfCoinsOnTable;
        double my_coins = (double)gameState.PlayerOnTurn.CoinsAmount;
        int cards_left = gameState.Deck.AmountOfCardsLeft();
        if (gameState.PlayerOnTurn.Cards.Count == 0)
        {
            this._cardValues = new int[0];
        }

        limit = (1 - (((double)cards_left) / 24)) * (my_coins / max_coins);

        // YourThinker
        if (my_coins > 0 && this.EstimateCardValue(card_value, coins_on_table) < 20 * Math.Pow(limit, 0.1) - 10)
        {
            this._cardValues.Append(card_value);
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
        if (Array.Find(this._cardValues, x => x == cardValue - 1) != 0)
        {
            if (Array.Find(this._cardValues, x => x == cardValue + 1) != 0)
            {
                return -coinsOnTable - cardValue - 1;
            }
            return -coinsOnTable;
        }
        else if (Array.Find(this._cardValues, x => x == cardValue + 1) != 0)
        {
            return -coinsOnTable - 1;
        }


        return cardValue - coinsOnTable;
    }
}

