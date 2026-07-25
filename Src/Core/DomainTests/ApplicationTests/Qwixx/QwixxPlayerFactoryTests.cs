using Application.Interfaces;
using Application.Qwixx;
using Domain.Entities.Game.Qwixx;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainTests.ApplicationTests.Qwixx;

[TestClass]
public class QwixxPlayerFactoryTests
{
    private Mock<IPlayerInputProvider> inputProviderMock;
    private QwixxPlayerFactory factory;

    [TestInitialize]
    public void Setup()
    {
        inputProviderMock = new Mock<IPlayerInputProvider>();
        factory = new QwixxPlayerFactory(inputProviderMock.Object);
    }

    // QX-006: the default roster has to be directly playable, so it must already sit inside
    // the player-count limits Play() enforces.
    [TestMethod]
    public void Create_ProducesALegalNumberOfPlayers()
    {
        var players = factory.Create().ToList();

        players.Count.Should().BeInRange(QwixxRules.MinNumberOfPlayers, QwixxRules.MaxNumberOfPlayers);
    }

    // Names identify players in every console message and on every score sheet, so duplicates
    // would make the output ambiguous.
    [TestMethod]
    public void Create_GivesEveryPlayerItsOwnName()
    {
        var players = factory.Create().ToList();

        players.Select(p => p.Name).Should().OnlyHaveUniqueItems();
    }

    [TestMethod]
    public void Create_ProducesFreshPlayersOnEveryCall()
    {
        var first = factory.Create().ToList();
        var second = factory.Create().ToList();

        second.Should().NotIntersectWith(first);
    }

    // The two hardcoded lists in this factory are kept in sync by hand, so a mismatch between
    // them is exactly the kind of thing that slips through unnoticed.
    [TestMethod]
    public void AllPlayerFactories_MatchesTheDefaultRoster()
    {
        var factories = factory.AllPlayerFactories().ToList();

        factories.Should().HaveCount(factory.Create().Count());
    }

    [TestMethod]
    public void AllPlayerFactories_ProducesAFreshPlayerOnEveryInvocation()
    {
        var createPlayer = factory.AllPlayerFactories().First();

        var first = createPlayer();
        var second = createPlayer();

        second.Should().NotBeSameAs(first);
        second.Name.Should().Be(first.Name);
    }

    [TestMethod]
    public void CreateManualPlayer_TakesItsNameFromTheInputProvider()
    {
        inputProviderMock.Setup(p => p.GetPlayerInput(It.IsAny<string>())).Returns("Alice");

        var player = factory.CreateManualPlayer();

        player.Name.Should().Be("Alice");
    }

    [TestMethod]
    public void CreateManualPlayers_CreatesExactlyTheRequestedNumberOfPlayers()
    {
        inputProviderMock.SetupSequence(p => p.GetPlayerInput(It.IsAny<string>()))
            .Returns("Alice")
            .Returns("Bob")
            .Returns("Carol");

        var players = factory.CreateManualPlayers(3).ToList();

        players.Should().HaveCount(3);
        players.Select(p => p.Name).Should().Equal("Alice", "Bob", "Carol");
    }
}
