using Autofac;
using Hashi.Gui.Interfaces.Views;

namespace Hashi.Gui.Views
{
    /// <inheritdoc />
    public class AutoFacViewsModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            // Register your views here
            builder.RegisterType<HashiMainView>().As<IHashiMainView>().SingleInstance();

            //builder.Register<Func<IMainViewModel, IHashiMainView>>(context =>
            //{
            //    var c = context.Resolve<IComponentContext>();
            //    return (mainViewModel) => c.Resolve<IHashiMainView>(new NamedParameter("mainViewModel", mainViewModel));
            //});
        }
    }
}
