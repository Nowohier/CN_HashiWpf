using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Providers;
using Hashi.Logging.Interfaces;
using Moq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Hashi.Gui.Test.Providers;

/// <summary>
/// Unit tests for TestSolutionProvider class.
/// </summary>
public class TestSolutionProviderTests
{
    private Mock<IJsonWrapper> jsonWrapperMock;
    private Mock<IPathProvider> pathProviderMock;
    private Mock<Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider>> solutionProviderFactoryMock;
    private Mock<Func<ISolutionProvider, ISetTestSolutionMessage>> setTestSolutionMessageFactoryMock;
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<ILogger> loggerMock;
    private TestSolutionProvider sut;

    [SetUp]
    public void SetUp()
    {
        jsonWrapperMock = new Mock<IJsonWrapper>(MockBehavior.Strict);
        pathProviderMock = new Mock<IPathProvider>(MockBehavior.Strict);
        solutionProviderFactoryMock = new Mock<Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider>>(MockBehavior.Strict);
        setTestSolutionMessageFactoryMock = new Mock<Func<ISolutionProvider, ISetTestSolutionMessage>>(MockBehavior.Strict);
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);

        loggerFactoryMock.Setup(f => f.CreateLogger<TestSolutionProvider>()).Returns(loggerMock.Object);
        loggerMock.Setup(l => l.Info(It.IsAny<string>()));

        // Setup for LoadSettings method call in constructor
        pathProviderMock.Setup(p => p.HashiTestFieldsFilePath).Returns("/test/path/testfields.json");
        jsonWrapperMock.Setup(j => j.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(new List<ISolutionProvider>());

        sut = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        jsonWrapperMock.VerifyAll();
        pathProviderMock.VerifyAll();
        loggerFactoryMock.VerifyAll();
        loggerMock.VerifyAll();
    }

    [Test]
    public void Constructor_WhenJsonWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        var action = () => new TestSolutionProvider(
            null!,
            pathProviderMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("jsonWrapper");
    }

    [Test]
    public void Constructor_WhenPathProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        var action = () => new TestSolutionProvider(
            jsonWrapperMock.Object,
            null!,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("pathProvider");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act (done in SetUp)

        // Assert
        sut.Should().NotBeNull();
        sut.Should().BeAssignableTo<ITestSolutionProvider>();
        sut.Should().BeAssignableTo<INotifyPropertyChanged>();
    }

    [Test]
    public void HashiFieldReference_WhenAccessed_ShouldReturnFixedSizeArray()
    {
        // Arrange & Act
        var result = sut.HashiFieldReference;

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(6);
        result.All(row => row.Length == 6).Should().BeTrue();
        result.All(row => row.All(cell => cell == 0)).Should().BeTrue();
    }

    [Test]
    public void SolutionProviders_WhenAccessed_ShouldReturnObservableCollection()
    {
        // Arrange & Act
        var result = sut.SolutionProviders;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObservableCollection<ISolutionProvider>>();
    }

    [Test]
    public void SelectedSolutionProvider_WhenSetToNewValue_ShouldUpdateProperty()
    {
        // Arrange
        var mockSolutionProvider = new Mock<ISolutionProvider>(MockBehavior.Strict);
        var mockMessage = new Mock<ISetTestSolutionMessage>(MockBehavior.Strict);

        setTestSolutionMessageFactoryMock.Setup(f => f.Invoke(mockSolutionProvider.Object))
            .Returns(mockMessage.Object);

        // Act
        sut.SelectedSolutionProvider = mockSolutionProvider.Object;

        // Assert
        sut.SelectedSolutionProvider.Should().Be(mockSolutionProvider.Object);
        setTestSolutionMessageFactoryMock.Verify(f => f.Invoke(mockSolutionProvider.Object), Times.Once);
    }

    [Test]
    public void SelectedSolutionProvider_WhenSetToNull_ShouldUpdatePropertyButNotSendMessage()
    {
        // Arrange & Act
        sut.SelectedSolutionProvider = null;

        // Assert
        sut.SelectedSolutionProvider.Should().BeNull();
        setTestSolutionMessageFactoryMock.Verify(f => f.Invoke(It.IsAny<ISolutionProvider>()), Times.Never);
    }

    [Test]
    public void SelectedSolutionProvider_WhenSetToSameValue_ShouldNotSendMessage()
    {
        // Arrange
        var mockSolutionProvider = new Mock<ISolutionProvider>(MockBehavior.Strict);
        var mockMessage = new Mock<ISetTestSolutionMessage>(MockBehavior.Strict);

        setTestSolutionMessageFactoryMock.Setup(f => f.Invoke(mockSolutionProvider.Object))
            .Returns(mockMessage.Object);

        sut.SelectedSolutionProvider = mockSolutionProvider.Object;
        setTestSolutionMessageFactoryMock.Reset(); // Reset the mock to clear previous invocations

        // Act
        sut.SelectedSolutionProvider = mockSolutionProvider.Object;

        // Assert
        setTestSolutionMessageFactoryMock.Verify(f => f.Invoke(It.IsAny<ISolutionProvider>()), Times.Never);
    }

    [Test]
    public void ResetSettings_WhenCalled_ShouldClearAndReloadSolutionProviders()
    {
        // Arrange
        var initialCount = sut.SolutionProviders.Count;

        // Additional setup for the LoadSettings call within ResetSettings
        jsonWrapperMock.Setup(j => j.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(new List<ISolutionProvider>());

        // Act
        sut.ResetSettings();

        // Assert
        sut.SolutionProviders.Should().BeEmpty();
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenAllIslandEnumerableIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        var action = () => sut.ConvertIslandsToSolutionProvider(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("allIslandEnumerable");
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenSelectedSolutionProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockIslands = new List<IIslandViewModel>();
        sut.SelectedSolutionProvider = null;

        // Act & Assert
        var action = () => sut.ConvertIslandsToSolutionProvider(mockIslands);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("SelectedSolutionProvider");
    }
}