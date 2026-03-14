using Hashi.Enums;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Messages.MessageContainers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Messages.MessageContainers;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Messages.Extensions;

/// <summary>
///     Extension methods for registering message services.
/// </summary>
public static class MessageServiceExtensions
{
    /// <summary>
    ///     Adds message services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMessageServices(this IServiceCollection services)
    {
        services.AddTransient<IAllConnectionsSetMessage, AllConnectionsSetMessage>();
        services.AddTransient<IBridgeConnectionInformationContainer, BridgeConnectionInformationContainer>();
        services.AddTransient<IBridgeConnectionChangedMessage, BridgeConnectionChangedMessage>();
        services.AddTransient<IUpdateAllIslandColorsMessage, UpdateAllIslandColorsMessage>();
        services.AddTransient<IRuleMessageClearedMessage, RuleMessageClearedMessage>();
        services.AddTransient<IIsTestModeRequestMessage, IsTestModeRequestMessage>();
        services.AddTransient<IDragDirectionChangedRequestTargetMessage, DragDirectionChangedRequestTargetMessage>();
        services.AddTransient<ISetTestSolutionMessage, SetTestSolutionMessage>();

        services.AddSingleton<Func<IIslandViewModel, DirectionEnum, IDragDirectionChangedRequestTargetMessage>>(_ =>
            (source, direction) => new DragDirectionChangedRequestTargetMessage(source, direction));

        services
            .AddSingleton<Func<BridgeOperationTypeEnum, IIslandViewModel, IIslandViewModel?,
                IBridgeConnectionInformationContainer>>(_ =>
                (bridgeOperationType, sourceIsland, targetIsland) =>
                    new BridgeConnectionInformationContainer(bridgeOperationType, sourceIsland, targetIsland));

        services.AddSingleton<Func<bool?, IUpdateAllIslandColorsMessage>>(_ =>
            value => new UpdateAllIslandColorsMessage(value));

        services.AddSingleton<Func<bool?, IRuleMessageClearedMessage>>(_ =>
            value => new RuleMessageClearedMessage(value));

        services.AddSingleton<Func<ISolutionProvider, ISetTestSolutionMessage>>(_ =>
            value => new SetTestSolutionMessage(value));

        services.AddSingleton<Func<IBridgeConnectionInformationContainer, IBridgeConnectionChangedMessage>>(_ =>
            islandInfos => new BridgeConnectionChangedMessage(islandInfos));

        services.AddSingleton<Func<bool?, IAllConnectionsSetMessage>>(_ =>
            value => new AllConnectionsSetMessage(value));

        services.AddSingleton<Func<IIsTestModeRequestMessage>>(_ =>
            () => new IsTestModeRequestMessage());

        return services;
    }
}
