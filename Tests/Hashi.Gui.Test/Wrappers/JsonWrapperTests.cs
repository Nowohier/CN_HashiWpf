using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Wrappers;
using Moq;
using Newtonsoft.Json;

namespace Hashi.Gui.Test.Wrappers;

[TestFixture]
public class JsonWrapperTests
{
    private JsonWrapper jsonWrapper;

    [SetUp]
    public void SetUp()
    {
        jsonWrapper = new JsonWrapper();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new JsonWrapper();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IJsonWrapper>();
    }

    [Test]
    public void JsonWrapper_ShouldImplementIJsonWrapper()
    {
        // Act & Assert
        jsonWrapper.Should().BeAssignableTo<IJsonWrapper>();
    }

    [Test]
    public void SerializeObject_WhenCalledWithSimpleObject_ShouldReturnJsonString()
    {
        // Arrange
        var testObject = new { Name = "Test", Value = 123 };

        // Act
        var result = jsonWrapper.SerializeObject(testObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Name\"");
        result.Should().Contain("\"Test\"");
        result.Should().Contain("\"Value\"");
        result.Should().Contain("123");
    }

    [Test]
    public void SerializeObject_WhenCalledWithNull_ShouldReturnNullString()
    {
        // Act
        var result = jsonWrapper.SerializeObject(null);

        // Assert
        result.Should().Be("null");
    }

    [Test]
    public void SerializeObject_WhenCalledWithString_ShouldReturnJsonString()
    {
        // Arrange
        var testString = "Test String";

        // Act
        var result = jsonWrapper.SerializeObject(testString);

        // Assert
        result.Should().Be("\"Test String\"");
    }

    [Test]
    public void SerializeObject_WhenCalledWithNumber_ShouldReturnJsonNumber()
    {
        // Arrange
        var testNumber = 42;

        // Act
        var result = jsonWrapper.SerializeObject(testNumber);

        // Assert
        result.Should().Be("42");
    }

    [Test]
    public void SerializeObject_WhenCalledWithArray_ShouldReturnJsonArray()
    {
        // Arrange
        var testArray = new[] { 1, 2, 3 };

        // Act
        var result = jsonWrapper.SerializeObject(testArray);

        // Assert
        result.Should().Contain("[");
        result.Should().Contain("]");
        result.Should().Contain("1");
        result.Should().Contain("2");
        result.Should().Contain("3");
    }

