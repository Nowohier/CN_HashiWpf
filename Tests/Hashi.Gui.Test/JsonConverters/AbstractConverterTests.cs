using FluentAssertions;
using Hashi.Gui.JsonConverters;
using System.Text.Json;

namespace Hashi.Gui.Test.JsonConverters;

/// <summary>
/// Unit tests for <see cref="AbstractConverter{TReal, TAbstract}"/> class.
/// </summary>
[TestFixture]
public class AbstractConverterTests
{
    private AbstractConverter<TestDerived, TestBase> converter;
    private JsonSerializerOptions options;

    [SetUp]
    public void SetUp()
    {
        converter = new AbstractConverter<TestDerived, TestBase>();
        options = new JsonSerializerOptions
        {
            Converters = { converter }
        };
    }

    #region Read Tests

    [Test]
    public void Read_WhenValidJson_ShouldDeserializeToRealType()
    {
        // Arrange
        var json = """{"Name":"hello","Value":42}""";

        // Act
        var result = JsonSerializer.Deserialize<TestBase>(json, options);

        // Assert
        result.Should().BeOfType<TestDerived>();
        result!.Name.Should().Be("hello");
        ((TestDerived)result).Value.Should().Be(42);
    }

    #endregion

    #region Write Tests

    [Test]
    public void Write_WhenValidObject_ShouldSerialize()
    {
        // Arrange
        var obj = new TestDerived { Name = "test", Value = 99 };

        // Act
        var json = JsonSerializer.Serialize<TestBase>(obj, options);

        // Assert
        json.Should().Contain("\"Name\":\"test\"");
        json.Should().Contain("\"Value\":99");
    }

    #endregion

    #region Test Types

    /// <summary>
    /// Abstract base type for testing the converter.
    /// </summary>
    public abstract class TestBase
    {
        public string? Name { get; set; }
    }

    /// <summary>
    /// Concrete derived type for testing the converter.
    /// </summary>
    public class TestDerived : TestBase
    {
        public int Value { get; set; }
    }

    #endregion
}
