namespace Domain.Interfaces;

public interface IGameDealer
{
    IGameStateReader State { get; }
    void Play();
    IPlayerState Winner();
}
