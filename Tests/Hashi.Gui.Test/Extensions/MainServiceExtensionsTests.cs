using System.Drawing;
using FluentAssertions;
using Hashi.Enums;
using Hashi.Generator.Extensions;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Simulation;
using Hashi.Gui.Core.Extensions;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers.Extensions;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Managers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Messages.MessageContainers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Services;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Managers.Extensions;
using Hashi.Gui.Messages.Extensions;
using Hashi.Gui.Models.Extensions;
using Hashi.Gui.Providers.Extensions;
using Hashi.Gui.ViewModels.Extensions;
using Hashi.Gui.Wrappers.Extensions;
using Hashi.LinearSolver.Extensions;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.Logging.Extensions;
using Hashi.Logging.Interfaces;
using Hashi.Rules.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NRules.RuleModel;
using IIsland = Hashi.Generator.Interfaces.Models.IIsland;

namespace Hashi.Gui.Test.Extensions;

/// <summary>
///     Tests that all IoC container registrations can be resolved at runtime.
///     This test class builds the full DI container (with WPF-dependent services mocked)
///     and verifies every registered service type can be instantiated.
/// </summary>
[TestFixture]
public class MainServiceExtensionsTests
{
    private ServiceProvider serviceProvider;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddHashiServices();

        // Replace WPF-dependent services with mocks (they require a UI thread / XAML context)
        ReplaceWpfDependentServices(services);

        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        serviceProvider.Dispose();
    }

    /// <summary>
    ///     Replaces services that require a WPF Application/UI thread with mocks
    ///     so the container can be built and resolved in a unit test context.
    /// </summary>
    private static void ReplaceWpfDependentServices(ServiceCollection services)
    {
        // IWindow / IViewBoxControl require WPF Application context (InitializeComponent)
        var windowMock = new Mock<IWindow>(MockBehavior.Strict);
        var viewBoxMock = new Mock<IViewBoxControl>(MockBehavior.Strict);
        windowMock.SetupGet(w => w.DataContext).Returns(null!);
        windowMock.SetupSet(w => w.DataContext = It.IsAny<object?>());
        ReplaceService<IWindow>(services, windowMock.Object);
        ReplaceService<IViewBoxControl>(services, viewBoxMock.Object);
    }

    /// <summary>
    ///     Removes existing registrations for a service type and replaces with a singleton instance.
    /// </summary>
    private static void ReplaceService<TService>(ServiceCollection services, TService instance) where TService : class
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(TService)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton(instance);
    }

    #region Singleton Service Resolution Tests

    [Test]
    public void Resolve_WhenILoggerFactory_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<ILoggerFactory>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIHashiGenerator_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IHashiGenerator>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIDifficultySettingsProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IDifficultySettingsProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIBlockDetectionService_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IBlockDetectionService>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIIslandLayoutService_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IIslandLayoutService>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIBridgeManagementService_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IBridgeManagementService>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenISimulationFactory_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<ISimulationFactory>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIRuleSolvabilityValidator_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IRuleSolvabilityValidator>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIHashiSolver_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IHashiSolver>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIPuzzleReader_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IPuzzleReader>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIPuzzleSolver_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IPuzzleSolver>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIPuzzleVisualizer_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IPuzzleVisualizer>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIRuleRepository_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IRuleRepository>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIDialogWrapper_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IDialogWrapper>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIJsonWrapper_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IJsonWrapper>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIFileWrapper_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IFileWrapper>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIDirectoryWrapper_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IDirectoryWrapper>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIApplicationWrapper_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IApplicationWrapper>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIHashiBrushResolver_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IHashiBrushResolver>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIDragDropService_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IDragDropService>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIResourceManager_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IResourceManager>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenISettingsProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<ISettingsProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenITimerProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<ITimerProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIIslandViewModelHelper_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IIslandViewModelHelper>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIIslandProviderCore_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IIslandProviderCore>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIIslandProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IIslandProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIHintProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IHintProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIRuleInfoProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IRuleInfoProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenITestSolutionProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<ITestSolutionProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIPathProvider_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IPathProvider>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIGameCompletionHandler_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IGameCompletionHandler>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenITestFieldService_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<ITestFieldService>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIMainViewModel_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IMainViewModel>().Should().NotBeNull();
    }

    #endregion

    #region Transient Service Resolution Tests

    [Test]
    public void Resolve_WhenISettingsViewModel_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<ISettingsViewModel>().Should().NotBeNull();
    }

    #endregion

    #region Singleton Identity Tests

    [Test]
    public void Resolve_WhenIHashiSolverResolvedTwice_ShouldReturnSameInstance()
    {
        var instance1 = serviceProvider.GetRequiredService<IHashiSolver>();
        var instance2 = serviceProvider.GetRequiredService<IHashiSolver>();
        instance1.Should().BeSameAs(instance2);
    }

    [Test]
    public void Resolve_WhenMultiInterfaceHashiSolver_ShouldReturnSameInstance()
    {
        var solver = serviceProvider.GetRequiredService<IHashiSolver>();
        var reader = serviceProvider.GetRequiredService<IPuzzleReader>();
        var puzzleSolver = serviceProvider.GetRequiredService<IPuzzleSolver>();
        var visualizer = serviceProvider.GetRequiredService<IPuzzleVisualizer>();

        solver.Should().BeSameAs(reader);
        solver.Should().BeSameAs(puzzleSolver);
        solver.Should().BeSameAs(visualizer);
    }

    #endregion

    #region Factory Resolution Tests

    [Test]
    public void Resolve_WhenIslandViewModelFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<int, int, int, IIslandViewModel>>();
        factory.Should().NotBeNull();
    }

    [Test]
    [Description("Full invocation of IslandViewModel requires WPF Application context (FrameworkElement, Brush resources). Factory delegate resolution is verified here.")]
    public void Resolve_WhenIslandViewModelFactory_ShouldReturnDelegate()
    {
        // Act
        var factory = serviceProvider.GetRequiredService<Func<int, int, int, IIslandViewModel>>();

        // Assert
        factory.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenSettingsViewModelFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<ISettingsViewModel>>();
        factory.Should().NotBeNull();
        factory().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenGeneratorIslandFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<int, int, int, IIsland>>();
        factory.Should().NotBeNull();
        var island = factory(3, 1, 2);
        island.Should().NotBeNull();
        island.AmountBridgesConnectable.Should().Be(3);
    }

    [Test]
    public void Resolve_WhenBridgeCoordinatesFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<Point, Point, int, IBridgeCoordinates>>();
        factory.Should().NotBeNull();
        var coords = factory(new Point(1, 2), new Point(3, 4), 1);
        coords.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenSolutionProviderFactory2Args_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<int[][], IReadOnlyList<IBridgeCoordinates>, ISolutionProvider>>();
        factory.Should().NotBeNull();
        var sp = factory([[1, 2]], new List<IBridgeCoordinates>());
        sp.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenSolutionProviderFactory3Args_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider>>();
        factory.Should().NotBeNull();
        var sp = factory(null, null, "test");
        sp.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenBridgeFactory_ShouldSucceed()
    {
        var islandFactory = serviceProvider.GetRequiredService<Func<int, int, int, IIsland>>();
        var bridgeFactory = serviceProvider.GetRequiredService<Func<IIsland, IIsland, int, IBridge>>();
        bridgeFactory.Should().NotBeNull();

        var island1 = islandFactory(2, 0, 0);
        var island2 = islandFactory(2, 0, 1);
        var bridge = bridgeFactory(island1, island2, 1);
        bridge.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenLinearSolverIslandFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<int, int, int, int, LinearSolver.Interfaces.Models.IIsland>>();
        factory.Should().NotBeNull();
        var island = factory(0, 1, 2, 3);
        island.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenEdgeFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<int, int, int, IEdge>>();
        factory.Should().NotBeNull();
        var edge = factory(0, 1, 2);
        edge.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenHashiPointFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<int, int, HashiPointTypeEnum, IHashiPoint>>();
        factory.Should().NotBeNull();
        var point = factory(1, 2, HashiPointTypeEnum.Normal);
        point.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenHashiBridgeFactory_ShouldSucceed()
    {
        var pointFactory = serviceProvider.GetRequiredService<Func<int, int, HashiPointTypeEnum, IHashiPoint>>();
        var bridgeFactory = serviceProvider.GetRequiredService<Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge>>();
        bridgeFactory.Should().NotBeNull();

        var p1 = pointFactory(0, 0, HashiPointTypeEnum.Normal);
        var p2 = pointFactory(0, 1, HashiPointTypeEnum.Normal);
        var bridge = bridgeFactory(BridgeOperationTypeEnum.Add, p1, p2);
        bridge.Should().NotBeNull();
    }

    #endregion

    #region Message Factory Resolution Tests

    [Test]
    public void Resolve_WhenDragDirectionChangedRequestTargetMessageFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<IIslandViewModel, DirectionEnum, IDragDirectionChangedRequestTargetMessage>>();
        factory.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenBridgeConnectionInformationContainerFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<BridgeOperationTypeEnum, IIslandViewModel, IIslandViewModel?, IBridgeConnectionInformationContainer>>();
        factory.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenUpdateAllIslandColorsMessageFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<bool?, IUpdateAllIslandColorsMessage>>();
        factory.Should().NotBeNull();
        factory(true).Should().NotBeNull();
        factory(null).Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenRuleMessageClearedMessageFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<bool?, IRuleMessageClearedMessage>>();
        factory.Should().NotBeNull();
        factory(true).Should().NotBeNull();
        factory(null).Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenSetTestSolutionMessageFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<ISolutionProvider, ISetTestSolutionMessage>>();
        factory.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenBridgeConnectionChangedMessageFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<IBridgeConnectionInformationContainer, IBridgeConnectionChangedMessage>>();
        factory.Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenAllConnectionsSetMessageFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<bool?, IAllConnectionsSetMessage>>();
        factory.Should().NotBeNull();
        factory(true).Should().NotBeNull();
        factory(null).Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIsTestModeRequestMessageFactory_ShouldSucceed()
    {
        var factory = serviceProvider.GetRequiredService<Func<IIsTestModeRequestMessage>>();
        factory.Should().NotBeNull();
        factory().Should().NotBeNull();
    }

    #endregion

    #region WPF-Dependent Service Tests (Mocked)

    [Test]
    public void Resolve_WhenIWindow_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IWindow>().Should().NotBeNull();
    }

    [Test]
    public void Resolve_WhenIViewBoxControl_ShouldSucceed()
    {
        serviceProvider.GetRequiredService<IViewBoxControl>().Should().NotBeNull();
    }

    #endregion
}
