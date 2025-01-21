using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Game;

public  interface IGameState: IGameStateWriter, IGameStateReader
{
}
