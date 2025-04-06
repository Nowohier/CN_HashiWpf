using Autofac;
using Hashi.Gui.ViewModels;
using Hashi.Gui.Views;

namespace Hashi.Gui.AutoFac
{
    /// <inheritdoc />
    public class AutoFacMainModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<AutoFacViewModelsModule>();
            builder.RegisterModule<AutoFacViewsModule>();
        }
    }
}
