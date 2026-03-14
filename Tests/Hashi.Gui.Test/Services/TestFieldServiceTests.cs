using FluentAssertions;
using Hashi.Enums;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Services;
using Moq;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Test.Services;

[TestFixture]
public class TestFieldServiceTests
{
    private Mock<ITestSolutionProvider> testSolutionProviderMock;
    private Mock<IDialogWrapper> dialogWrapperMock;
    private Mock<ISolutionProvider> solutionProviderMock;
    private Mock<Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider>>
        solutionProviderFactoryMock;
    private TestFieldService service;

    [SetUp]
    public void SetUp()
    {
        testSolutionProviderMock = new Mock<ITestSolutionProvider>(MockBehavior.Strict);
        dialogWrapperMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
        solutionProviderMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        solutionProviderFactoryMock =
            new Mock<Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider>>(
                MockBehavior.Strict);

        testSolutionProviderMock.SetupProperty(x => x.SelectedSolutionProvider, solutionProviderMock.Object);
        testSolutionProviderMock.Setup(x => x.SolutionProviders)
            .Returns(new ObservableCollection<ISolutionProvider> { solutionProviderMock.Object });
        testSolutionProviderMock.Setup(x => x.ConvertIslandsToSolutionProvider(It.IsAny<IEnumerable<IIslandViewModel>>()));
        testSolutionProviderMock.Setup(x => x.SaveTestFields());
        testSolutionProviderMock.Setup(x => x.HashiFieldReference).Returns([[1, 2]]);

        solutionProviderMock.Setup(x => x.Name).Returns("TestSolution");

        solutionProviderFactoryMock
            .Setup(x => x(It.IsAny<IReadOnlyList<int[]>?>(), It.IsAny<IReadOnlyList<IBridgeCoordinates>?>(),
                It.IsAny<string?>()))
            .Returns(solutionProviderMock.Object);

        service = new TestFieldService(
            testSolutionProviderMock.Object,
            dialogWrapperMock.Object,
            solutionProviderFactoryMock.Object);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenTestSolutionProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new TestFieldService(null!, dialogWrapperMock.Object, solutionProviderFactoryMock.Object);
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenDialogWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () =>
            new TestFieldService(testSolutionProviderMock.Object, null!, solutionProviderFactoryMock.Object);
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () =>
            new TestFieldService(testSolutionProviderMock.Object, dialogWrapperMock.Object, null!);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region SaveTestField Tests

    [Test]
    public void SaveTestField_WhenCalled_ShouldConvertAndSave()
    {
        // Arrange
        var islands = new List<IIslandViewModel>();

        // Act
        service.SaveTestField(islands);

        // Assert
        testSolutionProviderMock.Verify(x => x.ConvertIslandsToSolutionProvider(islands), Times.Once);
        testSolutionProviderMock.Verify(x => x.SaveTestFields(), Times.Once);
    }

    #endregion

    #region DeleteTestField Tests

    [Test]
    public void DeleteTestField_WhenSelectedIsNull_ShouldNotShowDialog()
    {
        // Arrange
        testSolutionProviderMock.SetupGet(x => x.SelectedSolutionProvider).Returns((ISolutionProvider)null!);

        // Act
        service.DeleteTestField();

        // Assert
        dialogWrapperMock.Verify(
            x => x.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogButton>(), It.IsAny<DialogImage>()),
            Times.Never);
    }

    [Test]
    public void DeleteTestField_WhenUserClicksNo_ShouldNotDelete()
    {
        // Arrange
        dialogWrapperMock.Setup(x =>
                x.Show(It.IsAny<string>(), It.IsAny<string>(), DialogButton.YesNo, DialogImage.Question))
            .Returns(DialogResult.No);

        // Act
        service.DeleteTestField();

        // Assert
        testSolutionProviderMock.Object.SolutionProviders.Should().Contain(solutionProviderMock.Object);
    }

    [Test]
    public void DeleteTestField_WhenUserClicksYes_ShouldDeleteAndSave()
    {
        // Arrange
        dialogWrapperMock.Setup(x =>
                x.Show(It.IsAny<string>(), It.IsAny<string>(), DialogButton.YesNo, DialogImage.Question))
            .Returns(DialogResult.Yes);

        // Act
        service.DeleteTestField();

        // Assert
        testSolutionProviderMock.Object.SolutionProviders.Should().NotContain(solutionProviderMock.Object);
        testSolutionProviderMock.Verify(x => x.SaveTestFields(), Times.Once);
    }

    #endregion

    #region CreateTestField Tests

    [Test]
    public void CreateTestField_WhenCalled_ShouldCreateAndSave()
    {
        // Arrange
        service.NewRuleName = "NewRule";

        // Act
        service.CreateTestField();

        // Assert
        testSolutionProviderMock.Object.SolutionProviders.Should().HaveCount(2);
        testSolutionProviderMock.Verify(x => x.SaveTestFields(), Times.Once);
    }

    #endregion

    #region CanCreateTestField Tests

    [Test]
    public void CanCreateTestField_WhenNameIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        service.NewRuleName = string.Empty;

        // Act & Assert
        service.CanCreateTestField().Should().BeFalse();
    }

    [Test]
    public void CanCreateTestField_WhenNameAlreadyExists_ShouldReturnFalse()
    {
        // Arrange
        service.NewRuleName = "TestSolution";

        // Act & Assert
        service.CanCreateTestField().Should().BeFalse();
    }

    [Test]
    public void CanCreateTestField_WhenNameIsUnique_ShouldReturnTrue()
    {
        // Arrange
        service.NewRuleName = "UniqueNewRule";

        // Act & Assert
        service.CanCreateTestField().Should().BeTrue();
    }

    #endregion
}
