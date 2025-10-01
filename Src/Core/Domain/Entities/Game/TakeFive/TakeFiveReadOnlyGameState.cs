using Domain.Extensions;
using Domain.Interfaces.Games.TakeFive;
using System.Text;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveReadOnlyGameState(TakeFiveGameState gameState) : ITakeFiveReadOnlyGameState
{
    public IEnumerable<IEnumerable<TakeFiveCard>> CardRows => gameState.CardRows;

    // REFACTOR: cache -> move from method to local readonly
    public IEnumerable<ITakeFiveReadOnlyPlayer> Players => gameState.Players.Select(p => p.AsReadOnly());

    public override string ToString()
    {
        return CardRows.ToTableString();
    }
}
