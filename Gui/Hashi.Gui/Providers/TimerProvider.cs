using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Gui.Interfaces.Providers;
using System.Diagnostics;
using System.Windows.Threading;

namespace Hashi.Gui.Providers;

/// <inheritdoc cref="ITimerProvider" />
public class TimerProvider : ObservableObject, ITimerProvider
{
    private readonly DispatcherTimer dispatcherTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private bool isTimerRunning;

    private readonly Stopwatch stopwatch = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="TimerProvider" /> class.
    /// </summary>
    public TimerProvider()
    {
        dispatcherTimer.Tick += (_, _) => OnPropertyChanged(nameof(Elapsed));
    }

    /// <inheritdoc />
    public TimeSpan Elapsed => stopwatch.Elapsed;

    /// <inheritdoc />
    public bool IsRunning => stopwatch.IsRunning;

    /// <summary>
    ///     Gets a value indicating whether the timer is running.
    /// </summary>
    public bool IsTimerRunning
    {
        get => isTimerRunning;
        private set => SetProperty(ref isTimerRunning, value);
    }

    /// <inheritdoc />
    public void StartTimer()
    {
        if (stopwatch.IsRunning)
        {
            return;
        }

        dispatcherTimer.Start();
        stopwatch.Start();
        IsTimerRunning = true;
    }

    /// <inheritdoc />
    public void StopTimer()
    {
        stopwatch.Reset();
        dispatcherTimer.Stop();
        IsTimerRunning = false;
        OnPropertyChanged(nameof(Elapsed));
    }
}