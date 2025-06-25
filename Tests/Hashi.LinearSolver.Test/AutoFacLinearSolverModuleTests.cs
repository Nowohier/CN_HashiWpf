using Autofac;
using FluentAssertions;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.Logging.Interfaces;
using Moq;

namespace Hashi.LinearSolver.Test
{
    [TestFixture]
    public class AutoFacLinearSolverModuleTests
    {
        private IContainer container;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            
            // Register the logging dependencies as mocks since they're required
            builder.RegisterInstance(Mock.Of<ILoggerFactory>()).As<ILoggerFactory>();
            
            builder.RegisterModule<AutoFacLinearSolverModule>();
            container = builder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            container?.Dispose();
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterHashiSolver()
        {
            // arrange & act
            var hashiSolver = container.Resolve<IHashiSolver>();

            // assert
            hashiSolver.Should().NotBeNull();
            hashiSolver.Should().BeAssignableTo<IHashiSolver>();
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterHashiSolverAsSingleton()
        {
            // arrange & act
            var hashiSolver1 = container.Resolve<IHashiSolver>();
            var hashiSolver2 = container.Resolve<IHashiSolver>();

            // assert
            hashiSolver1.Should().BeSameAs(hashiSolver2);
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterIslandFactory()
        {
            // arrange & act
            var islandFactory = container.Resolve<Func<int, int, int, int, IIsland>>();

            // assert
            islandFactory.Should().NotBeNull();
        }

        [Test]
        public void Load_WhenCalled_ShouldRegisterEdgeFactory()
        {
            // arrange & act
            var edgeFactory = container.Resolve<Func<int, int, int, IEdge>>();

            // assert
            edgeFactory.Should().NotBeNull();
        }

        [Test]
        public void IslandFactory_WhenInvoked_ShouldCreateIslandWithCorrectProperties()
        {
            // arrange
            var islandFactory = container.Resolve<Func<int, int, int, int, IIsland>>();
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
            var edgeFactory = container.Resolve<Func<int, int, int, IEdge>>();
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
            var islandFactory = container.Resolve<Func<int, int, int, int, IIsland>>();

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
            var edgeFactory = container.Resolve<Func<int, int, int, IEdge>>();

            // act
            var edge1 = edgeFactory(1, 1, 2);
            var edge2 = edgeFactory(2, 3, 4);

            // assert
            edge1.Should().NotBeSameAs(edge2);
        }
    }
}