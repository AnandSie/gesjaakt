using Domain.Interfaces.Components;
using Domain.Interfaces.Games.Gesjaakt;
using System.Collections.Immutable;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktReadOnlyPlayer : IGesjaaktReadOnlyPlayer
{
    // FIXME: Cache the player attributes to avoid reflection

    private IGesjaaktPlayer _player;

    public GesjaaktReadOnlyPlayer(IGesjaaktPlayer player)
    {
        this._player = player;
    }

    public int CoinsAmount => this._player.CoinsAmount;

    public IReadOnlyCollection<ICard> Cards => this._player.Cards.ToImmutableList();

    public string Name => this._player.Name;

    public int CardPoints() => this._player.CardPoints();

    public int Points() => this._player.Points();
}
