using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Providers;
using Hashi.Gui.ViewModels.Settings;
using Hashi.Logging.Interfaces;
using Moq;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class SettingsProviderTests
{
    [SetUp]
    public void SetUp()
    {
        jsonWrapperMock = new Mock<IJsonWrapper>(MockBehavior.Strict);
        settingsFactoryMock = new Mock<Func<ISettingsViewModel>>(MockBehavior.Strict);
        pathProviderMock = new Mock<IPathProvider>(MockBehavior.Strict);
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        settingsViewModelMock = new Mock<ISettingsViewModel>(MockBehavior.Strict);
        languageViewModelMock = new Mock<ILanguageViewModel>(MockBehavior.Strict);
        fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);
        directoryWrapperMock = new Mock<IDirectoryWrapper>(MockBehavior.Strict);

        // Setup logger
        loggerFactoryMock.Setup(x => x.CreateLogger<SettingsProvider>()).Returns(loggerMock.Object);
        loggerMock.Setup(x => x.Info(It.IsAny<string>()));
        loggerMock.Setup(x => x.Debug(It.IsAny<string>()));
        loggerMock.Setup(x => x.Error(It.IsAny<string>()));
        loggerMock.Setup(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()));

        // Setup path provider
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns("test-settings.json");
        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns("test-settings");

        // Setup settings factory
        settingsFactoryMock.Setup(x => x.Invoke()).Returns(settingsViewModelMock.Object);

        // Setup settings view model
        settingsViewModelMock.Setup(x => x.InitializeHighScores());
        settingsViewModelMock.Setup(x => x.Languages).Returns([languageViewModelMock.Object]);
        settingsViewModelMock.SetupProperty(x => x.SelectedLanguageCulture, "en-GB");

        // Setup language view model
        languageViewModelMock.Setup(x => x.Culture).Returns("en-GB");

        // Setup json wrapper for the constructor (loading settings)
        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(SettingsViewModel)))
            .Returns(settingsViewModelMock.Object);
        jsonWrapperMock.Setup(x => x.SerializeObject(It.IsAny<object>())).Returns("{}");

        // Setup file wrapper
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
        fileWrapperMock.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("{}");
        fileWrapperMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

        // Setup directory wrapper
        directoryWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        directoryWrapperMock.Setup(x => x.CreateDirectory(It.IsAny<string>()));
    }

    private Mock<IJsonWrapper> jsonWrapperMock;
    private Mock<Func<ISettingsViewModel>> settingsFactoryMock;
    private Mock<IPathProvider> pathProviderMock;
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<ILogger> loggerMock;
    private Mock<ISettingsViewModel> settingsViewModelMock;
    private Mock<ILanguageViewModel> languageViewModelMock;
    private Mock<IFileWrapper> fileWrapperMock;
    private Mock<IDirectoryWrapper> directoryWrapperMock;
    private SettingsProvider settingsProvider = null!;

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Act
        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Assert
        settingsProvider.Should().NotBeNull();
        settingsProvider.Settings.Should().Be(settingsViewModelMock.Object);
        loggerMock.Verify(x => x.Info("SettingsProvider initialized"), Times.Once);
    }

    [Test]
    public void Constructor_WhenJsonWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new SettingsProvider(
            null!,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("jsonWrapper");
    }

    [Test]
    public void Constructor_WhenSettingsFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new SettingsProvider(
            jsonWrapperMock.Object,
            null!,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("settingsFactory");
    }

    [Test]
    public void Constructor_WhenPathProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            null!,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("pathProvider");
    }

    [Test]
    public void Constructor_WhenLoggerFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            null!,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("loggerFactory");
    }

    [Test]
    public void Constructor_WhenFileWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            null!,
            directoryWrapperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("fileWrapper");
    }

    [Test]
    public void Constructor_WhenDirectoryWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            null!);

        action.Should().Throw<ArgumentNullException>().WithParameterName("directoryWrapper");
    }

    [Test]
    public void LoadSettings_WhenFileExists_ShouldLoadSettingsFromFile()
    {
        // Arrange
        var testFilePath = "test-settings.json";
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(testFilePath);
        fileWrapperMock.Setup(x => x.Exists(testFilePath)).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText(testFilePath)).Returns("{\"test\": \"settings\"}");

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Act
        var result = settingsProvider.LoadSettings();

        // Assert
        result.Should().Be(settingsViewModelMock.Object);
        jsonWrapperMock.Verify(x => x.DeserializeObject(It.IsAny<string>(), typeof(SettingsViewModel)),
            Times.AtLeastOnce);
        loggerMock.Verify(x => x.Info("Settings loaded successfully from file"), Times.AtLeastOnce);
    }

    [Test]
    public void LoadSettings_WhenFileDoesNotExist_ShouldCreateDefaultSettings()
    {
        // Arrange
        var nonExistentPath = "non-existent-settings.json";
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(nonExistentPath);
        fileWrapperMock.Setup(x => x.Exists(nonExistentPath)).Returns(false);

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Act
        var result = settingsProvider.LoadSettings();

        // Assert
        result.Should().Be(settingsViewModelMock.Object);
        settingsFactoryMock.Verify(x => x.Invoke(), Times.AtLeastOnce);
        settingsViewModelMock.Verify(x => x.InitializeHighScores(), Times.AtLeastOnce);
        loggerMock.Verify(x => x.Info("Created new default settings"), Times.AtLeastOnce);
    }

    [Test]
    public void LoadSettings_WhenExceptionOccurs_ShouldCreateDefaultSettings()
    {
        // Arrange
        var testFilePath = "test-exception-settings.json";
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(testFilePath);
        fileWrapperMock.Setup(x => x.Exists(testFilePath)).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText(testFilePath)).Returns("{\"test\": \"settings\"}");

        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(SettingsViewModel)))
            .Throws(new Exception("Test exception"));

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Act
        var result = settingsProvider.LoadSettings();

        // Assert
        result.Should().Be(settingsViewModelMock.Object);
        settingsFactoryMock.Verify(x => x.Invoke(), Times.AtLeastOnce);
        settingsViewModelMock.Verify(x => x.InitializeHighScores(), Times.AtLeastOnce);
        loggerMock.Verify(x => x.Error("Failed to load settings, using defaults", It.IsAny<Exception>()),
            Times.AtLeastOnce);
        loggerMock.Verify(x => x.Info("Created new default settings"), Times.AtLeastOnce);
    }

    [Test]
    public void ResetSettings_WhenCalled_ShouldReloadSettings()
    {
        // Arrange
        var nonExistentPath = "reset-test-settings.json";
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(nonExistentPath);
        fileWrapperMock.Setup(x => x.Exists(nonExistentPath)).Returns(false);

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Act
        settingsProvider.ResetSettings();

        // Assert
        settingsProvider.Settings.Should().Be(settingsViewModelMock.Object);
        settingsFactoryMock.Verify(x => x.Invoke(), Times.AtLeast(2)); // Once in constructor, once in reset
    }

    [Test]
    public void SaveSettings_WhenSettingsIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var nonExistentPath = "null-test-settings.json";
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(nonExistentPath);
        fileWrapperMock.Setup(x => x.Exists(nonExistentPath)).Returns(false);

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Use reflection to set Settings to null
        var settingsProperty = typeof(SettingsProvider).GetProperty("Settings");
        settingsProperty!.SetValue(settingsProvider, null);

        // Act & Assert
        var action = () => settingsProvider.SaveSettings();
        action.Should().Throw<InvalidOperationException>().WithMessage("Settings cannot be null.");
    }

    [Test]
    public void SaveSettings_WhenDirectoryDoesNotExist_ShouldCreateDirectoryAndSaveFile()
    {
        // Arrange
        var testDir = "test-settings-dir";
        var testFilePath = "test-settings-dir/test-settings.json";

        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(testFilePath);
        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns(testDir);
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
        directoryWrapperMock.Setup(x => x.Exists(testDir)).Returns(false);

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Act
        settingsProvider.SaveSettings();

        // Assert
        jsonWrapperMock.Verify(x => x.SerializeObject(settingsViewModelMock.Object), Times.Once);
        directoryWrapperMock.Verify(x => x.CreateDirectory(testDir), Times.Once);
        fileWrapperMock.Verify(x => x.WriteAllText(testFilePath, "{}"), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenDirectoryExists_ShouldNotCreateDirectoryButSaveFile()
    {
        // Arrange
        var testDir = "existing-settings-dir";
        var testFilePath = "existing-settings-dir/test-settings.json";

        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(testFilePath);
        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns(testDir);
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
        directoryWrapperMock.Setup(x => x.Exists(testDir)).Returns(true);

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Act
        settingsProvider.SaveSettings();

        // Assert
        jsonWrapperMock.Verify(x => x.SerializeObject(settingsViewModelMock.Object), Times.Once);
        directoryWrapperMock.Verify(x => x.CreateDirectory(testDir), Times.Never);
        fileWrapperMock.Verify(x => x.WriteAllText(testFilePath, "{}"), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        var nonExistentPath = "exception-test-settings.json";
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(nonExistentPath);
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Setup the exception to be thrown during the SaveSettings call
        jsonWrapperMock.Setup(x => x.SerializeObject(settingsViewModelMock.Object))
            .Throws(new Exception("Test exception"));

        // Act & Assert - Since serialization exception is not caught, it should be thrown
        var action = () => settingsProvider.SaveSettings();
        action.Should().Throw<Exception>().WithMessage("Test exception");
    }

    [Test]
    public void SaveSettings_WhenFileWriteThrowsException_ShouldLogError()
    {
        // Arrange
        var testFilePath = "write-exception-settings.json";
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(testFilePath);
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
        fileWrapperMock.Setup(x => x.WriteAllText(testFilePath, It.IsAny<string>()))
            .Throws(new Exception("File write exception"));

        settingsProvider = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Act
        settingsProvider.SaveSettings();

        // Assert
        loggerMock.Verify(x => x.Error("Failed to save settings", It.IsAny<Exception>()), Times.Once);
    }
}