using FluentAssertions;

namespace Hashi.Rules.Test;

/// <summary>
/// Unit tests for _0AllRules class.
/// </summary>
[TestFixture]
public class _0AllRulesTests
{
    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var action = () => new _0AllRules();
        action.Should().NotThrow();
    }

    [Test]
    public void Constructor_WhenCreated_ShouldCreateValidInstance()
    {
        // Arrange & Act
        var result = new _0AllRules();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<_0AllRules>();
    }

    [Test]
    public void Class_WhenInstantiated_ShouldHaveCorrectType()
    {
        // Arrange & Act
        var instance = new _0AllRules();

        // Assert
        instance.GetType().Name.Should().Be("_0AllRules");
        instance.GetType().Namespace.Should().Be("Hashi.Rules");
    }
}