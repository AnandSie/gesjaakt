using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers
{
    public class JeremyThinker : IThinker
    {
        private TurnAction previousTurnAction;
        private double shift = 0;
        private static readonly Random random = new Random();
        private static readonly double comfortableCoins = 3;
        public TurnAction Decide(IGameStateReader gameState)
        {
            shift = gameState.PlayerOnTurn.CoinsAmount - comfortableCoins;
            var tippingPoint = 15 - shift;

            int nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
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
    }
}
