using Autofac;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.LinearSolver.Models;

namespace Hashi.LinearSolver
{
    /// <inheritdoc />
    public class AutoFacLinearSolverModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            // Register your linear solver classes here
            builder.RegisterType<Bridge>().As<IBridge>().InstancePerDependency();
            builder.RegisterType<BridgePair>().As<IBridgePair>().InstancePerDependency();
            builder.RegisterType<Island>().As<IIsland>().InstancePerDependency();
            builder.RegisterType<Helper>().As<IHelper>().InstancePerDependency();
            builder.RegisterType<LinearSolutionSolverWithIterativ>().As<ILinearSolutionSolverWithIterativ>().SingleInstance();

            builder.Register<Func<IIsland, IIsland, IBridge>>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return (island1, island2) => c.Resolve<IBridge>(
                    new NamedParameter("island1", island1),
                    new NamedParameter("island2", island2));
            });

            builder.Register<Func<IList<int>, IList<int>, IHelper>>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return (islands, bridges) => c.Resolve<IHelper>(
                    new NamedParameter("islands", islands),
                    new NamedParameter("bridges", bridges));
            });

            builder.Register<Func<int, int, int, int, IBridgePair>>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return (bridge1Node1, bridge1Node2, bridge2Node1, bridge2Node2) => c.Resolve<IBridgePair>(
                    new NamedParameter("bridge1Node1", bridge1Node1),
                    new NamedParameter("bridge1Node2", bridge1Node2),
                    new NamedParameter("bridge2Node1", bridge2Node1),
                    new NamedParameter("bridge2Node2", bridge2Node2));
            });

            builder.Register<Func<int, int, int, int, IBridgePair>>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return (bridge1Node1, bridge1Node2, bridge2Node1, bridge2Node2) => c.Resolve<IBridgePair>(
                    new NamedParameter("bridge1Node1", bridge1Node1),
                    new NamedParameter("bridge1Node2", bridge1Node2),
                    new NamedParameter("bridge2Node1", bridge2Node1),
                    new NamedParameter("bridge2Node2", bridge2Node2));
            });

            builder.Register<Func<int, int, int, int, IIsland>>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return (value, y, x, number) => c.Resolve<IIsland>(
                    new NamedParameter("value", value),
                    new NamedParameter("y", y),
                    new NamedParameter("x", x),
                    new NamedParameter("number", number));
            });
        }
    }
}
