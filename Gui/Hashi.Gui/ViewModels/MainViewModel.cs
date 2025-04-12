using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Messages;
using Hashi.Gui.Messaging;
using Hashi.Gui.Translation;
using Hashi.Rules;
using NRules;
using NRules.Diagnostics;
using NRules.Fluent;
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
    private readonly IJsonWrapper jsonWrapper;
    private ISession? session;

    internal readonly string SaveFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CN_Hashi\Settings");

    private readonly Func<ISettingsViewModel> settingsFactory;
    private bool isCheating;
    private bool isGeneratingHashiPuzzle;
    private bool isTimerRunning;
    private DifficultyEnum selectedDifficulty = DifficultyEnum.Easy3;
    private Type selectedRule;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
    /// </summary>
    /// <param name="brushFactory">The solid color brush factory.</param>
    /// <param name="settingsFactory">The settings factory.</param>
    /// <param name="connectionManager">The connection manager.</param>
    /// <param name="dialogWrapper">The dialog wrapper.</param>
    /// <param name="jsonWrapper">The json wrapper.</param>
    /// <param name="hashiGenerator">The hashi generator.</param>
    public MainViewModel(
        Func<SolidColorBrush, IHashiBrush> brushFactory,
        Func<ISettingsViewModel> settingsFactory,
        IConnectionManagerViewModel connectionManager,
        IDialogWrapper dialogWrapper,
        IJsonWrapper jsonWrapper,
        IHashiGenerator hashiGenerator)
    {
        this.brushFactory = brushFactory;
        this.settingsFactory = settingsFactory;
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

        selectedRule = Rules.First();
        dispatcherTimer.Tick += (_, _) => OnPropertyChanged(nameof(Timer));
        Settings = LoadSettings();
    }

    /// <summary>
    /// Gets the connection manager view model.
    /// </summary>
    public IConnectionManagerViewModel ConnectionManager { get; }

    /// <summary>
    /// Gets the settings view model.
    /// </summary>
    public ISettingsViewModel Settings { get; }

    /// <summary>
    /// Gets the highscore for the selected difficulty.
    /// </summary>
    public TimeSpan? HighscoreForSelectedDifficulty => Settings.HighScores
        .FirstOrDefault(x => x.Difficulty.Equals(SelectedDifficulty))?.HighScoreTime;

    /// <summary>
    /// Determines if the timer is running.
    /// </summary>
    public bool IsTimerRunning
    {
        get => isTimerRunning;
        set => SetProperty(ref isTimerRunning, value);
    }

    /// <summary>
    /// Determines if the game is in cheating mode.
    /// </summary>
    public bool IsCheating
    {
        get => isCheating;
        set => SetProperty(ref isCheating, value);
    }

    /// <summary>
    /// Determines if the game is generating a new Hashi puzzle.
    /// </summary>
    public bool IsGeneratingHashiPuzzle
    {
        get => isGeneratingHashiPuzzle;
        set => SetProperty(ref isGeneratingHashiPuzzle, value);
    }

    /// <summary>
    /// Gets the list of rules available for the game.
    /// </summary>
    public IList<Type> Rules { get; } = typeof(_1ConnectionRule1).Assembly.GetTypes().Where(static x => x.Name.StartsWith("_")).ToList();

    /// <summary>
    /// Gets or sets the selected rule for the game.
    /// </summary>
    public Type SelectedRule
    {
        get => selectedRule;
        set
        {
            if (!SetProperty(ref selectedRule, value)) return;

            ConnectionManager.RuleMessage = TranslationSource.Instance[selectedRule.Name] ?? string.Empty;
            ConnectionManager.AreRulesBeingApplied = false;

            if (session == null) return;
            session.Events.RhsExpressionEvaluatedEvent -= OnRhsExpressionEvaluated;
            session = null;
        }
    }

    /// <summary>
    /// Gets or sets the selected difficulty for the game.
    /// </summary>
    public DifficultyEnum SelectedDifficulty
    {
        get => selectedDifficulty;
        set
        {
            if (SetProperty(ref selectedDifficulty, value)) OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
        }
    }

    /// <summary>
    /// Creates a new game.
    /// </summary>
    public ICommand CreateNewGameCommand { get; }

    /// <summary>
    /// Removes all bridges from the game.
    /// </summary>
    public ICommand RemoveAllBridgesCommand { get; }

    /// <summary>
    /// Generates a hint for the game.
    /// </summary>
    public ICommand GenerateHintCommand { get; }

    /// <summary>
    /// Handles the mouse click event on the window.
    /// </summary>
    public ICommand WindowMouseClickedCommand { get; }

    /// <summary>
    /// Executes the undo command.
    /// </summary>
    public ICommand UndoCommand { get; }

    /// <summary>
    /// Executes the redo command.
    /// </summary>
    public ICommand RedoCommand { get; }

    /// <summary>
    /// Changes the language of the game.
    /// </summary>
    public ICommand ChangeLanguageCommand { get; }

    /// <summary>
    /// Gets the timer for the game.
    /// </summary>
    public Stopwatch Timer { get; } = new();

    /// <summary>
    /// Loads the settings from a JSON file.
    /// </summary>
    /// <returns>an <see cref="ISettingsViewModel"/>.</returns>
    public ISettingsViewModel LoadSettings()
    {
        ISettingsViewModel loadedSettings;
        try
        {
            var path = Path.Combine(SaveFilePath, HashiSettingsFileName);
            if (File.Exists(path))
            {
                loadedSettings =
                    (SettingsViewModel)jsonWrapper.DeserializeObject(File.ReadAllText(path),
                        typeof(SettingsViewModel))!;
                OnPropertyChanged(nameof(ISettingsViewModel));
                TranslationSource.Instance.CurrentCulture =
                    new CultureInfo(loadedSettings.SelectedLanguageCulture ?? "en-GB");
                return loadedSettings;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.StackTrace);
        }

        loadedSettings = settingsFactory.Invoke();
        loadedSettings.InitializeHighScores();
        loadedSettings.SelectedLanguageCulture = loadedSettings.Languages[0].Culture;
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

        if (session != null)
        {
            ConnectionManager.AreRulesBeingApplied = false;
            session.Events.RhsExpressionEvaluatedEvent -= OnRhsExpressionEvaluated;
            session = null;
        }

        ConnectionManager.InitializeNewSolution(solutionContainer);

        StopTimer();
        IsGeneratingHashiPuzzle = false;
    }

    /// <summary>
    /// Removes all bridges from the game.
    /// </summary>
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
        GenerateHint();
    }

    private void GenerateHint()
    {
        if (ConnectionManager.AreRulesBeingApplied)
        {
            return;
        }

        ConnectionManager.AreRulesBeingApplied = true;

        if (session == null)
        {
            //Load rules
            var repository = new RuleRepository();

            if (SelectedRule != typeof(_0AllRules))
            {
                repository.Load(x => x.From(SelectedRule));
            }
            else
            {
                repository.Load(x => x.From(Rules));
            }

            //Compile rules
            var factory = repository.Compile();

            //Create rules session
            session = factory.CreateSession();
            session.Events.RhsExpressionEvaluatedEvent += OnRhsExpressionEvaluated;

            session.InsertAll(ConnectionManager.Islands.SelectMany(x => x));
            session.Insert(ConnectionManager);
        }
        else
        {
            session.UpdateAll(ConnectionManager.Islands.SelectMany(x => x));
            session.Update(ConnectionManager);
        }

        var rulesFired = session.Fire();
        ConnectionManager.AreRulesBeingApplied = false;
    }

    private void OnRhsExpressionEvaluated(object? sender, RhsExpressionEventArgs e)
    {
        ConnectionManager.AreRulesBeingApplied = false;
    }

    private void ChangeLanguageCommandExecute(string? culture)
    {
        if (string.IsNullOrEmpty(culture)) return;
        TranslationSource.Instance.CurrentCulture = new CultureInfo(culture);
        Settings.SelectedLanguageCulture = culture;
    }

    private void UndoCommandExecute()
    {
        if (!ConnectionManager.History.Any()) return;

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
        // ToDo: Implement this correctly
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