using Autofac;
using FluentAssertions;
using Hashi.SolvedPuzzles.Interfaces;
using System.Text.Json;

namespace Hashi.SolvedPuzzles.Test;

[TestFixture]
public class HashiPuzzleLoaderTests
{
    private HashiPuzzleLoader hashiPuzzleLoader;
    private IContainer container;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacHashiSolvedPuzzlesModule>();
        container = builder.Build();
        hashiPuzzleLoader = new HashiPuzzleLoader();
    }

    [TearDown]
    public void Teardown()
    {
        container.Dispose();
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

        // Create a test puzzle file
        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileContent = JsonSerializer.Serialize(testPuzzleData).Replace("[", "{").Replace("]", "}");
        var testFileDirectory = Path.GetDirectoryName(testFileName);

        if (!Directory.Exists(testFileDirectory))
        {
            Directory.CreateDirectory(testFileDirectory!);
        }

        File.WriteAllText(testFileName, testFileContent);

        try
        {
            // Act
            var result = hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(testPuzzleData);
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
        }
    }

    [Test]
    public void LoadPuzzle_WhenFileNotFound_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_002;
        var testFileName = GetTestFileName(testPuzzleEnum);

        // Ensure file doesn't exist
        if (File.Exists(testFileName))
        {
            File.Delete(testFileName);
        }

        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum));
        exception.Message.Should().Contain($"File {testFileName} not found.");
    }

    [Test]
    public void LoadPuzzle_WhenInvalidFileFormat_ShouldThrowException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_003;
        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileDirectory = Path.GetDirectoryName(testFileName);

        if (!Directory.Exists(testFileDirectory))
        {
            Directory.CreateDirectory(testFileDirectory!);
        }

        // Create an invalid JSON file
        File.WriteAllText(testFileName, "invalid json content");

        try
        {
            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum));
            exception.Should().NotBeNull();
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
        }
    }

    [Test]
    public void LoadPuzzle_WhenFileContainsNull_ShouldThrowException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_004;
        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileDirectory = Path.GetDirectoryName(testFileName);

        if (!Directory.Exists(testFileDirectory))
        {
            Directory.CreateDirectory(testFileDirectory!);
        }

        // Create a file with null content
        File.WriteAllText(testFileName, "null");

        try
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidDataException>(() => hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum));
            exception.Message.Should().Contain($"Failed to deserialize the puzzle from file {testFileName}.");
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
        }
    }

    [Test]
    public void LoadPuzzle_WhenEmptyFile_ShouldThrowException()
    {
        // Arrange
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_005;
        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileDirectory = Path.GetDirectoryName(testFileName);

        if (!Directory.Exists(testFileDirectory))
        {
            Directory.CreateDirectory(testFileDirectory!);
        }

        // Create an empty file
        File.WriteAllText(testFileName, "");

        try
        {
            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum));
            exception.Should().NotBeNull();
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
        }
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
        var testFileDirectory = Path.GetDirectoryName(testFileName);

        if (!Directory.Exists(testFileDirectory))
        {
            Directory.CreateDirectory(testFileDirectory!);
        }

        File.WriteAllText(testFileName, testFileContent);

        try
        {
            // Act
            var result = hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(5);
            result[0].Length.Should().Be(5);
            result.Should().BeEquivalentTo(testPuzzleData);
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
        }
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
        // Act
        var loader = new HashiPuzzleLoader();

        // Assert
        loader.Should().NotBeNull();
        loader.Should().BeOfType<HashiPuzzleLoader>();
    }

    #endregion

    #region Interface Implementation Tests

    [Test]
    public void HashiPuzzleLoader_ShouldImplementIHashiPuzzleLoader()
    {
        // Arrange & Act
        var loader = new HashiPuzzleLoader();

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
        var testFileDirectory = Path.GetDirectoryName(testFileName);

        if (!Directory.Exists(testFileDirectory))
        {
            Directory.CreateDirectory(testFileDirectory!);
        }

        File.WriteAllText(testFileName, testFileContent);

        try
        {
            // Act
            var result1 = hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);
            var result2 = hashiPuzzleLoader.LoadPuzzle(testPuzzleEnum);

            // Assert
            result1.Should().BeEquivalentTo(result2);
            result1.Should().BeEquivalentTo(testPuzzleData);
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
        }
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