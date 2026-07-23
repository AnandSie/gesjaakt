using Domain.Entities.Game.Qwixx;

namespace Domain.Interfaces.Games.Qwixx;

public interface IQwixxDiceRoller
{
    // QX-008: rolls all 6 dice (2 white + 4 colored) at once.
    QwixxDiceRoll Roll();
}
