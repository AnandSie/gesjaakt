using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Thinkers;

public class MatsThinker_R3 : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var cards = gameState.PlayerOnTurn.Cards;
        var coins = gameState.PlayerOnTurn.CoinsAmount;
        var openCard = gameState.OpenCardValue;
        var coinsOnCard = gameState.AmountOfCoinsOnTable;
        var players = gameState.Players;
        var totalCoins = players.Sum(player => player.CoinsAmount) + coinsOnCard;

        if (coinsOnCard >= openCard)
        {
            return GesjaaktTurnOption.TAKECARD;
        }

        if (NewCardIsAdjacent(cards, openCard))
        {
            if (!AdjacentToOtherPlayers(gameState, openCard))
            {
                return WaitForCoins(coins, openCard, players);
            }
            else
            {
                return GesjaaktTurnOption.TAKECARD;
            }
        }

        return GetCardBasedOnRatio(openCard, coinsOnCard, coins, players.Count());
    }

    private static GesjaaktTurnOption WaitForCoins(int coins, int openCard, IEnumerable<IGesjaaktReadOnlyPlayer> players)
    {
        if (openCard < 3 * players.Count())
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            if (coins < players.Count() || (coins * 3) + players.Count() < openCard)
            {
                return GesjaaktTurnOption.SKIPWITHCOIN;
            }
            else
            {
                return GesjaaktTurnOption.TAKECARD;
            }
        }
    }

    private GesjaaktTurnOption GetCardBasedOnRatio(int openCard, int coinsOnCard, int coins, int playerCount)
    {
        if (coinsOnCard == 0)
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }

        var cardBonus = Math.Max(0, playerCount - coins);
        var adjustedCardValue = openCard - cardBonus;

        var ratio = adjustedCardValue / coinsOnCard;
        var ratioThreshold = 3;
        //if (coins > 5)
        //{
        //    ratioThreshold = 2;
        //}
        if (coins > 2 * playerCount)
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }

        if (ratio < ratioThreshold)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }

    private bool NewCardIsAdjacent(IReadOnlyCollection<ICard> cards, int openCard)
    {
        return cards.Any(card => Math.Abs(card.Value - openCard) == 1);
    }

    private bool AdjacentToOtherPlayers(IGesjaaktReadOnlyGameState gameState, int openCard)
    {
        var otherPlayers = gameState.Players
            .Where(player => player.Name != gameState.PlayerOnTurn.Name);

        return otherPlayers.Any(player => NewCardIsAdjacent(player.Cards, openCard));
    }
}