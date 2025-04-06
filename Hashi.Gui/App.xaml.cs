using Autofac;
using Hashi.Gui.AutoFac;
using Hashi.Gui.ViewModels;
using Hashi.Gui.Views;
using System.Windows;

namespace Hashi.Gui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly MainViewModel mainViewModel = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        // Initialize the application
        InitializeComponent();
    }

    /// <summary>
    /// Handles the startup event of the application.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacMainModule>();
        var container = builder.Build();

        using var scope = container.BeginLifetimeScope();

        mainViewModel.CreateNewGame();

        new HashiMainView { DataContext = mainViewModel }.Show();
    }

    /// <summary>
    /// Handles the exit event of the application.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        mainViewModel.SaveSettings();
    }
}

