using FluentAssertions;
using Hashi.Gui.JsonConverters;
using Newtonsoft.Json;

namespace Hashi.Gui.Test.JsonConverters;

/// <summary>
/// Unit tests for <see cref="HashiJsonTextWriter"/> class.
/// </summary>
[TestFixture]
public class HashiJsonTextWriterTests
{
    [Test]
    public void Constructor_WhenTextWriterProvided_ShouldNotThrow()
    {
        // Arrange
        using var stringWriter = new StringWriter();

        // Act
        var action = () => new HashiJsonTextWriter(stringWriter);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void WriteIndent_WhenWriteStateIsArray_ShouldWriteSpace()
    {
        // Arrange
        using var stringWriter = new StringWriter();
        using var writer = new HashiJsonTextWriter(stringWriter)
        {
            Formatting = Formatting.Indented
        };

        // Act — write an array with values to trigger array indent
        writer.WriteStartArray();
        writer.WriteValue(1);
        writer.WriteValue(2);
        writer.WriteEndArray();
        writer.Flush();

        var result = stringWriter.ToString();

        // Assert — array elements should be space-separated, not newline-indented
        result.Should().NotContain("\n  1");
        result.Should().Contain("1, 2");
    }

    [Test]
    public void WriteIndent_WhenWriteStateIsNotArray_ShouldWriteNormalIndent()
    {
        // Arrange
        using var stringWriter = new StringWriter();
        using var writer = new HashiJsonTextWriter(stringWriter)
        {
            Formatting = Formatting.Indented
        };

        // Act — write an object with a property to trigger object indent
        writer.WriteStartObject();
        writer.WritePropertyName("key");
        writer.WriteValue("value");
        writer.WriteEndObject();
        writer.Flush();

        var result = stringWriter.ToString();

        // Assert — object properties should be indented normally
        result.Should().Contain("\n");
        result.Should().Contain("\"key\"");
    }
}
