using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels.Settings;

namespace Hashi.Gui.Test.ViewModels.Settings;

[TestFixture]
public class HighScorePerDifficultyViewModelTests
{
    private HighScorePerDifficultyViewModel highScoreViewModel;

    [SetUp]
    public void SetUp()
    {
        highScoreViewModel = new HighScorePerDifficultyViewModel(DifficultyEnum.Easy);
    }

    [Test]
    public void Constructor_WhenValidDifficulty_ShouldInitializeProperties()
    {
        // Arrange
        var difficulty = DifficultyEnum.Hard;

        // Act
        var result = new HighScorePerDifficultyViewModel(difficulty);

        // Assert
        result.Difficulty.Should().Be(difficulty);
        result.HighScoreTime.Should().BeNull();
    }

    [Test]
    public void HighScoreTime_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newHighScoreTime = TimeSpan.FromMinutes(5);

        // Act
        highScoreViewModel.HighScoreTime = newHighScoreTime;

        // Assert
        highScoreViewModel.HighScoreTime.Should().Be(newHighScoreTime);
    }

    [Test]
    public void HighScoreTime_WhenSetToNull_ShouldAcceptNullValue()
    {
        // Arrange
        highScoreViewModel.HighScoreTime = TimeSpan.FromMinutes(5);

        // Act
        highScoreViewModel.HighScoreTime = null;

        // Assert
        highScoreViewModel.HighScoreTime.Should().BeNull();
    }

    [Test]
    public void Difficulty_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Arrange
        var difficulty = DifficultyEnum.Medium;
        var viewModel = new HighScorePerDifficultyViewModel(difficulty);

        // Act
        var result = viewModel.Difficulty;

        // Assert
        result.Should().Be(difficulty);
    }

    [Test]
    public void ToString_WhenHighScoreTimeIsNull_ShouldReturnFormattedStringWithDash()
    {
        // Arrange
        highScoreViewModel.HighScoreTime = null;

        // Act
        var result = highScoreViewModel.ToString();

        // Assert
        result.Should().Be($"Difficulty: {DifficultyEnum.Easy} | HighScoreTime: -");
    }

    [Test]
    public void ToString_WhenHighScoreTimeIsSet_ShouldReturnFormattedStringWithTime()
    {
        // Arrange
        var highScoreTime = new TimeSpan(1, 30, 45); // 1 hour, 30 minutes, 45 seconds
        highScoreViewModel.HighScoreTime = highScoreTime;

        // Act
        var result = highScoreViewModel.ToString();

        // Assert
        result.Should().Be($"Difficulty: {DifficultyEnum.Easy} | HighScoreTime: 01:30:45");
    }

    [Test]
    [TestCase(DifficultyEnum.Easy, "Easy")]
    [TestCase(DifficultyEnum.Medium, "Medium")]
    [TestCase(DifficultyEnum.Hard, "Hard")]
    public void Constructor_WhenDifferentDifficulties_ShouldSetDifficultyCorrectly(
        DifficultyEnum difficulty, string expectedDifficultyString)
    {
        // Act
        var result = new HighScorePerDifficultyViewModel(difficulty);

        // Assert
        result.Difficulty.Should().Be(difficulty);
        result.ToString().Should().Contain($"Difficulty: {expectedDifficultyString}");
    }

    [Test]
    public void HighScorePerDifficultyViewModel_ShouldImplementIHighScorePerDifficultyViewModel()
    {
        // Act & Assert
        highScoreViewModel.Should().BeAssignableTo<IHighScorePerDifficultyViewModel>();
    }

    [Test]
    public void ToString_WhenZeroTimeSpan_ShouldReturnFormattedStringWithZeroTime()
    {
        // Arrange
        highScoreViewModel.HighScoreTime = TimeSpan.Zero;

        // Act
        var result = highScoreViewModel.ToString();

        // Assert
        result.Should().Be($"Difficulty: {DifficultyEnum.Easy} | HighScoreTime: 00:00:00");
    }

    [Test]
    public void ToString_WhenLargeTimeSpan_ShouldReturnFormattedStringWithCorrectTime()
    {
        // Arrange
        var highScoreTime = new TimeSpan(23, 59, 59); // 23 hours, 59 minutes, 59 seconds
        highScoreViewModel.HighScoreTime = highScoreTime;

        // Act
        var result = highScoreViewModel.ToString();

        // Assert
        result.Should().Be($"Difficulty: {DifficultyEnum.Easy} | HighScoreTime: 23:59:59");
    }

    [Test]
    public void ToString_WhenSecondsOnly_ShouldReturnFormattedStringWithCorrectTime()
    {
        // Arrange
        var highScoreTime = TimeSpan.FromSeconds(45);
        highScoreViewModel.HighScoreTime = highScoreTime;

        // Act
        var result = highScoreViewModel.ToString();

        // Assert
        result.Should().Be($"Difficulty: {DifficultyEnum.Easy} | HighScoreTime: 00:00:45");
    }

    [Test]
    public void ToString_WhenMinutesOnly_ShouldReturnFormattedStringWithCorrectTime()
    {
        // Arrange
        var highScoreTime = TimeSpan.FromMinutes(30);
        highScoreViewModel.HighScoreTime = highScoreTime;

        // Act
        var result = highScoreViewModel.ToString();

        // Assert
        result.Should().Be($"Difficulty: {DifficultyEnum.Easy} | HighScoreTime: 00:30:00");
    }

    [Test]
    public void ToString_WhenHoursOnly_ShouldReturnFormattedStringWithCorrectTime()
    {
        // Arrange
        var highScoreTime = TimeSpan.FromHours(2);
        highScoreViewModel.HighScoreTime = highScoreTime;

        // Act
        var result = highScoreViewModel.ToString();

        // Assert
        result.Should().Be($"Difficulty: {DifficultyEnum.Easy} | HighScoreTime: 02:00:00");
    }
}