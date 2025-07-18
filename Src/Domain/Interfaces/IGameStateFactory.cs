namespace Domain.Interfaces;

public interface IGameStateFactory
{
    IGameState Create(IEnumerable<IPlayer> players);
}