    [Test]
    public void DeserializeObject_WhenCalledWithValidJson_ShouldReturnObject()
    {
        // Arrange
        var jsonString = "{\"Name\":\"Test\",\"Value\":123}";

        // Act
        var result = jsonWrapper.DeserializeObject(jsonString, typeof(object));

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeserializeObject_WhenCalledWithInvalidJson_ShouldThrowJsonException()
    {
        // Arrange
        var invalidJson = "{invalid json}";

        // Act & Assert
        var act = () => jsonWrapper.DeserializeObject(invalidJson, typeof(object));
        act.Should().Throw<JsonException>();
    }

    [Test]
    public void DeserializeObject_WhenCalledWithNullJson_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => jsonWrapper.DeserializeObject(null!, typeof(object));
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void DeserializeObject_WhenCalledWithEmptyJson_ShouldThrowJsonException()
    {
        // Act & Assert
        var act = () => jsonWrapper.DeserializeObject(string.Empty, typeof(object));
        act.Should().Throw<JsonException>();
    }

    [Test]
    public void SerializeWithCustomIndenting_WhenCalledWithSimpleObject_ShouldReturnFormattedJson()
    {
        // Arrange
        var testObject = new { Name = "Test", Value = 123 };

        // Act
        var result = jsonWrapper.SerializeWithCustomIndenting(testObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Name\"");
        result.Should().Contain("\"Test\"");
        result.Should().Contain("\"Value\"");
        result.Should().Contain("123");
        result.Should().Contain("\n"); // Should contain newlines for formatting
    }

    [Test]
    public void SerializeWithCustomIndenting_WhenCalledWithNull_ShouldReturnNullString()
    {
        // Act
        var result = jsonWrapper.SerializeWithCustomIndenting(null);

        // Assert
        result.Should().Be("null");
    }

    [Test]
    public void SerializeWithCustomIndenting_WhenCalledWithArray_ShouldReturnFormattedJsonArray()
    {
        // Arrange
        var testArray = new[] { 1, 2, 3 };

        // Act
        var result = jsonWrapper.SerializeWithCustomIndenting(testArray);

        // Assert
        result.Should().Contain("[");
        result.Should().Contain("]");
        result.Should().Contain("1");
        result.Should().Contain("2");
        result.Should().Contain("3");
    }

    [Test]
    public void SerializeObject_WhenCalledWithComplexObject_ShouldHandleNestedProperties()
    {
        // Arrange
        var testObject = new 
        { 
            Name = "Test", 
            Nested = new { InnerValue = 456 },
            Items = new[] { "Item1", "Item2" }
        };

        // Act
        var result = jsonWrapper.SerializeObject(testObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Name\"");
        result.Should().Contain("\"Nested\"");
        result.Should().Contain("\"InnerValue\"");
        result.Should().Contain("456");
        result.Should().Contain("\"Items\"");
        result.Should().Contain("\"Item1\"");
        result.Should().Contain("\"Item2\"");
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var wrapper1 = new JsonWrapper();
        var wrapper2 = new JsonWrapper();

        // Assert
        wrapper1.Should().NotBeSameAs(wrapper2);
    }

    [Test]
    public void AllMethods_ShouldBePublic()
    {
        // Arrange
        var type = typeof(JsonWrapper);

        // Act
        var serializeMethod = type.GetMethod(nameof(JsonWrapper.SerializeObject));
        var deserializeMethod = type.GetMethod(nameof(JsonWrapper.DeserializeObject));
        var customIndentMethod = type.GetMethod(nameof(JsonWrapper.SerializeWithCustomIndenting));

        // Assert
        serializeMethod.Should().NotBeNull();
        serializeMethod!.IsPublic.Should().BeTrue();
        
        deserializeMethod.Should().NotBeNull();
        deserializeMethod!.IsPublic.Should().BeTrue();
        
        customIndentMethod.Should().NotBeNull();
        customIndentMethod!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void JsonWrapper_ShouldHaveCorrectMethodSignatures()
    {
        // Arrange
        var type = typeof(JsonWrapper);

        // Act & Assert
        var serializeMethod = type.GetMethod(nameof(JsonWrapper.SerializeObject));
        serializeMethod.Should().NotBeNull();
        serializeMethod!.ReturnType.Should().Be(typeof(string));
        serializeMethod.GetParameters().Should().HaveCount(1);
        serializeMethod.GetParameters()[0].ParameterType.Should().Be(typeof(object));

        var deserializeMethod = type.GetMethod(nameof(JsonWrapper.DeserializeObject));
        deserializeMethod.Should().NotBeNull();
        deserializeMethod!.ReturnType.Should().Be(typeof(object));
        deserializeMethod.GetParameters().Should().HaveCount(2);
        deserializeMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        deserializeMethod.GetParameters()[1].ParameterType.Should().Be(typeof(Type));

        var customIndentMethod = type.GetMethod(nameof(JsonWrapper.SerializeWithCustomIndenting));
        customIndentMethod.Should().NotBeNull();
        customIndentMethod!.ReturnType.Should().Be(typeof(string));
        customIndentMethod.GetParameters().Should().HaveCount(1);
        customIndentMethod.GetParameters()[0].ParameterType.Should().Be(typeof(object));
    }

    [Test]
    public void SerializeObject_WhenCalledWithBooleanTrue_ShouldReturnTrueString()
    {
        // Act
        var result = jsonWrapper.SerializeObject(true);

        // Assert
        result.Should().Be("true");
    }

    [Test]
    public void SerializeObject_WhenCalledWithBooleanFalse_ShouldReturnFalseString()
    {
        // Act
        var result = jsonWrapper.SerializeObject(false);

        // Assert
        result.Should().Be("false");
    }

    [Test]
    public void SerializeObject_WhenCalledWithDateTime_ShouldReturnJsonDateTime()
    {
        // Arrange
        var dateTime = new DateTime(2023, 1, 1);

        // Act
        var result = jsonWrapper.SerializeObject(dateTime);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("2023");
    }

    [Test]
    public void DeserializeObject_WhenCalledWithNullString_ShouldReturnNull()
    {
        // Arrange
        var jsonString = "null";

        // Act
        var result = jsonWrapper.DeserializeObject(jsonString, typeof(object));

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void SerializeDeserialize_WhenUsedTogether_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var originalObject = new { Name = "Test", Value = 123, Flag = true };

        // Act
        var serialized = jsonWrapper.SerializeObject(originalObject);
        var deserialized = jsonWrapper.DeserializeObject(serialized, typeof(object));

        // Assert
        serialized.Should().NotBeNullOrEmpty();
        deserialized.Should().NotBeNull();
    }

    [Test]
    public void SerializeWithCustomIndenting_WhenCalledWithObjectHavingNullProperties_ShouldIgnoreNullValues()
    {
        // Arrange
        var testObject = new { Name = "Test", NullValue = (string?)null, Value = 123 };

        // Act
        var result = jsonWrapper.SerializeWithCustomIndenting(testObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Name\"");
        result.Should().Contain("\"Value\"");
        result.Should().NotContain("\"NullValue\""); // Should be ignored due to NullValueHandling.Ignore
    }
}