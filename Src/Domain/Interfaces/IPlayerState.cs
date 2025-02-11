using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IPlayerState
{
    string Name { get; }
    int CoinsAmount { get; }
    ICollection<ICard> Cards { get; }
    public int CardPoints();
}
