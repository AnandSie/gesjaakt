using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model;

public interface IDeck
{
    int Min { get; }
    int Max { get; }
    void TakeOut(int amount);
    void DrawCard();
    bool IsEmpty();
}
