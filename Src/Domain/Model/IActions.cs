using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model;

internal interface IActions
{
    void TakeCard();
    void GiveCoin();
}
