using Domain.Entities.Components;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveCard : Card
{
    public TakeFiveCard(int value, int cowHeads) : base(value)
    {
        CowHeads = cowHeads;
    }

    public int CowHeads { get; }

    public override string ToString()
    {
        // Example: "42(3)" => value=42, cowHeads=3
        return $"[{Value,2}(P{CowHeads})]";
    }
}
