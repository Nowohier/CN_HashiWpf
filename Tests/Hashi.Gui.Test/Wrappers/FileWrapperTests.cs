using FluentAssertions;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Wrappers;
using System.IO;

namespace Hashi.Gui.Test.Wrappers;

[TestFixture]
public class FileWrapperTests
{
    private FileWrapper fileWrapper;
    private string testFilePath;
    private string testDirectoryPath;

    [SetUp]
    public void SetUp()
    {
        fileWrapper = new FileWrapper();
        testDirectoryPath = Path.Combine(Path.GetTempPath(), "HashiFileWrapperTest_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(testDirectoryPath);
        testFilePath = Path.Combine(testDirectoryPath, "testfile.txt");
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test directory and files
        if (Directory.Exists(testDirectoryPath))
        {
            Directory.Delete(testDirectoryPath, true);
        }
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new FileWrapper();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IFileWrapper>();
    }

    [Test]
    public void FileWrapper_ShouldImplementIFileWrapper()
    {
        // Act & Assert
        fileWrapper.Should().BeAssignableTo<IFileWrapper>();
    }

    [Test]
    public void Exists_WhenFileExists_ShouldReturnTrue()
    {
        // Arrange
        File.WriteAllText(testFilePath, "test content");

        // Act
        var result = fileWrapper.Exists(testFilePath);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenFileDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentPath = Path.Combine(testDirectoryPath, "nonexistent.txt");

        // Act
        var result = fileWrapper.Exists(nonExistentPath);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Exists_WhenPathIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => fileWrapper.Exists(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Exists_WhenPathIsEmpty_ShouldReturnFalse()
    {
        // Act
        var result = fileWrapper.Exists(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Exists_WhenPathIsDirectory_ShouldReturnFalse()
    {
        // Act
        var result = fileWrapper.Exists(testDirectoryPath);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ReadAllText_WhenFileExists_ShouldReturnFileContent()
    {
        // Arrange
        var expectedContent = "This is test content";
        File.WriteAllText(testFilePath, expectedContent);

        // Act
        var result = fileWrapper.ReadAllText(testFilePath);

        // Assert
        result.Should().Be(expectedContent);
    }

    [Test]
    public void ReadAllText_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(testDirectoryPath, "nonexistent.txt");

        // Act & Assert
        var act = () => fileWrapper.ReadAllText(nonExistentPath);
        act.Should().Throw<FileNotFoundException>();
    }

    [Test]
    public void ReadAllText_WhenPathIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => fileWrapper.ReadAllText(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ReadAllText_WhenPathIsEmpty_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => fileWrapper.ReadAllText(string.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ReadAllText_WhenFileIsEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        File.WriteAllText(testFilePath, string.Empty);

        // Act
        var result = fileWrapper.ReadAllText(testFilePath);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Test]
    public void ReadAllText_WhenFileContainsUnicodeCharacters_ShouldReturnCorrectContent()
    {
        // Arrange
        var expectedContent = "Test with üäöß and 中文 characters";
        File.WriteAllText(testFilePath, expectedContent);

        // Act
        var result = fileWrapper.ReadAllText(testFilePath);

        // Assert
        result.Should().Be(expectedContent);
    }

    [Test]
    public void WriteAllText_WhenCalled_ShouldCreateFileWithContent()
    {
        // Arrange
        var content = "This is test content";

        // Act
        fileWrapper.WriteAllText(testFilePath, content);

        // Assert
        File.Exists(testFilePath).Should().BeTrue();
        File.ReadAllText(testFilePath).Should().Be(content);
    }

    [Test]
    public void WriteAllText_WhenFileExists_ShouldOverwriteContent()
    {
        // Arrange
        var originalContent = "Original content";
        var newContent = "New content";
        File.WriteAllText(testFilePath, originalContent);

        // Act
        fileWrapper.WriteAllText(testFilePath, newContent);

        // Assert
        File.ReadAllText(testFilePath).Should().Be(newContent);
    }

    [Test]
    public void WriteAllText_WhenDirectoryDoesNotExist_ShouldThrowDirectoryNotFoundException()
    {
        // Arrange
        var nonExistentDirectoryPath = Path.Combine(testDirectoryPath, "nonexistent", "testfile.txt");

        // Act & Assert
        var act = () => fileWrapper.WriteAllText(nonExistentDirectoryPath, "content");
        act.Should().Throw<DirectoryNotFoundException>();
    }

    [Test]
    public void WriteAllText_WhenPathIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => fileWrapper.WriteAllText(null!, "content");
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void WriteAllText_WhenPathIsEmpty_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => fileWrapper.WriteAllText(string.Empty, "content");
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void WriteAllText_WhenContentIsNull_ShouldCreateEmptyFile()
    {
        // Act
        fileWrapper.WriteAllText(testFilePath, null!);

        // Assert
        File.Exists(testFilePath).Should().BeTrue();
        File.ReadAllText(testFilePath).Should().Be(string.Empty);
    }

    [Test]
    public void WriteAllText_WhenContentIsEmpty_ShouldCreateEmptyFile()
    {
        // Act
        fileWrapper.WriteAllText(testFilePath, string.Empty);

        // Assert
        File.Exists(testFilePath).Should().BeTrue();
        File.ReadAllText(testFilePath).Should().Be(string.Empty);
    }

    [Test]
    public void WriteAllText_WhenContentContainsUnicodeCharacters_ShouldWriteCorrectly()
    {
        // Arrange
        var content = "Test with üäöß and 中文 characters";

        // Act
        fileWrapper.WriteAllText(testFilePath, content);

        // Assert
        File.ReadAllText(testFilePath).Should().Be(content);
    }

    [Test]
    public void ReadWriteAllText_WhenUsedTogether_ShouldMaintainContent()
    {
        // Arrange
        var originalContent = "Test content with multiple lines\nLine 2\nLine 3";

        // Act
        fileWrapper.WriteAllText(testFilePath, originalContent);
        var readContent = fileWrapper.ReadAllText(testFilePath);

        // Assert
        readContent.Should().Be(originalContent);
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var wrapper1 = new FileWrapper();
        var wrapper2 = new FileWrapper();

        // Assert
        wrapper1.Should().NotBeSameAs(wrapper2);
    }

    [Test]
    public void AllMethods_ShouldBePublic()
    {
        // Arrange
        var type = typeof(FileWrapper);

        // Act
        var existsMethod = type.GetMethod(nameof(FileWrapper.Exists));
        var readMethod = type.GetMethod(nameof(FileWrapper.ReadAllText));
        var writeMethod = type.GetMethod(nameof(FileWrapper.WriteAllText));

        // Assert
        existsMethod.Should().NotBeNull();
        existsMethod!.IsPublic.Should().BeTrue();
        
        readMethod.Should().NotBeNull();
        readMethod!.IsPublic.Should().BeTrue();
        
        writeMethod.Should().NotBeNull();
        writeMethod!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void FileWrapper_ShouldHaveCorrectMethodSignatures()
    {
        // Arrange
        var type = typeof(FileWrapper);

        // Act & Assert
        var existsMethod = type.GetMethod(nameof(FileWrapper.Exists));
        existsMethod.Should().NotBeNull();
        existsMethod!.ReturnType.Should().Be(typeof(bool));
        existsMethod.GetParameters().Should().HaveCount(1);
        existsMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));

        var readMethod = type.GetMethod(nameof(FileWrapper.ReadAllText));
        readMethod.Should().NotBeNull();
        readMethod!.ReturnType.Should().Be(typeof(string));
        readMethod.GetParameters().Should().HaveCount(1);
        readMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));

        var writeMethod = type.GetMethod(nameof(FileWrapper.WriteAllText));
        writeMethod.Should().NotBeNull();
        writeMethod!.ReturnType.Should().Be(typeof(void));
        writeMethod.GetParameters().Should().HaveCount(2);
        writeMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        writeMethod.GetParameters()[1].ParameterType.Should().Be(typeof(string));
    }

    [Test]
    public void WriteAllText_WhenContentContainsNewlines_ShouldPreserveFormatting()
    {
        // Arrange
        var content = "Line 1\r\nLine 2\nLine 3\r\n";

        // Act
        fileWrapper.WriteAllText(testFilePath, content);
        var result = fileWrapper.ReadAllText(testFilePath);

        // Assert
        result.Should().Be(content);
    }

    [Test]
    public void WriteAllText_WhenLargeContent_ShouldWriteCorrectly()
    {
        // Arrange
        var content = new string('A', 10000); // Large content

        // Act
        fileWrapper.WriteAllText(testFilePath, content);
        var result = fileWrapper.ReadAllText(testFilePath);

        // Assert
        result.Should().Be(content);
        result.Length.Should().Be(10000);
    }
}