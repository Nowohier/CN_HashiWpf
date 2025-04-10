using Autofac;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages;

/// <inheritdoc />
public class AutoFacMessagesModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register your messages here
        builder.RegisterType<AllConnectionsSetMessage>().As<IAllConnectionsSetMessage>().InstancePerDependency();
        builder.RegisterType<BridgeConnectionChangedMessage>().As<IBridgeConnectionChangedMessage>()
            .InstancePerDependency();
        builder.RegisterType<DropTargetIslandChangedMessage>().As<IDropTargetIslandChangedMessage>()
            .InstancePerDependency();
        builder.RegisterType<UpdateAllIslandColorsMessage>().As<IUpdateAllIslandColorsMessage>()
            .InstancePerDependency();
        builder.RegisterType<AllIslandsRequestMessage>().As<IAllIslandsRequestMessage>().InstancePerDependency();
    }
}