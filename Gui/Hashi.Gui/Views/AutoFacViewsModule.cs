using Autofac;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;

namespace Hashi.Gui.Views;

/// <inheritdoc />
public class AutoFacViewsModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register your views here
        builder.RegisterType<HashiMainView>().As<IWindow<IMainViewModel>>().SingleInstance();
    }
}