using Autofac;
using FluentAssertions;
using Hashi.SolvedPuzzles.Interfaces;

namespace Hashi.SolvedPuzzles.Test;

[TestFixture]
public class AutoFacHashiSolvedPuzzlesModuleTests
{
    private IContainer container;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacHashiSolvedPuzzlesModule>();
        container = builder.Build();
    }

    [TearDown]
    public void Teardown()
    {
        container.Dispose();
    }

    #region Module Registration Tests

    [Test]
    public void Load_WhenCalled_ShouldRegisterHashiPuzzleLoader()
    {
        // Act
        var puzzleLoader = container.Resolve<IHashiPuzzleLoader>();

        // Assert
        puzzleLoader.Should().NotBeNull();
        puzzleLoader.Should().BeOfType<HashiPuzzleLoader>();
    }

    [Test]
    public void Load_WhenResolvedMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var puzzleLoader1 = container.Resolve<IHashiPuzzleLoader>();
        var puzzleLoader2 = container.Resolve<IHashiPuzzleLoader>();

        // Assert
        puzzleLoader1.Should().BeSameAs(puzzleLoader2);
    }

    [Test]
    public void Load_WhenResolvedInDifferentScopes_ShouldReturnSameInstance()
    {
        // Arrange
        IHashiPuzzleLoader loaderFromRoot;
        IHashiPuzzleLoader loaderFromScope;

        // Act
        loaderFromRoot = container.Resolve<IHashiPuzzleLoader>();

        using (var scope = container.BeginLifetimeScope())
        {
            loaderFromScope = scope.Resolve<IHashiPuzzleLoader>();
        }

        // Assert
        loaderFromRoot.Should().BeSameAs(loaderFromScope);
    }

    #endregion

    #region Module Tests

    [Test]
    public void AutoFacHashiSolvedPuzzlesModule_ShouldInheritFromModule()
    {
        // Arrange & Act
        var module = new AutoFacHashiSolvedPuzzlesModule();

        // Assert
        module.Should().BeAssignableTo<Module>();
    }

    [Test]
    public void AutoFacHashiSolvedPuzzlesModule_WhenInstantiated_ShouldNotThrow()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        var act = () => new AutoFacHashiSolvedPuzzlesModule();

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Integration Tests

    [Test]
    public void ResolvedHashiPuzzleLoader_WhenUsed_ShouldImplementInterface()
    {
        // Arrange
        var puzzleLoader = container.Resolve<IHashiPuzzleLoader>();

        // Assert
        puzzleLoader.Should().BeAssignableTo<IHashiPuzzleLoader>();
    }

    [Test]
    public void ResolvedHashiPuzzleLoader_WhenUsedWithValidFile_ShouldWork()
    {
        // Arrange
        var puzzleLoader = container.Resolve<IHashiPuzzleLoader>();
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_001;
        var testPuzzleData = new int[][]
        {
            [1, 2],
            [3, 4]
        };

        // Create test file
        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileContent = System.Text.Json.JsonSerializer.Serialize(testPuzzleData).Replace("[", "{").Replace("]", "}");
        var testFileDirectory = Path.GetDirectoryName(testFileName);

        if (!Directory.Exists(testFileDirectory))
        {
            Directory.CreateDirectory(testFileDirectory!);
        }

        File.WriteAllText(testFileName, testFileContent);

        try
        {
            // Act
            var act = () =>
            {
                var result = puzzleLoader.LoadPuzzle(testPuzzleEnum);
                result.Should().NotBeNull();
            };

            // Assert
            act.Should().NotThrow();
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

    #endregion
}