using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Logging;
using Hashi.Gui.Interfaces.Providers;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.IO;
using ILogger = Hashi.Gui.Interfaces.Logging.ILogger;

namespace Hashi.Gui.Logging;

/// <summary>
/// NLog implementation of ILoggerFactory.
/// </summary>
public class NLogLoggerFactory : ILoggerFactory
{
    private readonly IPathProvider _pathProvider;
    private static bool _isConfigured = false;
    private static readonly object _lock = new object();

    public NLogLoggerFactory(IPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
        ConfigureNLog();
    }

    public ILogger CreateLogger<T>()
    {
        return CreateLogger(typeof(T).FullName ?? typeof(T).Name);
    }

    public ILogger CreateLogger(string name)
    {
        var nlogLogger = LogManager.GetLogger(name);
        return new NLogLogger(nlogLogger);
    }

    private void ConfigureNLog()
    {
        lock (_lock)
        {
            if (_isConfigured)
                return;

            var config = new LoggingConfiguration();

            // Create file target
            var fileTarget = new FileTarget("fileTarget")
            {
                FileName = Path.Combine(_pathProvider.SettingsDirectoryPath, "${date:format=yyyyddMM}.hashi_log.txt"),
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                MaxArchiveFiles = 30,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                CreateDirs = true
            };

            config.AddTarget(fileTarget);

            // Configure logging rules based on build configuration
            if (DebugHelper.IsDebugBuild)
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
            _isConfigured = true;
        }
    }
}