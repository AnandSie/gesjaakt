using Domain.Cards;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Domain.Game;
using Domain.Players;

namespace DomainTests.GameTests;

[TestClass]
public class GameDealerTests
{
    IGameDealer sut;

    [TestInitialize]
    public void Setup()
    {
        sut = new GameDealer();
    }

    [TestMethod]
    public void AddPlayerTest()
    {
        var player = new ScaredPlayer();

        sut.AddPlayer(player);

        var expectedResult = new List<IPlayer> { player };
        sut.State.Players.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void EqualCoinsPerPlayer()
    {
        var player1 = new ScaredPlayer();
        var player2 = new ScaredPlayer();
        sut.AddPlayer(player1);
        sut.AddPlayer(player2);

        var coinsAmount = 10;
        var amountOfPlayers = 2;

        sut.DivideCoins(coinsAmount);

        sut.State.Players.Select(p => p.CoinsAmount).Sum().Should().Be(coinsAmount);
        foreach(var player in sut.State.Players)
        {
            player.CoinsAmount.Should().Be(coinsAmount/amountOfPlayers);
        }
    }

    [TestMethod]
    public void ScaredPlayerGivesCoin_StateUpdatesTest()
    {
        var player1 = new ScaredPlayer();
        var player2 = new ScaredPlayer();
        sut.AddPlayer(player1);
        sut.AddPlayer(player2);
        var coinsAmount = 10;
        sut.DivideCoins(coinsAmount);

        sut.NextPlayerPlays();

        sut.State.Players.First().CoinsAmount.Should().Be(coinsAmount/2 - 1);
        sut.State.AmountOfCoinsOnTable.Should().Be(1);
    }

    [TestMethod]
    public void GreedyPlayerTakesCard_StateUpdatesTest()
    {
        var player1 = new GreedyPlayer();
        var player2 = new GreedyPlayer();
        sut.AddPlayer(player1);
        sut.AddPlayer(player2);
        var coinsAmount = 10;
        sut.DivideCoins(coinsAmount);

        sut.NextPlayerPlays();

        sut.State.Players.First().Cards.Should().HaveCount(1);
        sut.State.Players.First().CoinsAmount.Should().Be(coinsAmount / 2);
        sut.State.AmountOfCoinsOnTable.Should().Be(0);
    }
}