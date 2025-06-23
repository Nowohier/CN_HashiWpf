using Autofac;
using Hashi.Logging.Interfaces;

namespace Hashi.Logging;

/// <summary>
/// AutoFac module for registering logging services.
/// </summary>
public class AutoFacLoggingModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register logging services
        builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
    }
}