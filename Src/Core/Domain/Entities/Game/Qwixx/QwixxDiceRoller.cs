using Domain.Entities.Components;
using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxDiceRollerTests.
public class QwixxDiceRoller : IQwixxDiceRoller
{
    private readonly Dice _die = new(QwixxRules.MinDieValue, QwixxRules.MaxDieValue);

    public QwixxDiceRoll Roll()
    {
        return new QwixxDiceRoll(_die.Roll(), _die.Roll(), _die.Roll(), _die.Roll(), _die.Roll(), _die.Roll());
    }
}
