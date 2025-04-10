using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using NRules.Testing;
using System.Collections.ObjectModel;

namespace Hashi.Rules.Test.Helpers
{
    public abstract class TestBase : RulesTestFixture
    {
        protected Mock<IConnectionManagerViewModel> ConnectionManagerMock;

        [SetUp]
        public void SetUp()
        {
            ConnectionManagerMock = new Mock<IConnectionManagerViewModel>(MockBehavior.Strict);
            ConnectionManagerMock.SetupProperty(mock => mock.AreRulesBeingApplied, true);
            ConnectionManagerMock.SetupProperty(mock => mock.RuleMessage, string.Empty);
            ConnectionManagerMock.Setup(mock => mock.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true));
            Session.Insert(ConnectionManagerMock.Object);
        }

        protected Mock<IIslandViewModel> CreateTestIslandMock(TestIslandEnum islandEnum, int maxConnections)
        {
            return islandEnum switch
            {
                TestIslandEnum.TestIsland => CreateIslandMock(1, 1, maxConnections),
                TestIslandEnum.LeftIsland => CreateIslandMock(0, 1, maxConnections),
                TestIslandEnum.RightIsland => CreateIslandMock(2, 1, maxConnections),
                TestIslandEnum.UpIsland => CreateIslandMock(1, 0, maxConnections),
                TestIslandEnum.DownIsland => CreateIslandMock(1, 2, maxConnections),
                _ => throw new ArgumentOutOfRangeException(nameof(islandEnum), islandEnum, null)
            };
        }

        private Mock<IIslandViewModel> CreateIslandMock(int x, int y, int maxConnections)
        {
            var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
            islandMock.Setup(mock => mock.Coordinates).Returns(CreateHashiPointMock(x, y).Object);
            islandMock.Setup(mock => mock.MaxConnections).Returns(maxConnections);
            islandMock.Setup(mock => mock.MaxConnectionsReached).Returns(maxConnections == 0);
            islandMock.Setup(mock => mock.GetAllVisibleNeighbors()).Returns(new List<IIslandViewModel>());
            islandMock.Setup(mock => mock.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
            islandMock.Setup(mock => mock.RefreshIslandColor());
            return islandMock;
        }

        private Mock<IHashiPoint> CreateHashiPointMock(int x, int y)
        {
            var hashPointMock = new Mock<IHashiPoint>(MockBehavior.Strict);
            hashPointMock.Setup(mock => mock.X).Returns(x);
            hashPointMock.Setup(mock => mock.Y).Returns(y);
            return hashPointMock;
        }
    }
}
