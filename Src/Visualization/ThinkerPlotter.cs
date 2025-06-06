﻿using Domain.Interfaces;
using ScottPlot;
using Domain.Entities.Players;
using System.Diagnostics;
using Application.Interfaces;

namespace Visualization;

public class ThinkerPlotter(IThinker thinker): IThinkerPlotter
{
    private readonly IThinker thinker = thinker;

    public void Plot()
    {
        var cards = Enumerable.Range(3, 35 - 2); // Cards range from 3 till 35
        var coinsOnTable = Enumerable.Range(0, 30);

        var decisions = SimulateDecisions(cards, coinsOnTable);
        (var takeCardsXs, var takeCardsYs, var skipCoinsXs, var skipCoinsYs) = CreateCoordinates(cards, coinsOnTable, decisions);
        Plot(takeCardsXs, takeCardsYs, skipCoinsXs, skipCoinsYs);
    }

    private static void Plot(List<double> takeCardsXs, List<double> takeCardsYs, List<double> skipCoinsXs, List<double> skipCoinsYs)
    {
        Plot plt = new();
        var scatTakeCard = plt.Add.Scatter(
            takeCardsXs.ToArray(),
            takeCardsYs.ToArray(),
            color: Color.FromColor(System.Drawing.Color.Blue)
            );
        scatTakeCard.LineWidth = 0;
        scatTakeCard.LegendText = "Take Card";

        var scatSkipCoin = plt.Add.Scatter(
            skipCoinsXs.ToArray(),
            skipCoinsYs.ToArray(),
            color: Color.FromColor(System.Drawing.Color.Red)
            );
        scatSkipCoin.LegendText = "Skip With Coin";

        scatSkipCoin.LineWidth = 0;

        plt.Add.Legend();

        plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(1);
        plt.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(1);

        plt.XLabel("Card Value");
        plt.YLabel("Coins on Table");
        plt.Title("Decision Map");

        string path = "thinker.png";
        plt.SavePng(path, 800, 600);
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    private static (List<double> takeCardsXs, List<double> takeCardsYs, List<double> skipCoinsXs, List<double> skipCoinsYs) CreateCoordinates(IEnumerable<int> cards, IEnumerable<int> coinsOnTable, TurnAction[,] decisions)
    {
        var takeCardsXs = new List<double>();
        var takeCardsYs = new List<double>();
        var skipCoinsXs = new List<double>();
        var skipCoinsYs = new List<double>();

        int rows = decisions.GetLength(0);
        int cols = decisions.GetLength(1);

        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                double cardValue = cards.ElementAt(x);
                double coinCount = coinsOnTable.ElementAt(y);

                if (decisions[x, y] == TurnAction.TAKECARD)
                {
                    takeCardsXs.Add(cardValue);
                    takeCardsYs.Add(coinCount);
                }
                else if (decisions[x, y] == TurnAction.SKIPWITHCOIN)
                {
                    skipCoinsXs.Add(cardValue);
                    skipCoinsYs.Add(coinCount);
                }
            }
        }
        return (takeCardsXs, takeCardsYs, skipCoinsXs, skipCoinsYs);
    }

    private TurnAction[,] SimulateDecisions(IEnumerable<int> cards, IEnumerable<int> coinsOnTable)
    {

        var decisions = new TurnAction[cards.Count(), coinsOnTable.Count()];

        for (int x = 0; x < cards.Count(); x++)
        {
            for (int y = 0; y < coinsOnTable.Count(); y++)
            {
                var state = new TestGameState()
                {
                    AmountOfCoinsOnTable = coinsOnTable.ElementAt(y),
                    OpenCardValue = cards.ElementAt(x)
                };
                decisions[x, y] = thinker.Decide(state);
            }
        }

        return decisions;
    }

    private class TestGameState : IGameStateReader
    {
        public IEnumerable<IPlayerState> Players => throw new NotImplementedException();
        public IPlayerState PlayerOnTurn => throw new NotImplementedException();
        public int OpenCardValue { get; set; }
        public IDeckState Deck => throw new NotImplementedException();
        public int AmountOfCoinsOnTable { get; set; }
        public bool HasOpenCard => throw new NotImplementedException();
    }
}
