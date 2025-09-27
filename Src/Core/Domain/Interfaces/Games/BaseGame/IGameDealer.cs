namespace Domain.Interfaces.Games.BaseGame;

// REFACTOR - add IGameDealer (zonder generic) zodat we niet e.g. IGesjaaktGameDealer heoven te maken => houdt simpler
public interface IGameDealer<TPlayer> where TPlayer : INamed
{
    public void Add(IEnumerable<TPlayer> players);

    public void Prepare();
    public void Play();

    public IOrderedEnumerable<TPlayer> GetPlayerResults();
}
