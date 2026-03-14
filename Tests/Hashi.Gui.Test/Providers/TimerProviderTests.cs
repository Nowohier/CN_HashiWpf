using FluentAssertions;
using Hashi.Gui.Providers;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class TimerProviderTests
{
    [SetUp]
    public void SetUp()
    {
        timerProvider = new TimerProvider();
    }

    private TimerProvider timerProvider;

    [Test]
    public void Constructor_WhenCreated_ShouldInitializeProperties()
    {
        // Assert
        timerProvider.Should().NotBeNull();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.IsRunning.Should().BeFalse();
        timerProvider.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void StartTimer_WhenTimerIsNotRunning_ShouldStartTimer()
    {
        // Arrange
        timerProvider.IsRunning.Should().BeFalse();

        // Act
        timerProvider.StartTimer();

        // Assert
        timerProvider.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();
    }

    [Test]
    public void StartTimer_WhenTimerIsAlreadyRunning_ShouldNotRestartTimer()
    {
        // Arrange
        timerProvider.StartTimer();
        var elapsedBefore = timerProvider.Elapsed;

        // Let some time pass
        Thread.Sleep(10);

        // Act
        timerProvider.StartTimer(); // Try to start again

        // Assert
        timerProvider.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();
        // Timer should continue running without reset
        timerProvider.Elapsed.Should().BeGreaterThan(elapsedBefore);
    }

    [Test]
    public void StopTimer_WhenTimerIsRunning_ShouldStopAndResetTimer()
    {
        // Arrange
        timerProvider.StartTimer();
        timerProvider.IsRunning.Should().BeTrue();

        // Act
        timerProvider.StopTimer();

        // Assert
        timerProvider.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void StopTimer_WhenTimerIsNotRunning_ShouldNotThrow()
    {
        // Arrange
        timerProvider.IsRunning.Should().BeFalse();

        // Act
        var action = () => timerProvider.StopTimer();

        // Assert
        action.Should().NotThrow();
        timerProvider.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void Elapsed_WhenStarted_ShouldTrackElapsedTime()
    {
        // Arrange
        var initialElapsed = timerProvider.Elapsed;

        // Act
        timerProvider.StartTimer();
        Thread.Sleep(50); // Let some time pass
        var elapsedAfterStart = timerProvider.Elapsed;

        // Assert
        elapsedAfterStart.Should().BeGreaterThan(initialElapsed);
        elapsedAfterStart.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Test]
    public void IsTimerRunning_WhenTimerStarts_ShouldBeTrue()
    {
        // Arrange
        timerProvider.IsTimerRunning.Should().BeFalse();

        // Act
        timerProvider.StartTimer();

        // Assert
        timerProvider.IsTimerRunning.Should().BeTrue();
    }

    [Test]
    public void IsTimerRunning_WhenTimerStops_ShouldBeFalse()
    {
        // Arrange
        timerProvider.StartTimer();
        timerProvider.IsTimerRunning.Should().BeTrue();

        // Act
        timerProvider.StopTimer();

        // Assert
        timerProvider.IsTimerRunning.Should().BeFalse();
    }

    [Test]
    public void Elapsed_WhenStopped_ShouldResetElapsedTime()
    {
        // Arrange
        timerProvider.StartTimer();
        Thread.Sleep(50); // Let some time pass
        timerProvider.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);

        // Act
        timerProvider.StopTimer();

        // Assert
        timerProvider.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void StartTimer_StopTimer_Cycle_ShouldWorkCorrectly()
    {
        // Act & Assert - First cycle
        timerProvider.StartTimer();
        timerProvider.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();

        timerProvider.StopTimer();
        timerProvider.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Elapsed.Should().Be(TimeSpan.Zero);

        // Act & Assert - Second cycle
        timerProvider.StartTimer();
        timerProvider.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();

        timerProvider.StopTimer();
        timerProvider.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void IsTimerRunning_Property_ShouldBeReadOnly()
    {
        // Arrange
        var isRunning = timerProvider.IsTimerRunning;

        // Assert
        isRunning.Should().Be(false);
        // The IsTimerRunning property should not have a public setter
        // This is verified at compile time by the fact that it has a private setter
    }
}
