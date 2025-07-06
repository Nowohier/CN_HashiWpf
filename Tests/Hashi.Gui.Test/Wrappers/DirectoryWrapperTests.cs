using FluentAssertions;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Wrappers;
using System.IO;

namespace Hashi.Gui.Test.Wrappers;

[TestFixture]
public class DirectoryWrapperTests
{
    private DirectoryWrapper directoryWrapper;
    private string testDirectoryPath;

    [SetUp]
    public void SetUp()
    {
        directoryWrapper = new DirectoryWrapper();
        testDirectoryPath = Path.Combine(Path.GetTempPath(), "HashiDirectoryWrapperTest_" + Guid.NewGuid().ToString("N")[..8]);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test directory if it exists
        if (Directory.Exists(testDirectoryPath))
        {
            Directory.Delete(testDirectoryPath, true);
        }
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new DirectoryWrapper();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IDirectoryWrapper>();
    }

    [Test]
    public void DirectoryWrapper_ShouldImplementIDirectoryWrapper()
    {
        // Act & Assert
        directoryWrapper.Should().BeAssignableTo<IDirectoryWrapper>();
    }

    [Test]
    public void Exists_WhenDirectoryExists_ShouldReturnTrue()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);

        // Act
        var result = directoryWrapper.Exists(testDirectoryPath);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenDirectoryDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "NonExistentDirectory" + Guid.NewGuid().ToString("N"));

        // Act
        var result = directoryWrapper.Exists(nonExistentPath);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Exists_WhenPathIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => directoryWrapper.Exists(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Exists_WhenPathIsEmpty_ShouldReturnFalse()
    {
        // Act
        var result = directoryWrapper.Exists(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CreateDirectory_WhenCalled_ShouldCreateDirectory()
    {
        // Act
        directoryWrapper.CreateDirectory(testDirectoryPath);

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeTrue();
    }

    [Test]
    public void CreateDirectory_WhenDirectoryAlreadyExists_ShouldNotThrow()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);

        // Act & Assert
        var act = () => directoryWrapper.CreateDirectory(testDirectoryPath);
        act.Should().NotThrow();
        Directory.Exists(testDirectoryPath).Should().BeTrue();
    }

    [Test]
    public void CreateDirectory_WhenPathIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => directoryWrapper.CreateDirectory(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void CreateDirectory_WhenPathIsEmpty_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => directoryWrapper.CreateDirectory(string.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void CreateDirectory_WhenNestedPath_ShouldCreateAllDirectories()
    {
        // Arrange
        var nestedPath = Path.Combine(testDirectoryPath, "SubDirectory", "DeepDirectory");

        // Act
        directoryWrapper.CreateDirectory(nestedPath);

        // Assert
        Directory.Exists(nestedPath).Should().BeTrue();
        Directory.Exists(Path.GetDirectoryName(nestedPath)).Should().BeTrue();
        Directory.Exists(testDirectoryPath).Should().BeTrue();
    }

    [Test]
    public void Delete_WhenDirectoryExistsAndEmpty_ShouldDeleteDirectory()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);

        // Act
        directoryWrapper.Delete(testDirectoryPath, false);

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeFalse();
    }

    [Test]
    public void Delete_WhenDirectoryExistsWithFilesAndRecursiveTrue_ShouldDeleteDirectoryAndContents()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        var testFile = Path.Combine(testDirectoryPath, "testfile.txt");
        File.WriteAllText(testFile, "test content");

        // Act
        directoryWrapper.Delete(testDirectoryPath, true);

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeFalse();
    }

    [Test]
    public void Delete_WhenDirectoryExistsWithFilesAndRecursiveFalse_ShouldThrowException()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        var testFile = Path.Combine(testDirectoryPath, "testfile.txt");
        File.WriteAllText(testFile, "test content");

        // Act & Assert
        var act = () => directoryWrapper.Delete(testDirectoryPath, false);
        act.Should().Throw<IOException>();
    }

    [Test]
    public void Delete_WhenDirectoryDoesNotExist_ShouldThrowDirectoryNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "NonExistentDirectory" + Guid.NewGuid().ToString("N"));

        // Act & Assert
        var act = () => directoryWrapper.Delete(nonExistentPath, false);
        act.Should().Throw<DirectoryNotFoundException>();
    }

    [Test]
    public void Delete_WhenPathIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => directoryWrapper.Delete(null!, false);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Delete_WhenPathIsEmpty_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => directoryWrapper.Delete(string.Empty, false);
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Delete_WhenDirectoryHasSubdirectoriesAndRecursiveTrue_ShouldDeleteAll()
    {
        // Arrange
        Directory.CreateDirectory(testDirectoryPath);
        var subDirectory = Path.Combine(testDirectoryPath, "SubDirectory");
        Directory.CreateDirectory(subDirectory);
        var testFile = Path.Combine(subDirectory, "testfile.txt");
        File.WriteAllText(testFile, "test content");

        // Act
        directoryWrapper.Delete(testDirectoryPath, true);

        // Assert
        Directory.Exists(testDirectoryPath).Should().BeFalse();
        Directory.Exists(subDirectory).Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var wrapper1 = new DirectoryWrapper();
        var wrapper2 = new DirectoryWrapper();

        // Assert
        wrapper1.Should().NotBeSameAs(wrapper2);
    }

    [Test]
    public void AllMethods_ShouldBePublic()
    {
        // Arrange
        var type = typeof(DirectoryWrapper);

        // Act
        var existsMethod = type.GetMethod(nameof(DirectoryWrapper.Exists));
        var createMethod = type.GetMethod(nameof(DirectoryWrapper.CreateDirectory));
        var deleteMethod = type.GetMethod(nameof(DirectoryWrapper.Delete));

        // Assert
        existsMethod.Should().NotBeNull();
        existsMethod!.IsPublic.Should().BeTrue();
        
        createMethod.Should().NotBeNull();
        createMethod!.IsPublic.Should().BeTrue();
        
        deleteMethod.Should().NotBeNull();
        deleteMethod!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void DirectoryWrapper_ShouldHaveCorrectMethodSignatures()
    {
        // Arrange
        var type = typeof(DirectoryWrapper);

        // Act & Assert
        var existsMethod = type.GetMethod(nameof(DirectoryWrapper.Exists));
        existsMethod.Should().NotBeNull();
        existsMethod!.ReturnType.Should().Be(typeof(bool));
        existsMethod.GetParameters().Should().HaveCount(1);
        existsMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));

        var createMethod = type.GetMethod(nameof(DirectoryWrapper.CreateDirectory));
        createMethod.Should().NotBeNull();
        createMethod!.ReturnType.Should().Be(typeof(void));
        createMethod.GetParameters().Should().HaveCount(1);
        createMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));

        var deleteMethod = type.GetMethod(nameof(DirectoryWrapper.Delete));
        deleteMethod.Should().NotBeNull();
        deleteMethod!.ReturnType.Should().Be(typeof(void));
        deleteMethod.GetParameters().Should().HaveCount(2);
        deleteMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        deleteMethod.GetParameters()[1].ParameterType.Should().Be(typeof(bool));
    }
}