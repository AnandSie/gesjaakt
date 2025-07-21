using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Interfaces;

public interface IGameDealer
{
    public void Prepare();
    void Play();
    IGesjaaktPlayerState Winner();
    IEnumerable<IGesjaaktPlayerState> GameResultOrdended();
}
