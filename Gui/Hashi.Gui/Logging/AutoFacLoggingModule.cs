using Autofac;
using Hashi.Gui.Interfaces.Logging;
using Hashi.Gui.Logging;

namespace Hashi.Gui.Logging;

/// <summary>
/// Autofac module for logging services.
/// </summary>
public class AutoFacLoggingModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<NLogLoggerFactory>().As<ILoggerFactory>().SingleInstance();
    }
}