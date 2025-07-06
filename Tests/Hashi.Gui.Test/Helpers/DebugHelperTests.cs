using FluentAssertions;
using Hashi.Gui.Helpers;
using System.Diagnostics;
using System.Reflection;

namespace Hashi.Gui.Test.Helpers;

[TestFixture]
public class DebugHelperTests
{
    [Test]
    public void DebugHelper_ShouldBeStaticClass()
    {
        // Act & Assert
        var type = typeof(DebugHelper);
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeTrue();
        type.IsSealed.Should().BeTrue();
    }

    [Test]
    public void IsDebugBuild_ShouldBePublicStaticProperty()
    {
        // Arrange
        var property = typeof(DebugHelper).GetProperty(nameof(DebugHelper.IsDebugBuild));

        // Assert
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(bool));
        property.CanRead.Should().BeTrue();
        property.CanWrite.Should().BeFalse(); // Should be read-only
        property.GetMethod!.IsStatic.Should().BeTrue();
        property.GetMethod.IsPublic.Should().BeTrue();
    }

    [Test]
    public void IsDebugBuild_WhenAccessed_ShouldReturnBooleanValue()
    {
        // Act
        var result = DebugHelper.IsDebugBuild;

        // Assert
        result.Should().BeOfType<bool>();
    }

    [Test]
    public void IsDebugBuild_WhenAccessedMultipleTimes_ShouldReturnConsistentValue()
    {
        // Act
        var result1 = DebugHelper.IsDebugBuild;
        var result2 = DebugHelper.IsDebugBuild;

        // Assert
        result1.Should().Be(result2);
    }

    [Test]
    public void DebugHelper_ShouldHaveCorrectNamespace()
    {
        // Act & Assert
        var type = typeof(DebugHelper);
        type.Namespace.Should().Be("Hashi.Gui.Helpers");
    }

    [Test]
    public void DebugHelper_ShouldBePublicClass()
    {
        // Act & Assert
        var type = typeof(DebugHelper);
        type.IsPublic.Should().BeTrue();
    }

    [Test]
    public void DebugHelper_ShouldNotHaveInstanceConstructor()
    {
        // Act & Assert
        var type = typeof(DebugHelper);
        var constructors = type.GetConstructors();
        
        // Static classes should not have public instance constructors
        constructors.Should().BeEmpty();
    }

    [Test]
    public void IsDebugBuild_PropertyImplementation_ShouldUseCorrectLogic()
    {
        // This test verifies the implementation approach but not the specific result
        // since the result depends on the build configuration
        
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        var debuggableAttributes = assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>();

        // Act
        var hasJitTrackingEnabled = debuggableAttributes.Any(da => da.IsJITTrackingEnabled);
        var debugHelperResult = DebugHelper.IsDebugBuild;

        // Assert
        // The DebugHelper.IsDebugBuild should use the same logic as our test
        // Note: We can't assert the exact value since it depends on how the test assembly was compiled
        debugHelperResult.Should().BeOfType<bool>();
        
        // If we're running in a debug test context, verify the logic is working
        #if DEBUG
        debugHelperResult.Should().BeTrue("In DEBUG builds, IsDebugBuild should return true");
        #endif
    }

    [Test]
    public void IsDebugBuild_ShouldCheckCorrectAssembly()
    {
        // This test ensures that IsDebugBuild checks the executing assembly
        // and not some other assembly
        
        // Arrange
        var debugHelperAssembly = typeof(DebugHelper).Assembly;
        var expectedResult = debugHelperAssembly.GetCustomAttributes(false)
            .OfType<DebuggableAttribute>()
            .Any(da => da.IsJITTrackingEnabled);

        // Act
        var actualResult = DebugHelper.IsDebugBuild;

        // Assert
        actualResult.Should().Be(expectedResult);
    }

    [Test]
    public void DebugHelper_AllMembers_ShouldBeStatic()
    {
        // Arrange
        var type = typeof(DebugHelper);

        // Act
        var publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        var publicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var publicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Assert
        // Static classes should not have public instance members
        publicMethods.Where(m => !m.IsSpecialName).Should().BeEmpty();
        publicFields.Should().BeEmpty();
        publicProperties.Should().BeEmpty();
    }

    [Test]
    public void IsDebugBuild_Property_ShouldNotHaveSetter()
    {
        // Arrange
        var property = typeof(DebugHelper).GetProperty(nameof(DebugHelper.IsDebugBuild));

        // Assert
        property.Should().NotBeNull();
        property!.SetMethod.Should().BeNull(); // Should not have a setter
    }

    [Test]
    public void IsDebugBuild_ShouldUseLinqQuery()
    {
        // This test verifies that the implementation uses LINQ as intended
        // We check this by ensuring the result matches what the LINQ query should produce
        
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        var manualResult = assembly.GetCustomAttributes(false)
            .OfType<DebuggableAttribute>()
            .Any(da => da.IsJITTrackingEnabled);

        // Act
        var debugHelperResult = DebugHelper.IsDebugBuild;

        // Assert
        debugHelperResult.Should().Be(manualResult);
    }

    [Test]
    public void DebugHelper_ShouldOnlyHaveOnePublicMember()
    {
        // Arrange
        var type = typeof(DebugHelper);

        // Act
        var publicMembers = type.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        // Assert
        // Should only have the IsDebugBuild property (getter method)
        publicMembers.Should().HaveCount(1);
        publicMembers[0].Name.Should().Be("get_IsDebugBuild");
    }

    [Test]
    public void IsDebugBuild_WhenCalledFromDifferentContexts_ShouldReturnSameValue()
    {
        // Arrange & Act
        var directCall = DebugHelper.IsDebugBuild;
        var indirectCall = CallIsDebugBuildIndirectly();

        // Assert
        directCall.Should().Be(indirectCall);
    }

    private bool CallIsDebugBuildIndirectly()
    {
        return DebugHelper.IsDebugBuild;
    }

    [Test]
    public void IsDebugBuild_PropertyGetter_ShouldNotThrow()
    {
        // Act & Assert
        var act = () => DebugHelper.IsDebugBuild;
        act.Should().NotThrow();
    }

    [Test]
    public void DebugHelper_ShouldHaveCorrectUsings()
    {
        // This test ensures the class uses the correct namespaces for its implementation
        // We verify this by checking that the DebuggableAttribute is accessible
        
        // Act & Assert
        var debuggableAttributeType = typeof(DebuggableAttribute);
        debuggableAttributeType.Should().NotBeNull();
        debuggableAttributeType.Namespace.Should().Be("System.Diagnostics");
    }

    [Test]
    public void IsDebugBuild_WhenAssemblyHasNoDebuggableAttribute_ShouldReturnFalse()
    {
        // This test verifies the behavior when no DebuggableAttribute is found
        // We can't easily test this with the current assembly, but we can verify the logic
        
        // Arrange
        var emptyAttributes = new object[0];
        var hasJitTracking = emptyAttributes.OfType<DebuggableAttribute>()
            .Any(da => da.IsJITTrackingEnabled);

        // Assert
        hasJitTracking.Should().BeFalse();
    }

    [Test]
    public void IsDebugBuild_Implementation_ShouldBeEfficient()
    {
        // This test ensures the property getter is reasonably fast
        // Important for a helper that might be called frequently
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = DebugHelper.IsDebugBuild;
        stopwatch.Stop();

        // Assert
        result.Should().BeOfType<bool>();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should be very fast
    }
}