using FluentAssertions;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Test.AutoFac;

/// <summary>
/// Unit tests for ViewModelServiceExtensions class.
/// </summary>
[TestFixture]
public class AutoFacViewModelsModuleTests
{
    private ServiceCollection serviceCollection;

    [SetUp]
    public void SetUp()
    {
        serviceCollection = new ServiceCollection();
    }

    [Test]
    public void AddViewModelServices_WhenCalled_ShouldNotThrow()
    {
        // Arrange & Act
        var action = () => serviceCollection.AddViewModelServices();

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void AddViewModelServices_WhenServiceProviderBuilt_ShouldRegisterExpectedServices()
    {
        // Arrange & Act
        serviceCollection.AddViewModelServices();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert - Verify services that can be resolved without constructor parameters
        serviceProvider.GetService<ISettingsViewModel>().Should().NotBeNull();
        serviceProvider.GetService<Func<int, int, int, IIslandViewModel>>().Should().NotBeNull();

        serviceProvider.Dispose();
    }
}
