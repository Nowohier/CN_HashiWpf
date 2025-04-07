using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Generator.Interfaces;
using Hashi.Gui.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Messages;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="IMainViewModel" />
[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public class MainViewModel : BaseViewModel,
    IMainViewModel,
    IRecipient<IBridgeConnectionChangedMessage>,
    IRecipient<IUpdateAllIslandColorsMessage>,
    IRecipient<IAllConnectionsSetMessage>,
    IRecipient<ICurrentSourceIslandChangedMessage>,
    IRecipient<IPotentialTargetIslandChangedMessage>
{
    private readonly Func<SolidColorBrush, IHashiBrush> brushFactory;
    private readonly IDialogWrapper dialogWrapper;
    private readonly DispatcherTimer dispatcherTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly IHashiGenerator hashiGenerator;
    internal readonly string HashiSettingsFileName = "HashiSettings.json";
    private readonly Func<DifficultyEnum, IHighScorePerDifficultyViewModel> highScorePerDifficultyFactory;
    private readonly Func<int, int, int, IIslandViewModel> islandFactory;
    private readonly IJsonWrapper jsonWrapper;

    internal readonly string SaveFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CN_Hashi\Settings");

    private readonly Func<ISettingsViewModel> settingsFactory;
    private IIslandViewModel? currentSourceIsland;
    private bool isTimerRunning;
    private IIslandViewModel? potentialTargetIsland;
    private DifficultyEnum selectedDifficulty = DifficultyEnum.Easy3;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
    /// </summary>
    /// <param name="islandFactory">The island factory.</param>
    /// <param name="brushFactory">The solid color brush factory.</param>
    /// <param name="settingsFactory">The settings factory.</param>
    /// <param name="highScorePerDifficultyFactory">The highscore per difficulty factory.</param>
    /// <param name="connectionManager">The connection manager.</param>
    /// <param name="dialogWrapper">The dialog wrapper.</param>
    /// <param name="jsonWrapper">The json wrapper.</param>
    /// <param name="hashiGenerator">The hashi generator.</param>
    public MainViewModel(
        Func<int, int, int, IIslandViewModel> islandFactory,
        Func<SolidColorBrush, IHashiBrush> brushFactory,
        Func<ISettingsViewModel> settingsFactory,
        Func<DifficultyEnum, IHighScorePerDifficultyViewModel> highScorePerDifficultyFactory,
        IConnectionManagerViewModel connectionManager,
        IDialogWrapper dialogWrapper,
        IJsonWrapper jsonWrapper,
        IHashiGenerator hashiGenerator)
    {
        this.islandFactory = islandFactory;
        this.brushFactory = brushFactory;
        this.settingsFactory = settingsFactory;
        this.highScorePerDifficultyFactory = highScorePerDifficultyFactory;
        ConnectionManager = connectionManager;
        this.dialogWrapper = dialogWrapper;
        this.jsonWrapper = jsonWrapper;
        this.hashiGenerator = hashiGenerator;

        WeakReferenceMessenger.Default.Register<BridgeConnectionChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<UpdateAllIslandColorsMessage>(this);
        WeakReferenceMessenger.Default.Register<AllConnectionsSetMessage>(this);
        WeakReferenceMessenger.Default.Register<CurrentSourceIslandChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<PotentialTargetIslandChangedMessage>(this);
        CreateNewGameCommand = new RelayCommand(CreateNewGame);
        RemoveAllBridgesCommand = new RelayCommand(RemoveAllBridgesExecute);

        dispatcherTimer.Tick += (_, _) => OnPropertyChanged(nameof(Timer));
        Settings = LoadSettings();
    }

    /// <inheritdoc />
    public IConnectionManagerViewModel ConnectionManager { get; }

    /// <inheritdoc />
    public ISettingsViewModel Settings { get; }

    /// <inheritdoc />
    public TimeSpan? HighscoreForSelectedDifficulty => Settings.HighScores
        .FirstOrDefault(x => x.Difficulty.Equals(SelectedDifficulty))?.HighScoreTime;

    /// <inheritdoc />
    public bool IsTimerRunning
    {
        get => isTimerRunning;
        set => Set(ref isTimerRunning, value);
    }

    /// <inheritdoc />
    public DifficultyEnum SelectedDifficulty
    {
        get => selectedDifficulty;
        set
        {
            if (Set(ref selectedDifficulty, value)) OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
        }
    }

    /// <inheritdoc />
    public ICommand CreateNewGameCommand { get; }

    /// <inheritdoc />
    public ICommand RemoveAllBridgesCommand { get; }

    /// <inheritdoc />
    public IIslandViewModel? CurrentSourceIsland
    {
        get => currentSourceIsland;
        set => Set(ref currentSourceIsland, value);
    }

    /// <inheritdoc />
    public Stopwatch Timer { get; } = new();

    /// <inheritdoc />
    public IIslandViewModel? PotentialTargetIsland
    {
        get => potentialTargetIsland;
        set => Set(ref potentialTargetIsland, value);
    }

    /// <inheritdoc />
    public ISettingsViewModel LoadSettings()
    {
        ISettingsViewModel loadedSettings;
        try
        {
            var path = Path.Combine(SaveFilePath, HashiSettingsFileName);
            if (File.Exists(path))
            {
                using var file = File.OpenText(path);
                loadedSettings = (ISettingsViewModel)jsonWrapper.Deserialize(file, typeof(SettingsViewModel))!;
                OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
                return loadedSettings;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.StackTrace);
        }

        loadedSettings = settingsFactory.Invoke();
        loadedSettings.HighScores.AddRange(Enum.GetValues<DifficultyEnum>()
            .Select(x => highScorePerDifficultyFactory.Invoke(x)));
        OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
        return loadedSettings;
    }

    /// <inheritdoc />
    public void SaveSettings()
    {
        if (Settings == null) throw new InvalidOperationException("Settings cannot be null.");

        var jsonArray = jsonWrapper.SerializeObject(Settings, Formatting.Indented);
        var path = Path.Combine(SaveFilePath, HashiSettingsFileName);

        try
        {
            if (!Directory.Exists(SaveFilePath)) Directory.CreateDirectory(SaveFilePath);

            File.WriteAllText(path, jsonArray);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.StackTrace);
        }
    }

    /// <inheritdoc />
    public void CreateNewGame()
    {
        var result = hashiGenerator.GenerateHash((int)SelectedDifficulty);
        DrawGame(result);

        // Stop timers
        Timer.Reset();
        IsTimerRunning = false;
        dispatcherTimer.Stop();
        OnPropertyChanged(nameof(Timer));
    }

    /// <inheritdoc />
    public void RemoveAllBridgesExecute()
    {
        foreach (var row in ConnectionManager.Islands)
            foreach (var island in row)
            {
                island.AllConnections.Clear();
                island.NotifyBridgeConnections();
            }

        if (Timer.IsRunning)
        {
            Timer.Reset();
            IsTimerRunning = false;
        }

        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IBridgeConnectionChangedMessage)" />
    public void Receive(IBridgeConnectionChangedMessage message)
    {
        var bridgeOperationType = message.Value.BridgeOperationType;
        var sourceIsland = message.Value.SourceIsland;

        if (bridgeOperationType == BridgeOperationTypeEnum.Add &&
            !ConnectionManager.IsValidDropTarget(CurrentSourceIsland, PotentialTargetIsland))
            return;

        Action bridgeAction = bridgeOperationType switch
        {
            BridgeOperationTypeEnum.Add => () =>
            {
                if (!Timer.IsRunning)
                {
                    // Start timers
                    dispatcherTimer.Start();
                    Timer.Start();
                    IsTimerRunning = true;
                }

                ConnectionManager.AddConnection(CurrentSourceIsland, PotentialTargetIsland);
            }
            ,
            BridgeOperationTypeEnum.RemoveAll => () => ConnectionManager.RemoveAllConnections(sourceIsland, null),
            _ => throw new ArgumentOutOfRangeException()
        };

        bridgeAction();
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IUpdateAllIslandColorsMessage)" />
    public void Receive(IUpdateAllIslandColorsMessage message)
    {
        foreach (var row in ConnectionManager.Islands)
            foreach (var island in row)
                island.IslandColor = island.MaxConnectionsReached
                    ? brushFactory.Invoke(HashiColors.MaxBridgesReachedBrush)
                    : brushFactory.Invoke(HashiColors.BasicIslandBrush);
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IAllConnectionsSetMessage)" />
    public void Receive(IAllConnectionsSetMessage message)
    {
        Timer.Stop();
        var caption = "Game Over";
        var dialogMessage = "All connections are set!";

        //ToDo: Check if all islands are connected

        //Check if highscore - if yes, write highscore to json and show message
        var actualScore = Timer.Elapsed;
        var currentSettingForSetDifficulty =
            Settings.HighScores.FirstOrDefault(x => x.Difficulty == SelectedDifficulty);
        var currentHighScore = currentSettingForSetDifficulty?.HighScoreTime;
        if (currentSettingForSetDifficulty != null && (currentHighScore == null || actualScore < currentHighScore))
        {
            caption = "New Highscore";
            dialogMessage += $"\n\nCongratulations! You have achieved a new highscore for {SelectedDifficulty}.\n" +
                             $"Your time: {actualScore:hh\\:mm\\:ss}\n" +
                             $"Previous highscore: {currentHighScore:hh\\:mm\\:ss}";
            currentSettingForSetDifficulty.HighScoreTime = actualScore;
            SaveSettings();
            OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
            OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
        }

        dialogWrapper.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);

        CreateNewGame();
    }

    /// <inheritdoc cref="IMainViewModel.Receive(ICurrentSourceIslandChangedMessage)" />
    public void Receive(ICurrentSourceIslandChangedMessage message)
    {
        CurrentSourceIsland = message.Value;
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IPotentialTargetIslandChangedMessage)" />
    public void Receive(IPotentialTargetIslandChangedMessage islandChangedMessage)
    {
        if (CurrentSourceIsland == null) return;

        if (islandChangedMessage.Value == null)
        {
            ConnectionManager.RemoveAllHighlights();
            ConnectionManager.RemoveAllPotentialIslandCoordinates();
            PotentialTargetIsland = null;
            return;
        }

        var target = ConnectionManager.GetPotentialTargetIsland(CurrentSourceIsland, islandChangedMessage.Value);

        if (target == null)
        {
            ConnectionManager.RemoveAllHighlights();
            ConnectionManager.RemoveAllPotentialIslandCoordinates();
            PotentialTargetIsland = null;
            return;
        }

        target.IslandColor = brushFactory.Invoke(HashiColors.GreenIslandBrush);
        PotentialTargetIsland = target;

        ConnectionManager.RemoveAllHighlights();
        ConnectionManager.HighlightPathToTargetIsland(CurrentSourceIsland, PotentialTargetIsland);
    }

    private void DrawGame(IReadOnlyList<int[]> mainArray)
    {
        ConnectionManager.Islands.Clear();
        for (var row = 0; row < mainArray.Count; row++)
        {
            var rowCollection = new ObservableCollection<IIslandViewModel>();
            for (var column = 0; column < mainArray[0].Length; column++)
                rowCollection.Add(islandFactory.Invoke(column, row, mainArray[row][column]));
            ConnectionManager.Islands.Add(rowCollection);
        }
    }
}