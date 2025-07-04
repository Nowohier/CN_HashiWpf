using Autofac;
using FluentAssertions;
using ILogger = Hashi.Logging.Interfaces.ILogger;
using ILoggerFactory = Hashi.Logging.Interfaces.ILoggerFactory;

namespace Hashi.Logging.Tests;

[TestFixture]
public class AutoFacLoggingModuleTests
{
    private IContainer container;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacLoggingModule>();
        container = builder.Build();
    }

    [TearDown]
    public void Teardown()
    {
        container.Dispose();
    }

    #region Module Registration Tests

    [Test]
    public void Load_WhenCalled_ShouldRegisterLoggerFactory()
    {
        // Act
        var loggerFactory = container.Resolve<ILoggerFactory>();

        // Assert
        loggerFactory.Should().NotBeNull();
        loggerFactory.Should().BeOfType<LoggerFactory>();
    }

    [Test]
    public void Load_WhenResolvedMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var loggerFactory1 = container.Resolve<ILoggerFactory>();
        var loggerFactory2 = container.Resolve<ILoggerFactory>();

        // Assert
        loggerFactory1.Should().BeSameAs(loggerFactory2);
    }

    [Test]
    public void Load_WhenResolvedInDifferentScopes_ShouldReturnSameInstance()
    {
        // Arrange
        ILoggerFactory factoryFromRoot;
        ILoggerFactory factoryFromScope;

        // Act
        factoryFromRoot = container.Resolve<ILoggerFactory>();

        using (var scope = container.BeginLifetimeScope())
        {
            factoryFromScope = scope.Resolve<ILoggerFactory>();
        }

        // Assert
        factoryFromRoot.Should().BeSameAs(factoryFromScope);
    }

    #endregion

    #region Module Tests

    [Test]
    public void AutoFacLoggingModule_ShouldInheritFromModule()
    {
        // Arrange & Act
        var module = new AutoFacLoggingModule();

        // Assert
        module.Should().BeAssignableTo<Module>();
    }

    [Test]
    public void AutoFacLoggingModule_WhenInstantiated_ShouldNotThrow()
    {
        // Act & Assert
        // ReSharper disable once ObjectCreationAsStatement
        Assert.DoesNotThrow(() => new AutoFacLoggingModule());
    }

    #endregion

    #region Integration Tests

    [Test]
    public void ResolvedLoggerFactory_WhenUsed_ShouldCreateValidLoggers()
    {
        // Arrange
        var loggerFactory = container.Resolve<ILoggerFactory>();

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
        var loggerFactory = container.Resolve<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<AutoFacLoggingModuleTests>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            logger.Trace("Trace message");
            logger.Debug("Debug message");
            logger.Info("Info message");
            logger.Warn("Warn message");
            logger.Error("Error message");
            logger.Fatal("Fatal message");
        });
    }

    #endregion
}