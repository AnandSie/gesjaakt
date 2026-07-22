using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

public class QwixxReadOnlyPlayer : IQwixxReadOnlyPlayer
{
    private readonly IQwixxPlayer _player;

    public QwixxReadOnlyPlayer(IQwixxPlayer player)
    {
        _player = player;
    }

    public string Name => _player.Name;

    public int Score => _player.Score;

    public int Penalties => _player.Penalties;

    public bool HasMaxPenalties => _player.HasMaxPenalties;

    public int MarkedCount(QwixxColor color) => _player.Row(color).MarkedCount;

    public bool IsRowLocked(QwixxColor color) => _player.Row(color).IsLocked;

    public bool CanMark(QwixxColor color, int number) => _player.Row(color).CanMark(number);
}
