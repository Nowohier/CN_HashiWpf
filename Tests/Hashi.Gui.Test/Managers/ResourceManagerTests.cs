using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Wrappers;
using Moq;

namespace Hashi.Gui.Test.Managers;

[TestFixture]
public class ResourceManagerTests
{
    private Mock<IPathProvider> pathProviderMock;
    private Mock<IFileWrapper> fileWrapperMock;
    private Mock<IDirectoryWrapper> directoryWrapperMock;
    private Gui.Managers.ResourceManager resourceManager;

    [SetUp]
    public void SetUp()
    {
        pathProviderMock = new Mock<IPathProvider>(MockBehavior.Strict);
        fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);
        directoryWrapperMock = new Mock<IDirectoryWrapper>(MockBehavior.Strict);

        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns("settings");
        pathProviderMock.Setup(x => x.HashiSettingsFilePath).Returns("settings/settings.json");
        pathProviderMock.Setup(x => x.HashiSettingsFileName).Returns("settings.json");
        pathProviderMock.Setup(x => x.HashiTestFieldsFilePath).Returns("settings/testfields.json");
        pathProviderMock.Setup(x => x.HashiTestFieldsFileName).Returns("testfields.json");

        resourceManager = new Gui.Managers.ResourceManager(
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object);
    }

    #region PrepareUi Tests

    [Test]
    public void PrepareUi_WhenDirectoryMissing_ShouldCreateDirectory()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("settings")).Returns(false);
        directoryWrapperMock.Setup(x => x.CreateDirectory("settings"));
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

        // Act
        resourceManager.PrepareUi();

        // Assert
        directoryWrapperMock.Verify(x => x.CreateDirectory("settings"), Times.Once);
    }

    [Test]
    public void PrepareUi_WhenDirectoryExists_ShouldNotCreateDirectory()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("settings")).Returns(true);
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

        // Act
        resourceManager.PrepareUi();

        // Assert
        directoryWrapperMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region ResetSettingsAndLoadFromDefault Tests

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenDirectoryExists_ShouldDeleteAndRecreate()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("settings")).Returns(true);
        directoryWrapperMock.Setup(x => x.Delete("settings", true));
        directoryWrapperMock.Setup(x => x.CreateDirectory("settings"));
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

        // Act
        resourceManager.ResetSettingsAndLoadFromDefault();

        // Assert
        directoryWrapperMock.Verify(x => x.Delete("settings", true), Times.Once);
    }

    [Test]
    public void ResetSettingsAndLoadFromDefault_WhenDirectoryMissing_ShouldJustPrepare()
    {
        // Arrange
        directoryWrapperMock.Setup(x => x.Exists("settings")).Returns(false);
        directoryWrapperMock.Setup(x => x.CreateDirectory("settings"));
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

        // Act
        resourceManager.ResetSettingsAndLoadFromDefault();

        // Assert
        directoryWrapperMock.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        directoryWrapperMock.Verify(x => x.CreateDirectory("settings"), Times.Once);
    }

    #endregion
}
