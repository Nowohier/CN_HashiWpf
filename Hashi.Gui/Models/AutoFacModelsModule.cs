using Autofac;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Models
{
    public class AutoFacModelsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register your models here
            builder.RegisterType<HashiBrush>().As<IHashiBrush>().InstancePerDependency();
            builder.RegisterType<HashiPoint>().As<IHashiPoint>().InstancePerDependency();
        }
    }
}
