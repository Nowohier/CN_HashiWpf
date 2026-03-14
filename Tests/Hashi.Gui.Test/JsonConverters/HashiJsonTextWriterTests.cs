using FluentAssertions;
using Hashi.Gui.JsonConverters;

namespace Hashi.Gui.Test.JsonConverters;

/// <summary>
/// Unit tests for <see cref="InlineArrayJsonFormatter"/> class.
/// </summary>
[TestFixture]
public class HashiJsonTextWriterTests
{
    [Test]
    public void FormatInlineArrays_WhenArrayContainsNumbers_ShouldCollapseToSingleLine()
    {
        // Arrange
        var json = """
            {
              "Field": [
                1,
                2,
                3
              ]
            }
            """;

        // Act
        var result = InlineArrayJsonFormatter.FormatInlineArrays(json);

        // Assert
        result.Should().Contain("[1, 2, 3]");
        result.Should().Contain("\"Field\"");
    }

    [Test]
    public void FormatInlineArrays_WhenObjectProperties_ShouldPreserveNormalIndent()
    {
        // Arrange
        var json = """
            {
              "key": "value"
            }
            """;

        // Act
        var result = InlineArrayJsonFormatter.FormatInlineArrays(json);

        // Assert
        result.Should().Contain("\"key\"");
        result.Should().Contain("\n");
    }

    [Test]
    public void FormatInlineArrays_WhenNestedArrays_ShouldCollapseInnerArrays()
    {
        // Arrange
        var json = """
            [
              [
                1,
                2
              ],
              [
                3,
                4
              ]
            ]
            """;

        // Act
        var result = InlineArrayJsonFormatter.FormatInlineArrays(json);

        // Assert
        result.Should().Contain("[1, 2]");
        result.Should().Contain("[3, 4]");
    }
}
