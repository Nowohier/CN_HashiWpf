using NLog;
using System.Diagnostics;
using System.Reflection;

namespace Hashi.Generator.Logging;

/// <summary>
/// Simple logger wrapper for Generator using NLog.
/// </summary>
public static class Logger
{
    private static readonly Lazy<NLog.Logger> logger = new Lazy<NLog.Logger>(CreateLogger);

    private static NLog.Logger CreateLogger()
    {
        // Configure NLog if not already configured
        if (LogManager.Configuration == null)
        {
            var config = new NLog.Config.LoggingConfiguration();
            
            var fileTarget = new NLog.Targets.FileTarget("fileTarget")
            {
                FileName = Path.Combine(GetLogsDirectory(), "${date:format=yyyyddMM}.hashi_log.txt"),
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
                ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Date,
                MaxArchiveFiles = 30,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                CreateDirs = true
            };

            config.AddTarget(fileTarget);

            // Configure logging rules based on build configuration
            if (IsDebugBuild())
            {
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, fileTarget);
            }
            else
            {
                config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, fileTarget);
            }

            LogManager.Configuration = config;
        }

        return LogManager.GetLogger("Hashi.Generator");
    }

    private static string GetLogsDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "CN_Hashi", "Settings");
    }

    private static bool IsDebugBuild()
    {
        return Assembly.GetExecutingAssembly().GetCustomAttributes(false)
            .OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled);
    }

    public static void Trace(string message) => logger.Value.Trace(message);
    public static void Debug(string message) => logger.Value.Debug(message);
    public static void Info(string message) => logger.Value.Info(message);
    public static void Warn(string message) => logger.Value.Warn(message);
    public static void Error(string message) => logger.Value.Error(message);
    public static void Error(string message, Exception exception) => logger.Value.Error(exception, message);
    public static void Fatal(string message) => logger.Value.Fatal(message);
    public static void Fatal(string message, Exception exception) => logger.Value.Fatal(exception, message);
}