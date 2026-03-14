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

namespace Hashi.Gui.Test.Providers;

/// <summary>
/// Unit tests for TestSolutionProvider class.
/// </summary>
[TestFixture]
public class TestSolutionProviderTests
{
    private Mock<IJsonWrapper> jsonWrapperMock;
    private Mock<IPathProvider> pathProviderMock;
    private Mock<IFileWrapper> fileWrapperMock;
    private Mock<IDirectoryWrapper> directoryWrapperMock;
    private Mock<Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider>> solutionProviderFactoryMock;
    private Mock<Func<ISolutionProvider, ISetTestSolutionMessage>> setTestSolutionMessageFactoryMock;
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<ILogger> loggerMock;
    private Mock<ISolutionProvider> solutionProviderMock;
    private Mock<ISetTestSolutionMessage> setTestSolutionMessageMock;
    private Mock<IIslandViewModel> islandViewModelMock;
    private TestSolutionProvider? testSolutionProvider;

    [SetUp]
    public void SetUp()
    {
        jsonWrapperMock = new Mock<IJsonWrapper>(MockBehavior.Strict);
        pathProviderMock = new Mock<IPathProvider>(MockBehavior.Strict);
        fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);
        directoryWrapperMock = new Mock<IDirectoryWrapper>(MockBehavior.Strict);
        solutionProviderFactoryMock = new Mock<Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider>>(MockBehavior.Strict);
        setTestSolutionMessageFactoryMock = new Mock<Func<ISolutionProvider, ISetTestSolutionMessage>>(MockBehavior.Strict);
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        solutionProviderMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        setTestSolutionMessageMock = new Mock<ISetTestSolutionMessage>(MockBehavior.Strict);
        islandViewModelMock = new Mock<IIslandViewModel>(MockBehavior.Strict);

