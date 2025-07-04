using Autofac;
using FluentAssertions;
using Moq;
using NLog;
using ILogger = Hashi.Logging.Interfaces.ILogger;

namespace Hashi.Logging.Tests;

[TestFixture]
public class LoggerTests
{
    private Logger logger;
    private IContainer container;

    [SetUp]
    public void Setup()
    {
        // Create a real NLog logger for testing
        var nlogLogger = LogManager.GetLogger("TestLogger");
        logger = new Logger(nlogLogger);
        
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
    public void Constructor_WhenNLogLoggerProvided_ShouldCreateInstance()
    {
        // Arrange
        var nlogLogger = LogManager.GetLogger("TestLogger");

        // Act
        var logger = new Logger(nlogLogger);

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<Logger>();
    }

    [Test]
    public void Constructor_WhenNullLoggerProvided_ShouldNotThrowButMayFailAtRuntime()
    {
        // Act & Assert
        // Note: This will create a logger but may fail when methods are called
        var logger = new Logger(null!);
        logger.Should().NotBeNull();
    }

    #endregion

    #region Trace Tests

    [Test]
    public void Trace_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test trace message";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Trace(message));
    }

    [Test]
    public void Trace_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Trace(message));
    }

    [Test]
    public void Trace_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Trace(message!));
    }

    #endregion

    #region Debug Tests

    [Test]
    public void Debug_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test debug message";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Debug(message));
    }

    [Test]
    public void Debug_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Debug(message));
    }

    [Test]
    public void Debug_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Debug(message!));
    }

    #endregion

    #region Info Tests

    [Test]
    public void Info_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test info message";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Info(message));
    }

    [Test]
    public void Info_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Info(message));
    }

    [Test]
    public void Info_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Info(message!));
    }

    #endregion

    #region Warn Tests

    [Test]
    public void Warn_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test warn message";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Warn(message));
    }

    [Test]
    public void Warn_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Warn(message));
    }

    [Test]
    public void Warn_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Warn(message!));
    }

    #endregion

    #region Error Tests

    [Test]
    public void Error_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test error message";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Error(message));
    }

    [Test]
    public void Error_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Error(message));
    }

    [Test]
    public void Error_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Error(message!));
    }

    [Test]
    public void Error_WhenMessageAndExceptionProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test error message with exception";
        var exception = new InvalidOperationException("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Error(message, exception));
    }

    [Test]
    public void Error_WhenMessageAndNullException_ShouldNotThrow()
    {
        // Arrange
        var message = "Test error message with null exception";
        Exception? exception = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Error(message, exception!));
    }

    #endregion

    #region Fatal Tests

    [Test]
    public void Fatal_WhenMessageProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test fatal message";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Fatal(message));
    }

    [Test]
    public void Fatal_WhenEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var message = "";

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Fatal(message));
    }

    [Test]
    public void Fatal_WhenNullMessage_ShouldNotThrow()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Fatal(message!));
    }

    [Test]
    public void Fatal_WhenMessageAndExceptionProvided_ShouldNotThrow()
    {
        // Arrange
        var message = "Test fatal message with exception";
        var exception = new InvalidOperationException("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Fatal(message, exception));
    }

    [Test]
    public void Fatal_WhenMessageAndNullException_ShouldNotThrow()
    {
        // Arrange
        var message = "Test fatal message with null exception";
        Exception? exception = null;

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Fatal(message, exception!));
    }

    #endregion

    #region Interface Implementation Tests

    [Test]
    public void Logger_ShouldImplementILogger()
    {
        // Arrange & Act
        var nlogLogger = LogManager.GetLogger("TestLogger");
        var logger = new Logger(nlogLogger);

        // Assert
        logger.Should().BeAssignableTo<ILogger>();
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

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            logger.Trace(traceMessage);
            logger.Debug(debugMessage);
            logger.Info(infoMessage);
            logger.Warn(warnMessage);
            logger.Error(errorMessage);
            logger.Fatal(fatalMessage);
        });
    }

    [Test]
    public void Logger_WhenCreatedFromFactory_ShouldWorkCorrectly()
    {
        // Arrange
        var factory = new LoggerFactory();
        var factoryLogger = factory.CreateLogger<LoggerTests>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            factoryLogger.Info("Test message from factory logger");
            factoryLogger.Error("Test error from factory logger");
        });
    }

    [Test]
    public void Logger_WhenLoggingWithExceptions_ShouldHandleAllCases()
    {
        // Arrange
        var simpleException = new Exception("Simple exception");
        var complexException = new InvalidOperationException("Complex exception", 
            new ArgumentException("Inner exception"));

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            logger.Error("Error with simple exception", simpleException);
            logger.Fatal("Fatal with complex exception", complexException);
            logger.Error("Error with null exception", null!);
        });
    }

    #endregion

    #region Property and Field Tests

    [Test]
    public void Logger_WhenCreated_ShouldHaveNLogLoggerField()
    {
        // Arrange
        var nlogLogger = LogManager.GetLogger("TestLogger");
        var logger = new Logger(nlogLogger);

        // Act
        var field = typeof(Logger).GetField("logger", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Assert
        field.Should().NotBeNull();
        var fieldValue = field!.GetValue(logger);
        fieldValue.Should().Be(nlogLogger);
    }

    #endregion
}