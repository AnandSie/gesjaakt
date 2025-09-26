namespace Domain.Interfaces.Games.BaseGame;

public interface IGameDealer<TPlayer> where TPlayer : INamed
{
    public void Add(IEnumerable<TPlayer> players);

    public void Prepare();
    public void Play();

    public IOrderedEnumerable<TPlayer> GetPlayerResults();
}
