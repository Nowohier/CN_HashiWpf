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
        builder.RegisterType<CurrentSourceIslandChangedMessage>().As<ICurrentSourceIslandChangedMessage>()
            .InstancePerDependency();
        builder.RegisterType<PotentialTargetIslandChangedMessage>().As<IPotentialTargetIslandChangedMessage>()
            .InstancePerDependency();
        builder.RegisterType<UpdateAllIslandColorsMessage>().As<IUpdateAllIslandColorsMessage>()
            .InstancePerDependency();
    }
}