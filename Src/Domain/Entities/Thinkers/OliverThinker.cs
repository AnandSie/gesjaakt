using System.Runtime.CompilerServices;

using System.Security.Cryptography.X509Certificates;

using Domain.Entities.Game;

using Domain.Entities.Players;

using Domain.Interfaces;

public class OliverThinker : IThinker

{

    public TurnAction Decide(IGameStateReader gameState)

    {

        int test = 5;

        if (gameState.AmountOfCoinsOnTable > test && gameState.OpenCardValue < test * 4)

        {

            return TurnAction.TAKECARD;

        }

        else

        {

            return TurnAction.SKIPWITHCOIN;

        }

    }

}
