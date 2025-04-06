using Autofac;
using Hashi.Gui.Messages;
using Hashi.Gui.Models;
using Hashi.Gui.ViewModels;
using Hashi.Gui.Views;
using Hashi.Gui.Wrappers;

namespace Hashi.Gui.AutoFac
{
    /// <inheritdoc />
    public class AutoFacMainModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            // Register your modules here
            builder.RegisterModule<AutoFacViewModelsModule>();
            builder.RegisterModule<AutoFacViewsModule>();
            builder.RegisterModule<AutoFacMessagesModule>();
            builder.RegisterModule<AutoFacModelsModule>();
            builder.RegisterModule<AutoFacWrapperModule>();
        }
    }
}
