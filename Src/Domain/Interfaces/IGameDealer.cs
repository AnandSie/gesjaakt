using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Interfaces;

public interface IGameDealer<out TPlayer> where TPlayer: INamed
{
    public void Prepare();
    public void Play();
    public TPlayer Winner();
    public IEnumerable<TPlayer> GameResultOrdended();
}
