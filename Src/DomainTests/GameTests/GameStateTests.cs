using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Entities.Game;

namespace DomainTests.GameTests;

[TestClass]
public class GameStateTests
{
    GameState sut;

    [TestInitialize]
    public void Setup()
    {
        sut = new GameState();
    }

    //[TestMethod]
    //public void AddPlayer()
    //{
    //    var player = new GreedyPlayer();

    //    sut.Players.Concat(new { player});

    //    var expectedResult = new List<IPlayer> { player };
    //    sut.Players.Should().BeEquivalentTo(expectedResult);
    //}
}