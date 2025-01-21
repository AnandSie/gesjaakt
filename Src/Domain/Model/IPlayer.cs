using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model;

internal interface IPlayer
{
    int CoinsAmount { get; }
    ICard[] Cards { get; }
}
