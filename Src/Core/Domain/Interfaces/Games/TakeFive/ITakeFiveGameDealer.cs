using Domain.Entities.Events;
using Domain.Interfaces.Games.BaseGame;
namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveGameDealer : IGameDealer<ITakeFivePlayer>
{
}
