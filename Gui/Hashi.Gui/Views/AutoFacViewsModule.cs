using Autofac;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;
using System.Windows;

namespace Hashi.Gui.Views;

/// <inheritdoc />
public class AutoFacViewsModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register your views here
        builder.RegisterType<HashiMainView>().As<IWindow<IMainViewModel>>().SingleInstance();
        builder.RegisterType<GenerateTestFieldView>().As<IWindow<IGenerateTestFieldViewModel>>().OnActivating(c =>
        {
            c.Instance.Owner = Application.Current.MainWindow;
        }).InstancePerDependency();
    }
}