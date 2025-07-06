using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Providers;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class PathProviderTests
{
    private PathProvider pathProvider;

    [SetUp]
    public void SetUp()
    {
        pathProvider = new PathProvider();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeProvider()
    {
        // Act
        var result = new PathProvider();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IPathProvider>();
    }

    [Test]
    public void LocalAppDataPath_WhenAccessed_ShouldReturnCorrectPath()
    {
        // Act
        var result = pathProvider.LocalAppDataPath;

        // Assert
        result.Should().Be(@"CN_Hashi\Settings");
    }

    [Test]
    public void HashiSettingsFileName_WhenAccessed_ShouldReturnCorrectFileName()
    {
        // Act
        var result = pathProvider.HashiSettingsFileName;

        // Assert
        result.Should().Be("HashiSettings.json");
    }

    [Test]
    public void HashiTestFieldsFileName_WhenAccessed_ShouldReturnCorrectFileName()
    {
        // Act
        var result = pathProvider.HashiTestFieldsFileName;

        // Assert
        result.Should().Be("HashiTestfields.json");
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldReturnValidPath()
    {
        // Act
        var result = pathProvider.SettingsDirectoryPath;

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("CN_Hashi");
        result.Should().Contain("Settings");
        result.Should().Contain(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessed_ShouldReturnCorrectPath()
    {
        // Act
        var result = pathProvider.HashiSettingsFilePath;

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().EndWith("HashiSettings.json");
        result.Should().Contain("CN_Hashi");
        result.Should().Contain("Settings");
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessed_ShouldReturnCorrectPath()
    {
        // Act
        var result = pathProvider.HashiTestFieldsFilePath;

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().EndWith("HashiTestfields.json");
        result.Should().Contain("CN_Hashi");
        result.Should().Contain("Settings");
    }

    [Test]
    public void PathProvider_ShouldImplementIPathProvider()
    {
        // Act & Assert
        pathProvider.Should().BeAssignableTo<IPathProvider>();
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessedMultipleTimes_ShouldReturnSameValue()
    {
        // Act
        var result1 = pathProvider.HashiSettingsFilePath;
        var result2 = pathProvider.HashiSettingsFilePath;

        // Assert
        result1.Should().Be(result2);
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessedMultipleTimes_ShouldReturnSameValue()
    {
        // Act
        var result1 = pathProvider.HashiTestFieldsFilePath;
        var result2 = pathProvider.HashiTestFieldsFilePath;

        // Assert
        result1.Should().Be(result2);
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessedMultipleTimes_ShouldReturnSameValue()
    {
        // Act
        var result1 = pathProvider.SettingsDirectoryPath;
        var result2 = pathProvider.SettingsDirectoryPath;

        // Assert
        result1.Should().Be(result2);
    }

    [Test]
    public void AllPaths_WhenAccessed_ShouldBeValidPaths()
    {
        // Act
        var settingsFilePath = pathProvider.HashiSettingsFilePath;
        var testFieldsFilePath = pathProvider.HashiTestFieldsFilePath;
        var settingsDirectoryPath = pathProvider.SettingsDirectoryPath;

        // Assert
        settingsFilePath.Should().NotBeNullOrEmpty();
        testFieldsFilePath.Should().NotBeNullOrEmpty();
        settingsDirectoryPath.Should().NotBeNullOrEmpty();

        // Verify paths don't contain invalid characters
        var invalidChars = Path.GetInvalidPathChars();
        settingsFilePath.Should().NotContainAny(invalidChars);
        testFieldsFilePath.Should().NotContainAny(invalidChars);
        settingsDirectoryPath.Should().NotContainAny(invalidChars);
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessed_ShouldCombineDirectoryAndFileName()
    {
        // Act
        var settingsDirectoryPath = pathProvider.SettingsDirectoryPath;
        var settingsFileName = pathProvider.HashiSettingsFileName;
        var expectedPath = Path.Combine(settingsDirectoryPath, settingsFileName);
        var actualPath = pathProvider.HashiSettingsFilePath;

        // Assert
        actualPath.Should().Be(expectedPath);
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessed_ShouldCombineDirectoryAndFileName()
    {
        // Act
        var settingsDirectoryPath = pathProvider.SettingsDirectoryPath;
        var testFieldsFileName = pathProvider.HashiTestFieldsFileName;
        var expectedPath = Path.Combine(settingsDirectoryPath, testFieldsFileName);
        var actualPath = pathProvider.HashiTestFieldsFilePath;

        // Assert
        actualPath.Should().Be(expectedPath);
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldCombineLocalAppDataAndAppDataPath()
    {
        // Act
        var localAppDataPath = pathProvider.LocalAppDataPath;
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var expectedPath = Path.Combine(localAppDataFolder, localAppDataPath);
        var actualPath = pathProvider.SettingsDirectoryPath;

        // Assert
        actualPath.Should().Be(expectedPath);
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var provider1 = new PathProvider();
        var provider2 = new PathProvider();

        // Assert
        provider1.Should().NotBeSameAs(provider2);
        provider1.HashiSettingsFilePath.Should().Be(provider2.HashiSettingsFilePath);
        provider1.HashiTestFieldsFilePath.Should().Be(provider2.HashiTestFieldsFilePath);
    }
}