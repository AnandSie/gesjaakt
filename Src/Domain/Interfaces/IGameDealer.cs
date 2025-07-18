namespace Domain.Interfaces;

public interface IGameDealer
{
    public void Prepare();
    void Play();
    IPlayerState Winner();
    IEnumerable<IPlayer> GameResultOrdended();
}
