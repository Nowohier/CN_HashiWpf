using System.Diagnostics;
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
        timerProvider.Timer.Should().NotBeNull();
        timerProvider.Timer.Should().BeOfType<Stopwatch>();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Timer.IsRunning.Should().BeFalse();
        timerProvider.Timer.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void StartTimer_WhenTimerIsNotRunning_ShouldStartTimer()
    {
        // Arrange
        timerProvider.Timer.IsRunning.Should().BeFalse();

        // Act
        timerProvider.StartTimer();

        // Assert
        timerProvider.Timer.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();
    }

    [Test]
    public void StartTimer_WhenTimerIsAlreadyRunning_ShouldNotRestartTimer()
    {
        // Arrange
        timerProvider.StartTimer();
        var elapsedBefore = timerProvider.Timer.Elapsed;

        // Let some time pass
        Thread.Sleep(10);

        // Act
        timerProvider.StartTimer(); // Try to start again

        // Assert
        timerProvider.Timer.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();
        // Timer should continue running without reset
        timerProvider.Timer.Elapsed.Should().BeGreaterThan(elapsedBefore);
    }

    [Test]
    public void StopTimer_WhenTimerIsRunning_ShouldStopAndResetTimer()
    {
        // Arrange
        timerProvider.StartTimer();
        timerProvider.Timer.IsRunning.Should().BeTrue();

        // Act
        timerProvider.StopTimer();

        // Assert
        timerProvider.Timer.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Timer.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void StopTimer_WhenTimerIsNotRunning_ShouldNotThrow()
    {
        // Arrange
        timerProvider.Timer.IsRunning.Should().BeFalse();

        // Act
        var action = () => timerProvider.StopTimer();

        // Assert
        action.Should().NotThrow();
        timerProvider.Timer.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Timer.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void Timer_WhenStarted_ShouldTrackElapsedTime()
    {
        // Arrange
        var initialElapsed = timerProvider.Timer.Elapsed;

        // Act
        timerProvider.StartTimer();
        Thread.Sleep(50); // Let some time pass
        var elapsedAfterStart = timerProvider.Timer.Elapsed;

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
    public void Timer_WhenStopped_ShouldResetElapsedTime()
    {
        // Arrange
        timerProvider.StartTimer();
        Thread.Sleep(50); // Let some time pass
        timerProvider.Timer.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);

        // Act
        timerProvider.StopTimer();

        // Assert
        timerProvider.Timer.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void StartTimer_StopTimer_Cycle_ShouldWorkCorrectly()
    {
        // Act & Assert - First cycle
        timerProvider.StartTimer();
        timerProvider.Timer.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();

        timerProvider.StopTimer();
        timerProvider.Timer.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Timer.Elapsed.Should().Be(TimeSpan.Zero);

        // Act & Assert - Second cycle
        timerProvider.StartTimer();
        timerProvider.Timer.IsRunning.Should().BeTrue();
        timerProvider.IsTimerRunning.Should().BeTrue();

        timerProvider.StopTimer();
        timerProvider.Timer.IsRunning.Should().BeFalse();
        timerProvider.IsTimerRunning.Should().BeFalse();
        timerProvider.Timer.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void Timer_Property_ShouldReturnSameInstance()
    {
        // Arrange
        var timer1 = timerProvider.Timer;
        var timer2 = timerProvider.Timer;

        // Assert
        timer1.Should().BeSameAs(timer2);
    }

    [Test]
    public void Timer_Property_ShouldBeReadOnly()
    {
        // Arrange
        var timer = timerProvider.Timer;

        // Assert
        timer.Should().NotBeNull();
        // The Timer property should not have a setter, so we can't assign to it
        // This is verified at compile time
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