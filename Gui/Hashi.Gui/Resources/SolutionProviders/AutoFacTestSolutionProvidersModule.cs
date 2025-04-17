using Autofac;
using Hashi.Gui.Interfaces.Resources.SolutionProviders;

namespace Hashi.Gui.Resources.SolutionProviders;

/// <inheritdoc />
public class AutoFacTestSolutionProvidersModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TestSolutionProvider>().As<ITestSolutionProvider>().SingleInstance();
    }
}