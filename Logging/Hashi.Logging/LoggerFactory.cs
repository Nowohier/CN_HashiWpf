using System.Diagnostics;
using System.Reflection;
using Hashi.Logging.Interfaces;
using NLog;
using NLog.Config;
using NLog.Targets;
using ILogger = Hashi.Logging.Interfaces.ILogger;

namespace Hashi.Logging;

/// <summary>
/// NLog implementation of ILoggerFactory.
/// </summary>
public class LoggerFactory : ILoggerFactory
{
    private static bool isConfigured = false;
    private static readonly object lockObject = new object();

    /// <summary>
    /// Initializes a new instance of the LoggerFactory class.
    /// </summary>
    public LoggerFactory()
    {
        ConfigureNLog();
    }

    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create logger for.</typeparam>
    /// <returns>A logger instance.</returns>
    public ILogger CreateLogger<T>()
    {
        return CreateLogger(typeof(T).FullName ?? typeof(T).Name);
    }

    /// <summary>
    /// Creates a logger with the specified name.
    /// </summary>
    /// <param name="name">The logger name.</param>
    /// <returns>A logger instance.</returns>
    public ILogger CreateLogger(string name)
    {
        var nlogLogger = LogManager.GetLogger(name);
        return new Logger(nlogLogger);
    }

    private void ConfigureNLog()
    {
        lock (lockObject)
        {
            if (isConfigured)
                return;

            var config = new LoggingConfiguration();

            // Create file target
            var fileTarget = new FileTarget("fileTarget")
            {
                FileName = Path.Combine(GetLogsDirectory(), "${date:format=yyyyddMM}.hashi_log.txt"),
                Layout = "${longdate}\t${level:uppercase=true}\t${logger}\t${message}",
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                MaxArchiveFiles = 30,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                CreateDirs = true
            };

            config.AddTarget(fileTarget);

            // Configure logging rules based on build configuration
            if (Debugger.IsAttached)
            {
                // Debug build: log from Trace level onwards
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
            }
            else
            {
                // Release build: log from Info level onwards
                config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            }

            LogManager.Configuration = config;
            isConfigured = true;
        }
    }

    private static string GetLogsDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "CN_Hashi", "Settings");
    }

}