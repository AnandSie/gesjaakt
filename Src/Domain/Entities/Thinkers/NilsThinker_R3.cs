using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class NilsThinker_R3 : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        var deckOfCards = 24;
        var cardsLeft = gameState.Deck.AmountOfCardsLeft();
        float progression = (float)cardsLeft / (float)deckOfCards;

        if (freeCoins(gameState))
        {
            return TurnAction.TAKECARD;
        }

        if (gameState.PlayerOnTurn.Cards.Count == 0 && AnchorCard(gameState))
        {
            //Console.WriteLine("Anchor " + gameState.OpenCardValue);
            return TurnAction.TAKECARD;
        }

        if (IsStreet(gameState))
        {
            //Console.WriteLine("Street " + gameState.OpenCardValue);
            if (pushGreed(gameState))
            {
                //Console.WriteLine(" value " + (gameState.OpenCardValue - gameState.AmountOfCoinsOnTable + gameState.Players.Count()).ToString());
                return TurnAction.TAKECARD;
            }
        }

        if (IsStreetPotential(gameState, 2) && progression < 0.3 && gameState.PlayerOnTurn.Cards.Count < 3)
        {
            //Console.WriteLine("Street Potential" + gameState.OpenCardValue);
            return TurnAction.TAKECARD;
        }


        if (lowValue(gameState, 7) && gameState.AmountOfCoinsOnTable < 4 && gameState.PlayerOnTurn.CoinsAmount < 3)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }
    }

    public bool lowValue(IGameStateReader gameState, int value)
    {
        var ga = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
        return ga < value;
    }

    public bool freeCoins(IGameStateReader gameState)
    {
        var value = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
        var result = value < 0;
        return value < 0;
    }

    private static bool IsStreet(IGameStateReader gameState)
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

    private static bool IsStreetPotential(IGameStateReader gameState, int cardsBetweenAllowed)
    {
        foreach (var player in gameState.Players)
        {
            if (player == gameState.PlayerOnTurn)
            {
                break;
            }
            foreach (var existingCard in player.Cards)
            {
                if (Math.Abs(gameState.OpenCardValue - existingCard.Value) < cardsBetweenAllowed)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CardPlayed(IGameStateReader gameState, int cardValue)
    {
        foreach (var player in gameState.Players)
        {
            if (player == gameState.PlayerOnTurn)
            {
                break;
            }
            foreach (var existingCard in player.Cards)
            {
                if (existingCard.Value == cardValue)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool AnchorCard(IGameStateReader gameState)
    {
        if (gameState.OpenCardValue > 15 && gameState.OpenCardValue < 25 &&
            (!CardPlayed(gameState, gameState.OpenCardValue) && !(CardPlayed(gameState, gameState.OpenCardValue)))
&& gameState.AmountOfCoinsOnTable >= gameState.Players.Count() - 2)
        {
            return true;
        }
        return false;
    }

    public bool pushGreed(IGameStateReader gameState)
    {
        //not a lot of likelihood someone takes
        if (gameState.OpenCardValue - gameState.AmountOfCoinsOnTable + gameState.Players.Count() > 10)
        {
            return true;

        }
        return false;
    }
}