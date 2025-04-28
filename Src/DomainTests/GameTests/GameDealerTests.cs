using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;
using System.Numerics;
using Domain.Entities.Thinkers;

namespace DomainTests.GameTests;

[TestClass]
public class GameDealerTests
{
    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    public void AddPlayerTest()
    {
        var scaredPlayer = new Player(new ScaredThinker());

        var sut = new GameDealer(new[] { scaredPlayer });

        var expectedResult = new List<IPlayerActions> { scaredPlayer };
        sut.State.Players.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void PlaceCoinAction()
    {
        var scaredPlayer = new Player(new ScaredThinker());
        scaredPlayer.AcceptCoins(new List<Coin>{ new() });
        var sut = new GameDealer(new[] { scaredPlayer });
        sut.PlayFirstCard();
        sut.PlayTurn();

        sut.State.AmountOfCoinsOnTable.Should().Be(1);
        scaredPlayer.CoinsAmount.Should().Be(0);
    }

    [TestMethod]
    public void TakeCardAction()
    {
        var scaredPlayer = new Player(new ScaredThinker(), "Donald");
        scaredPlayer.AcceptCoins(new List<Coin> { new() });
        var greedyPlayer = new Player(new GreedyThinker(), "Dagobert");
        greedyPlayer.AcceptCoins(new List<Coin> { new() });

        var sut = new GameDealer(new[] { scaredPlayer, greedyPlayer });
        sut.PlayFirstCard();
        sut.State.PlayerOnTurn.Name.Should().Be("Donald");
        sut.PlayTurn();
        sut.NextPlayer();
        sut.State.PlayerOnTurn.Name.Should().Be("Dagobert");
        sut.PlayTurn();
        sut.State.AmountOfCoinsOnTable.Should().Be(1);

        scaredPlayer.CoinsAmount.Should().Be(0);
        greedyPlayer.CoinsAmount.Should().Be(1);
        scaredPlayer.Cards.Count.Should().Be(0);
        greedyPlayer.Cards.Count.Should().Be(1);
    }

    [TestMethod]
    public void GreedyPlayerTakesAllCards_LosesGameTest()
    {
        var scaredPlayer1 = new Player(new ScaredThinker());
        var greedyPlayer = new Player(new GreedyThinker());
        var scaredPlayer2 = new Player(new ScaredThinker());
        var sut = new GameDealer(new Player[] { scaredPlayer1, greedyPlayer, scaredPlayer2 });

        sut.Play();
        var winner = sut.Winner();

        var ranking = sut.State.Players.OrderBy(p => p.Points()).ToList();

        winner.Should().Be(ranking.First());
        greedyPlayer.Should().Be(ranking.Last());
    }
}