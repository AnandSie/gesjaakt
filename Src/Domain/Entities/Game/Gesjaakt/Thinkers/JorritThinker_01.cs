using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Thinkers
{
    public class JorritThinker_01 : IGesjaaktThinker
    {
        static int turnCount = 0;
        const int TotalCards = 24;
        public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
        {
            turnCount++;
            int CardsLeft = TotalCards - GetPlayedCards(gameState);

            var perceivedCoinValue = gameState.AmountOfCoinsOnTable * 2.9875 + (((double)CardsLeft / (double)TotalCards) * 0.55);
            //Console.WriteLine($"{CardsLeft} {TotalCards} {((double)CardsLeft / (double)TotalCards)} perceivedCoinValue: {perceivedCoinValue}");
            //Console.WriteLine($"perceivedCoinValue: {perceivedCoinValue}");
            if (IsStreetLower(gameState))
            {
                return GesjaaktTurnOption.TAKECARD;
            }
            else if (perceivedCoinValue > gameState.OpenCardValue)
            {
                return GesjaaktTurnOption.TAKECARD;
            }
            //else if (IsStreetHigher(gameState) && turnCount < 2)
            //{
            //    return TurnAction.TAKECARD;
            //}
            //else if (IsStreetHigher(gameState) && gameState.AmountOfCoinsOnTable > 1)
            //{
            //    return TurnAction.TAKECARD;
            //}
            else if (IsStreet(gameState) && gameState.AmountOfCoinsOnTable > 0)
            {
                return GesjaaktTurnOption.TAKECARD;
            }
            else
            {
                //if(IsStreet(gameState) && gameState.AmountOfCoinsOnTable > 0) {
                //    return TurnAction.TAKECARD;
                //}
                return GesjaaktTurnOption.SKIPWITHCOIN;
            }
        }

        private static int GetPlayedCards(IGesjaaktReadOnlyGameState gameState)
        {
            int cardCount = 0;
            foreach (var player in gameState.Players)
            {
                cardCount += player.Cards.Count;
            }
            return cardCount;
        }
        private static bool IsStreet(IGesjaaktReadOnlyGameState gameState)
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
        private static bool IsStreetLower(IGesjaaktReadOnlyGameState gameState)
        {
            int openCard = gameState.OpenCardValue;
            foreach (var card in gameState.PlayerOnTurn.Cards)
            {
                if (openCard == card.Value - 1)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsStreetHigher(IGesjaaktReadOnlyGameState gameState)
        {
            int openCard = gameState.OpenCardValue;
            foreach (var card in gameState.PlayerOnTurn.Cards)
            {
                if (openCard == card.Value + 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
