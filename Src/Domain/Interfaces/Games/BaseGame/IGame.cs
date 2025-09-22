using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Games.BaseGame;

public interface IGame
{

    // TODO: the implementation will probably make use of something like
    // IPlayerMenuChooser -> e.g. ConsolePlayerMenuChooser

    // AND the GameRunner also needs it to choose between these

    // TODO/NOTE: How is this different from "Simulator"?
    // => Simulator handles scoring and saving

    // TODO: in app, everything to setup a game in app and Simulator should be in the same factoryclass (which can have multiple options)

    public void Simulate();

    public void SimulateAllPossiblePlayerCombinations();

    public void RunManualGame(int amountOfPlayers);

    public void ShowStatistics();
}
