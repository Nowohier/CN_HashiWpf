using System.Windows;
using Autofac;
using Hashi.Gui.AutoFac;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;

namespace Hashi.Gui;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IMainViewModel mainViewModel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="App" /> class.
    /// </summary>
    public App()
    {
        // Initialize the application
        InitializeComponent();
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
        mainViewModel = scope.Resolve<IMainViewModel>();
        mainViewModel.CreateNewGame();

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

        mainViewModel.SaveSettings();
    }
}