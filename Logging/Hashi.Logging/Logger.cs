using System.Diagnostics;
using NLog;
using ILogger = Hashi.Logging.Interfaces.ILogger;

namespace Hashi.Logging;

/// <summary>
/// NLog implementation of ILogger.
/// </summary>
public class Logger : ILogger
{
    private readonly NLog.Logger logger;

    /// <summary>
    /// Initializes a new instance of the Logger class.
    /// </summary>
    /// <param name="logger">The NLog logger instance.</param>
    public Logger(NLog.Logger logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Trace(string message)
    {
        logger.Trace(message);
        System.Diagnostics.Debug.WriteLine($"TRACE|{logger.Name}|{message}");
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Debug(string message)
    {
        logger.Debug(message);
        System.Diagnostics.Debug.WriteLine($"DEBUG|{logger.Name}|{message}");
    }

    /// <summary>
    /// Logs an info message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Info(string message)
    {
        logger.Info(message);
        System.Diagnostics.Debug.WriteLine($"INFO|{logger.Name}|{message}");
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Warn(string message)
    {
        logger.Warn(message);
        System.Diagnostics.Debug.WriteLine($"WARN|{logger.Name}|{message}");
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Error(string message)
    {
        logger.Error(message);
        System.Diagnostics.Debug.WriteLine($"ERROR|{logger.Name}|{message}");
    }

    /// <summary>
    /// Logs an error message with exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Error(string message, Exception exception)
    {
        logger.Error(exception, message);
        System.Diagnostics.Debug.WriteLine($"ERROR|{logger.Name}|{message}|{exception}");
    }

    /// <summary>
    /// Logs a fatal message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Fatal(string message)
    {
        logger.Fatal(message);
        System.Diagnostics.Debug.WriteLine($"FATAL|{logger.Name}|{message}");
    }

    /// <summary>
    /// Logs a fatal message with exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Fatal(string message, Exception exception)
    {
        logger.Fatal(exception, message);
        System.Diagnostics.Debug.WriteLine($"FATAL|{logger.Name}|{message}|{exception}");
    }
}