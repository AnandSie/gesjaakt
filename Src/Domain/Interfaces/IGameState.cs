namespace Domain.Interfaces;

public interface IGameState : IGameStateWriter, IGameStateReader
{
    IEnumerable<IPlayer> Players { get; }
}
