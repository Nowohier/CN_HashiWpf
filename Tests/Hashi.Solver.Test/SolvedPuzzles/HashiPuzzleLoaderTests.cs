using FluentAssertions;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.SolvedPuzzles.Interfaces;
using Moq;
using System.Text.Json;

namespace Hashi.SolvedPuzzles.Test;

[TestFixture]
public class HashiPuzzleLoaderTests
{
    private Mock<IFileWrapper> _fileWrapperMock;
    private HashiPuzzleLoader _hashiPuzzleLoader;

    [SetUp]
    public void Setup()
    {
        _fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);
        _hashiPuzzleLoader = new HashiPuzzleLoader(_fileWrapperMock.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _fileWrapperMock.VerifyAll();
    }

    #region LoadPuzzle Tests

    [Test]
    public void LoadPuzzle_WhenValidEnumProvided_ShouldReturnPuzzleArray()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_001;
        var testPuzzleData = new int[][]
        {
            [1, 2, 3],
            [4, 5, 6],
            [7, 8, 9]
        };
        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileContent = JsonSerializer.Serialize(testPuzzleData).Replace("[", "{").Replace("]", "}");

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(true);
        _fileWrapperMock.Setup(f => f.ReadAllText(testFileName)).Returns(testFileContent);

        // Act
        var result = _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testPuzzleData);
    }

    [Test]
    public void LoadPuzzle_WhenFileNotFound_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_002;
        var testFileName = GetTestFileName(testPuzzleEnum);

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(false);

        // Act
        var act = () => _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

        // Assert
        act.Should().Throw<FileNotFoundException>().WithMessage($"*File {testFileName} not found.*");
    }

    [Test]
    public void LoadPuzzle_WhenInvalidFileFormat_ShouldThrowJsonException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_003;
        var testFileName = GetTestFileName(testPuzzleEnum);

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(true);
        _fileWrapperMock.Setup(f => f.ReadAllText(testFileName)).Returns("invalid json content");

        // Act
        var act = () => _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Test]
    public void LoadPuzzle_WhenFileContainsNull_ShouldThrowInvalidDataException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_004;
        var testFileName = GetTestFileName(testPuzzleEnum);

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(true);
        _fileWrapperMock.Setup(f => f.ReadAllText(testFileName)).Returns("null");

        // Act
        var act = () => _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

        // Assert
        act.Should().Throw<InvalidDataException>()
            .WithMessage($"*Failed to deserialize the puzzle from file {testFileName}.*");
    }

    [Test]
    public void LoadPuzzle_WhenEmptyFile_ShouldThrowJsonException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_005;
        var testFileName = GetTestFileName(testPuzzleEnum);

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(true);
        _fileWrapperMock.Setup(f => f.ReadAllText(testFileName)).Returns("");

        // Act
        var act = () => _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Test]
    public void LoadPuzzle_WhenValidComplexPuzzle_ShouldReturnCorrectData()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_006;
        var testPuzzleData = new int[][]
        {
            [1, 0, 2, 0, 3],
            [0, 4, 0, 5, 0],
            [2, 0, 6, 0, 1],
            [0, 3, 0, 2, 0],
            [4, 0, 1, 0, 5]
        };

        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileContent = JsonSerializer.Serialize(testPuzzleData).Replace("[", "{").Replace("]", "}");

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(true);
        _fileWrapperMock.Setup(f => f.ReadAllText(testFileName)).Returns(testFileContent);

        // Act
        var result = _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(5);
        result[0].Length.Should().Be(5);
        result.Should().BeEquivalentTo(testPuzzleData);
    }

    #endregion

    #region GetHashiFileName Tests (Testing via reflection)

    [Test]
    public void GetHashiFileName_WhenCalled_ShouldReturnCorrectPath()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_001;
        var expectedPath = GetTestFileName(testPuzzleEnum);

        // Act
        var actualPath = GetHashiFileNameViaReflection(testPuzzleEnum);

        // Assert
        actualPath.Should().Be(expectedPath);
    }

    [Test]
    public void GetHashiFileName_WhenDifferentEnums_ShouldReturnDifferentPaths()
    {
        // Arrange
        var enum1 = HashiFileEnum.Hs_16_100_25_00_001;
        var enum2 = HashiFileEnum.Hs_24_200_25_00_001;

        // Act
        var path1 = GetHashiFileNameViaReflection(enum1);
        var path2 = GetHashiFileNameViaReflection(enum2);

        // Assert
        path1.Should().NotBe(path2);
        path1.Should().Contain("100");
        path2.Should().Contain("200");
    }

    [Test]
    public void GetHashiFileName_WhenCalled_ShouldFollowNamingConvention()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_007;

        // Act
        var path = GetHashiFileNameViaReflection(testPuzzleEnum);

        // Assert
        path.Should().EndWith(".has");
        path.Should().Contain("Hs_16_100_25_00_007");
        path.Should().Contain("100"); // folder name
    }

    #endregion

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Arrange
        var fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);

        // Act
        var loader = new HashiPuzzleLoader(fileWrapperMock.Object);

        // Assert
        loader.Should().NotBeNull();
        loader.Should().BeOfType<HashiPuzzleLoader>();
    }

    #endregion

    #region Interface Implementation Tests

    [Test]
    public void HashiPuzzleLoader_ShouldImplementIHashiPuzzleLoader()
    {
        // Arrange
        var fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);

        // Act
        var loader = new HashiPuzzleLoader(fileWrapperMock.Object);

        // Assert
        loader.Should().BeAssignableTo<IHashiPuzzleLoader>();
    }

    #endregion

    #region Integration Tests

    [Test]
    public void LoadPuzzle_WhenCalledMultipleTimes_ShouldReturnConsistentResults()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_008;
        var testPuzzleData = new int[][]
        {
            [1, 2],
            [3, 4]
        };

        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileContent = JsonSerializer.Serialize(testPuzzleData).Replace("[", "{").Replace("]", "}");

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(true);
        _fileWrapperMock.Setup(f => f.ReadAllText(testFileName)).Returns(testFileContent);

        // Act
        var result1 = _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);
        var result2 = _hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

        // Assert
        result1.Should().BeEquivalentTo(result2);
        result1.Should().BeEquivalentTo(testPuzzleData);
    }

    #endregion

    #region Helper Methods

    private string GetTestFileName(HashiFileEnum hashiFileEnum)
    {
        var hashiId = hashiFileEnum.ToString();
        var folderName = hashiId.Substring(6, 3);
        var hashiName = $"{hashiId}.has";
        var puzzleBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Hashi_Puzzles");
        return Path.Combine(Path.Combine(puzzleBasePath, folderName), hashiName);
    }

    private string GetHashiFileNameViaReflection(HashiFileEnum hashiFileEnum)
    {
        var method = typeof(HashiPuzzleLoader).GetMethod("GetHashiFileName",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        method.Should().NotBeNull();
        return (string)method?.Invoke(null, [hashiFileEnum])!;
    }

    #endregion
}
