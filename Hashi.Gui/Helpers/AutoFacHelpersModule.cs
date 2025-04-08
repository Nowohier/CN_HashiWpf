using Autofac;
using Hashi.Gui.Interfaces.Helpers;

namespace Hashi.Gui.Helpers
{
    /// <inheritdoc />
    public class AutoFacHelpersModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SolutionHelper>().As<ISolutionHelper>().SingleInstance();
        }
    }
}
