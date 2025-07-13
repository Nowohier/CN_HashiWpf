using Autofac;
using FluentAssertions;
using System.Reflection;
using ILogger = Hashi.Logging.Interfaces.ILogger;
using ILoggerFactory = Hashi.Logging.Interfaces.ILoggerFactory;

namespace Hashi.Logging.Tests;

[TestFixture]
public class LoggerFactoryTests
{
    private LoggerFactory loggerFactory;
    private IContainer container;

    [SetUp]
    public void Setup()
    {
        loggerFactory = new LoggerFactory();

        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacLoggingModule>();
        container = builder.Build();
    }

    [TearDown]
    public void Teardown()
    {
        container.Dispose();
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var factory = new LoggerFactory();

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeOfType<LoggerFactory>();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldConfigureNLog()
    {
        // Arrange & Act
        // ReSharper disable once UnusedVariable
        var factory = new LoggerFactory();

        // Assert
        // Verify that NLog is configured by checking if configuration exists
        NLog.LogManager.Configuration.Should().NotBeNull();
    }

    #endregion

    #region CreateLogger<T> Tests

    [Test]
    public void CreateLogger_WhenGenericTypeProvided_ShouldReturnLoggerInstance()
    {
        // Act
        var logger = loggerFactory.CreateLogger<LoggerFactoryTests>();

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
        logger.Should().BeAssignableTo<ILogger>();
    }

    [Test]
    public void CreateLogger_WhenDifferentGenericTypes_ShouldReturnDifferentLoggers()
    {
        // Act
        var logger1 = loggerFactory.CreateLogger<LoggerFactoryTests>();
        var logger2 = loggerFactory.CreateLogger<LoggerFactory>();

        // Assert
        logger1.Should().NotBeNull();
        logger2.Should().NotBeNull();
        logger1.Should().NotBeSameAs(logger2);
    }

    [Test]
    public void CreateLogger_WhenSameGenericType_ShouldReturnDifferentInstances()
    {
        // Act
        var logger1 = loggerFactory.CreateLogger<LoggerFactoryTests>();
        var logger2 = loggerFactory.CreateLogger<LoggerFactoryTests>();

        // Assert
        logger1.Should().NotBeNull();
        logger2.Should().NotBeNull();
        logger1.Should().NotBeSameAs(logger2);
    }

    [Test]
    public void CreateLogger_WhenClassWithNamespace_ShouldReturnValidLogger()
    {
        // Act
        var logger = loggerFactory.CreateLogger<List<string>>();

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
    }

    #endregion

    #region CreateLogger(string) Tests

    [Test]
    public void CreateLogger_WhenStringNameProvided_ShouldReturnLoggerInstance()
    {
        // Arrange
        var loggerName = "TestLogger";

        // Act
        var logger = loggerFactory.CreateLogger(loggerName);

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
        logger.Should().BeAssignableTo<ILogger>();
    }

    [Test]
    public void CreateLogger_WhenEmptyStringProvided_ShouldReturnLoggerInstance()
    {
        // Arrange
        var loggerName = "";

        // Act
        var logger = loggerFactory.CreateLogger(loggerName);

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
    }

    [Test]
    public void CreateLogger_WhenNullStringProvided_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? loggerName = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => loggerFactory.CreateLogger(loggerName!));
        exception.Should().NotBeNull();
    }

    [Test]
    public void CreateLogger_WhenDifferentNames_ShouldReturnDifferentLoggers()
    {
        // Arrange
        var name1 = "Logger1";
        var name2 = "Logger2";

        // Act
        var logger1 = loggerFactory.CreateLogger(name1);
        var logger2 = loggerFactory.CreateLogger(name2);

        // Assert
        logger1.Should().NotBeNull();
        logger2.Should().NotBeNull();
        logger1.Should().NotBeSameAs(logger2);
    }

    [Test]
    public void CreateLogger_WhenSameName_ShouldReturnDifferentInstances()
    {
        // Arrange
        var loggerName = "TestLogger";

        // Act
        var logger1 = loggerFactory.CreateLogger(loggerName);
        var logger2 = loggerFactory.CreateLogger(loggerName);

        // Assert
        logger1.Should().NotBeNull();
        logger2.Should().NotBeNull();
        logger1.Should().NotBeSameAs(logger2);
    }

    [Test]
    public void CreateLogger_WhenSpecialCharactersInName_ShouldReturnValidLogger()
    {
        // Arrange
        var loggerName = "Test.Logger-With_Special@Characters#123";

        // Act
        var logger = loggerFactory.CreateLogger(loggerName);

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
    }

    #endregion

    #region Interface Implementation Tests

    [Test]
    public void LoggerFactory_ShouldImplementILoggerFactory()
    {
        // Arrange & Act
        var factory = new LoggerFactory();

        // Assert
        factory.Should().BeAssignableTo<ILoggerFactory>();
    }

    #endregion

    #region Configuration Tests

    [Test]
    public void ConfigureNLog_WhenCalledMultipleTimes_ShouldOnlyConfigureOnce()
    {
        // Arrange
        // ReSharper disable once UnusedVariable
        var factory1 = new LoggerFactory();
        var initialConfig = NLog.LogManager.Configuration;

        // Act
        // ReSharper disable once UnusedVariable
        var factory2 = new LoggerFactory();
        var secondConfig = NLog.LogManager.Configuration;

        // Assert
        initialConfig.Should().BeSameAs(secondConfig);
    }

    [Test]
    public void GetLogsDirectory_WhenCalled_ShouldReturnValidPath()
    {
        // Arrange
        var method = typeof(LoggerFactory).GetMethod("GetLogsDirectory",
            BindingFlags.NonPublic | BindingFlags.Static);
        method.Should().NotBeNull();

        // Act
        var logsDirectory = (string)method?.Invoke(null, null)!;

        // Assert
        logsDirectory.Should().NotBeNullOrEmpty();
        logsDirectory.Should().Contain("CN_Hashi");
        logsDirectory.Should().Contain("Settings");
    }

    [Test]
    public void ConfigureNLog_ShouldSetupFileTarget()
    {
        // Arrange & Act
        // ReSharper disable once UnusedVariable
        var factory = new LoggerFactory();

        // Assert
        var config = NLog.LogManager.Configuration;
        config.Should().NotBeNull();
        config.AllTargets.Should().NotBeEmpty();
        config.AllTargets.Should().Contain(t => t.Name == "fileTarget");
    }

    [Test]
    public void ConfigureNLog_ShouldSetupLoggingRules()
    {
        // Arrange & Act
        // ReSharper disable once UnusedVariable
        var factory = new LoggerFactory();

        // Assert
        var config = NLog.LogManager.Configuration;
        config.Should().NotBeNull();
        config.LoggingRules.Should().NotBeEmpty();
    }

    #endregion

    #region Integration Tests

    [Test]
    public void CreateLogger_WhenUsedWithAutofac_ShouldWork()
    {
        // Arrange
        var factory = container.Resolve<ILoggerFactory>();

        // Act
        var logger = factory.CreateLogger<LoggerFactoryTests>();

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeAssignableTo<ILogger>();
    }

    [Test]
    public void CreateLogger_WhenCreatedLoggersUsed_ShouldLogCorrectly()
    {
        // Arrange
        var logger1 = loggerFactory.CreateLogger("TestLogger1");
        var logger2 = loggerFactory.CreateLogger<LoggerFactoryTests>();

        // Act & Assert (No exceptions should be thrown)
        Assert.DoesNotThrow(() =>
        {
            logger1.Info("Test message from logger1");
            logger2.Debug("Test message from logger2");
        });
    }

    [Test]
    public void LoggerFactory_WhenMultipleInstances_ShouldShareConfiguration()
    {
        // Arrange
        var factory1 = new LoggerFactory();
        var factory2 = new LoggerFactory();

        // Act
        var logger1 = factory1.CreateLogger("Test1");
        var logger2 = factory2.CreateLogger("Test2");

        // Assert
        logger1.Should().NotBeNull();
        logger2.Should().NotBeNull();

        // Both should be able to log without issues
        Assert.DoesNotThrow(() =>
        {
            logger1.Info("Message from factory 1");
            logger2.Info("Message from factory 2");
        });
    }

    #endregion

    #region Edge Cases Tests

    [Test]
    public void CreateLogger_WhenVeryLongName_ShouldReturnValidLogger()
    {
        // Arrange
        var longName = new string('A', 1000);

        // Act
        var logger = loggerFactory.CreateLogger(longName);

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
    }

    [Test]
    public void CreateLogger_WhenUnicodeCharacters_ShouldReturnValidLogger()
    {
        // Arrange
        var unicodeName = "TestLogger_🚀_测试_🎯";

        // Act
        var logger = loggerFactory.CreateLogger(unicodeName);

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
    }

    #endregion
}