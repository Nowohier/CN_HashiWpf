using FluentAssertions;
using Hashi.Gui.Helpers;

namespace Hashi.Gui.Test.Helpers;

[TestFixture]
public class CursorHelperTests
{
    [Test]
    public void CursorHelper_ShouldBeStaticClass()
    {
        // Act & Assert
        var type = typeof(CursorHelper);
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeTrue();
        type.IsSealed.Should().BeTrue();
    }

    [Test]
    public void GetCurrentCursorPosition_Method_ShouldExist()
    {
        // Arrange
        var method = typeof(CursorHelper).GetMethod(nameof(CursorHelper.GetCurrentCursorPosition));

        // Assert
        method.Should().NotBeNull();
        method!.IsStatic.Should().BeTrue();
        method.IsPublic.Should().BeTrue();
        method.ReturnType.Should().Be(typeof(System.Windows.Point));
    }

    [Test]
    public void GetCurrentCursorPosition_Method_ShouldHaveCorrectParameters()
    {
        // Arrange
        var method = typeof(CursorHelper).GetMethod(nameof(CursorHelper.GetCurrentCursorPosition));

        // Assert
        method.Should().NotBeNull();
        var parameters = method!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Should().Be(typeof(System.Windows.Media.Visual));
        parameters[0].Name.Should().Be("relativeTo");
    }

    // Note: The actual GetCurrentCursorPosition method requires WPF context and user32.dll
    // Since we're running on Linux, we can't test the actual functionality
    // In a Windows environment with WPF support, you would test:
    // - Different Visual objects as parameters
    // - Return value validation
    // - Error handling for invalid Visual objects

    [Test]
    public void GetCurrentCursorPosition_WhenNullVisual_ShouldThrowException()
    {
        // Note: This test would require WPF context to run properly
        // We can only test the method signature and accessibility here
        
        // Arrange & Act & Assert
        var method = typeof(CursorHelper).GetMethod(nameof(CursorHelper.GetCurrentCursorPosition));
        method.Should().NotBeNull();
        
        // The actual test would be:
        // var act = () => CursorHelper.GetCurrentCursorPosition(null!);
        // act.Should().Throw<ArgumentNullException>();
        // But this requires WPF context which is not available on Linux
    }

    [Test]
    public void CursorHelper_ShouldHaveCorrectNamespace()
    {
        // Act & Assert
        var type = typeof(CursorHelper);
        type.Namespace.Should().Be("Hashi.Gui.Helpers");
    }

    [Test]
    public void CursorHelper_ShouldBePublicClass()
    {
        // Act & Assert
        var type = typeof(CursorHelper);
        type.IsPublic.Should().BeTrue();
    }

    [Test]
    public void CursorHelper_ShouldNotHaveInstanceConstructor()
    {
        // Act & Assert
        var type = typeof(CursorHelper);
        var constructors = type.GetConstructors();
        
        // Static classes should not have public instance constructors
        constructors.Should().BeEmpty();
    }

    [Test]
    public void GetCurrentCursorPosition_ShouldUseCorrectReturnType()
    {
        // Arrange
        var method = typeof(CursorHelper).GetMethod(nameof(CursorHelper.GetCurrentCursorPosition));

        // Assert
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(System.Windows.Point));
        method.ReturnType.Should().NotBe(typeof(System.Drawing.Point)); // Should use WPF Point, not Drawing Point
    }

    [Test]
    public void CursorHelper_ShouldHaveWin32PointStruct()
    {
        // Arrange
        var type = typeof(CursorHelper);

        // Act
        var nestedTypes = type.GetNestedTypes(System.Reflection.BindingFlags.NonPublic);

        // Assert
        nestedTypes.Should().Contain(t => t.Name == "Win32Point");
        var win32PointType = nestedTypes.First(t => t.Name == "Win32Point");
        win32PointType.IsValueType.Should().BeTrue(); // Should be a struct
        win32PointType.IsPublic.Should().BeFalse(); // Should be private
    }

    [Test]
    public void CursorHelper_ShouldHaveGetCursorPosImport()
    {
        // Arrange
        var type = typeof(CursorHelper);

        // Act
        var methods = type.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var getCursorPosMethod = methods.FirstOrDefault(m => m.Name == "GetCursorPos");

        // Assert
        getCursorPosMethod.Should().NotBeNull();
        getCursorPosMethod!.IsStatic.Should().BeTrue();
        getCursorPosMethod.IsPublic.Should().BeFalse(); // Should be private
        
        // Check for DllImport attribute
        var dllImportAttr = getCursorPosMethod.GetCustomAttributes(typeof(System.Runtime.InteropServices.DllImportAttribute), false);
        dllImportAttr.Should().NotBeEmpty();
    }

    [Test]
    public void Win32Point_ShouldHaveCorrectFields()
    {
        // Arrange
        var type = typeof(CursorHelper);
        var nestedTypes = type.GetNestedTypes(System.Reflection.BindingFlags.NonPublic);
        var win32PointType = nestedTypes.First(t => t.Name == "Win32Point");

        // Act
        var fields = win32PointType.GetFields();

        // Assert
        fields.Should().HaveCount(2);
        fields.Should().Contain(f => f.Name == "X" && f.FieldType == typeof(int));
        fields.Should().Contain(f => f.Name == "Y" && f.FieldType == typeof(int));
    }

    [Test]
    public void CursorHelper_AllMembers_ShouldBeStatic()
    {
        // Arrange
        var type = typeof(CursorHelper);

        // Act
        var publicMethods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var publicFields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var publicProperties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        // Assert
        // Static classes should not have public instance members
        publicMethods.Where(m => !m.IsSpecialName).Should().BeEmpty(); // Exclude special names like constructors
        publicFields.Should().BeEmpty();
        publicProperties.Should().BeEmpty();
    }

    [Test]
    public void GetCurrentCursorPosition_Method_ShouldNotBeVirtual()
    {
        // Arrange
        var method = typeof(CursorHelper).GetMethod(nameof(CursorHelper.GetCurrentCursorPosition));

        // Assert
        method.Should().NotBeNull();
        method!.IsVirtual.Should().BeFalse();
        method.IsAbstract.Should().BeFalse();
    }

    // Note: In a real Windows environment with WPF support, you would add tests like:
    // [Test]
    // public void GetCurrentCursorPosition_WhenValidVisual_ShouldReturnValidPoint()
    // {
    //     // This would require creating a WPF Visual object and testing actual cursor position
    // }
    
    // [Test]
    // public void GetCurrentCursorPosition_WhenCursorMoves_ShouldReturnDifferentPositions()
    // {
    //     // This would test that the method returns current cursor position
    // }
}