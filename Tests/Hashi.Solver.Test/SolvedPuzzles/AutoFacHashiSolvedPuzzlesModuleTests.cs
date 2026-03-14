using FluentAssertions;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.SolvedPuzzles.Extensions;
using Hashi.SolvedPuzzles.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Hashi.SolvedPuzzles.Test;

[TestFixture]
public class AutoFacHashiSolvedPuzzlesModuleTests
{
    private ServiceProvider serviceProvider;
    private Mock<IFileWrapper> _fileWrapperMock;

    [SetUp]
    public void Setup()
    {
        _fileWrapperMock = new Mock<IFileWrapper>(MockBehavior.Strict);

        var services = new ServiceCollection();
        services.AddSolvedPuzzlesServices();
        services.AddSingleton(_fileWrapperMock.Object);
        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        serviceProvider.Dispose();
        _fileWrapperMock.VerifyAll();
    }

    #region Service Registration Tests

    [Test]
    public void AddSolvedPuzzlesServices_WhenCalled_ShouldRegisterHashiPuzzleLoader()
    {
        // Act
        var puzzleLoader = serviceProvider.GetRequiredService<IHashiPuzzleLoader>();

        // Assert
        puzzleLoader.Should().NotBeNull();
        puzzleLoader.Should().BeOfType<HashiPuzzleLoader>();
    }

    [Test]
    public void AddSolvedPuzzlesServices_WhenResolvedMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var puzzleLoader1 = serviceProvider.GetRequiredService<IHashiPuzzleLoader>();
        var puzzleLoader2 = serviceProvider.GetRequiredService<IHashiPuzzleLoader>();

        // Assert
        puzzleLoader1.Should().BeSameAs(puzzleLoader2);
    }

    [Test]
    public void AddSolvedPuzzlesServices_WhenResolvedInDifferentScopes_ShouldReturnSameInstance()
    {
        // Arrange
        IHashiPuzzleLoader loaderFromRoot;
        IHashiPuzzleLoader loaderFromScope;

        // Act
        loaderFromRoot = serviceProvider.GetRequiredService<IHashiPuzzleLoader>();

        using (var scope = serviceProvider.CreateScope())
        {
            loaderFromScope = scope.ServiceProvider.GetRequiredService<IHashiPuzzleLoader>();
        }

        // Assert
        loaderFromRoot.Should().BeSameAs(loaderFromScope);
    }

    #endregion

    #region Extension Method Tests

    [Test]
    public void SolvedPuzzlesServiceExtensions_ShouldBePublicStaticClass()
    {
        // Arrange & Act
        var type = typeof(SolvedPuzzlesServiceExtensions);

        // Assert
        type.IsPublic.Should().BeTrue();
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeTrue();
        type.IsSealed.Should().BeTrue();
    }

    [Test]
    public void AddSolvedPuzzlesServices_WhenCalled_ShouldNotThrow()
    {
        // Act
        var act = () =>
        {
            var services = new ServiceCollection();
            services.AddSolvedPuzzlesServices();
        };

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Integration Tests

    [Test]
    public void ResolvedHashiPuzzleLoader_WhenUsed_ShouldImplementInterface()
    {
        // Arrange
        var puzzleLoader = serviceProvider.GetRequiredService<IHashiPuzzleLoader>();

        // Assert
        puzzleLoader.Should().BeAssignableTo<IHashiPuzzleLoader>();
    }

    [Test]
    public void ResolvedHashiPuzzleLoader_WhenUsedWithValidFile_ShouldWork()
    {
        // Arrange
        var puzzleLoader = serviceProvider.GetRequiredService<IHashiPuzzleLoader>();
        var testPuzzleEnum = HashiFileEnum.Hs_16_100_25_00_001;
        var testPuzzleData = new int[][]
        {
            [1, 2],
            [3, 4]
        };

        var testFileName = GetTestFileName(testPuzzleEnum);
        var testFileContent = System.Text.Json.JsonSerializer.Serialize(testPuzzleData).Replace("[", "{").Replace("]", "}");

        _fileWrapperMock.Setup(f => f.Exists(testFileName)).Returns(true);
        _fileWrapperMock.Setup(f => f.ReadAllText(testFileName)).Returns(testFileContent);

        // Act
        var act = () =>
        {
            var result = puzzleLoader.LoadPuzzle(testPuzzleEnum);
            result.Should().NotBeNull();
        };

        // Assert
        act.Should().NotThrow();
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
