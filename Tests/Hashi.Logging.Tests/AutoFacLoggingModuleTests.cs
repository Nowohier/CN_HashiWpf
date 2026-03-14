using FluentAssertions;
using Hashi.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ILogger = Hashi.Logging.Interfaces.ILogger;
using ILoggerFactory = Hashi.Logging.Interfaces.ILoggerFactory;

namespace Hashi.Logging.Tests;

[TestFixture]
public class AutoFacLoggingModuleTests
{
    private ServiceProvider serviceProvider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLoggingServices();
        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        serviceProvider.Dispose();
    }

    #region Service Registration Tests

    [Test]
    public void AddLoggingServices_WhenCalled_ShouldRegisterLoggerFactory()
    {
        // Act
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Assert
        loggerFactory.Should().NotBeNull();
        loggerFactory.Should().BeOfType<LoggerFactory>();
    }

    [Test]
    public void AddLoggingServices_WhenResolvedMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var loggerFactory1 = serviceProvider.GetRequiredService<ILoggerFactory>();
        var loggerFactory2 = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Assert
        loggerFactory1.Should().BeSameAs(loggerFactory2);
    }

    [Test]
    public void AddLoggingServices_WhenResolvedInDifferentScopes_ShouldReturnSameInstance()
    {
        // Arrange
        ILoggerFactory factoryFromRoot;
        ILoggerFactory factoryFromScope;

        // Act
        factoryFromRoot = serviceProvider.GetRequiredService<ILoggerFactory>();

        using (var scope = serviceProvider.CreateScope())
        {
            factoryFromScope = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        }

        // Assert
        factoryFromRoot.Should().BeSameAs(factoryFromScope);
    }

    #endregion

    #region Extension Method Tests

    [Test]
    public void LoggingServiceExtensions_ShouldBePublicStaticClass()
    {
        // Arrange
        var type = typeof(LoggingServiceExtensions);

        // Act & Assert
        type.IsPublic.Should().BeTrue();
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeTrue();
        type.IsSealed.Should().BeTrue();
    }

    [Test]
    public void AddLoggingServices_WhenCalled_ShouldNotThrow()
    {
        // Act
        var act = () =>
        {
            var services = new ServiceCollection();
            services.AddLoggingServices();
        };

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Integration Tests

    [Test]
    public void ResolvedLoggerFactory_WhenUsed_ShouldCreateValidLoggers()
    {
        // Arrange
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Act
        var logger1 = loggerFactory.CreateLogger<AutoFacLoggingModuleTests>();
        var logger2 = loggerFactory.CreateLogger("TestLogger");

        // Assert
        logger1.Should().NotBeNull();
        logger1.Should().BeAssignableTo<ILogger>();
        logger2.Should().NotBeNull();
        logger2.Should().BeAssignableTo<ILogger>();
    }

    [Test]
    public void ResolvedLoggers_WhenUsed_ShouldLogWithoutErrors()
    {
        // Arrange
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<AutoFacLoggingModuleTests>();

        // Act
        var act = () =>
        {
            logger.Trace("Trace message");
            logger.Debug("Debug message");
            logger.Info("Info message");
            logger.Warn("Warn message");
            logger.Error("Error message");
            logger.Fatal("Fatal message");
        };

        // Assert
        act.Should().NotThrow();
    }

    #endregion
}
