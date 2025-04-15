using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.ViewModels;
using System.Diagnostics;
using System.Windows.Threading;

namespace Hashi.Gui.Providers;

/// <inheritdoc cref="ITimerProvider" />
public class TimerProvider : NotifyPropertyChangedBase, ITimerProvider
{
    private readonly DispatcherTimer dispatcherTimer = new() { Interval = TimeSpan.FromSeconds(1) };

    public TimerProvider()
    {
        dispatcherTimer.Tick += (_, _) => OnPropertyChanged(nameof(Timer));
    }

    /// <inheritdoc />
    public Stopwatch Timer { get; } = new();

    /// <inheritdoc />
    public bool IsTimerRunning => Timer.IsRunning;

    /// <inheritdoc />
    public void StartTimer()
    {
        if (Timer.IsRunning) return;

        dispatcherTimer.Start();
        Timer.Start();
        OnPropertyChanged(nameof(IsTimerRunning));
    }

    /// <inheritdoc />
    public void StopTimer()
    {
        Timer.Reset();
        dispatcherTimer.Stop();
        OnPropertyChanged(nameof(Timer));
        OnPropertyChanged(nameof(IsTimerRunning));
    }
}