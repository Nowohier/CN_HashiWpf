using System.Drawing;
using Autofac;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Generator.Services;
using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Core.Providers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Logging;
using Hashi.Logging.Interfaces;

namespace Hashi.Generator;

/// <inheritdoc />
public class AutoFacGeneratorModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HashiGenerator>().As<IHashiGenerator>().SingleInstance();
        builder.RegisterType<DifficultySettingsProvider>().As<IDifficultySettingsProvider>().SingleInstance();
        builder.RegisterType<BlockDetectionService>().As<IBlockDetectionService>().SingleInstance();
        builder.RegisterType<IslandLayoutService>().As<IIslandLayoutService>().SingleInstance();
        builder.RegisterType<BridgeManagementService>().As<IBridgeManagementService>().SingleInstance();
        builder.RegisterType<RuleSolvabilityValidator>().As<IRuleSolvabilityValidator>().SingleInstance();
        builder.RegisterType<IslandViewModelHelper>().As<IIslandViewModelHelper>().SingleInstance().IfNotRegistered(typeof(IIslandViewModelHelper));
        builder.RegisterType<IslandProviderCore>().As<IIslandProviderCore>().SingleInstance().IfNotRegistered(typeof(IIslandProviderCore));
        builder.RegisterType<Island>().As<IIsland>().InstancePerDependency();
        builder.RegisterType<Bridge>().As<IBridge>().InstancePerDependency();
        builder.RegisterType<BridgeCoordinates>().As<IBridgeCoordinates>().InstancePerDependency();
        builder.RegisterType<SolutionProvider>().As<ISolutionProvider>().InstancePerDependency();

        builder.Register<Func<int, int, int, IIsland>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (amountBridgesConnectable, row, column) => c.Resolve<IIsland>(
                new NamedParameter("amountBridgesConnectable", amountBridgesConnectable), new NamedParameter("y", row),
                new NamedParameter("x", column));
        });

        builder.Register<Func<Point, Point, int, IBridgeCoordinates>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (location1, location2, amountBridges) => c.Resolve<IBridgeCoordinates>(
                new NamedParameter("location1", location1), new NamedParameter("location2", location2),
                new NamedParameter("amountBridges", amountBridges));
        });

        builder.Register<Func<int[][], List<IBridgeCoordinates>, ISolutionProvider>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (hashiField, bridgeCoordinates) => c.Resolve<ISolutionProvider>(
                new NamedParameter("hashiField", hashiField),
                new NamedParameter("bridgeCoordinates", bridgeCoordinates));
        });

        builder.Register<Func<IIsland, IIsland, int, IBridge>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (island1, island2, amountBridgesSet) => c.Resolve<IBridge>(new NamedParameter("island1", island1),
                new NamedParameter("island2", island2), new NamedParameter("amountBridgesSet", amountBridgesSet));
        });
    }
}