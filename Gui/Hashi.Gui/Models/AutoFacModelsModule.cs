using Autofac;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Models;

public class AutoFacModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register your models here
        builder.RegisterType<HashiBrush>().As<IHashiBrush>().InstancePerDependency();
        builder.RegisterType<HashiPoint>().As<IHashiPoint>().InstancePerDependency();
        builder.RegisterType<HashiBridge>().As<IHashiBridge>().InstancePerDependency();

        builder.Register<Func<int, int, HashiPointTypeEnum, IHashiPoint>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (x, y, pointType) => c.Resolve<IHashiPoint>(
                new NamedParameter("x", x),
                new NamedParameter("y", y),
                new NamedParameter("pointType", pointType));
        });

        builder.Register<Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (actionType, point1, point2) => c.Resolve<IHashiBridge>(
                new NamedParameter("actionType", actionType),
                new NamedParameter("point1", point1),
                new NamedParameter("point2", point2));
        });
    }
}