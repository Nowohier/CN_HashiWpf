using Autofac;

namespace Hashi.Gui.ViewModels
{
    /// <inheritdoc />
    public class AutoFacViewModelsModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            // Register your view models here
            // builder.RegisterType<YourViewModel>().As<IYourViewModel>();
        }
    }
}
