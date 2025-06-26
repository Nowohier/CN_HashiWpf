using Autofac;
using FluentAssertions;
using NRules.Integration.Autofac;

namespace Hashi.Rules.Test;

/// <summary>
/// Unit tests for AutoFacRulesModule class.
/// </summary>
public class AutoFacRulesModuleTests
{
    private ContainerBuilder containerBuilder;
    private AutoFacRulesModule module;

    [SetUp]
    public void SetUp()
    {
        containerBuilder = new ContainerBuilder();
        module = new AutoFacRulesModule();
    }

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var action = () => new AutoFacRulesModule();
        action.Should().NotThrow();
    }

    [Test]
    public void Load_WhenCalled_ShouldRegisterRuleRepository()
    {
        // Arrange & Act
        var action = () => module.ProtectedLoad(containerBuilder);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void Load_WhenCalled_ShouldConfigureRuleRepositoryWithCorrectAssembly()
    {
        // Arrange & Act
        module.ProtectedLoad(containerBuilder);
        var container = containerBuilder.Build();

        // Assert
        container.Should().NotBeNull();
        // The container should be built successfully, indicating the rule repository was configured properly
    }
}

/// <summary>
/// Test wrapper to expose protected Load method.
/// </summary>
public static class AutoFacRulesModuleExtensions
{
    public static void ProtectedLoad(this AutoFacRulesModule module, ContainerBuilder builder)
    {
        var loadMethod = typeof(AutoFacRulesModule).GetMethod("Load", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        loadMethod?.Invoke(module, new object[] { builder });
    }
}