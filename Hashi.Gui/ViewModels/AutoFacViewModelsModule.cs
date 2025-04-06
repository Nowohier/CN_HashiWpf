using Autofac;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.ViewModels
{
    /// <inheritdoc />
    public class AutoFacViewModelsModule : Module
    {
        public delegate IIslandViewModel IslandFactory(int column, int row, int value);

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            // Register your view models here
            builder.RegisterType<IslandViewModel>().As<IIslandViewModel>().InstancePerDependency();
            builder.RegisterType<SettingsViewModel>().As<ISettingsViewModel>().InstancePerDependency();
            builder.RegisterType<HighScorePerDifficultyViewModel>().As<IHighScorePerDifficultyViewModel>()
                .InstancePerDependency();
            builder.RegisterType<ConnectionManagerViewModel>().As<IConnectionManagerViewModel>().SingleInstance();
            builder.RegisterType<MainViewModel>().As<IMainViewModel>().SingleInstance();

            builder.Register<Func<int, int, int, IIslandViewModel>>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return (column, row, value) => c.Resolve<IIslandViewModel>(new NamedParameter("x", column), new NamedParameter("y", row), new NamedParameter("maxConnections", value));
            });



        }
    }
}
