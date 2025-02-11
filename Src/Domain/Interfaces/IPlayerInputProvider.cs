using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IPlayerInputProvider
{
    string GetPlayerInput(string question);
    // TODO: Another method where you can give an enum
    int GetPlayerInputAsInt(string question, IEnumerable<int> allowedInts);
    int GetPlayerInputAsInt(IEnumerable<int> allowedInts);
    bool GetPlayerInputForYesNo();
}
