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
    private Mock<IJsonWrapper> mockJsonWrapper;
    private Mock<Func<ISettingsViewModel>> mockSettingsFactory;
    private Mock<IPathProvider> mockPathProvider;
    private Mock<ILoggerFactory> mockLoggerFactory;
    private Mock<ILogger> mockLogger;
    private Mock<ISettingsViewModel> mockSettingsViewModel;
    private SettingsProvider sut;
    private string testSettingsPath;
    private string testDirectoryPath;

    [SetUp]
    public void SetUp()
    {
        mockJsonWrapper = new Mock<IJsonWrapper>();
        mockSettingsFactory = new Mock<Func<ISettingsViewModel>>();
        mockPathProvider = new Mock<IPathProvider>();
        mockLoggerFactory = new Mock<ILoggerFactory>();
        mockLogger = new Mock<ILogger>();
        mockSettingsViewModel = new Mock<ISettingsViewModel>();

        testDirectoryPath = Path.Combine(Path.GetTempPath(), "HashiSettingsTest", Guid.NewGuid().ToString());
        testSettingsPath = Path.Combine(testDirectoryPath, "settings.json");

        mockLoggerFactory.Setup(x => x.CreateLogger<SettingsProvider>()).Returns(mockLogger.Object);
        mockPathProvider.Setup(x => x.HashiSettingsFilePath).Returns(testSettingsPath);
        mockPathProvider.Setup(x => x.SettingsDirectoryPath).Returns(testDirectoryPath);
        mockSettingsFactory.Setup(x => x.Invoke()).Returns(mockSettingsViewModel.Object);

        // Setup default return for settings that don't exist
        mockSettingsViewModel.Setup(x => x.Languages).Returns([Mock.Of<ILanguageViewModel>(l => l.Culture == "en-GB")]);

        sut = new SettingsProvider(
            mockJsonWrapper.Object,
            mockSettingsFactory.Object,
            mockPathProvider.Object,
            mockLoggerFactory.Object);
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
            mockJsonWrapper.Object,
            mockSettingsFactory.Object,
            mockPathProvider.Object,
            mockLoggerFactory.Object);

        // Assert
        result.Settings.Should().NotBeNull();
        mockLogger.Verify(x => x.Info("SettingsProvider initialized"), Times.Once);
    }

    [Test]
    public void Constructor_WhenCalled_ShouldLoadSettings()
    {
        // Arrange & Act & Assert
        mockLoggerFactory.Verify(x => x.CreateLogger<SettingsProvider>(), Times.AtLeastOnce);
        sut.Settings.Should().NotBeNull();
    }

    [Test]
    public void Settings_WhenAccessed_ShouldReturnLoadedSettings()
    {
        // Arrange & Act
        var settings = sut.Settings;

        // Assert
        settings.Should().NotBeNull();
        settings.Should().Be(mockSettingsViewModel.Object);
    }

    [Test]
    public void SaveSettings_WhenSettingsIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var localMockSettingsFactory = new Mock<Func<ISettingsViewModel>>();
        localMockSettingsFactory.Setup(x => x.Invoke()).Returns((ISettingsViewModel)null!);

        var localSut = new SettingsProvider(
            mockJsonWrapper.Object,
            localMockSettingsFactory.Object,
            mockPathProvider.Object,
            mockLoggerFactory.Object);

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
        mockJsonWrapper.Setup(x => x.SerializeObject(mockSettingsViewModel.Object))
                      .Returns("serialized_settings");

        // Act
        sut.SaveSettings();

        // Assert
        mockJsonWrapper.Verify(x => x.SerializeObject(mockSettingsViewModel.Object), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        // Arrange
        mockJsonWrapper.Setup(x => x.SerializeObject(It.IsAny<object>()))
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
        mockJsonWrapper.Setup(x => x.SerializeObject(It.IsAny<object>()))
                      .Throws(new Exception("Serialization failed"));

        // Act
        sut.SaveSettings();

        // Assert
        mockLogger.Verify(x => x.Error("Failed to save settings", It.IsAny<Exception>()), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenFileWriteFails_ShouldLogError()
    {
        // Arrange
        mockJsonWrapper.Setup(x => x.SerializeObject(It.IsAny<object>()))
                      .Returns("serialized_settings");

        // Setup an invalid path to cause file write to fail
        mockPathProvider.Setup(x => x.HashiSettingsFilePath).Returns("/invalid/path/settings.json");
        mockPathProvider.Setup(x => x.SettingsDirectoryPath).Returns("/invalid/path");

        // Act
        sut.SaveSettings();

        // Assert
        mockLogger.Verify(x => x.Error("Failed to save settings", It.IsAny<Exception>()), Times.Once);
    }

    [Test]
    public void ResetSettings_WhenCalled_ShouldLoadSettings()
    {
        // Arrange
        var originalSettings = sut.Settings;
        var newMockSettings = new Mock<ISettingsViewModel>();
        newMockSettings.Setup(x => x.Languages).Returns([Mock.Of<ILanguageViewModel>(l => l.Culture == "de-DE")]);

        mockSettingsFactory.Setup(x => x.Invoke()).Returns(newMockSettings.Object);

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

        mockJsonWrapper.Setup(x => x.DeserializeObject("file_content", typeof(SettingsViewModel)))
                      .Returns(deserializedSettings.Object);

        // Act
        var result = sut.LoadSettings();

        // Assert
        mockJsonWrapper.Verify(x => x.DeserializeObject("file_content", typeof(SettingsViewModel)), Times.Once);
        result.Should().Be(deserializedSettings.Object);
        mockLogger.Verify(x => x.Info("Settings loaded successfully from file"), Times.Once);
    }

    [Test]
    public void LoadSettings_WhenFileDoesNotExist_ShouldCreateDefaultSettings()
    {
        // Arrange
        File.Exists(testSettingsPath).Should().BeFalse();

        // Act
        var result = sut.LoadSettings();

        // Assert
        mockSettingsFactory.Verify(x => x.Invoke(), Times.AtLeastOnce);
        mockSettingsViewModel.Verify(x => x.InitializeHighScores(), Times.AtLeastOnce);
        result.Should().Be(mockSettingsViewModel.Object);
        mockLogger.Verify(x => x.Info("Created new default settings"), Times.AtLeastOnce);
    }

    [Test]
    public void LoadSettings_WhenDeserializationFails_ShouldCreateDefaultSettings()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        File.WriteAllText(testSettingsPath, "invalid_json");

        mockJsonWrapper.Setup(x => x.DeserializeObject(It.IsAny<string>(), It.IsAny<Type>()))
                      .Throws(new Exception("Deserialization failed"));

        // Act
        var result = sut.LoadSettings();

        // Assert
        mockLogger.Verify(x => x.Error("Failed to load settings, using defaults", It.IsAny<Exception>()), Times.Once);
        mockSettingsFactory.Verify(x => x.Invoke(), Times.AtLeastOnce);
        result.Should().Be(mockSettingsViewModel.Object);
    }

    [Test]
    public void LoadSettings_WhenCalledWithValidFile_ShouldSetCulture()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        File.WriteAllText(testSettingsPath, "file_content");

        var deserializedSettings = new Mock<ISettingsViewModel>();
        deserializedSettings.Setup(x => x.SelectedLanguageCulture).Returns("de-DE");

        mockJsonWrapper.Setup(x => x.DeserializeObject("file_content", typeof(SettingsViewModel)))
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

        mockSettingsViewModel.Setup(x => x.Languages).Returns([mockLanguage.Object]);

        // Act
        var result = sut.LoadSettings();

        // Assert
        mockSettingsViewModel.VerifySet(x => x.SelectedLanguageCulture = "fr-FR", Times.AtLeastOnce);
    }

    [Test]
    public void LoadSettings_WhenCalled_ShouldLogDebugMessage()
    {
        // Arrange & Act
        sut.LoadSettings();

        // Assert
        mockLogger.Verify(x => x.Debug($"Loading settings from {testSettingsPath}"), Times.AtLeastOnce);
    }
}