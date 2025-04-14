using FluentAssertions;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using NRules.Fluent.Dsl;
using NRules.Testing;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Hashi.Rules.Test.Helpers;

[ExcludeFromCodeCoverage]
public abstract class TestBase<T> : RulesTestFixture
    where T : Rule
{
    protected Mock<IIslandProvider> IslandProviderMock;
    protected Mock<IRuleInfoProvider> RuleInfoProviderMock;

    protected TestBase()
    {
        // Create rule instance with mocks and add rule to the setup
        IslandProviderMock = new Mock<IIslandProvider>(MockBehavior.Strict);
        IslandProviderMock.Setup(mock => mock.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true));

        RuleInfoProviderMock = new Mock<IRuleInfoProvider>(MockBehavior.Strict);
        RuleInfoProviderMock.SetupProperty(mock => mock.AreRulesBeingApplied, true);
        RuleInfoProviderMock.SetupProperty(mock => mock.RuleMessage, string.Empty);

        if (Activator.CreateInstance(typeof(T), RuleInfoProviderMock.Object, IslandProviderMock.Object) is not T rule)
        {
            throw new ArgumentNullException();
        }

        Setup.Rule(rule);
    }

    [SetUp]
    public void SetUp()
    {
        Recorder.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        Session.RetractAll(Session.Query<IIslandViewModel>());
    }

    [TestCase(true, true, true, TestName = "TestRuleClassConstructors_WhenIslandProviderAndRuleInfoProviderNull_ShouldThrowException")]
    [TestCase(true, false, true, TestName = "TestRuleClassConstructors_WhenRuleInfoProviderNull_ShouldThrowException")]
    [TestCase(false, true, true, TestName = "TestRuleClassConstructors_WhenIslandProviderNull_ShouldThrowException")]
    [TestCase(false, false, false, TestName = "TestRuleClassConstructors_WhenIslandProviderAndRuleInfoProviderNotNull_ShouldNotThrowException")]
    public void TestRuleClassConstructors_WhenConstructorIsTested_ShouldBehaveCorrectly(bool isIslandProviderNull, bool isRuleInfoProviderNull, bool throwsException)
    {
        // arrange
        var islandProviderMockObject = isIslandProviderNull ? null : IslandProviderMock.Object;
        var ruleInfoProviderMockObject = isRuleInfoProviderNull ? null : RuleInfoProviderMock.Object;

        var action = () => { var test = (T)Activator.CreateInstance(typeof(T), ruleInfoProviderMockObject!, islandProviderMockObject!); };

        // act, assert
        if (throwsException)
        {
            action.Should().Throw<TargetInvocationException>().WithInnerException<ArgumentNullException>();
        }
        else
        {
            action.Should().NotThrow();
        }
    }


    protected Mock<IIslandViewModel> SetupTestIsland(int maxConnections, params Mock<IIslandViewModel>[] neighbors)
    {
        var testIsland = CreateIslandMock(TestIslandEnum.TestIsland, maxConnections);
        testIsland.Setup(mock => mock.GetAllVisibleNeighbors()).Returns(neighbors.Select(n => n.Object).ToList());
        return testIsland;
    }


    protected Mock<IIslandViewModel> CreateIslandMock(TestIslandEnum islandEnum, int maxConnections,
        bool maxConnectionsReached = false)
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
        islandMock.Setup(mock => mock.RemainingConnections).Returns(maxConnectionsReached ? 0 : maxConnections);
        islandMock.Setup(mock => mock.RefreshIslandColor());
        return islandMock;
    }
}