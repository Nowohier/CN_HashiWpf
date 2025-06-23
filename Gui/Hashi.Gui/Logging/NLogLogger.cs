using Hashi.Gui.Interfaces.Logging;
using NLog;

namespace Hashi.Gui.Logging;

/// <summary>
/// NLog implementation of ILogger.
/// </summary>
public class NLogLogger : ILogger
{
    private readonly Logger logger;

    public NLogLogger(Logger logger)
    {
        this.logger = logger;
    }

    public void Trace(string message)
    {
        logger.Trace(message);
    }

    public void Debug(string message)
    {
        logger.Debug(message);
    }

    public void Info(string message)
    {
        logger.Info(message);
    }

    public void Warn(string message)
    {
        logger.Warn(message);
    }

    public void Error(string message)
    {
        logger.Error(message);
    }

    public void Error(string message, Exception exception)
    {
        logger.Error(exception, message);
    }

    public void Fatal(string message)
    {
        logger.Fatal(message);
    }

    public void Fatal(string message, Exception exception)
    {
        logger.Fatal(exception, message);
    }
}