using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class MatsThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        var cards = gameState.PlayerOnTurn.Cards;
        var coins = gameState.PlayerOnTurn.CoinsAmount;
        var openCard = gameState.OpenCardValue;
        var coinsOnCard = gameState.AmountOfCoinsOnTable;
        var players = gameState.Players;

        if (coinsOnCard >= openCard)
        {
            return TurnAction.TAKECARD;
        }

        if (NewCardIsAdjacent(cards, openCard))
        {
            if (!AdjacentToOtherPlayers(gameState, openCard))
            {
                return WaitForCoins(coins, openCard, players);
            }
            else
            {
                return TurnAction.TAKECARD;
            }
        }

        return GetCardBasedOnRatio(openCard, coinsOnCard, coins, players.Count());
    }

    private static TurnAction WaitForCoins(int coins, int openCard, IEnumerable<IPlayerState> players)
    {
        if (openCard < players.Count() + 7)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            if (coins < players.Count())
            {
                return TurnAction.SKIPWITHCOIN;
            }
            else
            {
                return TurnAction.TAKECARD;
            }
        }
    }

    private TurnAction GetCardBasedOnRatio(int openCard, int coinsOnCard, int coins, int playerCount)
    {
        if (coinsOnCard == 0)
        {
            return TurnAction.SKIPWITHCOIN;
        }

        var cardBonus = Math.Max(0, playerCount - coins);

        var ratio = (openCard - 2 * cardBonus) / coinsOnCard;

        if (openCard < 16)
        {
            if (ratio <= 1)
            {
                return TurnAction.TAKECARD;
            }
            else
            {
                return TurnAction.SKIPWITHCOIN;
            }
        }
        else
        {
            if (ratio <= 2)
            {
                return TurnAction.TAKECARD;
            }
            else
            {
                return TurnAction.SKIPWITHCOIN;
            }
        }
    }

    private bool NewCardIsAdjacent(ICollection<ICard> cards, int openCard)
    {
        return cards.Any(card => Math.Abs(card.Value - openCard) == 1);
    }

    private bool AdjacentToOtherPlayers(IGameStateReader gameState, int openCard)
    {
        var otherPlayers = gameState.Players
            .Where(player => player.Name != gameState.PlayerOnTurn.Name);

        return otherPlayers.Any(player => NewCardIsAdjacent(player.Cards, openCard));
    }
}


