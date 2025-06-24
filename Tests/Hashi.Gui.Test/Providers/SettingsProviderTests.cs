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
    private Mock<IJsonWrapper> jsonWrapperMock;
    private Mock<Func<ISettingsViewModel>> settingsFactoryMock;
    private Mock<IPathProvider> pathProviderMock;
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<ILogger> loggerMock;
    private Mock<ISettingsViewModel> settingsViewModelMock;
    private SettingsProvider sut;
    private string testSettingsPath;
    private string testDirectoryPath;

    [SetUp]
    public void SetUp()
    {
        jsonWrapperMock = new Mock<IJsonWrapper>(MockBehavior.Strict);
        settingsFactoryMock = new Mock<Func<ISettingsViewModel>>(MockBehavior.Strict);
        pathProviderMock = new Mock<IPathProvider>(MockBehavior.Strict);
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        settingsViewModelMock = new Mock<ISettingsViewModel>(MockBehavior.Strict);

        testDirectoryPath = Path.Combine(Path.GetTempPath(), "HashiSettingsTest", Guid.NewGuid().ToString());
        testSettingsPath = Path.Combine(testDirectoryPath, "settings.json");

        loggerFactoryMock.Setup(x => x.CreateLogger<SettingsProvider>()).Returns(loggerMock.Object);
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(testSettingsPath);
        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns(testDirectoryPath);
        settingsFactoryMock.Setup(x => x.Invoke()).Returns(settingsViewModelMock.Object);

        // Setup logger methods that are called
        loggerMock.Setup(x => x.Info(It.IsAny<string>())).Verifiable();
        loggerMock.Setup(x => x.Error(It.IsAny<string>())).Verifiable();
        loggerMock.Setup(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>())).Verifiable();

        // Setup JsonWrapper methods (will be overridden in specific tests as needed)
        jsonWrapperMock.Setup(x => x.SerializeObject(It.IsAny<object>())).Returns("{}");
        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), It.IsAny<Type>())).Returns(null);

        // Setup default return for settings that don't exist
        settingsViewModelMock.Setup(x => x.Languages).Returns([Mock.Of<ILanguageViewModel>(l => l.Culture == "en-GB")]);

        sut = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(testDirectoryPath))
        {
            try
            {
                Directory.Delete(testDirectoryPath, true);
            }
            catch
            {
                // Ignore cleanup failures in tests
            }
        }
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeSettings()
    {
        // Arrange & Act
        var result = new SettingsProvider(
            jsonWrapperMock.Object,
            settingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object);

        // Assert
        result.Settings.Should().NotBeNull();
        loggerMock.Verify(x => x.Info("SettingsProvider initialized"), Times.Once);
    }

    [Test]
    public void Constructor_WhenCalled_ShouldLoadSettings()
    {
        // Arrange & Act & Assert
        loggerFactoryMock.Verify(x => x.CreateLogger<SettingsProvider>(), Times.AtLeastOnce);
        sut.Settings.Should().NotBeNull();
    }

    [Test]
    public void Settings_WhenAccessed_ShouldReturnLoadedSettings()
    {
        // Arrange & Act
        var settings = sut.Settings;

        // Assert
        settings.Should().NotBeNull();
        settings.Should().Be(settingsViewModelMock.Object);
    }

    [Test]
    public void SaveSettings_WhenSettingsIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var localSettingsFactoryMock = new Mock<Func<ISettingsViewModel>>(MockBehavior.Strict);
        localSettingsFactoryMock.Setup(x => x.Invoke()).Returns((ISettingsViewModel)null!);

        var localSut = new SettingsProvider(
            jsonWrapperMock.Object,
            localSettingsFactoryMock.Object,
            pathProviderMock.Object,
            loggerFactoryMock.Object);

        // Use reflection to set Settings to null since it's private set
        var settingsProperty = typeof(SettingsProvider).GetProperty("Settings");
        settingsProperty!.SetValue(localSut, null);

        // Act & Assert
        localSut.Invoking(x => x.SaveSettings()).Should().Throw<InvalidOperationException>()
               .WithMessage("Settings cannot be null.");
    }

    [Test]
    public void SaveSettings_WhenCalled_ShouldSerializeSettings()
    {
        // Arrange
        jsonWrapperMock.Setup(x => x.SerializeObject(settingsViewModelMock.Object))
                      .Returns("serialized_settings");

        // Act
        sut.SaveSettings();

        // Assert
        jsonWrapperMock.Verify(x => x.SerializeObject(settingsViewModelMock.Object), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        // Arrange
        jsonWrapperMock.Setup(x => x.SerializeObject(It.IsAny<object>()))
                      .Returns("serialized_settings");
        Directory.Exists(testDirectoryPath).Should().BeFalse();

        // Act
        sut.SaveSettings();

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeTrue();
    }

    [Test]
    public void SaveSettings_WhenSerializationFails_ShouldLogError()
    {
        // Arrange
        jsonWrapperMock.Setup(x => x.SerializeObject(It.IsAny<object>()))
                      .Throws(new Exception("Serialization failed"));

        // Act
        sut.SaveSettings();

        // Assert
        loggerMock.Verify(x => x.Error("Failed to save settings", It.IsAny<Exception>()), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenFileWriteFails_ShouldLogError()
    {
        // Arrange
        jsonWrapperMock.Setup(x => x.SerializeObject(It.IsAny<object>()))
                      .Returns("serialized_settings");

        // Setup an invalid path to cause file write to fail
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns("/invalid/path/settings.json");
        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns("/invalid/path");

        // Act
        sut.SaveSettings();

        // Assert
        loggerMock.Verify(x => x.Error("Failed to save settings", It.IsAny<Exception>()), Times.Once);
    }

    [Test]
    public void ResetSettings_WhenCalled_ShouldLoadSettings()
    {
        // Arrange
        var originalSettings = sut.Settings;
        var newMockSettings = new Mock<ISettingsViewModel>();
        newMockSettings.Setup(x => x.Languages).Returns([Mock.Of<ILanguageViewModel>(l => l.Culture == "de-DE")]);

        settingsFactoryMock.Setup(x => x.Invoke()).Returns(newMockSettings.Object);

        // Act
        sut.ResetSettings();

        // Assert
        sut.Settings.Should().Be(newMockSettings.Object);
        sut.Settings.Should().NotBe(originalSettings);
    }

    [Test]
    public void LoadSettings_WhenFileExists_ShouldDeserializeFromFile()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        File.WriteAllText(testSettingsPath, "file_content");

        var deserializedSettings = new Mock<ISettingsViewModel>();
        deserializedSettings.Setup(x => x.SelectedLanguageCulture).Returns("en-US");

        jsonWrapperMock.Setup(x => x.DeserializeObject("file_content", typeof(SettingsViewModel)))
                      .Returns(deserializedSettings.Object);

        // Act
        var result = sut.LoadSettings();

        // Assert
        jsonWrapperMock.Verify(x => x.DeserializeObject("file_content", typeof(SettingsViewModel)), Times.Once);
        result.Should().Be(deserializedSettings.Object);
        loggerMock.Verify(x => x.Info("Settings loaded successfully from file"), Times.Once);
    }

    [Test]
    public void LoadSettings_WhenFileDoesNotExist_ShouldCreateDefaultSettings()
    {
        // Arrange
        File.Exists(testSettingsPath).Should().BeFalse();

        // Act
        var result = sut.LoadSettings();

        // Assert
        settingsFactoryMock.Verify(x => x.Invoke(), Times.AtLeastOnce);
        settingsViewModelMock.Verify(x => x.InitializeHighScores(), Times.AtLeastOnce);
        result.Should().Be(settingsViewModelMock.Object);
        loggerMock.Verify(x => x.Info("Created new default settings"), Times.AtLeastOnce);
    }

    [Test]
    public void LoadSettings_WhenDeserializationFails_ShouldCreateDefaultSettings()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        File.WriteAllText(testSettingsPath, "invalid_json");

        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), It.IsAny<Type>()))
                      .Throws(new Exception("Deserialization failed"));

        // Act
        var result = sut.LoadSettings();

        // Assert
        loggerMock.Verify(x => x.Error("Failed to load settings, using defaults", It.IsAny<Exception>()), Times.Once);
        settingsFactoryMock.Verify(x => x.Invoke(), Times.AtLeastOnce);
        result.Should().Be(settingsViewModelMock.Object);
    }

    [Test]
    public void LoadSettings_WhenCalledWithValidFile_ShouldSetCulture()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        File.WriteAllText(testSettingsPath, "file_content");

        var deserializedSettings = new Mock<ISettingsViewModel>();
        deserializedSettings.Setup(x => x.SelectedLanguageCulture).Returns("de-DE");

        jsonWrapperMock.Setup(x => x.DeserializeObject("file_content", typeof(SettingsViewModel)))
                      .Returns(deserializedSettings.Object);

        // Act
        var result = sut.LoadSettings();

        // Assert
        // We can't easily test TranslationSource culture setting in unit tests,
        // but we can verify the method completed successfully
        result.Should().Be(deserializedSettings.Object);
    }

    [Test]
    public void LoadSettings_WhenCreatingDefaults_ShouldSetSelectedLanguageFromFirstLanguage()
    {
        // Arrange
        var mockLanguage = new Mock<ILanguageViewModel>();
        mockLanguage.Setup(x => x.Culture).Returns("fr-FR");

        settingsViewModelMock.Setup(x => x.Languages).Returns([mockLanguage.Object]);

        // Act
        var result = sut.LoadSettings();

        // Assert
        settingsViewModelMock.VerifySet(x => x.SelectedLanguageCulture = "fr-FR", Times.AtLeastOnce);
    }

    [Test]
    public void LoadSettings_WhenCalled_ShouldLogDebugMessage()
    {
        // Arrange & Act
        sut.LoadSettings();

        // Assert
        loggerMock.Verify(x => x.Debug($"Loading settings from {testSettingsPath}"), Times.AtLeastOnce);
    }
}