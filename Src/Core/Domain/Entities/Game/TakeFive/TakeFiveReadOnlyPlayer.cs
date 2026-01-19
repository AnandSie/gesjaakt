using Domain.Interfaces.Games.TakeFive;
using System.Collections.Immutable;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveReadOnlyPlayer : ITakeFiveReadOnlyPlayer
{
    private readonly ITakeFivePlayer player;

    public TakeFiveReadOnlyPlayer(ITakeFivePlayer player)
    {
        this.player = player;
    }

    public string Name => player.Name;

    // REFACTOR: how can we create an Immutable list..?
    public IReadOnlyCollection<TakeFiveCard> PenaltyCards => player.PenaltyCards.ToImmutableList();

    public int CardsCount => this.player.CardsCount;
}