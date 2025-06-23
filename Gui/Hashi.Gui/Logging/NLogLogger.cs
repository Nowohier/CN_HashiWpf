using Hashi.Gui.Interfaces.Logging;
using NLog;

namespace Hashi.Gui.Logging;

/// <summary>
/// NLog implementation of ILogger.
/// </summary>
public class NLogLogger : ILogger
{
    private readonly Logger _logger;

    public NLogLogger(Logger logger)
    {
        _logger = logger;
    }

    public void Trace(string message)
    {
        _logger.Trace(message);
    }

    public void Debug(string message)
    {
        _logger.Debug(message);
    }

    public void Info(string message)
    {
        _logger.Info(message);
    }

    public void Warn(string message)
    {
        _logger.Warn(message);
    }

    public void Error(string message)
    {
        _logger.Error(message);
    }

    public void Error(string message, Exception exception)
    {
        _logger.Error(exception, message);
    }

    public void Fatal(string message)
    {
        _logger.Fatal(message);
    }

    public void Fatal(string message, Exception exception)
    {
        _logger.Fatal(exception, message);
    }
}