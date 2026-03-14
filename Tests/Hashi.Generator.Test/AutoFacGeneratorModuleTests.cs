using FluentAssertions;
using Hashi.Generator.Extensions;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Core.Extensions;
using Hashi.LinearSolver.Interfaces;
using Hashi.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Drawing;

namespace Hashi.Generator.Test;

[TestFixture]
public class AutoFacGeneratorModuleTests
{
    private ServiceProvider serviceProvider = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        // Register the logging dependencies as mocks since they're required
        var loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger>(MockBehavior.Strict);

        // Setup the logger factory to return a mock logger
        loggerFactoryMock.Setup(f => f.CreateLogger<It.IsAnyType>()).Returns(loggerMock.Object);

        services.AddSingleton(loggerFactoryMock.Object);

        // Register the solver dependency as mock since it's required
        var hashiSolverMock = new Mock<IHashiSolver>(MockBehavior.Strict);
        services.AddSingleton(hashiSolverMock.Object);
        services.AddSingleton<IPuzzleSolver>(hashiSolverMock.As<IPuzzleSolver>().Object);

        services.AddGuiCoreServices();
        services.AddGeneratorServices();
        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        serviceProvider.Dispose();
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterHashiGenerator()
    {
        // Arrange & Act
        var hashiGenerator = serviceProvider.GetRequiredService<IHashiGenerator>();

        // Assert
        hashiGenerator.Should().NotBeNull();
        hashiGenerator.Should().BeAssignableTo<IHashiGenerator>();
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterHashiGeneratorAsSingleton()
    {
        // Arrange & Act
        var hashiGenerator1 = serviceProvider.GetRequiredService<IHashiGenerator>();
        var hashiGenerator2 = serviceProvider.GetRequiredService<IHashiGenerator>();

        // Assert
        hashiGenerator1.Should().BeSameAs(hashiGenerator2);
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterIslandFactory()
    {
        // Arrange & Act
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, IIsland>>();

        // Assert
        islandFactory.Should().NotBeNull();
    }

    [Test]
    public void IslandFactory_WhenInvoked_ShouldCreateIslandWithCorrectProperties()
    {
        // Arrange
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, IIsland>>();
        var amountBridges = 3;
        var row = 5;
        var column = 7;

        // Act
        var island = islandFactory(amountBridges, row, column);

        // Assert
        island.Should().NotBeNull();
        island.AmountBridgesConnectable.Should().Be(amountBridges);
        island.Y.Should().Be(row);
        island.X.Should().Be(column);
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterBridgeCoordinatesFactory()
    {
        // Arrange & Act
        var bridgeCoordinatesFactory = serviceProvider.GetRequiredService<Func<Point, Point, int, IBridgeCoordinates>>();

        // Assert
        bridgeCoordinatesFactory.Should().NotBeNull();
    }

    [Test]
    public void BridgeCoordinatesFactory_WhenInvoked_ShouldCreateBridgeCoordinatesWithCorrectProperties()
    {
        // Arrange
        var bridgeCoordinatesFactory = serviceProvider.GetRequiredService<Func<Point, Point, int, IBridgeCoordinates>>();
        var location1 = new Point(1, 2);
        var location2 = new Point(3, 4);
        var amountBridges = 2;

        // Act
        var bridgeCoordinates = bridgeCoordinatesFactory(location1, location2, amountBridges);

        // Assert
        bridgeCoordinates.Should().NotBeNull();
        bridgeCoordinates.Location1.Should().Be(location1);
        bridgeCoordinates.Location2.Should().Be(location2);
        bridgeCoordinates.AmountBridges.Should().Be(amountBridges);
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterSolutionProviderFactory()
    {
        // Arrange & Act
        var solutionProviderFactory = serviceProvider.GetRequiredService<Func<int[][], IReadOnlyList<IBridgeCoordinates>, ISolutionProvider>>();

        // Assert
        solutionProviderFactory.Should().NotBeNull();
    }

    [Test]
    public void SolutionProviderFactory_WhenInvoked_ShouldCreateSolutionProviderWithCorrectProperties()
    {
        // Arrange
        var solutionProviderFactory = serviceProvider.GetRequiredService<Func<int[][], IReadOnlyList<IBridgeCoordinates>, ISolutionProvider>>();
        var hashiField = new int[][]
        {
            [0, 1, 0],
            [1, 0, 2],
            [0, 2, 0]
        };
        var bridgeCoordinates = new List<IBridgeCoordinates>();

        // Act
        var solutionProvider = solutionProviderFactory(hashiField, bridgeCoordinates);

        // Assert
        solutionProvider.Should().NotBeNull();
        solutionProvider.HashiField.Should().BeEquivalentTo(hashiField);
        solutionProvider.BridgeCoordinates.Should().BeEquivalentTo(bridgeCoordinates);
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterBridgeFactory()
    {
        // Arrange & Act
        var bridgeFactory = serviceProvider.GetRequiredService<Func<IIsland, IIsland, int, IBridge>>();

        // Assert
        bridgeFactory.Should().NotBeNull();
    }

    [Test]
    public void BridgeFactory_WhenInvoked_ShouldCreateBridgeWithCorrectProperties()
    {
        // Arrange
        var bridgeFactory = serviceProvider.GetRequiredService<Func<IIsland, IIsland, int, IBridge>>();
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, IIsland>>();

        var island1 = islandFactory(2, 1, 1);
        var island2 = islandFactory(2, 1, 3);
        var amountBridgesSet = 1;

        // Act
        var bridge = bridgeFactory(island1, island2, amountBridgesSet);

        // Assert
        bridge.Should().NotBeNull();
        bridge.Island1.Should().Be(island1);
        bridge.Island2.Should().Be(island2);
        bridge.AmountBridgesSet.Should().Be(amountBridgesSet);
    }

    [Test]
    public void IslandFactory_WhenInvokedMultipleTimes_ShouldCreateDifferentInstances()
    {
        // Arrange
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, IIsland>>();

        // Act
        var island1 = islandFactory(2, 1, 1);
        var island2 = islandFactory(2, 1, 1);

        // Assert
        island1.Should().NotBeSameAs(island2); // Should be different instances
        island1.Should().BeEquivalentTo(island2, options => options.ExcludingMissingMembers()); // But equivalent data
    }

    [Test]
    public void BridgeFactory_WhenInvokedMultipleTimes_ShouldCreateDifferentInstances()
    {
        // Arrange
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, IIsland>>();
        var island1 = islandFactory(2, 1, 1);
        var island2 = islandFactory(2, 1, 3);

        var bridgeFactory = serviceProvider.GetRequiredService<Func<IIsland, IIsland, int, IBridge>>();

        // Act
        var bridge1 = bridgeFactory(island1, island2, 1);
        var bridge2 = bridgeFactory(island1, island2, 1);

        // Assert
        bridge1.Should().NotBeSameAs(bridge2); // Should be different instances
    }

    [Test]
    public void BridgeCoordinatesFactory_WhenInvokedMultipleTimes_ShouldCreateDifferentInstances()
    {
        // Arrange
        var location1 = new Point(1, 2);
        var location2 = new Point(3, 4);
        var bridgeCoordinatesFactory = serviceProvider.GetRequiredService<Func<Point, Point, int, IBridgeCoordinates>>();

        // Act
        var bridgeCoordinates1 = bridgeCoordinatesFactory(location1, location2, 1);
        var bridgeCoordinates2 = bridgeCoordinatesFactory(location1, location2, 1);

        // Assert
        bridgeCoordinates1.Should().NotBeSameAs(bridgeCoordinates2); // Should be different instances
    }

    [Test]
    public void SolutionProviderFactory_WhenInvokedMultipleTimes_ShouldCreateDifferentInstances()
    {
        // Arrange
        var hashiField = new int[][]
        {
            [1, 2, 1]
        };
        var bridgeCoordinates = new List<IBridgeCoordinates>();
        var solutionProviderFactory = serviceProvider.GetRequiredService<Func<int[][], IReadOnlyList<IBridgeCoordinates>, ISolutionProvider>>();

        // Act
        var solutionProvider1 = solutionProviderFactory(hashiField, bridgeCoordinates);
        var solutionProvider2 = solutionProviderFactory(hashiField, bridgeCoordinates);

        // Assert
        solutionProvider1.Should().NotBeSameAs(solutionProvider2); // Should be different instances
    }
}
