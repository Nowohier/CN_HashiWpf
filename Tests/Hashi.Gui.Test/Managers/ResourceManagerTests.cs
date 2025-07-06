using FluentAssertions;
using Hashi.Gui.Interfaces.Managers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Managers;
using Moq;

namespace Hashi.Gui.Test.Managers;

[TestFixture]
public class ResourceManagerTests
{
    private ResourceManager resourceManager;
    private Mock<IPathProvider> pathProviderMock;
    private Mock<IFileWrapper> fileWrapperMock;
    private Mock<IDirectoryWrapper> directoryWrapperMock;

    [SetUp]
    public void SetUp()
    {
        pathProviderMock = new Mock<IPathProvider>(MockBehavior.Strict);
        fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);
        directoryWrapperMock = new Mock<IDirectoryWrapper>(MockBehavior.Strict);

        // Setup path provider
        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns("/test/settings");
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns("/test/settings/HashiSettings.json");
        pathProviderMock.Setup(x => x.HashiTestFieldsFilePath).Returns("/test/settings/HashiTestfields.json");
        pathProviderMock.Setup(x => x.HashiSettingsFileName).Returns("HashiSettings.json");
        pathProviderMock.Setup(x => x.HashiTestFieldsFileName).Returns("HashiTestfields.json");

        resourceManager = new ResourceManager(
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Act
        var result = new ResourceManager(
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResourceManager>();
    }

    [Test]
    public void Constructor_WhenPathProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new ResourceManager(
            null!,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenFileWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new ResourceManager(
            pathProviderMock.Object,
            null!,
            directoryWrapperMock.Object);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenDirectoryWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new ResourceManager(
            pathProviderMock.Object,
            fileWrapperMock.Object,
            null!);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void PrepareUi_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(false);
        directoryWrapperMock.Setup(x => x.CreateDirectory("/test/settings"));
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json")).Returns(true);

        // Act
        resourceManager.PrepareUi();

        // Assert
        directoryWrapperMock.Verify(x => x.CreateDirectory("/test/settings"), Times.Once);
    }

    [Test]
    public void PrepareUi_WhenDirectoryExists_ShouldNotCreateDirectory()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json")).Returns(true);

        // Act
        resourceManager.PrepareUi();

        // Assert
        directoryWrapperMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void PrepareUi_WhenCalled_ShouldCheckBothFiles()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json")).Returns(true);

        // Act
        resourceManager.PrepareUi();

        // Assert
        fileWrapperMock.Verify(x => x.Exists("/test/settings/HashiSettings.json"), Times.Once);
        fileWrapperMock.Verify(x => x.Exists("/test/settings/HashiTestfields.json"), Times.Once);
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenDirectoryExists_ShouldDeleteDirectoryAndRecreate()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(true);
        directoryWrapperMock.Setup(x => x.Delete("/test/settings", true));
        
        // For the PrepareUi call after deletion
        directoryWrapperMock.Setup(x => x.CreateDirectory("/test/settings"));
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json")).Returns(true);

        // Act
        resourceManager.ResetSettingsAndLoadFromDefault();

        // Assert
        directoryWrapperMock.Verify(x => x.Delete("/test/settings", true), Times.Once);
        directoryWrapperMock.Verify(x => x.CreateDirectory("/test/settings"), Times.Once);
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenDirectoryDoesNotExist_ShouldNotDeleteButStillPrepare()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(false);
        
        // For the PrepareUi call
        directoryWrapperMock.Setup(x => x.CreateDirectory("/test/settings"));
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json")).Returns(true);

        // Act
        resourceManager.ResetSettingsAndLoadFromDefault();

        // Assert
        directoryWrapperMock.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        directoryWrapperMock.Verify(x => x.CreateDirectory("/test/settings"), Times.Once);
    }

    [Test]
    public void ResourceManager_ShouldImplementIResourceManager()
    {
        // Act & Assert
        resourceManager.Should().BeAssignableTo<IResourceManager>();
    }

