using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Cards;

public class Deck : IDeck
{
    public int Min => throw new NotImplementedException();

    public int Max => throw new NotImplementedException();

    public void DrawCard()
    {
        throw new NotImplementedException();
    }

    public bool IsEmpty()
    {
        throw new NotImplementedException();
    }

    public void TakeOut(int amount)
    {
        throw new NotImplementedException();
    }
}
