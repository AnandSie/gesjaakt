using Domain.Interfaces.Games.BaseGame;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveCardFactory : ICardFactory<TakeFiveCard>
{
    public TakeFiveCard Create(int cardValue)
    {
        int numberOfCowHeads = cardValue switch
        {
            // Double digit AND ends in 5 (e.g. 55)
            _ when cardValue % 10 == 5 && cardValue >= 10 && cardValue / 10 == cardValue % 10 => 7,

            // Ends in 5
            _ when cardValue % 10 == 5 => 2,

            // Ends in 0
            _ when cardValue % 10 == 0 => 3,

            // Double digit (e.g. 11, 22)
            _ when cardValue >= 10 && cardValue / 10 == cardValue % 10 => 5,

            // Standard
            _ => 1
        };

        return new TakeFiveCard(cardValue, numberOfCowHeads);
    }
}
