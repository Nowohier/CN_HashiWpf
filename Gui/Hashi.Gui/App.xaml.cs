using Autofac;
using Hashi.Enums;
using Hashi.Gui.AutoFac;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;
using Hashi.Gui.Interfaces.Wrappers;
using System.Windows;
using System.Windows.Threading;

namespace Hashi.Gui;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
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

        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacMainModule>();
        var container = builder.Build();

        using var scope = container.BeginLifetimeScope();

        dialogWrapper = scope.Resolve<IDialogWrapper>();

        mainViewModel = scope.Resolve<IMainViewModel>();
        mainViewModel.CreateNewGameAsync();

        var gui = scope.Resolve<IHashiMainView>();
        gui.DataContext = mainViewModel;
        gui.Show();
    }

    /// <summary>
    ///     Handles the exit event of the application.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        mainViewModel?.SaveSettings();
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
            dialogWrapper?.Show("Error", $"An unhandled exception occurred: {ex.Message}", DialogButton.Ok,
                DialogImage.Error);
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