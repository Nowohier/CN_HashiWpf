using Autofac;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;

namespace Hashi.Generator;

/// <inheritdoc />
public class AutoFacGeneratorModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HashiGenerator>().As<IHashiGenerator>().SingleInstance();
        builder.RegisterType<Island>().As<IIsland>().InstancePerDependency();
        builder.RegisterType<Bridge>().As<IBridge>().InstancePerDependency();

        builder.Register<Func<int, int, int, IIsland>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (amountBridgesConnectable, row, column) => c.Resolve<IIsland>(
                new NamedParameter("amountBridgesConnectable", amountBridgesConnectable), new NamedParameter("y", row),
                new NamedParameter("x", column));
        });

        builder.Register<Func<IIsland, IIsland, int, IBridge>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (island1, island2, amountBridgesSet) => c.Resolve<IBridge>(new NamedParameter("island1", island1),
                new NamedParameter("island2", island2), new NamedParameter("amountBridgesSet", amountBridgesSet));
        });
    }
}