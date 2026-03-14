using FluentAssertions;
using Hashi.LinearSolver.Extensions;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Hashi.LinearSolver.Test;

[TestFixture]
public class AutoFacLinearSolverModuleTests
{
    private ServiceProvider serviceProvider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        // Register the logging dependencies as mocks since they're required
        var loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        var loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerFactoryMock.Setup(f => f.CreateLogger<It.IsAnyType>()).Returns(loggerMock.Object);
        services.AddSingleton(loggerFactoryMock.Object);

        services.AddLinearSolverServices();
        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        serviceProvider.Dispose();
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterHashiSolver()
    {
        // arrange & act
        var hashiSolver = serviceProvider.GetRequiredService<IHashiSolver>();

        // assert
        hashiSolver.Should().NotBeNull();
        hashiSolver.Should().BeAssignableTo<IHashiSolver>();
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterHashiSolverAsSingleton()
    {
        // arrange & act
        var hashiSolver1 = serviceProvider.GetRequiredService<IHashiSolver>();
        var hashiSolver2 = serviceProvider.GetRequiredService<IHashiSolver>();

        // assert
        hashiSolver1.Should().BeSameAs(hashiSolver2);
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterIslandFactory()
    {
        // arrange & act
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, int, IIsland>>();

        // assert
        islandFactory.Should().NotBeNull();
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterEdgeFactory()
    {
        // arrange & act
        var edgeFactory = serviceProvider.GetRequiredService<Func<int, int, int, IEdge>>();

        // assert
        edgeFactory.Should().NotBeNull();
    }

    [Test]
    public void IslandFactory_WhenInvoked_ShouldCreateIslandWithCorrectProperties()
    {
        // arrange
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, int, IIsland>>();
        const int id = 1;
        const int row = 2;
        const int col = 3;
        const int bridgesRequired = 4;

        // act
        var island = islandFactory(id, row, col, bridgesRequired);

        // assert
        island.Should().NotBeNull();
        island.Id.Should().Be(id);
        island.Row.Should().Be(row);
        island.Col.Should().Be(col);
        island.BridgesRequired.Should().Be(bridgesRequired);
        island.Neighbors.Should().NotBeNull();
    }

    [Test]
    public void EdgeFactory_WhenInvoked_ShouldCreateEdgeWithCorrectProperties()
    {
        // arrange
        var edgeFactory = serviceProvider.GetRequiredService<Func<int, int, int, IEdge>>();
        const int id = 1;
        const int islandA = 2;
        const int islandB = 3;

        // act
        var edge = edgeFactory(id, islandA, islandB);

        // assert
        edge.Should().NotBeNull();
        edge.Id.Should().Be(id);
        edge.IslandA.Should().Be(islandA);
        edge.IslandB.Should().Be(islandB);
    }

    [Test]
    public void IslandFactory_WhenInvokedMultipleTimes_ShouldCreateDifferentInstances()
    {
        // arrange
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, int, IIsland>>();

        // act
        var island1 = islandFactory(1, 1, 1, 1);
        var island2 = islandFactory(2, 2, 2, 2);

        // assert
        island1.Should().NotBeSameAs(island2);
    }

    [Test]
    public void EdgeFactory_WhenInvokedMultipleTimes_ShouldCreateDifferentInstances()
    {
        // arrange
        var edgeFactory = serviceProvider.GetRequiredService<Func<int, int, int, IEdge>>();

        // act
        var edge1 = edgeFactory(1, 1, 2);
        var edge2 = edgeFactory(2, 3, 4);

        // assert
        edge1.Should().NotBeSameAs(edge2);
    }
}
