using Autofac;
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
        builder.RegisterType<HashiMainView>().As<IHashiMainView>().SingleInstance();
        builder.RegisterType<GenerateTestFieldView>().As<IGenerateTestFieldView>().OnActivating(c =>
        {
            c.Instance.Owner = Application.Current.MainWindow;
        }).InstancePerDependency();
    }
}