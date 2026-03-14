using Hashi.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;
using Hashi.Gui.Interfaces.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace Hashi.Gui;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private ServiceProvider? serviceProvider;
    private IDialogWrapper? dialogWrapper;
    private IMainViewModel? mainViewModel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="App" /> class.
    /// </summary>
    public App()
    {
        // Initialize the application
        InitializeComponent();

        // Handle unhandled exceptions
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    /// <summary>
    ///     Handles the startup event of the application.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddHashiServices();
        serviceProvider = services.BuildServiceProvider();

        dialogWrapper = serviceProvider.GetRequiredService<IDialogWrapper>();
        mainViewModel = serviceProvider.GetRequiredService<IMainViewModel>();
        mainViewModel.Initialize();

        var gui = serviceProvider.GetRequiredService<IWindow>();
        gui.DataContext = mainViewModel;
        gui.ShowDialog();
    }

    /// <summary>
    ///     Handles the exit event of the application.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        mainViewModel?.SettingsProvider.SaveSettings();

        serviceProvider?.Dispose();
    }

    /// <summary>
    ///     Handles unhandled exceptions.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Log the exception, show a message to the user, etc.
        dialogWrapper?.Show("Error", $"An unhandled exception occurred: {e.Exception.Message}", DialogButton.Ok,
            DialogImage.Error);

        // Prevent the application from crashing
        e.Handled = true;
    }

    /// <summary>
    ///     Handles unhandled exceptions in non-UI threads.
    /// </summary>
    private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Log the exception, show a message to the user, etc.
        if (e.ExceptionObject is Exception ex)
        {
            dialogWrapper?.Show("Error", $"An unhandled exception occurred: {ex.Message}", DialogButton.Ok,
                DialogImage.Error);
        }
    }

    /// <summary>
    ///     Handles unobserved task exceptions.
    /// </summary>
    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        // Log the exception, show a message to the user, etc.
        dialogWrapper?.Show("Error", $"An unobserved task exception occurred: {e.Exception.Message}", DialogButton.Ok,
            DialogImage.Error);

        // Prevent the application from crashing
        e.SetObserved();
    }
}
