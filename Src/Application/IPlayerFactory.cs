using Domain.Interfaces;

namespace Application;

public interface IPlayerFactory
{
    public IEnumerable<IPlayer> Create();
}