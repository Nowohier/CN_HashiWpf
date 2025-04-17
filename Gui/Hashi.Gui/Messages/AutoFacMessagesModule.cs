using Autofac;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Messages.MessageContainers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Messages.MessageContainers;

namespace Hashi.Gui.Messages;

/// <inheritdoc />
public class AutoFacMessagesModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register your messages here
        builder.RegisterType<AllConnectionsSetMessage>().As<IAllConnectionsSetMessage>().InstancePerDependency();
        builder.RegisterType<BridgeConnectionInformationContainer>().As<IBridgeConnectionInformationContainer>()
            .InstancePerDependency();
        builder.RegisterType<BridgeConnectionChangedMessage>().As<IBridgeConnectionChangedMessage>()
            .InstancePerDependency();
        builder.RegisterType<UpdateAllIslandColorsMessage>().As<IUpdateAllIslandColorsMessage>()
            .InstancePerDependency();
        builder.RegisterType<RuleMessageClearedMessage>().As<IRuleMessageClearedMessage>().InstancePerDependency();
        builder.RegisterType<IsTestModeRequestMessage>().As<IIsTestModeRequestMessage>().InstancePerDependency();
        builder.RegisterType<DragDirectionChangedRequestTargetMessage>().As<IDragDirectionChangedRequestTargetMessage>()
            .InstancePerDependency();

        builder.Register<Func<IIslandViewModel, DirectionEnum, IDragDirectionChangedRequestTargetMessage>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (source, direction) => c.Resolve<IDragDirectionChangedRequestTargetMessage>(
                new NamedParameter("source", source),
                new NamedParameter("direction", direction));
        });

        builder
            .Register<Func<BridgeOperationTypeEnum, IIslandViewModel, IIslandViewModel?,
                IBridgeConnectionInformationContainer>>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return (bridgeOperationType, sourceIsland, targetIsland) =>
                    c.Resolve<IBridgeConnectionInformationContainer>(
                        new NamedParameter("bridgeOperationType", bridgeOperationType),
                        new NamedParameter("sourceIsland", sourceIsland),
                        new NamedParameter("targetIsland", targetIsland));
            });

        builder.Register<Func<bool?, IUpdateAllIslandColorsMessage>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return value => c.Resolve<IUpdateAllIslandColorsMessage>(
                new NamedParameter("value", value));
        });

        builder.Register<Func<bool?, IRuleMessageClearedMessage>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return value => c.Resolve<IRuleMessageClearedMessage>(
                new NamedParameter("value", value));
        });

        builder.Register<Func<IBridgeConnectionInformationContainer, IBridgeConnectionChangedMessage>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return islandInfos => c.Resolve<IBridgeConnectionChangedMessage>(
                new NamedParameter("islandInfos", islandInfos));
        });

        builder.Register<Func<bool?, IAllConnectionsSetMessage>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return value => c.Resolve<IAllConnectionsSetMessage>(
                new NamedParameter("value", value));
        });
    }
}