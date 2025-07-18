namespace Domain.Interfaces;

// TODO: SPlit in standard and gesjaakt gamestate
public interface IGameState : IGameStateWriter, IGameStateReader
{
    public new IEnumerable<IPlayer> Players { get; }
}
