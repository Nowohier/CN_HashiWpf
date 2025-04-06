using Autofac;

namespace Hashi.Gui.Views
{
    /// <inheritdoc />
    public class AutoFacViewsModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            // Register your views here
            // builder.RegisterType<YourView>().As<IYourView>();
        }
    }
}
