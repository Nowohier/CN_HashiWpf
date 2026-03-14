using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Rules.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NRules.RuleModel;

namespace Hashi.Rules.Test;

/// <summary>
/// Unit tests for RuleServiceExtensions class.
/// </summary>
[TestFixture]
public class AutoFacRulesModuleTests
{
    private ServiceCollection serviceCollection;

    [SetUp]
    public void SetUp()
    {
        serviceCollection = new ServiceCollection();
    }

    [Test]
    public void AddRuleServices_WhenCalled_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var action = () => serviceCollection.AddRuleServices();
        action.Should().NotThrow();
    }

    [Test]
    public void AddRuleServices_WhenCalled_ShouldRegisterRuleRepository()
    {
        // Arrange
        var ruleInfoProviderMock = new Mock<IRuleInfoProvider>(MockBehavior.Strict);
        var islandProviderMock = new Mock<IIslandProvider>(MockBehavior.Strict);
        serviceCollection.AddSingleton(ruleInfoProviderMock.Object);
        serviceCollection.AddSingleton(islandProviderMock.Object);

        // Act
        serviceCollection.AddRuleServices();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        var repository = serviceProvider.GetRequiredService<IRuleRepository>();
        repository.Should().NotBeNull();

        serviceProvider.Dispose();
    }
}
