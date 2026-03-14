using FluentAssertions;
using NLog;
using ILogger = Hashi.Logging.Interfaces.ILogger;

namespace Hashi.Logging.Tests;

[TestFixture]
public class LoggerTests
{
    private Logger logger;

    [SetUp]
    public void Setup()
    {
        // Create a real NLog logger for testing
        var nlogLogger = LogManager.GetLogger("TestLogger");
        logger = new Logger(nlogLogger);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenNLogLoggerProvided_ShouldCreateInstance()
    {
        // Arrange
        var nlogLogger = LogManager.GetLogger("TestLogger");

        // Act
        var logger1 = new Logger(nlogLogger);

        // Assert
        logger1.Should().NotBeNull();
        logger1.Should().BeOfType<Logger>();
    }

    [Test]
    public void Constructor_WhenNullLoggerProvided_ShouldNotThrowButMayFailAtRuntime()
    {
        // Act & Assert
        // Note: This will create a logger but may fail when methods are called
        var logger1 = new Logger(null!);
        logger1.Should().NotBeNull();
    }

    #endregion

    #region Trace Tests

    [Test]
    public void Trace_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test trace message";

        // Act
        var act = () => logger.Trace(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Trace_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act
        var act = () => logger.Trace(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Trace_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act
        var act = () => logger.Trace(message!);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Debug Tests

    [Test]
    public void Debug_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test debug message";

        // Act
        var act = () => logger.Debug(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Debug_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act
        var act = () => logger.Debug(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Debug_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act
        var act = () => logger.Debug(message!);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Info Tests

    [Test]
    public void Info_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test info message";

        // Act
        var act = () => logger.Info(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Info_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act
        var act = () => logger.Info(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Info_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act
        var act = () => logger.Info(message!);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Warn Tests

    [Test]
    public void Warn_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test warn message";

        // Act
        var act = () => logger.Warn(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Warn_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act
        var act = () => logger.Warn(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Warn_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act
        var act = () => logger.Warn(message!);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Error Tests

    [Test]
    public void Error_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var act = () => logger.Error(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Error_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act
        var act = () => logger.Error(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Error_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act
        var act = () => logger.Error(message!);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Error_WhenMessageAndExceptionProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test error message with exception";
        var exception = new InvalidOperationException("Test exception");

        // Act
        var act = () => logger.Error(message, exception);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Error_WhenMessageAndNullException_ShouldNotThrow()
    {
        // Arrange
        var message = "Test error message with null exception";
        Exception? exception = null;

        // Act
        var act = () => logger.Error(message, exception!);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Fatal Tests

    [Test]
    public void Fatal_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test fatal message";

        // Act
        var act = () => logger.Fatal(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Fatal_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act
        var act = () => logger.Fatal(message);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Fatal_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act
        var act = () => logger.Fatal(message!);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Fatal_WhenMessageAndExceptionProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test fatal message with exception";
        var exception = new InvalidOperationException("Test exception");

        // Act
        var act = () => logger.Fatal(message, exception);

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Fatal_WhenMessageAndNullException_ShouldNotThrow()
    {
        // Arrange
        var message = "Test fatal message with null exception";
        Exception? exception = null;

        // Act
        var act = () => logger.Fatal(message, exception!);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Interface Implementation Tests

    [Test]
    public void Logger_ShouldImplementILogger()
    {
        // Arrange & Act
        var nlogLogger = LogManager.GetLogger("TestLogger");
        var logger1 = new Logger(nlogLogger);

        // Assert
        logger1.Should().BeAssignableTo<ILogger>();
    }

    #endregion

    #region Integration Tests

    [Test]
    public void Logger_WhenCalledMultipleTimes_ShouldWorkCorrectly()
    {
        // Arrange
        var traceMessage = "Trace message";
        var debugMessage = "Debug message";
        var infoMessage = "Info message";
        var warnMessage = "Warn message";
        var errorMessage = "Error message";
        var fatalMessage = "Fatal message";

        // Act
        var act = () =>
        {
            logger.Trace(traceMessage);
            logger.Debug(debugMessage);
            logger.Info(infoMessage);
            logger.Warn(warnMessage);
            logger.Error(errorMessage);
            logger.Fatal(fatalMessage);
        };

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Logger_WhenCreatedFromFactory_ShouldWorkCorrectly()
    {
        // Arrange
        var factory = new LoggerFactory();
        var factoryLogger = factory.CreateLogger<LoggerTests>();

        // Act
        var act = () =>
        {
            factoryLogger.Info("Test message from factory logger");
            factoryLogger.Error("Test error from factory logger");
        };

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Logger_WhenLoggingWithExceptions_ShouldHandleAllCases()
    {
        // Arrange
        var simpleException = new Exception("Simple exception");
        var complexException = new InvalidOperationException("Complex exception",
            new ArgumentException("Inner exception"));

        // Act
        var act = () =>
        {
            logger.Error("Error with simple exception", simpleException);
            logger.Fatal("Fatal with complex exception", complexException);
            logger.Error("Error with null exception", null!);
        };

        // Assert
        act.Should().NotThrow();
    }

    #endregion

}