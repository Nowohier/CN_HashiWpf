using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Managers;
using Hashi.Logging.Interfaces;
using Moq;

namespace Hashi.Gui.Test.Managers;

[TestFixture]
public class ResourceManagerTests
{
    private Mock<IPathProvider> mockPathProvider;
    private Mock<ILogger> loggerMock;
    private ResourceManager sut;
    private string testDirectoryPath;
    private string testSettingsFilePath;
    private string testTestFieldsFilePath;

    [SetUp]
    public void SetUp()
    {
        testDirectoryPath = Path.Combine(Path.GetTempPath(), "HashiGuiTest", Guid.NewGuid().ToString());
        testSettingsFilePath = Path.Combine(testDirectoryPath, "hashisettings.json");
        testTestFieldsFilePath = Path.Combine(testDirectoryPath, "hashitestfields.json");

        mockPathProvider = new Mock<IPathProvider>();
        mockPathProvider.Setup(x => x.SettingsDirectoryPath).Returns(testDirectoryPath);
        mockPathProvider.Setup(x => x.HashiSettingsFilePath).Returns(testSettingsFilePath);
        mockPathProvider.Setup(x => x.HashiTestFieldsFilePath).Returns(testTestFieldsFilePath);
        mockPathProvider.Setup(x => x.HashiSettingsFileName).Returns("hashisettings.json");
        mockPathProvider.Setup(x => x.HashiTestFieldsFileName).Returns("hashitestfields.json");

        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        loggerMock.Setup(x => x.Error(It.IsAny<string>())).Verifiable();

        sut = new ResourceManager(mockPathProvider.Object, loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test directory
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
    public void Constructor_WhenCalledWithValidPathProvider_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var act = () => new ResourceManager(mockPathProvider.Object, loggerMock.Object);
        act.Should().NotThrow();
    }

    [Test]
    public void PrepareUi_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        // Arrange
        Directory.Exists(testDirectoryPath).Should().BeFalse();

        // Act
        sut.PrepareUi();

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeTrue();
    }

    [Test]
    public void PrepareUi_WhenDirectoryExists_ShouldNotThrow()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        Directory.Exists(testDirectoryPath).Should().BeTrue();

        // Act & Assert
        sut.Invoking(x => x.PrepareUi()).Should().NotThrow();
    }

    [Test]
    public void PrepareUi_WhenCalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        sut.Invoking(x => x.PrepareUi()).Should().NotThrow();
        sut.Invoking(x => x.PrepareUi()).Should().NotThrow();
        sut.Invoking(x => x.PrepareUi()).Should().NotThrow();
    }

    [Test]
    public void PrepareUi_WhenSettingsFileExists_ShouldNotOverwriteFile()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        var existingContent = "existing content";
        File.WriteAllText(testSettingsFilePath, existingContent);

        // Act
        sut.PrepareUi();

        // Assert
        File.ReadAllText(testSettingsFilePath).Should().Be(existingContent);
    }

    [Test]
    public void PrepareUi_WhenTestFieldsFileExists_ShouldNotOverwriteFile()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        var existingContent = "existing test fields content";
        File.WriteAllText(testTestFieldsFilePath, existingContent);

        // Act
        sut.PrepareUi();

        // Assert
        File.ReadAllText(testTestFieldsFilePath).Should().Be(existingContent);
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenDirectoryExists_ShouldDeleteAndRecreateDirectory()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        var testFile = Path.Combine(testDirectoryPath, "testfile.txt");
        File.WriteAllText(testFile, "test content");
        File.Exists(testFile).Should().BeTrue();

        // Act
        sut.ResetSettingsAndLoadFromDefault();

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeTrue();
        File.Exists(testFile).Should().BeFalse();
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        // Arrange
        Directory.Exists(testDirectoryPath).Should().BeFalse();

        // Act
        sut.ResetSettingsAndLoadFromDefault();

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeTrue();
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenCalled_ShouldCallPrepareUi()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        var testFile = Path.Combine(testDirectoryPath, "testfile.txt");
        File.WriteAllText(testFile, "test content");

        // Act
        sut.ResetSettingsAndLoadFromDefault();

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeTrue();
        File.Exists(testFile).Should().BeFalse();
    }

    [Test]
    public void PrepareUi_WithValidPaths_ShouldCallPathProviderProperties()
    {
        // Arrange & Act
        sut.PrepareUi();

        // Assert
        mockPathProvider.Verify(x => x.SettingsDirectoryPath, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiSettingsFilePath, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiTestFieldsFilePath, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiSettingsFileName, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiTestFieldsFileName, Times.AtLeastOnce);
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WithValidPaths_ShouldCallPathProviderProperties()
    {
        // Arrange & Act
        sut.ResetSettingsAndLoadFromDefault();

        // Assert
        mockPathProvider.Verify(x => x.SettingsDirectoryPath, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiSettingsFilePath, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiTestFieldsFilePath, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiSettingsFileName, Times.AtLeastOnce);
        mockPathProvider.Verify(x => x.HashiTestFieldsFileName, Times.AtLeastOnce);
    }

    [Test]
    public void PrepareUi_WhenPathProviderReturnsInvalidPath_ShouldHandleGracefully()
    {
        // Arrange
        var invalidPathProvider = new Mock<IPathProvider>();
        invalidPathProvider.Setup(x => x.SettingsDirectoryPath).Returns("");
        invalidPathProvider.Setup(x => x.HashiSettingsFilePath).Returns("");
        invalidPathProvider.Setup(x => x.HashiTestFieldsFilePath).Returns("");
        invalidPathProvider.Setup(x => x.HashiSettingsFileName).Returns("settings.json");
        invalidPathProvider.Setup(x => x.HashiTestFieldsFileName).Returns("testfields.json");

        var errorMessage = string.Format(ResourceManager.ErrorMessage, invalidPathProvider.Object.SettingsDirectoryPath, "*");

        var resourceManager = new ResourceManager(invalidPathProvider.Object, loggerMock.Object);

        // Act & Assert
        resourceManager
            .Invoking(x => x.PrepareUi())
            .Should()
            .Throw<ArgumentException>()
            .WithMessage(errorMessage);
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenPathProviderReturnsInvalidPath_ShouldHandleGracefully()
    {
        // Arrange
        var invalidPathProvider = new Mock<IPathProvider>();
        invalidPathProvider.Setup(x => x.SettingsDirectoryPath).Returns("");
        invalidPathProvider.Setup(x => x.HashiSettingsFilePath).Returns("");
        invalidPathProvider.Setup(x => x.HashiTestFieldsFilePath).Returns("");
        invalidPathProvider.Setup(x => x.HashiSettingsFileName).Returns("settings.json");
        invalidPathProvider.Setup(x => x.HashiTestFieldsFileName).Returns("testfields.json");

        var errorMessage = string.Format(ResourceManager.ErrorMessage, invalidPathProvider.Object.SettingsDirectoryPath, "*");

        var resourceManager = new ResourceManager(invalidPathProvider.Object, loggerMock.Object);

        // Act & Assert
        resourceManager
            .Invoking(x => x.ResetSettingsAndLoadFromDefault())
            .Should()
            .Throw<ArgumentException>()
            .WithMessage(errorMessage);
    }
}