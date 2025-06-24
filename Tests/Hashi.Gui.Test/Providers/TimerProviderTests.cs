using FluentAssertions;
using Hashi.Gui.Providers;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class TimerProviderTests
{
    private TimerProvider sut;

    [SetUp]
    public void SetUp()
    {
        sut = new TimerProvider();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeTimer()
    {
        // Arrange & Act
        var result = new TimerProvider();

        // Assert
        result.Timer.Should().NotBeNull();
        result.Timer.IsRunning.Should().BeFalse();
        result.IsTimerRunning.Should().BeFalse();
    }

    [Test]
    public void Timer_WhenAccessed_ShouldReturnStopwatchInstance()
    {
        // Arrange & Act
        var timer = sut.Timer;

        // Assert
        timer.Should().NotBeNull();
        timer.Should().BeOfType<System.Diagnostics.Stopwatch>();
    }

    [Test]
    public void IsTimerRunning_WhenTimerNotStarted_ShouldReturnFalse()
    {
        // Arrange & Act & Assert
        sut.IsTimerRunning.Should().BeFalse();
    }

    [Test]
    public void StartTimer_WhenCalled_ShouldStartTimer()
    {
        // Arrange & Act
        sut.StartTimer();

        // Assert
        sut.Timer.IsRunning.Should().BeTrue();
        sut.IsTimerRunning.Should().BeTrue();
    }

    [Test]
    public void StartTimer_WhenCalledMultipleTimes_ShouldNotStartTimerAgain()
    {
        // Arrange
        sut.StartTimer();
        var elapsedBefore = sut.Timer.Elapsed;

        // Act
        Thread.Sleep(10); // Small delay to ensure time passes
        sut.StartTimer();

        // Assert
        sut.Timer.IsRunning.Should().BeTrue();
        sut.IsTimerRunning.Should().BeTrue();
        // Timer should still be running and have elapsed time greater than or equal to before
        sut.Timer.Elapsed.Should().BeGreaterOrEqualTo(elapsedBefore);
    }

    [Test]
    public void StopTimer_WhenCalled_ShouldStopAndResetTimer()
    {
        // Arrange
        sut.StartTimer();
        Thread.Sleep(10); // Allow some time to pass
        sut.Timer.IsRunning.Should().BeTrue();

        // Act
        sut.StopTimer();

        // Assert
        sut.Timer.IsRunning.Should().BeFalse();
        sut.IsTimerRunning.Should().BeFalse();
        sut.Timer.Elapsed.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void StopTimer_WhenCalledWithoutStarting_ShouldNotThrowException()
    {
        // Arrange & Act & Assert
        sut.Invoking(x => x.StopTimer()).Should().NotThrow();
        sut.Timer.IsRunning.Should().BeFalse();
        sut.IsTimerRunning.Should().BeFalse();
    }

    [Test]
    public void StartTimer_StopTimer_Sequence_ShouldWorkCorrectly()
    {
        // Arrange & Act
        sut.StartTimer();
        sut.Timer.IsRunning.Should().BeTrue();
        sut.IsTimerRunning.Should().BeTrue();

        sut.StopTimer();
        sut.Timer.IsRunning.Should().BeFalse();
        sut.IsTimerRunning.Should().BeFalse();

        sut.StartTimer();
        sut.Timer.IsRunning.Should().BeTrue();
        sut.IsTimerRunning.Should().BeTrue();

        // Assert
        sut.Timer.Should().NotBeNull();
    }

    [Test]
    public void Timer_WhenRunning_ShouldAccumulateTime()
    {
        // Arrange
        sut.StartTimer();

        // Act
        Thread.Sleep(100); // Allow some time to pass

        // Assert
        sut.Timer.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        sut.Timer.ElapsedMilliseconds.Should().BeGreaterThan(0);
    }

    [Test]
    public void PropertyChanged_ShouldBeRaisedForIsTimerRunning()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.IsTimerRunning))
                propertyChangedRaised = true;
        };

        // Act
        sut.StartTimer();

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void PropertyChanged_ShouldBeRaisedForTimer_WhenStopped()
    {
        // Arrange
        var timerPropertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.Timer))
                timerPropertyChangedRaised = true;
        };

        sut.StartTimer();

        // Act
        sut.StopTimer();

        // Assert
        timerPropertyChangedRaised.Should().BeTrue();
    }
}