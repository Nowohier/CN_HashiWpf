using Autofac;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.LinearSolver.Models;
using Hashi.Logging;
using Hashi.Logging.Interfaces;

namespace Hashi.LinearSolver;

/// <inheritdoc />
public class AutoFacLinearSolverModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HashiSolver>().As<IHashiSolver>().SingleInstance();
        builder.RegisterType<Island>().As<IIsland>().InstancePerDependency();
        builder.RegisterType<Edge>().As<IEdge>().InstancePerDependency();

        builder.Register<Func<int, int, int, int, IIsland>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (id, row, col, bridgesRequired) => c.Resolve<IIsland>(
                new NamedParameter("id", id),
                new NamedParameter("row", row),
                new NamedParameter("col", col),
                new NamedParameter("bridgesRequired", bridgesRequired));
        });

        builder.Register<Func<int, int, int, IEdge>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (id, a, b) => c.Resolve<IEdge>(
                new NamedParameter("id", id),
                new NamedParameter("a", a),
                new NamedParameter("b", b));
        });
    }
}