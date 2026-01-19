using Domain.Entities.Game.TakeFive;
using System.Text;

namespace Domain.Extensions;

public static class TakeFiveCardExtensions
{
    public static string ToTableString(this IEnumerable<IEnumerable<TakeFiveCard>> cardRows)
    {
        if (cardRows == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        int rowIndex = 1;

        foreach (var row in cardRows)
        {
            sb.Append($"Row {rowIndex}: ");
            sb.AppendLine(string.Join(", ", row.Select(c => c.ToString())));
            rowIndex++;
        }

        return sb.ToString();
    }
}
