using FluentAssertions;
using Hashi.Gui.JsonConverters;
using Newtonsoft.Json;

namespace Hashi.Gui.Test.JsonConverters;

/// <summary>
/// Unit tests for <see cref="AbstractConverter{TReal, TAbstract}"/> class.
/// </summary>
[TestFixture]
public class AbstractConverterTests
{
    private AbstractConverter<TestDerived, TestBase> converter;

    [SetUp]
    public void SetUp()
    {
        converter = new AbstractConverter<TestDerived, TestBase>();
    }

    #region CanConvert Tests

    [Test]
    public void CanConvert_WhenAbstractType_ShouldReturnTrue()
    {
        // Act & Assert
        converter.CanConvert(typeof(TestBase)).Should().BeTrue();
    }

    [Test]
    public void CanConvert_WhenRealType_ShouldReturnFalse()
    {
        // Act & Assert
        converter.CanConvert(typeof(TestDerived)).Should().BeFalse();
    }

    [Test]
    public void CanConvert_WhenUnrelatedType_ShouldReturnFalse()
    {
        // Act & Assert
        converter.CanConvert(typeof(string)).Should().BeFalse();
    }

    #endregion

    #region ReadJson Tests

    [Test]
    public void ReadJson_WhenValidJson_ShouldDeserializeToRealType()
    {
        // Arrange
        var json = """{"Name":"hello","Value":42}""";
        var settings = new JsonSerializerSettings
        {
            Converters = { converter }
        };

        // Act
        var result = JsonConvert.DeserializeObject<TestBase>(json, settings);

        // Assert
        result.Should().BeOfType<TestDerived>();
        result!.Name.Should().Be("hello");
        ((TestDerived)result).Value.Should().Be(42);
    }

    #endregion

    #region WriteJson Tests

    [Test]
    public void WriteJson_WhenValidObject_ShouldSerialize()
    {
        // Arrange
        var obj = new TestDerived { Name = "test", Value = 99 };
        var settings = new JsonSerializerSettings
        {
            Converters = { converter }
        };

        // Act
        var json = JsonConvert.SerializeObject((TestBase)obj, settings);

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
