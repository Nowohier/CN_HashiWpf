namespace Hashi.Gui.Interfaces.Logging;

/// <summary>
/// Interface for logger factory.
/// </summary>
public interface ILoggerFactory
{
    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create logger for.</typeparam>
    /// <returns>A logger instance.</returns>
    ILogger CreateLogger<T>();

    /// <summary>
    /// Creates a logger with the specified name.
    /// </summary>
    /// <param name="name">The logger name.</param>
    /// <returns>A logger instance.</returns>
    ILogger CreateLogger(string name);
}