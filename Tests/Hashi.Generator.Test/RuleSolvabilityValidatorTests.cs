using FluentAssertions;
using Hashi.Generator;
using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Core.Providers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Logging.Interfaces;
using Moq;

namespace Hashi.Generator.Test;

[TestFixture]
public class RuleSolvabilityValidatorTests
{
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<ILogger> loggerMock;
    private IIslandViewModelHelper helper;
    private IIslandProviderCore core;
    private RuleSolvabilityValidator validator;

    [SetUp]
    public void SetUp()
    {
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        loggerFactoryMock.Setup(x => x.CreateLogger<RuleSolvabilityValidator>()).Returns(loggerMock.Object);
        loggerMock.Setup(x => x.Debug(It.IsAny<string>()));
        loggerMock.Setup(x => x.Info(It.IsAny<string>()));

        helper = new IslandViewModelHelper();
        core = new IslandProviderCore(helper);
        validator = new RuleSolvabilityValidator(loggerFactoryMock.Object, helper, core);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenLoggerFactoryIsNull_ShouldThrowException()
    {
        // Act & Assert
        var action = () => new RuleSolvabilityValidator(null!, helper, core);
        action.Should().Throw<Exception>();
    }

    [Test]
    public void Constructor_WhenHelperIsNull_ShouldNotThrow()
    {
        // Note: The constructor doesn't guard helper/core with null checks
        // This test documents the current behavior
        var action = () => new RuleSolvabilityValidator(loggerFactoryMock.Object, null!, core);
        action.Should().NotThrow();
    }

    [Test]
    public void Constructor_WhenCoreIsNull_ShouldNotThrow()
    {
        // Note: The constructor doesn't guard helper/core with null checks
        // This test documents the current behavior
        var action = () => new RuleSolvabilityValidator(loggerFactoryMock.Object, helper, null!);
        action.Should().NotThrow();
    }

    #endregion

    #region SimulateRuleSolving Tests

    [Test]
    public void SimulateRuleSolving_WhenTrivialSolvablePuzzle_ShouldReturnTrue()
    {
        // Arrange — a simple 2-island puzzle: both need 1 connection to each other
        var field = new[]
        {
            new[] { 1, 0, 1 }
        };

        // Act
        var result = validator.SimulateRuleSolving(field);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void SimulateRuleSolving_WhenEmptyField_ShouldReturnFalse()
    {
        // Arrange — no islands means nothing can be solved
        var field = new[]
        {
            new[] { 0, 0, 0 },
            new[] { 0, 0, 0 }
        };

        // Act
        var result = validator.SimulateRuleSolving(field);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void SimulateRuleSolving_WhenUnsolvablePuzzle_ShouldReturnFalse()
    {
        // Arrange — island with 3 connections but only 2 neighbors (max capacity 2 each)
        // This puzzle is structurally unsolvable by rules
        var field = new[]
        {
            new[] { 1, 0, 0 },
            new[] { 0, 0, 0 },
            new[] { 0, 0, 1 }
        };

        // Act
        var result = validator.SimulateRuleSolving(field);

        // Assert — two isolated islands with no neighbors can't connect
        result.Should().BeFalse();
    }

    #endregion

    #region IsFullySolvableByRules Tests

    [Test]
    public async Task IsFullySolvableByRules_WhenSolvable_ShouldReturnTrue()
    {
        // Arrange
        var field = new[]
        {
            new[] { 1, 0, 1 }
        };

        // Act
        var result = await validator.IsFullySolvableByRules(field);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
