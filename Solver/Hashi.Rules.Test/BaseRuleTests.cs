using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using System.Collections.ObjectModel;

namespace Hashi.Rules.Test
{
    public class BaseRuleTests
    {
        private Mock<IConnectionManagerViewModel> connectionManagerMock;
        private Mock<IIslandViewModel> sourceIslandMock;
        private Mock<IIslandViewModel> targetIslandMock;
        private TestableBaseRule testableBaseRule;

        [SetUp]
        public void SetUp()
        {
            connectionManagerMock = new Mock<IConnectionManagerViewModel>();
            sourceIslandMock = new Mock<IIslandViewModel>();
            targetIslandMock = new Mock<IIslandViewModel>();
            testableBaseRule = new TestableBaseRule();
        }

        [Test]
        public void AddConnection_WhenRulesAreNotBeingApplied_ShouldNotAddConnection()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(false);

            // act
            testableBaseRule.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true), Times.Never);
        }

        [Test]
        public void AddConnection_WhenMaxConnectionsReached_ShouldNotAddConnection()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(true);

            // act
            testableBaseRule.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true), Times.Never);
        }

        [Test]
        public void AddConnection_WhenValid_ShouldAddConnection()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
            targetIslandMock.Setup(ti => ti.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

            // act
            testableBaseRule.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true), Times.Once);
        }

        [Test]
        public void AddConnections_WhenMultipleTargets_ShouldAddConnections()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel> { targetIslandMock.Object };
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
            targetIslandMock.Setup(ti => ti.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

            // act
            testableBaseRule.AddConnections(sourceIslandMock.Object, targetIslands, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true), Times.Once);
        }

        [Test]
        public void AddMultipleConnections_WhenValid_ShouldAddTwoConnectionsPerTarget()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel> { targetIslandMock.Object };
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
            targetIslandMock.Setup(ti => ti.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

            // act
            testableBaseRule.AddMultipleConnections(sourceIslandMock.Object, targetIslands, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true), Times.Exactly(2));
        }

        [Test]
        public void AddMissingConnectionsToOneTarget_WhenValid_ShouldAddMissingConnections()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
            targetIslandMock.Setup(ti => ti.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

            // act
            testableBaseRule.AddMissingConnectionsToOneTarget(sourceIslandMock.Object, targetIslandMock.Object, 2, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true), Times.Exactly(2));
        }
    }

    /// <summary>
    /// A concrete implementation of BaseRule for testing purposes.
    /// </summary>
    public class TestableBaseRule : BaseRule
    {
        /// <summary>
        /// Provides a test-specific rule message.
        /// </summary>
        protected override string RuleMessage => "Test Rule Message";

        /// <summary>
        /// Allows overriding behavior for testing specific scenarios.
        /// </summary>
        public bool EnsureRulesAreBeingAppliedOverride { get; set; } = true;

        /// <summary>
        /// Overrides the EnsureRulesAreBeingApplied method for testing.
        /// </summary>
        protected override bool EnsureRulesAreBeingApplied(IConnectionManagerViewModel connectionManager)
        {
            return EnsureRulesAreBeingAppliedOverride && base.EnsureRulesAreBeingApplied(connectionManager);
        }

        public override void Define()
        {
            //Do nothing
        }
    }
}
