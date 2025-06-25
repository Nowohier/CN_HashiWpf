using Autofac;
using FluentAssertions;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Logging.Interfaces;
using Moq;
using System.Drawing;

namespace Hashi.Generator.Test
{
    [TestFixture]
    public class AutoFacGeneratorModuleTests
    {
        private IContainer container;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            // Register the logging dependencies as mocks since they're required
            builder.RegisterInstance(Mock.Of<ILoggerFactory>()).As<ILoggerFactory>();
            
            // Register the solver dependency as mock since it's required
            builder.RegisterInstance(Mock.Of<IHashiSolver>()).As<IHashiSolver>();

            builder.RegisterModule<AutoFacGeneratorModule>();
            container = builder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            container.Dispose();
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterHashiGenerator()
        {
            // Arrange & Act
            var hashiGenerator = container.Resolve<IHashiGenerator>();

            // Assert
            hashiGenerator.Should().NotBeNull();
            hashiGenerator.Should().BeAssignableTo<IHashiGenerator>();
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterHashiGeneratorAsSingleton()
        {
            // Arrange & Act
            var hashiGenerator1 = container.Resolve<IHashiGenerator>();
            var hashiGenerator2 = container.Resolve<IHashiGenerator>();

            // Assert
            hashiGenerator1.Should().BeSameAs(hashiGenerator2);
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterIslandFactory()
        {
            // Arrange & Act
            var islandFactory = container.Resolve<Func<int, int, int, IIsland>>();

            // Assert
            islandFactory.Should().NotBeNull();
        }

        [Test]
        public void IslandFactory_WhenInvoked_ShouldCreateIslandWithCorrectProperties()
        {
            // Arrange
            var islandFactory = container.Resolve<Func<int, int, int, IIsland>>();
            int amountBridges = 3;
            int row = 5;
            int column = 7;

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
            var bridgeCoordinatesFactory = container.Resolve<Func<Point, Point, int, IBridgeCoordinates>>();

            // Assert
            bridgeCoordinatesFactory.Should().NotBeNull();
        }

        [Test]
        public void BridgeCoordinatesFactory_WhenInvoked_ShouldCreateBridgeCoordinatesWithCorrectProperties()
        {
            // Arrange
            var bridgeCoordinatesFactory = container.Resolve<Func<Point, Point, int, IBridgeCoordinates>>();
            var location1 = new Point(1, 2);
            var location2 = new Point(3, 4);
            int amountBridges = 2;

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
            var solutionProviderFactory = container.Resolve<Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider>>();

            // Assert
            solutionProviderFactory.Should().NotBeNull();
        }

        [Test]
        public void SolutionProviderFactory_WhenInvoked_ShouldCreateSolutionProviderWithCorrectProperties()
        {
            // Arrange
            var solutionProviderFactory = container.Resolve<Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider>>();
            var hashiField = new int[][]
            {
                new int[] { 0, 1, 0 },
                new int[] { 1, 0, 2 },
                new int[] { 0, 2, 0 }
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
            var bridgeFactory = container.Resolve<Func<IIsland, IIsland, int, IBridge>>();

            // Assert
            bridgeFactory.Should().NotBeNull();
        }

        [Test]
        public void BridgeFactory_WhenInvoked_ShouldCreateBridgeWithCorrectProperties()
        {
            // Arrange
            var bridgeFactory = container.Resolve<Func<IIsland, IIsland, int, IBridge>>();
            var islandFactory = container.Resolve<Func<int, int, int, IIsland>>();
            
            var island1 = islandFactory(2, 1, 1);
            var island2 = islandFactory(2, 1, 3);
            int amountBridgesSet = 1;

            // Act
            var bridge = bridgeFactory(island1, island2, amountBridgesSet);

            // Assert
            bridge.Should().NotBeNull();
            bridge.Island1.Should().Be(island1);
            bridge.Island2.Should().Be(island2);
            bridge.AmountBridgesSet.Should().Be(amountBridgesSet);
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterIslandAsInstancePerDependency()
        {
            // Arrange & Act
            var island1 = container.Resolve<IIsland>(
                new NamedParameter("amountBridgesConnectable", 2),
                new NamedParameter("y", 1),
                new NamedParameter("x", 1));
            var island2 = container.Resolve<IIsland>(
                new NamedParameter("amountBridgesConnectable", 2),
                new NamedParameter("y", 1),
                new NamedParameter("x", 1));

            // Assert
            island1.Should().NotBeSameAs(island2); // Should be different instances
            island1.Should().BeEquivalentTo(island2, options => options.ExcludingMissingMembers()); // But equivalent data
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterBridgeAsInstancePerDependency()
        {
            // Arrange
            var islandFactory = container.Resolve<Func<int, int, int, IIsland>>();
            var island1 = islandFactory(2, 1, 1);
            var island2 = islandFactory(2, 1, 3);

            // Act
            var bridge1 = container.Resolve<IBridge>(
                new NamedParameter("island1", island1),
                new NamedParameter("island2", island2),
                new NamedParameter("amountBridgesSet", 1));
            var bridge2 = container.Resolve<IBridge>(
                new NamedParameter("island1", island1),
                new NamedParameter("island2", island2),
                new NamedParameter("amountBridgesSet", 1));

            // Assert
            bridge1.Should().NotBeSameAs(bridge2); // Should be different instances
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterBridgeCoordinatesAsInstancePerDependency()
        {
            // Arrange
            var location1 = new Point(1, 2);
            var location2 = new Point(3, 4);

            // Act
            var bridgeCoordinates1 = container.Resolve<IBridgeCoordinates>(
                new NamedParameter("location1", location1),
                new NamedParameter("location2", location2),
                new NamedParameter("amountBridges", 1));
            var bridgeCoordinates2 = container.Resolve<IBridgeCoordinates>(
                new NamedParameter("location1", location1),
                new NamedParameter("location2", location2),
                new NamedParameter("amountBridges", 1));

            // Assert
            bridgeCoordinates1.Should().NotBeSameAs(bridgeCoordinates2); // Should be different instances
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterSolutionProviderAsInstancePerDependency()
        {
            // Arrange
            var hashiField = new int[][]
            {
                new int[] { 1, 2, 1 }
            };
            var bridgeCoordinates = new List<IBridgeCoordinates>();

            // Act
            var solutionProvider1 = container.Resolve<ISolutionProvider>(
                new NamedParameter("hashiField", hashiField),
                new NamedParameter("bridgeCoordinates", bridgeCoordinates));
            var solutionProvider2 = container.Resolve<ISolutionProvider>(
                new NamedParameter("hashiField", hashiField),
                new NamedParameter("bridgeCoordinates", bridgeCoordinates));

            // Assert
            solutionProvider1.Should().NotBeSameAs(solutionProvider2); // Should be different instances
        }
    }
}