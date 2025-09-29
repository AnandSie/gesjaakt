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
        var sb = new StringBuilder();

        int rowIndex = 1;
        foreach (var row in CardRows)
        {
            sb.Append($"Row {rowIndex}: ");
            sb.AppendLine(string.Join(", ", row.Select(c => c.ToString())));
            rowIndex++;
        }

        return sb.ToString();
    }
}
