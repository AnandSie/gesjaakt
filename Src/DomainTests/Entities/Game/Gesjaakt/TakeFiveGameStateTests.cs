using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Game.Gesjaakt;

[TestClass]
public class TakeFiveGameStateTests
{
    private TakeFiveGameState _state;

    [TestInitialize]
    public void Setup()
    {
        _state = new TakeFiveGameState();
    }

    [TestMethod]
    public void AddPlayers_GetPlayersTest()
    {
        // Arrange
        var player = new TakeFivePlayer();

        // Act
        _state.AddPlayer(player);

        // Assert
        _state.Players.Single().Should().Be(player);    
    }
}
