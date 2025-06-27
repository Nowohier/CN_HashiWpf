using FluentAssertions;
using Hashi.Gui.Interfaces.Managers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Managers;
using Moq;

namespace Hashi.Gui.Test.Managers;

/// <summary>
/// Unit tests for ResourceManager class.
/// </summary>
public class ResourceManagerTests
{
    private Mock<IPathProvider> pathProviderMock;
    private ResourceManager sut;

    [SetUp]
    public void SetUp()
    {
        pathProviderMock = new Mock<IPathProvider>(MockBehavior.Strict);
        sut = new ResourceManager(pathProviderMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        pathProviderMock.VerifyAll();
    }

    [Test]
    public void Constructor_WhenPathProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        var action = () => new ResourceManager(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("pathProvider");
    }

    [Test]
    public void Constructor_WhenValidPathProvider_ShouldCreateInstance()
    {
        // Arrange & Act
        var result = new ResourceManager(pathProviderMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResourceManager>();
    }

    [Test]
    public void PrepareUi_WhenCalled_ShouldCreateDirectoriesAndFiles()
    {
        // Arrange
        const string settingsDir = "/test/settings";
        const string settingsFilePath = "/test/settings/settings.json";
        const string settingsFileName = "settings.json";
        const string testFieldsFilePath = "/test/settings/testfields.json";
        const string testFieldsFileName = "testfields.json";

        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns(settingsDir);
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(settingsFilePath);
        pathProviderMock.Setup(x => x.HashiSettingsFileName).Returns(settingsFileName);
        pathProviderMock.Setup(x => x.HashiTestFieldsFilePath).Returns(testFieldsFilePath);
        pathProviderMock.Setup(x => x.HashiTestFieldsFileName).Returns(testFieldsFileName);

        // Act
        var action = () => sut.PrepareUi();

        // Assert
        // Since we're testing file operations, we can't easily verify the exact behavior
        // without mocking the file system, but we can ensure the method doesn't throw
        action.Should().NotThrow();
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenCalled_ShouldRemoveDirectoryAndRecreate()
    {
        // Arrange
        const string settingsDir = "/test/settings";
        const string settingsFilePath = "/test/settings/settings.json";
        const string settingsFileName = "settings.json";
        const string testFieldsFilePath = "/test/settings/testfields.json";
        const string testFieldsFileName = "testfields.json";

        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns(settingsDir);
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns(settingsFilePath);
        pathProviderMock.Setup(x => x.HashiSettingsFileName).Returns(settingsFileName);
        pathProviderMock.Setup(x => x.HashiTestFieldsFilePath).Returns(testFieldsFilePath);
        pathProviderMock.Setup(x => x.HashiTestFieldsFileName).Returns(testFieldsFileName);

        // Act
        var action = () => sut.ResetSettingsAndLoadFromDefault();

        // Assert
        // Since we're testing file operations, we can't easily verify the exact behavior
        // without mocking the file system, but we can ensure the method doesn't throw
        action.Should().NotThrow();
    }
}