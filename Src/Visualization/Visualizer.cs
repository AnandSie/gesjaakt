﻿using Application.Interfaces;
using Domain.Entities.Thinkers;

namespace Visualization;

public class Visualizer
{
    private readonly IThinkerPlotter thinkerPlotter;

    public Visualizer()
    {
        // Note: Change with your thinker
        // Be aware: the thinker plotter only uses IGameStateReader.AmountOfCoinsOnTable and IGameStateReader.OpenCardValue. With current implemenation you can only asses those variables. So it is only for basis (parts) of your thinker algorithm
        var thinkerToTest = new AnandThinker(); 
        this.thinkerPlotter = new ThinkerPlotter(thinkerToTest);
    }

    public void Show()
    {
        this.thinkerPlotter.Plot();
    }
}
