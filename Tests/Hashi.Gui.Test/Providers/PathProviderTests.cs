using FluentAssertions;
using Hashi.Gui.Providers;
using System.IO;

namespace Hashi.Gui.Test.Providers;

/// <summary>
/// Unit tests for PathProvider class.
/// </summary>
[TestFixture]
public class PathProviderTests
{
    private PathProvider pathProvider;

    [SetUp]
    public void SetUp()
    {
        pathProvider = new PathProvider();
    }

    [TearDown]
    public void TearDown()
    {
        // No specific teardown needed as PathProvider doesn't hold resources
    }

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        // Arrange & Act
        var action = () => new PathProvider();

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void Constructor_WhenCreated_ShouldInitializeAllProperties()
    {
        // Arrange & Act
        var provider = new PathProvider();

        // Assert
        provider.Should().NotBeNull();
        provider.HashiSettingsFileName.Should().NotBeNullOrEmpty();
        provider.HashiTestFieldsFileName.Should().NotBeNullOrEmpty();
        provider.LocalAppDataPath.Should().NotBeNullOrEmpty();
        provider.SettingsDirectoryPath.Should().NotBeNullOrEmpty();
        provider.HashiSettingsFilePath.Should().NotBeNullOrEmpty();
        provider.HashiTestFieldsFilePath.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void HashiSettingsFileName_WhenAccessed_ShouldReturnExpectedValue()
    {
        // Arrange
        const string expectedFileName = "HashiSettings.json";

        // Act
        var result = pathProvider.HashiSettingsFileName;

        // Assert
        result.Should().Be(expectedFileName);
    }

    [Test]
    public void HashiTestFieldsFileName_WhenAccessed_ShouldReturnExpectedValue()
    {
        // Arrange
        const string expectedFileName = "HashiTestfields.json";

        // Act
        var result = pathProvider.HashiTestFieldsFileName;

        // Assert
        result.Should().Be(expectedFileName);
    }

    [Test]
    public void LocalAppDataPath_WhenAccessed_ShouldReturnExpectedValue()
    {
        // Arrange
        const string expectedPath = @"CN_Hashi\Settings";

        // Act
        var result = pathProvider.LocalAppDataPath;

        // Assert
        result.Should().Be(expectedPath);
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldCombineLocalAppDataAndLocalAppDataPath()
    {
        // Arrange
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var expectedPath = Path.Combine(localAppDataFolder, @"CN_Hashi\Settings");

        // Act
        var result = pathProvider.SettingsDirectoryPath;

        // Assert
        result.Should().Be(expectedPath);
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldUseEnvironmentLocalApplicationData()
    {
        // Arrange
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Act
        var result = pathProvider.SettingsDirectoryPath;

        // Assert
        result.Should().StartWith(localAppDataFolder);
        result.Should().EndWith(@"CN_Hashi\Settings");
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessed_ShouldCombineSettingsDirectoryAndFileName()
    {
        // Arrange
        var expectedPath = Path.Combine(pathProvider.SettingsDirectoryPath, pathProvider.HashiSettingsFileName);

        // Act
        var result = pathProvider.HashiSettingsFilePath;

        // Assert
        result.Should().Be(expectedPath);
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessed_ShouldEndWithCorrectFileName()
    {
        // Act
        var result = pathProvider.HashiSettingsFilePath;

        // Assert
        result.Should().EndWith("HashiSettings.json");
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessed_ShouldCombineSettingsDirectoryAndFileName()
    {
        // Arrange
        var expectedPath = Path.Combine(pathProvider.SettingsDirectoryPath, pathProvider.HashiTestFieldsFileName);

        // Act
        var result = pathProvider.HashiTestFieldsFilePath;

        // Assert
        result.Should().Be(expectedPath);
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessed_ShouldEndWithCorrectFileName()
    {
        // Act
        var result = pathProvider.HashiTestFieldsFilePath;

        // Assert
        result.Should().EndWith("HashiTestfields.json");
    }

    [Test]
    public void AllFilePaths_WhenAccessed_ShouldBeValidPaths()
    {
        // Act
        var settingsFilePath = pathProvider.HashiSettingsFilePath;
        var testFieldsFilePath = pathProvider.HashiTestFieldsFilePath;
        var settingsDirectoryPath = pathProvider.SettingsDirectoryPath;

        // Assert
        var pathValidationAction1 = () => Path.GetFullPath(settingsFilePath);
        var pathValidationAction2 = () => Path.GetFullPath(testFieldsFilePath);
        var pathValidationAction3 = () => Path.GetFullPath(settingsDirectoryPath);

        pathValidationAction1.Should().NotThrow();
        pathValidationAction2.Should().NotThrow();
        pathValidationAction3.Should().NotThrow();
    }

    [Test]
    public void AllFilePaths_WhenAccessed_ShouldNotBeEmpty()
    {
        // Act & Assert
        pathProvider.HashiSettingsFilePath.Should().NotBeNullOrEmpty();
        pathProvider.HashiTestFieldsFilePath.Should().NotBeNullOrEmpty();
        pathProvider.SettingsDirectoryPath.Should().NotBeNullOrEmpty();
        pathProvider.HashiSettingsFileName.Should().NotBeNullOrEmpty();
        pathProvider.HashiTestFieldsFileName.Should().NotBeNullOrEmpty();
        pathProvider.LocalAppDataPath.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void AllFilePaths_WhenAccessedMultipleTimes_ShouldReturnConsistentValues()
    {
        // Act
        var settingsFilePath1 = pathProvider.HashiSettingsFilePath;
        var settingsFilePath2 = pathProvider.HashiSettingsFilePath;
        var testFieldsFilePath1 = pathProvider.HashiTestFieldsFilePath;
        var testFieldsFilePath2 = pathProvider.HashiTestFieldsFilePath;
        var settingsDirectoryPath1 = pathProvider.SettingsDirectoryPath;
        var settingsDirectoryPath2 = pathProvider.SettingsDirectoryPath;

        // Assert
        settingsFilePath1.Should().Be(settingsFilePath2);
        testFieldsFilePath1.Should().Be(testFieldsFilePath2);
        settingsDirectoryPath1.Should().Be(settingsDirectoryPath2);
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldNotContainFileName()
    {
        // Act
        var result = pathProvider.SettingsDirectoryPath;

        // Assert
        result.Should().NotEndWith(".json");
        result.Should().NotContain("HashiSettings");
        result.Should().NotContain("HashiTestfields");
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessed_ShouldContainSettingsDirectory()
    {
        // Act
        var filePath = pathProvider.HashiSettingsFilePath;
        var directoryPath = pathProvider.SettingsDirectoryPath;

        // Assert
        filePath.Should().StartWith(directoryPath);
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessed_ShouldContainSettingsDirectory()
    {
        // Act
        var filePath = pathProvider.HashiTestFieldsFilePath;
        var directoryPath = pathProvider.SettingsDirectoryPath;

        // Assert
        filePath.Should().StartWith(directoryPath);
    }

    [Test]
    public void LocalAppDataPath_WhenAccessed_ShouldUseCorrectDirectorySeparator()
    {
        // Act
        var result = pathProvider.LocalAppDataPath;

        // Assert
        result.Should().Contain("CN_Hashi");
        result.Should().Contain("Settings");
    }

    [Test]
    public void AllFileNames_WhenAccessed_ShouldHaveJsonExtension()
    {
        // Act & Assert
        pathProvider.HashiSettingsFileName.Should().EndWith(".json");
        pathProvider.HashiTestFieldsFileName.Should().EndWith(".json");
    }

    [Test]
    public void AllFileNames_WhenAccessed_ShouldNotContainDirectorySeparators()
    {
        // Act
        var settingsFileName = pathProvider.HashiSettingsFileName;
        var testFieldsFileName = pathProvider.HashiTestFieldsFileName;

        // Assert
        settingsFileName.Should().NotContain("\\");
        settingsFileName.Should().NotContain("/");
        testFieldsFileName.Should().NotContain("\\");
        testFieldsFileName.Should().NotContain("/");
    }

    [Test]
    public void PathProvider_WhenMultipleInstancesCreated_ShouldReturnSameValues()
    {
        // Arrange
        var provider1 = new PathProvider();
        var provider2 = new PathProvider();

        // Act & Assert
        provider1.HashiSettingsFilePath.Should().Be(provider2.HashiSettingsFilePath);
        provider1.HashiTestFieldsFilePath.Should().Be(provider2.HashiTestFieldsFilePath);
        provider1.SettingsDirectoryPath.Should().Be(provider2.SettingsDirectoryPath);
        provider1.HashiSettingsFileName.Should().Be(provider2.HashiSettingsFileName);
        provider1.HashiTestFieldsFileName.Should().Be(provider2.HashiTestFieldsFileName);
        provider1.LocalAppDataPath.Should().Be(provider2.LocalAppDataPath);
    }
}