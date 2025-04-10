using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Gui.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Localization;
using Hashi.Gui.Messages;
using Hashi.Gui.Messaging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="IMainViewModel" />
[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public class MainViewModel : AsyncObservableRecipient,
    IMainViewModel,
    IRecipient<IBridgeConnectionChangedMessage>,
    IRecipient<IUpdateAllIslandColorsMessage>,
    IAsyncRecipient<IAllConnectionsSetMessage>,
    IRecipient<IDropTargetIslandChangedMessage>
{
    private readonly Func<SolidColorBrush, IHashiBrush> brushFactory;
    private readonly IDialogWrapper dialogWrapper;
    private readonly DispatcherTimer dispatcherTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly IHashiGenerator hashiGenerator;
    internal readonly string HashiSettingsFileName = "HashiSettings.json";
    private readonly Func<DifficultyEnum, IHighScorePerDifficultyViewModel> highScorePerDifficultyFactory;
    private readonly IJsonWrapper jsonWrapper;

    internal readonly string SaveFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CN_Hashi\Settings");

    private readonly Func<ISettingsViewModel> settingsFactory;
    private bool isTimerRunning;
    private bool isGeneratingHashiPuzzle;
    private bool isCheating;
    private DifficultyEnum selectedDifficulty = DifficultyEnum.Easy3;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
    /// </summary>
    /// <param name="brushFactory">The solid color brush factory.</param>
    /// <param name="settingsFactory">The settings factory.</param>
    /// <param name="highScorePerDifficultyFactory">The highscore per difficulty factory.</param>
    /// <param name="connectionManager">The connection manager.</param>
    /// <param name="dialogWrapper">The dialog wrapper.</param>
    /// <param name="jsonWrapper">The json wrapper.</param>
    /// <param name="hashiGenerator">The hashi generator.</param>
    public MainViewModel(
        Func<SolidColorBrush, IHashiBrush> brushFactory,
        Func<ISettingsViewModel> settingsFactory,
        Func<DifficultyEnum, IHighScorePerDifficultyViewModel> highScorePerDifficultyFactory,
        IConnectionManagerViewModel connectionManager,
        IDialogWrapper dialogWrapper,
        IJsonWrapper jsonWrapper,
        IHashiGenerator hashiGenerator)
    {
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
        WeakReferenceMessenger.Default.Register<DropTargetIslandChangedMessage>(this);
        CreateNewGameCommand = new AsyncRelayCommand(CreateNewGameAsync);
        RemoveAllBridgesCommand = new RelayCommand(RemoveAllBridgesExecute);
        GenerateHintCommand = new RelayCommand(GenerateHintCommandExecute);
        UndoCommand = new RelayCommand(UndoCommandExecute, UndoCommandCanExecute);
        RedoCommand = new RelayCommand(RedoCommandExecute, RedoCommandCanExecute);
        WindowMouseClickedCommand = new RelayCommand(() => ConnectionManager.RuleMessage = string.Empty);
        ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguageCommandExecute);

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
        set => SetProperty(ref isTimerRunning, value);
    }

    /// <inheritdoc />
    public bool IsCheating
    {
        get => isCheating;
        set => SetProperty(ref isCheating, value);
    }

    /// <inheritdoc />
    public bool IsGeneratingHashiPuzzle
    {
        get => isGeneratingHashiPuzzle;
        set => SetProperty(ref isGeneratingHashiPuzzle, value);
    }

    /// <inheritdoc />
    public DifficultyEnum SelectedDifficulty
    {
        get => selectedDifficulty;
        set
        {
            if (SetProperty(ref selectedDifficulty, value)) OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
        }
    }

    /// <inheritdoc />
    public ICommand CreateNewGameCommand { get; }

    /// <inheritdoc />
    public ICommand RemoveAllBridgesCommand { get; }

    /// <inheritdoc />
    public ICommand GenerateHintCommand { get; }

    /// <inheritdoc />
    public ICommand WindowMouseClickedCommand { get; }

    /// <inheritdoc />
    public ICommand UndoCommand { get; }

    /// <inheritdoc />
    public ICommand RedoCommand { get; }

    /// <inheritdoc />
    public ICommand ChangeLanguageCommand { get; }

    /// <inheritdoc />
    public Stopwatch Timer { get; } = new();

    /// <inheritdoc />
    public ISettingsViewModel LoadSettings()
    {
        ISettingsViewModel loadedSettings;
        try
        {
            var path = Path.Combine(SaveFilePath, HashiSettingsFileName);
            if (File.Exists(path))
            {
                loadedSettings = (SettingsViewModel)jsonWrapper.DeserializeObject(File.ReadAllText(path), typeof(SettingsViewModel))!;
                OnPropertyChanged(nameof(ISettingsViewModel));
                TranslationSource.Instance.CurrentCulture = new CultureInfo(loadedSettings.SelectedLanguageCulture ?? "en-GB");
                return loadedSettings;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.StackTrace);
        }

        loadedSettings = settingsFactory.Invoke();
        loadedSettings.Initialize();
        TranslationSource.Instance.CurrentCulture = new CultureInfo(loadedSettings.SelectedLanguageCulture ?? "en-GB");
        OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
        return loadedSettings;
    }

    /// <inheritdoc />
    public void SaveSettings()
    {
        if (Settings == null) throw new InvalidOperationException("Settings cannot be null.");

        var jsonArray = jsonWrapper.SerializeObject(Settings);
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
    public async Task CreateNewGameAsync()
    {
        IsGeneratingHashiPuzzle = true;
        IsCheating = false;
        var solutionContainer = await hashiGenerator.GenerateHashAsync((int)SelectedDifficulty);
        ConnectionManager.InitializeNewSolution(solutionContainer);

        StopTimer();
        IsGeneratingHashiPuzzle = false;
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

        IsCheating = false;
        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IBridgeConnectionChangedMessage)" />
    public void Receive(IBridgeConnectionChangedMessage message)
    {
        var bridgeOperationType = message.Value.BridgeOperationType;
        var sourceIsland = message.Value.SourceIsland;
        var targetIsland = message.Value.TargetIsland;

        if (bridgeOperationType == BridgeOperationTypeEnum.Add &&
            !ConnectionManager.IsValidDropTarget(sourceIsland, targetIsland))
            return;

        Action bridgeAction = bridgeOperationType switch
        {
            BridgeOperationTypeEnum.Add => () =>
            {
                StartTimer();
                ConnectionManager.AddConnection(sourceIsland, targetIsland);
            }
            ,
            BridgeOperationTypeEnum.RemoveAll => () => ConnectionManager.RemoveAllConnections(sourceIsland, null),
            _ => throw new ArgumentOutOfRangeException()
        };

        bridgeAction();

        sourceIsland.RefreshIslandColor();
        targetIsland?.RefreshIslandColor();

        ConnectionManager.RemoveAllHighlights();
        ConnectionManager.ClearTemporaryDropTargets();
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IUpdateAllIslandColorsMessage)" />
    public void Receive(IUpdateAllIslandColorsMessage message)
    {
        ConnectionManager.RefreshIslandColors();
    }

    /// <inheritdoc cref="IMainViewModel.ReceiveAsync(IAllConnectionsSetMessage,CancellationToken)" />
    public async Task ReceiveAsync(IAllConnectionsSetMessage message, CancellationToken cancellationToken)
    {
        var caption = "Game Over";
        var dialogMessage = "All connections are set!";
        Timer.Stop();

        //ToDo: Check if all islands are connected

        if (IsCheating)
        {
            dialogWrapper.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);
            await CreateNewGameAsync();
            return;
        }

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

        await CreateNewGameAsync();
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IDropTargetIslandChangedMessage)" />
    public void Receive(IDropTargetIslandChangedMessage islandChangedMessage)
    {
        if (islandChangedMessage.Value is not { SourceIsland: { } sourceIsland } ||
            islandChangedMessage.Value.TargetIsland is not { } dropTargetIsland ||
            sourceIsland.GetVisibleNeighbor(dropTargetIsland) is not { } targetIsland)
        {
            ConnectionManager.RemoveAllHighlights();
            ConnectionManager.ClearTemporaryDropTargets();
            return;
        }

        targetIsland.IslandColor = brushFactory.Invoke(HashiColorHelper.GreenIslandBrush);

        ConnectionManager.RemoveAllHighlights();
        ConnectionManager.HighlightPathToTargetIsland(sourceIsland, targetIsland);
    }

    private void GenerateHintCommandExecute()
    {
        IsCheating = true;
        StartTimer();
        ConnectionManager.GenerateHint();
    }

    private void ChangeLanguageCommandExecute(string? culture)
    {
        if (string.IsNullOrEmpty(culture)) return;
        TranslationSource.Instance.CurrentCulture = new CultureInfo(culture);
        Settings.SelectedLanguageCulture = culture;
    }

    private void UndoCommandExecute()
    {
        if (!ConnectionManager.History.Any())
        {
            return;
        }

        var lastEntry = ConnectionManager.History.Last();
        var island1 = ConnectionManager.Islands.SelectMany(x => x).First(x => x.Coordinates.Equals(lastEntry.Point1));
        var island2 = ConnectionManager.Islands.SelectMany(x => x).First(x => x.Coordinates.Equals(lastEntry.Point2));

        var islands = ConnectionManager.GetAllIslandsInvolvedInConnection(island1, island2);

        foreach (var island in islands.Where(x => x.MaxConnections == 0))
        {
            var firstConnection = island.AllConnections.First(x => x.Equals(lastEntry.Point2));
            var secondConnection = island.AllConnections.First(x => x.Equals(lastEntry.Point1));

            island.AllConnections.Remove(firstConnection);
            island.AllConnections.Remove(secondConnection);
        }

        var firstConnection1 = island1.AllConnections.First(x => x.Equals(lastEntry.Point2));
        var secondConnection1 = island2.AllConnections.First(x => x.Equals(lastEntry.Point1));
        island1.AllConnections.Remove(firstConnection1);
        island2.AllConnections.Remove(secondConnection1);
        island1.RefreshIslandColor();
        island2.RefreshIslandColor();
        ConnectionManager.History.Remove(lastEntry);
    }

    private bool UndoCommandCanExecute()
    {
        //return ConnectionManager.History.Any();
        return true;
    }

    private void RedoCommandExecute()
    {

    }

    private bool RedoCommandCanExecute()
    {
        return false;
    }

    private void StartTimer()
    {
        if (Timer.IsRunning) return;

        // Start timers
        dispatcherTimer.Start();
        Timer.Start();
        IsTimerRunning = true;
    }

    private void StopTimer()
    {
        // Stop timers
        Timer.Reset();
        IsTimerRunning = false;
        dispatcherTimer.Stop();
        OnPropertyChanged(nameof(Timer));
    }
}