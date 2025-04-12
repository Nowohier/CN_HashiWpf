using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using NRules.Testing;

namespace Hashi.Rules.Test.Helpers
{
    public abstract class TestBase : RulesTestFixture
    {
        protected Mock<IConnectionManagerViewModel> ConnectionManagerMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ConnectionManagerMock = new Mock<IConnectionManagerViewModel>(MockBehavior.Strict);
            ConnectionManagerMock.SetupProperty(mock => mock.AreRulesBeingApplied, true);
            ConnectionManagerMock.SetupProperty(mock => mock.RuleMessage, string.Empty);
            ConnectionManagerMock.Setup(mock => mock.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true));
            Session.Insert(ConnectionManagerMock.Object);
        }

        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
            Session.RetractAll(Session.Query<IIslandViewModel>());
        }

        protected Mock<IIslandViewModel> SetupTestee(int maxConnections, params Mock<IIslandViewModel>[] neighbors)
        {
            var testIsland = CreateIslandMock(TestIslandEnum.TestIsland, maxConnections);
            testIsland.Setup(mock => mock.GetAllVisibleNeighbors()).Returns(neighbors.Select(n => n.Object).ToList());
            return testIsland;
        }


        protected Mock<IIslandViewModel> CreateIslandMock(TestIslandEnum islandEnum, int maxConnections, bool maxConnectionsReached = false)
        {
            return islandEnum switch
            {
                TestIslandEnum.TestIsland => CreateIslandMock(1, 1, maxConnections, maxConnectionsReached),
                TestIslandEnum.LeftIsland => CreateIslandMock(0, 1, maxConnections, maxConnectionsReached),
                TestIslandEnum.RightIsland => CreateIslandMock(2, 1, maxConnections, maxConnectionsReached),
                TestIslandEnum.UpIsland => CreateIslandMock(1, 0, maxConnections, maxConnectionsReached),
                TestIslandEnum.DownIsland => CreateIslandMock(1, 2, maxConnections, maxConnectionsReached),
                _ => throw new ArgumentOutOfRangeException(nameof(islandEnum), islandEnum, null)
            };
        }

        protected Mock<IHashiPoint> CreateHashiPointMock(int x, int y)
        {
            var hashPointMock = new Mock<IHashiPoint>(MockBehavior.Strict);
            hashPointMock.Setup(mock => mock.X).Returns(x);
            hashPointMock.Setup(mock => mock.Y).Returns(y);
            return hashPointMock;
        }

        private Mock<IIslandViewModel> CreateIslandMock(int x, int y, int maxConnections, bool maxConnectionsReached)
        {
            var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
            islandMock.Setup(mock => mock.Coordinates).Returns(CreateHashiPointMock(x, y).Object);
            islandMock.Setup(mock => mock.MaxConnections).Returns(maxConnections);
            islandMock.Setup(mock => mock.MaxConnectionsReached).Returns(maxConnectionsReached || maxConnections == 0);
            islandMock.Setup(mock => mock.GetAllVisibleNeighbors()).Returns([]);
            islandMock.Setup(mock => mock.AllConnections).Returns([]);
            islandMock.Setup(mock => mock.RefreshIslandColor());
            return islandMock;
        }
    }
}
