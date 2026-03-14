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
    public void Trace(string message) => Log(logger.Trace, "TRACE", message);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Debug(string message) => Log(logger.Debug, "DEBUG", message);

    /// <summary>
    /// Logs an info message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Info(string message) => Log(logger.Info, "INFO", message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Warn(string message) => Log(logger.Warn, "WARN", message);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Error(string message) => Log(logger.Error, "ERROR", message);

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
    public void Fatal(string message) => Log(logger.Fatal, "FATAL", message);

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

    /// <summary>
    /// Logs a message using the specified log action and writes to the debug output.
    /// </summary>
    /// <param name="logAction">The NLog log action to invoke.</param>
    /// <param name="prefix">The log level prefix for debug output.</param>
    /// <param name="message">The message to log.</param>
    private void Log(Action<string> logAction, string prefix, string message)
    {
        logAction(message);
        System.Diagnostics.Debug.WriteLine($"{prefix}|{logger.Name}|{message}");
    }
}