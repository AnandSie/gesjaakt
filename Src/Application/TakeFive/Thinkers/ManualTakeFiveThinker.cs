using Application.Interfaces;
using Domain.Entities.Game.TakeFive;
using Domain.Extensions;
using Domain.Interfaces.Games.TakeFive;
using System.Text;

namespace Application.TakeFive.Thinkers;

public class ManualTakeFiveThinker(IPlayerInputProvider playerInputProvider, string name) : BaseTakeFiveThinker
{
    public override string Name => name;

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        var question = new StringBuilder();
        question.AppendLine(gameState.ToString());
        question.AppendLine($"Hi {name}, which card from your hand do you want to play?");

        // REFACTOR - create extension method for this IEnumerable<TakeFiveCard> tostring
        string cardsInHand = string.Join(", ", this._hand.OrderBy(c => c.Value).Select(c => c.ToString()));
        question.Append(cardsInHand);

        var cardValues = this._hand.Select(c => c.Value);
        int choice = playerInputProvider.GetPlayerInputAsInt(question.ToString(), cardValues);

        var cardChoosen = _hand.Single(c => c.Value == choice);
        return cardChoosen;
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        var question = new StringBuilder();
        question.AppendLine(cardsOnTable.ToTableString());
        question.AppendLine($"Hi {name}, which row do you want to take?");

        var amountOfRows = cardsOnTable.Count();
        var options = Enumerable.Range(1, amountOfRows);
        int choice = playerInputProvider.GetPlayerInputAsInt(question.ToString(), options);
        return choice - 1; // Zero Index
    }
}
