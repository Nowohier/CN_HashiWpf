using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.ViewModels.Settings;

namespace Hashi.Gui.Test.ViewModels.Settings;

[TestFixture]
public class HighScorePerDifficultyViewModelTests
{
    private HighScorePerDifficultyViewModel viewModel;

    [SetUp]
    public void SetUp()
    {
        viewModel = new HighScorePerDifficultyViewModel(DifficultyEnum.Easy1);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalledWithValidDifficulty_ShouldInitializeProperties()
    {
        // Arrange & Act
        var result = new HighScorePerDifficultyViewModel(DifficultyEnum.Hard1);

        // Assert
        result.Difficulty.Should().Be(DifficultyEnum.Hard1);
        result.HighScoreTime.Should().BeNull();
    }

    [Test]
    public void Constructor_WhenCalledWithEachDifficultyEnum_ShouldSetCorrectDifficulty()
    {
        // Arrange & Act & Assert
        foreach (var difficulty in Enum.GetValues<DifficultyEnum>())
        {
            var result = new HighScorePerDifficultyViewModel(difficulty);
            result.Difficulty.Should().Be(difficulty);
        }
    }

    #endregion

    #region Property Tests

    [Test]
    public void Difficulty_ShouldBeReadOnly()
    {
        // Arrange & Act
        var difficulty = viewModel.Difficulty;

        // Assert
        difficulty.Should().Be(DifficultyEnum.Easy1);
        
        // Verify it's read-only by checking there's no setter
        var property = typeof(HighScorePerDifficultyViewModel).GetProperty(nameof(HighScorePerDifficultyViewModel.Difficulty));
        property!.SetMethod.Should().BeNull();
    }

    [Test]
    public void HighScoreTime_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newTime = TimeSpan.FromMinutes(5);

        // Act
        viewModel.HighScoreTime = newTime;

        // Assert
        viewModel.HighScoreTime.Should().Be(newTime);
    }

    [Test]
    public void HighScoreTime_WhenSetToNull_ShouldAcceptNull()
    {
        // Arrange
        viewModel.HighScoreTime = TimeSpan.FromMinutes(5);

        // Act
        viewModel.HighScoreTime = null;

        // Assert
        viewModel.HighScoreTime.Should().BeNull();
    }

    [Test]
    public void HighScoreTime_WhenSetToZero_ShouldAcceptZero()
    {
        // Act
        viewModel.HighScoreTime = TimeSpan.Zero;

        // Assert
        viewModel.HighScoreTime.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void HighScoreTime_WhenSetToLargeValue_ShouldAcceptLargeValue()
    {
        // Arrange
        var largeTime = TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59)).Add(TimeSpan.FromSeconds(59));

        // Act
        viewModel.HighScoreTime = largeTime;

        // Assert
        viewModel.HighScoreTime.Should().Be(largeTime);
    }

    #endregion

    #region ToString Tests

    [Test]
    public void ToString_WhenHighScoreTimeIsNull_ShouldReturnStringWithDash()
    {
        // Arrange
        viewModel.HighScoreTime = null;

        // Act
        var result = viewModel.ToString();

        // Assert
        result.Should().Contain("Difficulty: Easy1");
        result.Should().Contain("HighScoreTime: -");
        result.Should().Contain(" | ");
    }

    [Test]
    public void ToString_WhenHighScoreTimeIsSet_ShouldReturnFormattedTime()
    {
        // Arrange
        viewModel.HighScoreTime = new TimeSpan(1, 23, 45); // 1 hour, 23 minutes, 45 seconds

        // Act
        var result = viewModel.ToString();

        // Assert
        result.Should().Contain("Difficulty: Easy1");
        result.Should().Contain("HighScoreTime: 01:23:45");
        result.Should().Contain(" | ");
    }

    [Test]
    public void ToString_WhenHighScoreTimeIsZero_ShouldReturnZeroTime()
    {
        // Arrange
        viewModel.HighScoreTime = TimeSpan.Zero;

        // Act
        var result = viewModel.ToString();

        // Assert
        result.Should().Contain("HighScoreTime: 00:00:00");
    }

    [Test]
    public void ToString_WhenHighScoreTimeIsLarge_ShouldReturnFormattedLargeTime()
    {
        // Arrange
        viewModel.HighScoreTime = new TimeSpan(23, 59, 59);

        // Act
        var result = viewModel.ToString();

        // Assert
        result.Should().Contain("HighScoreTime: 23:59:59");
    }

    [Test]
    public void ToString_WithDifferentDifficulties_ShouldShowCorrectDifficulty()
    {
        // Arrange & Act & Assert
        foreach (var difficulty in Enum.GetValues<DifficultyEnum>())
        {
            var vm = new HighScorePerDifficultyViewModel(difficulty);
            var result = vm.ToString();
            result.Should().Contain($"Difficulty: {difficulty}");
        }
    }

    #endregion

    #region Property Change Notification Tests

    [Test]
    public void HighScoreTime_WhenChanged_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(HighScorePerDifficultyViewModel.HighScoreTime))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.HighScoreTime = TimeSpan.FromMinutes(10);

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void HighScoreTime_WhenSetToSameValue_ShouldNotRaisePropertyChangedEvent()
    {
        // Arrange
        var time = TimeSpan.FromMinutes(5);
        viewModel.HighScoreTime = time;
        
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        viewModel.HighScoreTime = time; // Same value

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    [Test]
    public void HighScoreTime_WhenChangedFromNullToValue_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        viewModel.HighScoreTime = null;
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(HighScorePerDifficultyViewModel.HighScoreTime))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.HighScoreTime = TimeSpan.FromMinutes(5);

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void HighScoreTime_WhenChangedFromValueToNull_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        viewModel.HighScoreTime = TimeSpan.FromMinutes(5);
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(HighScorePerDifficultyViewModel.HighScoreTime))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.HighScoreTime = null;

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    #endregion

    #region JSON Serialization Tests

    [Test]
    public void JsonProperties_ShouldBeCorrectlyDecorated()
    {
        // Arrange & Act
        var highScoreTimeProperty = typeof(HighScorePerDifficultyViewModel).GetProperty(nameof(HighScorePerDifficultyViewModel.HighScoreTime));
        var difficultyProperty = typeof(HighScorePerDifficultyViewModel).GetProperty(nameof(HighScorePerDifficultyViewModel.Difficulty));

        // Assert
        highScoreTimeProperty.Should().NotBeNull();
        difficultyProperty.Should().NotBeNull();
        
        // Verify JSON attributes exist (the exact validation would require deeper reflection testing)
        var highScoreTimeHasJsonProperty = highScoreTimeProperty!.GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false).Any();
        var difficultyHasJsonProperty = difficultyProperty!.GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false).Any();
        
        highScoreTimeHasJsonProperty.Should().BeTrue();
        difficultyHasJsonProperty.Should().BeTrue();
    }

    #endregion
}