using FluentAssertions;
using Hashi.Gui.Helpers;

namespace Hashi.Gui.Test.Helpers;

/// <summary>
/// Unit tests for DebugHelper class.
/// </summary>
public class DebugHelperTests
{
    [Test]
    public void IsDebugBuild_WhenAccessed_ShouldReturnBooleanValue()
    {
        // Arrange & Act
        var result = DebugHelper.IsDebugBuild;

        // Assert
        result.Should().BeOfType<bool>();
    }

#if DEBUG
    [Test]
    public void IsDebugBuild_WhenInDebugConfiguration_ShouldReturnTrue()
    {
        // Arrange & Act
        var result = DebugHelper.IsDebugBuild;

        // Assert
        result.Should().BeTrue();
    }
#else
    [Test]
    public void IsDebugBuild_WhenInReleaseConfiguration_ShouldReturnFalse()
    {
        // Arrange & Act
        var result = DebugHelper.IsDebugBuild;

        // Assert
        result.Should().BeFalse();
    }
#endif
}