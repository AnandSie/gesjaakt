using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Thinkers;

public class RubenTHinker : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        IReadOnlyCollection<ICard> cards = gameState.PlayerOnTurn.Cards;
        int[] cardArray = cards.Select(card => card.Value).ToArray();
        int coinsOnTable = gameState.AmountOfCoinsOnTable;
        int currentCardValue = gameState.OpenCardValue;
        int counsInPocket = gameState.PlayerOnTurn.CoinsAmount;
        double startDeckAmount = 24;
        double coinsWeight = 2.2;
        double cardsLeft = gameState.Deck.AmountOfCardsLeft();
        coinsWeight -= (startDeckAmount - cardsLeft) / startDeckAmount;
        coinsWeight -= gameState.PlayerOnTurn.CoinsAmount / 15;
        double coinsOnTableValue = coinsOnTable * coinsWeight;
        int dealValue = -coinsOnTable;

        if (!IsSequence(currentCardValue, cards))
        {
            dealValue += currentCardValue;
        }

        if (dealValue < 4)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else if (coinsOnTable < 2)
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
        else if (coinsOnTableValue > dealValue)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
    private static bool IsSequence(int currentCardValue, IReadOnlyCollection<ICard> cards)
    {
        foreach (var card in cards)
        {
            if ((currentCardValue == card.Value - 1) || (currentCardValue == card.Value + 1))
            {
                return true;
            }
        }
        return false;
    }

}