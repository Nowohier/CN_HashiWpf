using FluentAssertions;
using Hashi.Gui.Providers;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class PathProviderTests
{
    private PathProvider sut;

    [SetUp]
    public void SetUp()
    {
        sut = new PathProvider();
    }

    [Test]
    public void LocalAppDataPath_WhenAccessed_ShouldReturnCorrectPath()
    {
        // Arrange & Act
        var path = sut.LocalAppDataPath;

        // Assert
        path.Should().Be(@"CN_Hashi\Settings");
    }

    [Test]
    public void HashiSettingsFileName_WhenAccessed_ShouldReturnCorrectFileName()
    {
        // Arrange & Act
        var fileName = sut.HashiSettingsFileName;

        // Assert
        fileName.Should().Be("HashiSettings.json");
    }

    [Test]
    public void HashiTestFieldsFileName_WhenAccessed_ShouldReturnCorrectFileName()
    {
        // Arrange & Act
        var fileName = sut.HashiTestFieldsFileName;

        // Assert
        fileName.Should().Be("HashiTestfields.json");
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldReturnValidPath()
    {
        // Arrange & Act
        var path = sut.SettingsDirectoryPath;

        // Assert
        path.Should().NotBeNullOrEmpty();
        path.Should().Contain("CN_Hashi");
        path.Should().Contain("Settings");
        Path.IsPathRooted(path).Should().BeTrue();
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldContainLocalApplicationData()
    {
        // Arrange
        var expectedLocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Act
        var path = sut.SettingsDirectoryPath;

        // Assert
        path.Should().StartWith(expectedLocalAppData);
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessed_ShouldCombineDirectoryAndFileName()
    {
        // Arrange
        var expectedDirectory = sut.SettingsDirectoryPath;
        var expectedFileName = sut.HashiSettingsFileName;

        // Act
        var filePath = sut.HashiSettingsFilePath;

        // Assert
        filePath.Should().Be(Path.Combine(expectedDirectory, expectedFileName));
        filePath.Should().EndWith("HashiSettings.json");
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessed_ShouldCombineDirectoryAndFileName()
    {
        // Arrange
        var expectedDirectory = sut.SettingsDirectoryPath;
        var expectedFileName = sut.HashiTestFieldsFileName;

        // Act
        var filePath = sut.HashiTestFieldsFilePath;

        // Assert
        filePath.Should().Be(Path.Combine(expectedDirectory, expectedFileName));
        filePath.Should().EndWith("HashiTestfields.json");
    }

    [Test]
    public void HashiSettingsFilePath_WhenAccessed_ShouldReturnValidPath()
    {
        // Arrange & Act
        var filePath = sut.HashiSettingsFilePath;

        // Assert
        filePath.Should().NotBeNullOrEmpty();
        Path.IsPathRooted(filePath).Should().BeTrue();
        Path.GetExtension(filePath).Should().Be(".json");
    }

    [Test]
    public void HashiTestFieldsFilePath_WhenAccessed_ShouldReturnValidPath()
    {
        // Arrange & Act
        var filePath = sut.HashiTestFieldsFilePath;

        // Assert
        filePath.Should().NotBeNullOrEmpty();
        Path.IsPathRooted(filePath).Should().BeTrue();
        Path.GetExtension(filePath).Should().Be(".json");
    }

    [Test]
    public void Properties_WhenAccessedMultipleTimes_ShouldReturnConsistentResults()
    {
        // Arrange & Act
        var settingsPath1 = sut.HashiSettingsFilePath;
        var settingsPath2 = sut.HashiSettingsFilePath;
        var testFieldsPath1 = sut.HashiTestFieldsFilePath;
        var testFieldsPath2 = sut.HashiTestFieldsFilePath;
        var directoryPath1 = sut.SettingsDirectoryPath;
        var directoryPath2 = sut.SettingsDirectoryPath;

        // Assert
        settingsPath1.Should().Be(settingsPath2);
        testFieldsPath1.Should().Be(testFieldsPath2);
        directoryPath1.Should().Be(directoryPath2);
    }

    [Test]
    public void SettingsDirectoryPath_WhenAccessed_ShouldNotContainFileExtension()
    {
        // Arrange & Act
        var path = sut.SettingsDirectoryPath;

        // Assert
        Path.HasExtension(path).Should().BeFalse();
    }

    [Test]
    public void FilePaths_WhenAccessed_ShouldBeDifferent()
    {
        // Arrange & Act
        var settingsPath = sut.HashiSettingsFilePath;
        var testFieldsPath = sut.HashiTestFieldsFilePath;

        // Assert
        settingsPath.Should().NotBe(testFieldsPath);
    }

    [Test]
    public void FileNames_WhenAccessed_ShouldBeDifferent()
    {
        // Arrange & Act
        var settingsFileName = sut.HashiSettingsFileName;
        var testFieldsFileName = sut.HashiTestFieldsFileName;

        // Assert
        settingsFileName.Should().NotBe(testFieldsFileName);
    }

    [Test]
    public void FilePaths_WhenAccessed_ShouldHaveSameDirectory()
    {
        // Arrange & Act
        var settingsPath = sut.HashiSettingsFilePath;
        var testFieldsPath = sut.HashiTestFieldsFilePath;
        var expectedDirectory = sut.SettingsDirectoryPath;

        // Assert
        Path.GetDirectoryName(settingsPath).Should().Be(expectedDirectory);
        Path.GetDirectoryName(testFieldsPath).Should().Be(expectedDirectory);
    }
}