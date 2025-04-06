using Autofac;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc />
    public class AutoFacMessagesModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AllConnectionsSetMessage>().As<IAllConnectionsSetMessage>();
            builder.RegisterType<BridgeConnectionChangedMessage>().As<IBridgeConnectionChangedMessage>();
            builder.RegisterType<CurrentSourceIslandChangedMessage>().As<ICurrentSourceIslandChangedMessage>();
            builder.RegisterType<PotentialTargetIslandChangedMessage>().As<IPotentialTargetIslandChangedMessage>();
            builder.RegisterType<UpdateAllIslandColorsMessage>().As<IUpdateAllIslandColorsMessage>();
        }
    }
}
