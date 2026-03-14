using FluentAssertions;
using Hashi.Generator.Interfaces;

namespace Hashi.Generator.Test;

[TestFixture]
public class DifficultySettingsProviderTests
{
    private DifficultySettingsProvider difficultySettingsProvider = null!;

    [SetUp]
    public void Setup()
    {
        difficultySettingsProvider = new DifficultySettingsProvider();
    }

    #region GetDifficultySettings Tests

    [Test]
    [TestCase(0, 5, 10, 5, 10, 4, 25, 20)]
    [TestCase(1, 14, 16, 14, 16, 4, 50, 20)]
    [TestCase(2, 10, 16, 10, 16, 3, 75, 20)]
    [TestCase(3, 11, 18, 11, 18, 3, 25, 15)]
    [TestCase(4, 10, 18, 10, 18, 3, 50, 15)]
    [TestCase(5, 13, 18, 13, 18, 3, 75, 15)]
    [TestCase(6, 15, 20, 15, 20, 3, 25, 10)]
    [TestCase(7, 14, 20, 14, 20, 3, 50, 10)]
    [TestCase(8, 16, 31, 16, 31, 3, 75, 10)]
    [TestCase(9, 20, 31, 20, 31, 3, 100, 0)]
    public void GetDifficultySettings_WhenValidDifficulty_ShouldReturnCorrectSettings(
        int difficulty, int expectedMinLength, int expectedMaxLength, int expectedMinWidth,
        int expectedMaxWidth, int expectedDivisor, int expectedAlpha, int expectedBeta)
    {
        // Act
        var result = difficultySettingsProvider.GetDifficultySettings(difficulty);

        // Assert
        result.Should().Be(new DifficultySettings(
            expectedMinLength, expectedMaxLength,
            expectedMinWidth, expectedMaxWidth,
            expectedDivisor, expectedAlpha, expectedBeta));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(10)]
    [TestCase(100)]
    public void GetDifficultySettings_WhenInvalidDifficulty_ShouldThrowArgumentException(int difficulty)
    {
        // Act & Assert
        Action act = () => difficultySettingsProvider.GetDifficultySettings(difficulty);
        act.Should().Throw<ArgumentException>().WithMessage("Invalid difficulty level.");
    }

    #endregion

}
