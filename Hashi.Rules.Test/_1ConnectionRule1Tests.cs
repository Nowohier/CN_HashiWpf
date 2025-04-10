using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using NRules.Testing;
using System.Collections.ObjectModel;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _1ConnectionRule1Tests : RulesTestFixture
    {
        private Mock<IConnectionManagerViewModel> connectionManagerMock;
        private IReadOnlyList<IReadOnlyList<Mock<IIslandViewModel>>> islandMockMatrix;
        private IReadOnlyList<IIslandViewModel> islandMockObjectList;
        private List<IIslandViewModel> neighborMockObjects = new();
        private static readonly List<int[]> HashiField =
        [
            new[] { 0, 0, 2, 0, 2 },
            new[] { 0, 0, 0, 0, 0 },
            new[] { 1, 0, 1, 0, 2 },
            new[] { 0, 0, 0, 0, 0 },
            new[] { 0, 0, 2, 0, 2 }
        ];

        public _1ConnectionRule1Tests()
        {
            Setup.Rule<_1ConnectionRule1>();
        }

        [SetUp]
        public void SetUp()
        {
            connectionManagerMock = new Mock<IConnectionManagerViewModel>(MockBehavior.Strict);
            connectionManagerMock.SetupProperty(mock => mock.AreRulesBeingApplied, true);
            connectionManagerMock.SetupProperty(mock => mock.RuleMessage, string.Empty);
            connectionManagerMock.Setup(mock => mock.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true));
            Session.Insert(connectionManagerMock.Object);

            islandMockMatrix = CreateTestMatrix(HashiField);
            islandMockObjectList = GetIslandMockObjectsAsList(islandMockMatrix);
            connectionManagerMock.Setup(mock => mock.Islands).Returns(GetIslandMockObjectsAsMatrix(islandMockMatrix));
            neighborMockObjects =
            [
                islandMockMatrix[0][2].Object, // Left neighbor
                islandMockMatrix[2][0].Object, // Top neighbor
                islandMockMatrix[2][4].Object, // Bottom neighbor
                islandMockMatrix[4][2].Object  // Right neighbor
            ];
        }

        [Test]
        public void _1ConnectionRule1_ShouldTrigger_WhenIslandHasOneValidNeighbor()
        {
            // arrange
            var neighbors = neighborMockObjects.Take(2).ToList();
            islandMockMatrix[2][2].Setup(mock => mock.GetAllVisibleNeighbors()).Returns(neighbors);

            // act
            Session.InsertAll(islandMockObjectList);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
        }

        [Test]
        public void _1ConnectionRule1_ShouldNotTrigger_WhenIslandHasMoreThanOneValidNeighbor()
        {
            // arrange
            var neighbors = neighborMockObjects.Take(3).ToList();
            islandMockMatrix[2][2].Setup(mock => mock.GetAllVisibleNeighbors()).Returns(neighbors);

            // act
            Session.InsertAll(islandMockObjectList);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void Rule_ShouldNotTrigger_WhenIslandHasNoValidNeighbors()
        {

        }

        private IReadOnlyList<IReadOnlyList<Mock<IIslandViewModel>>> CreateTestMatrix(IReadOnlyList<int[]> hashiField)
        {
            var list = new List<List<Mock<IIslandViewModel>>>();
            for (var row = 0; row < hashiField.Count; row++)
            {
                var rowCollection = new List<Mock<IIslandViewModel>>();
                for (var column = 0; column < hashiField[0].Length; column++)
                    rowCollection.Add(CreateIslandMock(column, row, hashiField[row][column]));
                list.Add(rowCollection);
            }

            return list;
        }

        private IReadOnlyList<IIslandViewModel> GetIslandMockObjectsAsList(IReadOnlyList<IReadOnlyList<Mock<IIslandViewModel>>> islandMockMatrix)
        {
            return (islandMockMatrix.SelectMany(row => row, (row, islandMock) => islandMock.Object)).ToList();
        }

        private ObservableCollection<ObservableCollection<IIslandViewModel>> GetIslandMockObjectsAsMatrix(IReadOnlyList<IReadOnlyList<Mock<IIslandViewModel>>> islandMockMatrix)
        {
            var observableMatrix = new ObservableCollection<ObservableCollection<IIslandViewModel>>();

            foreach (var row in islandMockMatrix)
            {
                var observableRow = new ObservableCollection<IIslandViewModel>(row.Select(mock => mock.Object));
                observableMatrix.Add(observableRow);
            }

            return observableMatrix;
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
