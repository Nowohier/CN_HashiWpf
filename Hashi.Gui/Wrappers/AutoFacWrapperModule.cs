using Autofac;
using Hashi.Gui.Interfaces.Wrappers;

namespace Hashi.Gui.Wrappers
{
    public class AutoFacWrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register your wrappers here
            builder.RegisterType<DialogWrapper>().As<IDialogWrapper>().SingleInstance();
            builder.RegisterType<JsonWrapper>().As<IJsonWrapper>().SingleInstance();
        }
    }
}
