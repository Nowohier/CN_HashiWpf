using Autofac;
using Hashi.Gui.Interfaces.Managers;

namespace Hashi.Gui.Managers
{
    public class AutoFacManagersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ResourceManager>().As<IResourceManager>().SingleInstance();
        }
    }
}