    [Test]
    public void PrepareUi_WhenCalledMultipleTimes_ShouldBehaveConsistently()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json")).Returns(true);

        // Act
        resourceManager.PrepareUi();
        resourceManager.PrepareUi();

        // Assert
        directoryWrapperMock.Verify(x => x.Exists("/test/settings"), Times.Exactly(2));
        fileWrapperMock.Verify(x => x.Exists("/test/settings/HashiSettings.json"), Times.Exactly(2));
        fileWrapperMock.Verify(x => x.Exists("/test/settings/HashiTestfields.json"), Times.Exactly(2));
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var manager1 = new ResourceManager(
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        var manager2 = new ResourceManager(
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);

        // Assert
        manager1.Should().NotBeSameAs(manager2);
    }

    [Test]
    public void PrepareUi_WhenDirectoryCreationIsNeeded_ShouldCreateDirectoryFirst()
    {
        // Arrange
        var callOrder = new List<string>();
        
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(false);
        directoryWrapperMock.Setup(x => x.CreateDirectory("/test/settings"))
            .Callback(() => callOrder.Add("CreateDirectory"));
        
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json"))
            .Returns(true)
            .Callback(() => callOrder.Add("CheckSettingsFile"));
        
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json"))
            .Returns(true)
            .Callback(() => callOrder.Add("CheckTestFieldsFile"));

        // Act
        resourceManager.PrepareUi();

        // Assert
        callOrder.Should().ContainInOrder("CreateDirectory", "CheckSettingsFile", "CheckTestFieldsFile");
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenCalled_ShouldCallPrepareUiAfterDeletion()
    {
        // Arrange
        var callOrder = new List<string>();
        
        directoryWrapperMock.Setup(x => x.Exists("/test/settings")).Returns(true);
        directoryWrapperMock.Setup(x => x.Delete("/test/settings", true))
            .Callback(() => callOrder.Add("Delete"));
        
        directoryWrapperMock.Setup(x => x.CreateDirectory("/test/settings"))
            .Callback(() => callOrder.Add("CreateDirectory"));
        
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiSettings.json"))
            .Returns(true)
            .Callback(() => callOrder.Add("CheckFile"));
        
        fileWrapperMock.Setup(x => x.Exists("/test/settings/HashiTestfields.json"))
            .Returns(true);

        // Act
        resourceManager.ResetSettingsAndLoadFromDefault();

        // Assert
        callOrder.Should().ContainInOrder("Delete", "CreateDirectory", "CheckFile");
    }

    [Test]
    public void AllMethods_ShouldBePublic()
    {
        // Arrange
        var type = typeof(ResourceManager);

        // Act
        var prepareUiMethod = type.GetMethod(nameof(ResourceManager.PrepareUi));
        var resetMethod = type.GetMethod(nameof(ResourceManager.ResetSettingsAndLoadFromDefault));

        // Assert
        prepareUiMethod.Should().NotBeNull();
        prepareUiMethod!.IsPublic.Should().BeTrue();
        
        resetMethod.Should().NotBeNull();
        resetMethod!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void ResourceManager_ShouldHaveCorrectMethodSignatures()
    {
        // Arrange
        var type = typeof(ResourceManager);

        // Act & Assert
        var prepareUiMethod = type.GetMethod(nameof(ResourceManager.PrepareUi));
        prepareUiMethod.Should().NotBeNull();
        prepareUiMethod!.ReturnType.Should().Be(typeof(void));
        prepareUiMethod.GetParameters().Should().BeEmpty();

        var resetMethod = type.GetMethod(nameof(ResourceManager.ResetSettingsAndLoadFromDefault));
        resetMethod.Should().NotBeNull();
        resetMethod!.ReturnType.Should().Be(typeof(void));
        resetMethod.GetParameters().Should().BeEmpty();
    }
}