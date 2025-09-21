using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Thinkers

{

    public class JeremyThinker : IGesjaaktThinker

    {

        private GesjaaktTurnOption previousTurnAction;

        private double shift = 0;

        private double comfortableCoins = 4.5;

        private double coinRelativeValue = 2.5;

        private Random random = new Random();

        public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)

        {

            shift = gameState.PlayerOnTurn.CoinsAmount - comfortableCoins;

            var tippingPoint = 15 - shift + 3 * (random.NextDouble() - 0.5);

            //if(IsGameAlmostEnding(gameState))

            //{

            //    HandleGameEnd(gameState);

            //}

            double nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable * coinRelativeValue;

            if (IsStraatje(gameState))

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

        private bool IsStraatje(IGesjaaktReadOnlyGameState gameState)

        {

            return gameState.PlayerOnTurn.Cards.Any(c => Math.Abs(c.Value - gameState.OpenCardValue) == 1);

        }

        //private bool IsGameAlmostEnding(IGameStateReader gameState)

        //{

        //    return gameState.Deck.AmountOfCardsLeft() == 2;

        //}

        //private void HandleGameEnd(IGameStateReader gameState)

        //{

        //    gameNumber += 1;

        //    results[^1] += gameState.PlayerOnTurn.Points() / gameState.Players.Sum(x => x.Points());

        //    if (gameNumber % 100 == 0) 

        //    {

        //        results.Append(0);

        //        double scoreChange = results[^1] - results[^2];

        //        comfortableCoins += - scoreChange / 1000 * 40;

        //    }

        //}

        //private double[] results = [];

        //private int gameNumber;

    }

}

