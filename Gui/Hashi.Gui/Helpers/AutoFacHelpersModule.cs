using Autofac;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Services;

namespace Hashi.Gui.Helpers;

/// <inheritdoc />
public class AutoFacHelpersModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HashiBrushResolver>().As<IHashiBrushResolver>().SingleInstance();
        builder.RegisterType<DragDropService>().As<IDragDropService>().SingleInstance();

        base.Load(builder);
    }
}