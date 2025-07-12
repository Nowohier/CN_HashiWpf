using Autofac;
using FluentAssertions;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels;
using System.Reflection;

namespace Hashi.Gui.Test.AutoFac;

/// <summary>
/// Unit tests for AutoFacViewModelsModule class.
/// </summary>
[TestFixture]
public class AutoFacViewModelsModuleTests
{
    private ContainerBuilder containerBuilder;
    private AutoFacViewModelsModule module;

    [SetUp]
    public void SetUp()
    {
        containerBuilder = new ContainerBuilder();
        module = new AutoFacViewModelsModule();
    }

    [TearDown]
    public void TearDown()
    {
        // No specific teardown needed as ContainerBuilder is recreated in SetUp
    }

    [Test]
    public void Load_WhenCalled_ShouldNotThrow()
    {
        // Arrange & Act
        var action = () => module.ProtectedLoad(containerBuilder);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void Load_WhenContainerBuilt_ShouldAllowServiceRegistrationChecking()
    {
        // Arrange & Act
        module.ProtectedLoad(containerBuilder);
        var container = containerBuilder.Build();

        // Assert - Verify all expected services are registered
        var serviceTypes = new[]
        {
            typeof(IIslandViewModel),
            typeof(ISettingsViewModel),
            typeof(ILanguageViewModel),
            typeof(IHighScorePerDifficultyViewModel),
            typeof(IMainViewModel),
            typeof(Func<int, int, int, IIslandViewModel>)
        };

        foreach (var serviceType in serviceTypes)
        {
            container.IsRegistered(serviceType).Should().BeTrue(
                $"Service {serviceType.Name} should be registered in the container");
        }

        container.Dispose();
    }
}

/// <summary>
/// Test wrapper to expose protected Load method.
/// </summary>
public static class AutoFacViewModelsModuleExtensions
{
    /// <summary>
    /// Exposes the protected Load method for testing purposes.
    /// </summary>
    /// <param name="module">The AutoFacViewModelsModule instance.</param>
    /// <param name="builder">The ContainerBuilder to configure.</param>
    public static void ProtectedLoad(this AutoFacViewModelsModule module, ContainerBuilder builder)
    {
        var loadMethod = typeof(AutoFacViewModelsModule).GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Instance);
        loadMethod?.Invoke(module, [builder]);
    }
}