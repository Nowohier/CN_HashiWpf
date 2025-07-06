using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Wrappers;

namespace Hashi.Gui.Test.Wrappers;

[TestFixture]
public class DialogWrapperTests
{
    private DialogWrapper dialogWrapper;

    [SetUp]
    public void SetUp()
    {
        dialogWrapper = new DialogWrapper();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new DialogWrapper();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IDialogWrapper>();
    }

    [Test]
    public void DialogWrapper_ShouldImplementIDialogWrapper()
    {
        // Act & Assert
        dialogWrapper.Should().BeAssignableTo<IDialogWrapper>();
    }

    // Note: These tests would require WPF application context to run properly
    // Since we're on Linux, we'll create basic interface compliance tests

    [Test]
    public void Show_Method_ShouldExist()
    {
        // Arrange
        var method = typeof(DialogWrapper).GetMethod(nameof(DialogWrapper.Show));

        // Assert
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(DialogResult));
    }

    [Test]
    public void Show_Method_ShouldHaveCorrectParameters()
    {
        // Arrange
        var method = typeof(DialogWrapper).GetMethod(nameof(DialogWrapper.Show));

        // Assert
        method.Should().NotBeNull();
        var parameters = method!.GetParameters();
        parameters.Should().HaveCount(4);
        parameters[0].ParameterType.Should().Be(typeof(string));
        parameters[0].Name.Should().Be("caption");
        parameters[1].ParameterType.Should().Be(typeof(string));
        parameters[1].Name.Should().Be("message");
        parameters[2].ParameterType.Should().Be(typeof(DialogButton));
        parameters[2].Name.Should().Be("button");
        parameters[3].ParameterType.Should().Be(typeof(DialogImage));
        parameters[3].Name.Should().Be("image");
    }

    [Test]
    public void Show_Method_ShouldHaveDefaultParameters()
    {
        // Arrange
        var method = typeof(DialogWrapper).GetMethod(nameof(DialogWrapper.Show));

        // Assert
        method.Should().NotBeNull();
        var parameters = method!.GetParameters();
        parameters[2].HasDefaultValue.Should().BeTrue();
        parameters[2].DefaultValue.Should().Be(DialogButton.Ok);
        parameters[3].HasDefaultValue.Should().BeTrue();
        parameters[3].DefaultValue.Should().Be(DialogImage.None);
    }

    // Note: The actual Show method would call Dialog.Show which requires WPF context
    // In a real test environment with WPF support, you would test:
    // - Different combinations of DialogButton and DialogImage
    // - Return values for different user interactions
    // - Proper handling of null/empty strings for caption and message

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var wrapper1 = new DialogWrapper();
        var wrapper2 = new DialogWrapper();

        // Assert
        wrapper1.Should().NotBeSameAs(wrapper2);
        wrapper1.Should().BeOfType<DialogWrapper>();
        wrapper2.Should().BeOfType<DialogWrapper>();
    }

    [Test]
    public void DialogWrapper_ShouldBePublicClass()
    {
        // Act & Assert
        var type = typeof(DialogWrapper);
        type.IsPublic.Should().BeTrue();
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeFalse();
        type.IsSealed.Should().BeFalse();
    }

    [Test]
    public void Show_Method_ShouldBePublic()
    {
        // Arrange
        var method = typeof(DialogWrapper).GetMethod(nameof(DialogWrapper.Show));

        // Assert
        method.Should().NotBeNull();
        method!.IsPublic.Should().BeTrue();
        method.IsStatic.Should().BeFalse();
        method.IsVirtual.Should().BeFalse();
    }

    [Test]
    public void DialogWrapper_ShouldHaveParameterlessConstructor()
    {
        // Arrange
        var constructor = typeof(DialogWrapper).GetConstructor(Type.EmptyTypes);

        // Assert
        constructor.Should().NotBeNull();
        constructor!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void DialogWrapper_ShouldInheritFromCorrectInterface()
    {
        // Act
        var interfaces = typeof(DialogWrapper).GetInterfaces();

        // Assert
        interfaces.Should().Contain(typeof(IDialogWrapper));
    }

    [Test]
    public void Show_Method_ShouldBeInterfaceImplementation()
    {
        // Arrange
        var interfaceMethod = typeof(IDialogWrapper).GetMethod(nameof(IDialogWrapper.Show));
        var implementationMethod = typeof(DialogWrapper).GetMethod(nameof(DialogWrapper.Show));

        // Assert
        interfaceMethod.Should().NotBeNull();
        implementationMethod.Should().NotBeNull();
        implementationMethod!.ReturnType.Should().Be(interfaceMethod!.ReturnType);
        
        var interfaceParams = interfaceMethod.GetParameters();
        var implementationParams = implementationMethod.GetParameters();
        implementationParams.Should().HaveCount(interfaceParams.Length);
        
        for (int i = 0; i < interfaceParams.Length; i++)
        {
            implementationParams[i].ParameterType.Should().Be(interfaceParams[i].ParameterType);
            implementationParams[i].Name.Should().Be(interfaceParams[i].Name);
        }
    }
}