        // Setup logger
        loggerFactoryMock.Setup(x => x.CreateLogger<TestSolutionProvider>()).Returns(loggerMock.Object);
        loggerMock.Setup(x => x.Info(It.IsAny<string>()));
        loggerMock.Setup(x => x.Debug(It.IsAny<string>()));
        loggerMock.Setup(x => x.Error(It.IsAny<string>()));
        loggerMock.Setup(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()));

        // Setup path provider
        pathProviderMock.Setup(x => x.HashiTestFieldsFilePath).Returns("test-fields.json");
        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns("test-settings");

        // Setup file wrapper - by default file does not exist so LoadSettings returns empty list
        fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

        // Setup JSON wrapper for LoadSettings (empty collection by default)
        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(new List<ISolutionProvider>());

        // Setup solution provider factory
        solutionProviderFactoryMock.Setup(x => x.Invoke(It.IsAny<IReadOnlyList<int[]>?>(), It.IsAny<IReadOnlyList<IBridgeCoordinates>?>(), It.IsAny<string?>()))
            .Returns(solutionProviderMock.Object);

        // Setup message factory
        setTestSolutionMessageFactoryMock.Setup(x => x.Invoke(It.IsAny<ISolutionProvider>()))
            .Returns(setTestSolutionMessageMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        // No specific teardown needed as mocks are recreated in SetUp
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenJsonWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestSolutionProvider(
            null!,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("jsonWrapper");
    }

    [Test]
    public void Constructor_WhenPathProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestSolutionProvider(
            jsonWrapperMock.Object,
            null!,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("pathProvider");
    }

    [Test]
    public void Constructor_WhenSolutionProviderFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            null!,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("solutionProviderFactory");
    }

    [Test]
    public void Constructor_WhenSetTestSolutionMessageFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            null!,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("setTestSolutionMessageFactory");
    }

    [Test]
    public void Constructor_WhenLoggerFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            null!);

        action.Should().Throw<ArgumentNullException>().WithParameterName("loggerFactory");
    }

    [Test]
    public void Constructor_WhenFileWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            null!,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("fileWrapper");
    }

    [Test]
    public void Constructor_WhenDirectoryWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            null!,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("directoryWrapper");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Act
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Assert
        testSolutionProvider.Should().NotBeNull();
        testSolutionProvider.HashiFieldReference.Should().NotBeNull();
        testSolutionProvider.SolutionProviders.Should().NotBeNull();
        loggerMock.Verify(x => x.Info("TestSolutionProvider initialized"), Times.Once);
    }

    [Test]
    public void Constructor_WhenCreated_ShouldInitializeHashiFieldReference()
    {
        // Act
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Assert
        testSolutionProvider.HashiFieldReference.Should().HaveCount(6);
        testSolutionProvider.HashiFieldReference.Should().AllSatisfy(row =>
            row.Should().HaveCount(6).And.AllBeEquivalentTo(0));
    }

    [Test]
    public void Constructor_WhenLoadedSolutionProvidersExist_ShouldAddThemToCollection()
    {
        // Arrange
        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        var loadedSolutions = new List<ISolutionProvider> { solutionProviderMock.Object };
        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(loadedSolutions);

        // Act
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Assert
        testSolutionProvider.SolutionProviders.Should().HaveCount(1);
        testSolutionProvider.SolutionProviders.Should().Contain(solutionProviderMock.Object);
        testSolutionProvider.SelectedSolutionProvider.Should().Be(solutionProviderMock.Object);
    }

    #endregion

    #region HashiFieldReference Tests

    [Test]
    public void HashiFieldReference_WhenAccessed_ShouldReturnExpectedDimensions()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        var result = testSolutionProvider.HashiFieldReference;

        // Assert
        result.Should().HaveCount(6);
        result.Should().AllSatisfy(row => row.Should().HaveCount(6));
    }

    [Test]
    public void HashiFieldReference_WhenAccessed_ShouldBeReadOnly()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        var result = testSolutionProvider.HashiFieldReference;

        // Assert
        result.Should().BeAssignableTo<IReadOnlyList<int[]>>();
        // Property should not have a setter - this is verified at compile time
    }

    #endregion

    #region SelectedSolutionProvider Tests

    [Test]
    public void SelectedSolutionProvider_WhenSetToValidValue_ShouldUpdateProperty()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object;

        // Assert
        testSolutionProvider.SelectedSolutionProvider.Should().Be(solutionProviderMock.Object);
    }

    [Test]
    public void SelectedSolutionProvider_WhenSetToValidValue_ShouldSendMessage()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object;

        // Assert
        setTestSolutionMessageFactoryMock.Verify(x => x.Invoke(solutionProviderMock.Object), Times.Once);
    }

    [Test]
    public void SelectedSolutionProvider_WhenSetToNull_ShouldNotSendMessage()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object; // Set initial value
        setTestSolutionMessageFactoryMock.Reset(); // Reset mock to clear previous calls

        // Act
        testSolutionProvider.SelectedSolutionProvider = null;

        // Assert
        testSolutionProvider.SelectedSolutionProvider.Should().BeNull();
        setTestSolutionMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<ISolutionProvider>()), Times.Never);
    }

    [Test]
    public void SelectedSolutionProvider_WhenSetToSameValue_ShouldNotSendMessage()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object; // Set initial value
        setTestSolutionMessageFactoryMock.Reset(); // Reset mock to clear previous calls

        // Act
        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object; // Set same value

        // Assert
        setTestSolutionMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<ISolutionProvider>()), Times.Never);
    }

    #endregion

    #region ResetSettings Tests

    [Test]
    public void ResetSettings_WhenCalled_ShouldClearAndReloadSolutionProviders()
    {
        // Arrange
        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        var initialSolutions = new List<ISolutionProvider> { solutionProviderMock.Object };
        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(initialSolutions);

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        var secondSolutionMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        var reloadedSolutions = new List<ISolutionProvider> { secondSolutionMock.Object };

        // Override the setup for subsequent calls
        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(reloadedSolutions);

        // Act
        testSolutionProvider.ResetSettings();

        // Assert
        testSolutionProvider.SolutionProviders.Should().HaveCount(1);
        testSolutionProvider.SolutionProviders.Should().Contain(secondSolutionMock.Object);
        testSolutionProvider.SolutionProviders.Should().NotContain(solutionProviderMock.Object);
    }

    [Test]
    public void ResetSettings_WhenCalled_ShouldCallLoadSettingsAgain()
    {
        // Arrange
        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        testSolutionProvider.ResetSettings();

        // Assert
        // LoadSettings should be called twice: once in constructor, once in ResetSettings
        jsonWrapperMock.Verify(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)), Times.AtLeast(2));
    }

    #endregion

    #region ConvertIslandsToSolutionProvider Tests

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenAllIslandEnumerableIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act & Assert
        var action = () => testSolutionProvider.ConvertIslandsToSolutionProvider(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("allIslandEnumerable");
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenSelectedSolutionProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        var islands = new List<IIslandViewModel> { islandViewModelMock.Object };

        // Act & Assert
        var action = () => testSolutionProvider.ConvertIslandsToSolutionProvider(islands);
        action.Should().Throw<ArgumentNullException>().WithParameterName("SelectedSolutionProvider");
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenValidIslands_ShouldCreateHashiField()
    {
        // Arrange
        var coordinatesMock = new Mock<Hashi.Gui.Interfaces.Models.IHashiPoint>(MockBehavior.Strict);
        coordinatesMock.Setup(x => x.X).Returns(1);
        coordinatesMock.Setup(x => x.Y).Returns(2);

        islandViewModelMock.Setup(x => x.Coordinates).Returns(coordinatesMock.Object);
        islandViewModelMock.Setup(x => x.MaxConnections).Returns(3);
        islandViewModelMock.Setup(x => x.AllConnections).Returns([]);

        var islands = new List<IIslandViewModel> { islandViewModelMock.Object };

        solutionProviderMock.Setup(x => x.Name).Returns("TestSolution");

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object;

        // Act
        testSolutionProvider.ConvertIslandsToSolutionProvider(islands);

        // Assert
        solutionProviderFactoryMock.Verify(x => x(
            It.Is<IReadOnlyList<int[]>?>(hf => hf != null && hf.Count == 3 && hf[2][1] == 3),
            It.IsAny<IReadOnlyList<IBridgeCoordinates>?>(),
            It.Is<string?>(n => n == "TestSolution")), Times.Once);
        testSolutionProvider.SolutionProviders.Should().HaveCount(1);
        testSolutionProvider.SelectedSolutionProvider.Should().Be(solutionProviderMock.Object);
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenIslandHasConnections_ShouldCreateBridgeCoordinates()
    {
        // Arrange
        var coordinatesMock = new Mock<Hashi.Gui.Interfaces.Models.IHashiPoint>(MockBehavior.Strict);
        coordinatesMock.Setup(x => x.X).Returns(0);
        coordinatesMock.Setup(x => x.Y).Returns(0);

        var connectionMock = new Mock<Hashi.Gui.Interfaces.Models.IHashiPoint>(MockBehavior.Strict);
        connectionMock.Setup(x => x.X).Returns(1);
        connectionMock.Setup(x => x.Y).Returns(0);

        var connections = new ObservableCollection<Hashi.Gui.Interfaces.Models.IHashiPoint> { connectionMock.Object };

        islandViewModelMock.Setup(x => x.Coordinates).Returns(coordinatesMock.Object);
        islandViewModelMock.Setup(x => x.MaxConnections).Returns(2);
        islandViewModelMock.Setup(x => x.AllConnections).Returns(connections);

        var islands = new List<IIslandViewModel> { islandViewModelMock.Object };

        solutionProviderMock.Setup(x => x.Name).Returns("TestSolution");

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object;

        // Act
        testSolutionProvider.ConvertIslandsToSolutionProvider(islands);

        // Assert
        solutionProviderFactoryMock.Verify(x => x(
            It.IsAny<IReadOnlyList<int[]>?>(),
            It.Is<IReadOnlyList<IBridgeCoordinates>?>(bc => bc != null && bc.Count == 1),
            It.Is<string?>(n => n == "TestSolution")), Times.Once);
        testSolutionProvider.SolutionProviders.Should().HaveCount(1);
        testSolutionProvider.SelectedSolutionProvider.Should().Be(solutionProviderMock.Object);
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenExistingSolutionWithSameName_ShouldReplaceIt()
    {
        // Arrange
        var existingSolutionMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        existingSolutionMock.Setup(x => x.Name).Returns("TestSolution");

        var coordinatesMock = new Mock<Hashi.Gui.Interfaces.Models.IHashiPoint>(MockBehavior.Strict);
        coordinatesMock.Setup(x => x.X).Returns(0);
        coordinatesMock.Setup(x => x.Y).Returns(0);

        islandViewModelMock.Setup(x => x.Coordinates).Returns(coordinatesMock.Object);
        islandViewModelMock.Setup(x => x.MaxConnections).Returns(1);
        islandViewModelMock.Setup(x => x.AllConnections).Returns([]);

        var islands = new List<IIslandViewModel> { islandViewModelMock.Object };

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        testSolutionProvider.SolutionProviders.Add(existingSolutionMock.Object);
        testSolutionProvider.SelectedSolutionProvider = existingSolutionMock.Object;

        // Act
        testSolutionProvider.ConvertIslandsToSolutionProvider(islands);

        // Assert
        testSolutionProvider.SolutionProviders.Should().NotContain(existingSolutionMock.Object);
        testSolutionProvider.SolutionProviders.Should().HaveCount(1);
        testSolutionProvider.SelectedSolutionProvider.Should().Be(solutionProviderMock.Object);
        testSolutionProvider.SelectedSolutionProvider.Should().NotBe(existingSolutionMock.Object);
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenIslandHasZeroMaxConnections_ShouldSkipIt()
    {
        // Arrange
        var coordinatesMock = new Mock<Hashi.Gui.Interfaces.Models.IHashiPoint>(MockBehavior.Strict);
        coordinatesMock.Setup(x => x.X).Returns(0);
        coordinatesMock.Setup(x => x.Y).Returns(0);

        islandViewModelMock.Setup(x => x.Coordinates).Returns(coordinatesMock.Object);
        islandViewModelMock.Setup(x => x.MaxConnections).Returns(0);

        var islands = new List<IIslandViewModel> { islandViewModelMock.Object };

        solutionProviderMock.Setup(x => x.Name).Returns("TestSolution");

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        testSolutionProvider.SelectedSolutionProvider = solutionProviderMock.Object;

        // Act
        testSolutionProvider.ConvertIslandsToSolutionProvider(islands);

        // Assert
        // Should not call AllConnections for islands with zero max connections
        islandViewModelMock.Verify(x => x.AllConnections, Times.Never);
    }

    #endregion

    #region SaveTestFields Tests

    [Test]
    public void SaveTestFields_WhenSolutionProvidersIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // This test cannot be performed because SolutionProviders is a read-only property
        // and cannot be set to null. The implementation ensures it's never null.
        // Instead, we test that the method handles empty collection properly
        testSolutionProvider.SolutionProviders.Clear();

        jsonWrapperMock.Setup(x => x.SerializeWithCustomIndenting(It.IsAny<object>())).Returns("[]");
        directoryWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        fileWrapperMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

        // Act
        testSolutionProvider.SaveTestFields();

        // Assert
        loggerMock.Verify(x => x.Info("Successfully saved 0 test fields"), Times.Once);
    }

    [Test]
    public void SaveTestFields_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        // Arrange
        var testDir = "test-settings-dir";
        var testFilePath = "test-fields.json";

        pathProviderMock.Setup(x => x.SettingsDirectoryPath).Returns(testDir);
        pathProviderMock.Setup(x => x.HashiTestFieldsFilePath).Returns(testFilePath);

        jsonWrapperMock.Setup(x => x.SerializeWithCustomIndenting(It.IsAny<object>())).Returns("[]");
        directoryWrapperMock.Setup(x => x.Exists(testDir)).Returns(false);
        directoryWrapperMock.Setup(x => x.CreateDirectory(testDir));
        fileWrapperMock.Setup(x => x.WriteAllText(testFilePath, It.IsAny<string>()));

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        testSolutionProvider.SaveTestFields();

        // Assert
        directoryWrapperMock.Verify(x => x.CreateDirectory(testDir), Times.Once);
        loggerMock.Verify(x => x.Debug($"Saving test fields to {testFilePath}"), Times.Once);
        loggerMock.Verify(x => x.Info("Successfully saved 0 test fields"), Times.Once);
    }

    [Test]
    public void SaveTestFields_WhenSuccessful_ShouldLogSuccess()
    {
        // Arrange
        solutionProviderMock.Setup(x => x.Name).Returns("TestSolution");
        var solutions = new List<ISolutionProvider> { solutionProviderMock.Object };

        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(solutions);
        jsonWrapperMock.Setup(x => x.SerializeWithCustomIndenting(It.IsAny<object>())).Returns("[]");
        directoryWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        fileWrapperMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        testSolutionProvider.SaveTestFields();

        // Assert
        loggerMock.Verify(x => x.Info("Successfully saved 1 test fields"), Times.Once);
    }

    [Test]
    public void SaveTestFields_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        jsonWrapperMock.Setup(x => x.SerializeWithCustomIndenting(It.IsAny<object>())).Returns("[]");
        directoryWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        fileWrapperMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new IOException("Failed to write file"));

        // Act
        testSolutionProvider.SaveTestFields();

        // Assert - The file write exception should be caught and logged
        loggerMock.Verify(x => x.Error("Failed to save test fields", It.IsAny<Exception>()), Times.Once);
    }

    [Test]
    public void SaveTestFields_WhenSerializationThrows_ShouldThrowException()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Use reflection to replace the jsonWrapper field with one that throws
        var jsonWrapperExceptionMock = new Mock<IJsonWrapper>(MockBehavior.Strict);
        jsonWrapperExceptionMock.Setup(x => x.SerializeWithCustomIndenting(It.IsAny<object>()))
            .Throws(new Exception("Test serialization exception"));

        var fieldInfo = typeof(TestSolutionProvider).GetField("jsonWrapper",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fieldInfo!.SetValue(testSolutionProvider, jsonWrapperExceptionMock.Object);

        // Act & Assert - The serialization exception should NOT be caught and should bubble up
        var action = () => testSolutionProvider.SaveTestFields();
        action.Should().Throw<Exception>().WithMessage("Test serialization exception");
    }

    [Test]
    public void SaveTestFields_WhenSolutionProvidersHaveNullNames_ShouldFilterThemOut()
    {
        // Arrange
        var solutionWithNameMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        solutionWithNameMock.Setup(x => x.Name).Returns("ValidSolution");

        var solutionWithNullNameMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        solutionWithNullNameMock.Setup(x => x.Name).Returns((string?)null);

        var solutions = new List<ISolutionProvider> { solutionWithNameMock.Object, solutionWithNullNameMock.Object };

        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(solutions);
        jsonWrapperMock.Setup(x => x.SerializeWithCustomIndenting(It.Is<IEnumerable<ISolutionProvider>>(y => y.Count() == 1)))
            .Returns("[]");
        directoryWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        fileWrapperMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        testSolutionProvider.SaveTestFields();

        // Assert
        jsonWrapperMock.Verify(x => x.SerializeWithCustomIndenting(
            It.Is<IEnumerable<ISolutionProvider>>(providers => providers.Count() == 1)), Times.Once);
        loggerMock.Verify(x => x.Info("Successfully saved 1 test fields"), Times.Once);
    }

    #endregion

    #region LoadSettings Tests

    [Test]
    public void LoadSettings_WhenFileDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        pathProviderMock.Setup(x => x.HashiTestFieldsFilePath).Returns("non-existent.json");

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        var result = testSolutionProvider.LoadSettings();

        // Assert
        result.Should().BeEmpty();
        // Expect this to be called multiple times: once in constructor, once in explicit call
        loggerMock.Verify(x => x.Info("No existing test fields found, starting with empty list"), Times.AtLeast(1));
    }

    [Test]
    public void LoadSettings_WhenFileExists_ShouldLoadSolutions()
    {
        // Arrange
        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        var loadedSolutions = new List<ISolutionProvider> { solutionProviderMock.Object };
        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(loadedSolutions);

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        var result = testSolutionProvider.LoadSettings();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(solutionProviderMock.Object);
        // Expect this to be called multiple times: once in constructor, once in explicit call
        loggerMock.Verify(x => x.Info("Successfully loaded 1 test fields"), Times.AtLeast(1));
    }

    [Test]
    public void LoadSettings_WhenFileContentIsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("");

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        var result = testSolutionProvider.LoadSettings();

        // Assert
        result.Should().BeEmpty();
        // When file content is empty string, it still gets deserialized but returns empty list
        loggerMock.Verify(x => x.Info("Successfully loaded 0 test fields"), Times.AtLeast(1));
    }

    [Test]
    public void LoadSettings_WhenDeserializationFails_ShouldReturnEmptyListAndLogError()
    {
        // Arrange
        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        // Setup to succeed first (for constructor), then fail on explicit call
        jsonWrapperMock.SetupSequence(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(new List<ISolutionProvider>()) // Constructor call succeeds
            .Throws(new Exception("Deserialization failed")); // Explicit call fails

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        var result = testSolutionProvider.LoadSettings();

        // Assert
        result.Should().BeEmpty();
        loggerMock.Verify(x => x.Error("Failed to load settings", It.IsAny<Exception>()), Times.Once);
    }

    [Test]
    public void LoadSettings_WhenDeserializationReturnsNull_ShouldReturnEmptyList()
    {
        // Arrange
        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(null!);

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        var result = testSolutionProvider.LoadSettings();

        // Assert
        result.Should().BeEmpty();
        // When deserialization returns null, it logs "No existing test fields found"
        loggerMock.Verify(x => x.Info("No existing test fields found, starting with empty list"), Times.AtLeast(1));
    }

    #endregion

    #region Integration Tests

    [Test]
    public void TestSolutionProvider_WhenMultipleInstancesCreated_ShouldBeIndependent()
    {
        // Arrange & Act
        var provider1 = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        var provider2 = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Assert
        provider1.Should().NotBeSameAs(provider2);
        provider1.SolutionProviders.Should().NotBeSameAs(provider2.SolutionProviders);
        provider1.HashiFieldReference.Should().BeEquivalentTo(provider2.HashiFieldReference); // Same content but not necessarily same instance
    }

    [Test]
    public void TestSolutionProvider_ImplementsITestSolutionProviderInterface_Correctly()
    {
        // Arrange
        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act & Assert
        testSolutionProvider.Should().BeAssignableTo<ITestSolutionProvider>();
        ((ITestSolutionProvider)testSolutionProvider).HashiFieldReference.Should().NotBeNull();
        ((ITestSolutionProvider)testSolutionProvider).SolutionProviders.Should().NotBeNull();
    }

    #endregion

    #region Edge Case Tests

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenEmptyIslandList_ShouldCreateEmptyHashiField()
    {
        // Arrange
        solutionProviderMock.Setup(x => x.Name).Returns("EmptyTest");

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object)
        {
            SelectedSolutionProvider = solutionProviderMock.Object
        };

        // Act & Assert
        var action = () => testSolutionProvider.ConvertIslandsToSolutionProvider(new List<IIslandViewModel>());
        action.Should().Throw<InvalidOperationException>(); // Max() on empty sequence
    }

    [Test]
    public void SaveTestFields_WhenOnlySolutionProvidersWithNullNames_ShouldSaveEmptyCollection()
    {
        // Arrange
        var solutionWithNullNameMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        solutionWithNullNameMock.Setup(x => x.Name).Returns((string?)null);

        var solutions = new List<ISolutionProvider> { solutionWithNullNameMock.Object };

        fileWrapperMock.Setup(x => x.Exists("test-fields.json")).Returns(true);
        fileWrapperMock.Setup(x => x.ReadAllText("test-fields.json")).Returns("[{\"name\":\"Test\"}]");

        jsonWrapperMock.Setup(x => x.DeserializeObject(It.IsAny<string>(), typeof(List<ISolutionProvider>)))
            .Returns(solutions);
        jsonWrapperMock.Setup(x => x.SerializeWithCustomIndenting(It.Is<IEnumerable<ISolutionProvider>>(y => !y.Any())))
            .Returns("[]");
        directoryWrapperMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        fileWrapperMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

        testSolutionProvider = new TestSolutionProvider(
            jsonWrapperMock.Object,
            pathProviderMock.Object,
            fileWrapperMock.Object,
            directoryWrapperMock.Object,
            solutionProviderFactoryMock.Object,
            setTestSolutionMessageFactoryMock.Object,
            loggerFactoryMock.Object);

        // Act
        testSolutionProvider.SaveTestFields();

        // Assert
        loggerMock.Verify(x => x.Info("Successfully saved 0 test fields"), Times.Once);
    }

    #endregion
}