using FluentAssertions;
using Hashi.Gui.Translation;
using System.ComponentModel;
using System.Globalization;

namespace Hashi.Gui.Test.Translation;

/// <summary>
/// Unit tests for TranslationSource class.
/// </summary>
public class TranslationSourceTests
{
    private TranslationSource sut;

    [SetUp]
    public void SetUp()
    {
        sut = TranslationSource.Instance;
    }

    [Test]
    public void Instance_WhenAccessed_ShouldReturnSingletonInstance()
    {
        // Arrange & Act
        var instance1 = TranslationSource.Instance;
        var instance2 = TranslationSource.Instance;

        // Assert
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
        instance1.Should().BeSameAs(instance2);
    }

    [Test]
    public void Instance_ShouldImplementINotifyPropertyChanged()
    {
        // Arrange & Act
        var instance = TranslationSource.Instance;

        // Assert
        instance.Should().BeAssignableTo<INotifyPropertyChanged>();
    }

    [Test]
    public void CurrentCulture_WhenSetToNewValue_ShouldUpdateProperty()
    {
        // Arrange
        var originalCulture = sut.CurrentCulture;
        var newCulture = new CultureInfo("de-DE");

        try
        {
            // Act
            sut.CurrentCulture = newCulture;

            // Assert
            sut.CurrentCulture.Should().Be(newCulture);
        }
        finally
        {
            // Cleanup
            sut.CurrentCulture = originalCulture;
        }
    }

    [Test]
    public void CurrentCulture_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        var originalCulture = sut.CurrentCulture;
        var eventRaised = false;
        sut.PropertyChanged += (_, _) => eventRaised = true;

        // Act
        sut.CurrentCulture = originalCulture;

        // Assert
        eventRaised.Should().BeFalse();
    }

    [Test]
    public void CurrentCulture_WhenSetToDifferentValue_ShouldRaisePropertyChanged()
    {
        // Arrange
        var originalCulture = sut.CurrentCulture;
        var newCulture = new CultureInfo("de-DE");
        var eventRaised = false;
        PropertyChangedEventArgs? eventArgs = null;

        sut.PropertyChanged += (_, e) =>
        {
            eventRaised = true;
            eventArgs = e;
        };

        try
        {
            // Act
            sut.CurrentCulture = newCulture;

            // Assert
            eventRaised.Should().BeTrue();
            eventArgs.Should().NotBeNull();
            eventArgs!.PropertyName.Should().Be(string.Empty);
        }
        finally
        {
            // Cleanup
            sut.CurrentCulture = originalCulture;
        }
    }

    [Test]
    public void Indexer_WhenAccessedWithValidKey_ShouldReturnString()
    {
        // Arrange
        const string testKey = "TestKey";

        // Act
        var result = sut[testKey];

        // Assert
        // Note: Since we don't know the exact content of the resource files,
        // we can only verify that the method doesn't throw and returns a string or null
        result.Should().BeOfType<string>();
    }

    [Test]
    public void Indexer_WhenAccessedWithNullKey_ShouldReturnNull()
    {
        // Arrange, Act & Assert
        var result = sut[null!];
        result.Should().BeNull();
    }

    [Test]
    public void Indexer_WhenAccessedWithEmptyKey_ShouldReturnNull()
    {
        // Arrange, Act & Assert
        var result = sut[string.Empty];
        result.Should().BeNull();
    }
}