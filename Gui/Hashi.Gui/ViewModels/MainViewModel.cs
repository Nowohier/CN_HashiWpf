using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Providers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Managers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Messaging;
using Hashi.Gui.Translation;
using Hashi.Logging.Interfaces;
using System.Globalization;
using System.Windows.Input;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="IMainViewModel" />
public class MainViewModel : AsyncObservableRecipient,
    IMainViewModel,
    IRecipient<IBridgeConnectionChangedMessage>,
    IRecipient<IAllConnectionsSetMessage>,
    IRecipient<ISetTestSolutionMessage>
{
    private readonly IDialogWrapper dialogWrapper;
    private readonly IHashiGenerator hashiGenerator;
    private readonly IResourceManager resourceManager;
    private readonly IHashiBrushResolver brushResolver;
    private readonly ILogger logger;

    private bool isCheating;
    private bool isGeneratingHashiPuzzle;
    private bool isTestFieldMode;
    private DifficultyEnum selectedDifficulty = DifficultyEnum.Easy3;
    private Type? selectedRule;
    private string newRuleName = string.Empty;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
    /// </summary>
    /// <param name="dialogWrapper">The dialog wrapper.</param>
    /// <param name="hashiGenerator">The hashi generator.</param>
    /// <param name="settingsProvider">The settings provider.</param>
    /// <param name="timerProvider">The timer provider.</param>
    /// <param name="islandProvider">The islands provider.</param>
    /// <param name="hintProvider">The hint provider.</param>
    /// <param name="testSolutionProvider">The test solution provider.</param>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="brushResolver">The brush resolver instance.</param>
    public MainViewModel
    (
        IDialogWrapper dialogWrapper,
        IHashiGenerator hashiGenerator,
        ISettingsProvider settingsProvider,
        ITimerProvider timerProvider,
        IIslandProvider islandProvider,
        IHintProvider hintProvider,
        ITestSolutionProvider testSolutionProvider,
        IResourceManager resourceManager,
        ILoggerFactory loggerFactory,
        IHashiBrushResolver brushResolver)
    {
        SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        TimerProvider = timerProvider ?? throw new ArgumentNullException(nameof(timerProvider));
        IslandProvider = islandProvider ?? throw new ArgumentNullException(nameof(islandProvider));
        HintProvider = hintProvider ?? throw new ArgumentNullException(nameof(hintProvider));
        TestSolutionProvider = testSolutionProvider ?? throw new ArgumentNullException(nameof(testSolutionProvider));
        this.dialogWrapper = dialogWrapper ?? throw new ArgumentNullException(nameof(dialogWrapper));
        this.hashiGenerator = hashiGenerator ?? throw new ArgumentNullException(nameof(hashiGenerator));
        this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
        this.brushResolver = brushResolver ?? throw new ArgumentNullException(nameof(brushResolver));
        logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger<MainViewModel>();

        WeakReferenceMessenger.Default.Register<IBridgeConnectionChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<IAllConnectionsSetMessage>(this);
        WeakReferenceMessenger.Default.Register<ISetTestSolutionMessage>(this);
        WeakReferenceMessenger.Default.Register<MainViewModel, IIsTestModeRequestMessage>(this,
            (_, message) => message.Reply(IsTestFieldMode));
        WeakReferenceMessenger.Default.Register<MainViewModel, IDragDirectionChangedRequestTargetMessage>(this,
            GetPotentialDropTarget);

        CreateNewGameCommand = new AsyncRelayCommand(CreateNewGameAsync);
        RemoveAllBridgesCommand = new RelayCommand(RemoveAllBridgesExecute);
        GenerateHintCommand = new RelayCommand(GenerateHintCommandExecute);
        UndoCommand = new RelayCommand(UndoCommandExecute, UndoCommandCanExecute);
        RedoCommand = new RelayCommand(RedoCommandExecute, RedoCommandCanExecute);
        WindowMouseClickedCommand = new RelayCommand(() => hintProvider.RuleInfoProvider.RuleMessage = string.Empty);
        ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguageCommandExecute);
        ToggleTestFieldCommand = new AsyncRelayCommand(ToggleTestFieldCommandExecute);
        ResetTestFieldCommand = new AsyncRelayCommand(ResetTestFieldCommandExecute);
        SaveTestFieldCommand = new RelayCommand(SaveTestFieldCommandExecute);
        DeleteTestFieldCommand = new RelayCommand(DeleteTestFieldCommandExecute);
        CreateTestFieldCommand = new RelayCommand(CreateTestFieldCommandExecute, CreateTestFieldCommandCanExecute);
        ResetAllSettingsToDefaultCommand = new RelayCommand(ResetAllSettingsToDefaultExecute);

    }

    /// <inheritdoc />
    public void Initialize()
    {
        resourceManager.PrepareUi();
        selectedRule = HintProvider.Rules.First();
        WindowColorBrush = brushResolver.ResolveBrush(HashiColor.BasicBrush);
        _ = CreateNewGameAsync();
    }

    /// <summary>
    ///     Gets the highscore for the selected difficulty.
    /// </summary>
    public TimeSpan? HighscoreForSelectedDifficulty => SettingsProvider.Settings.HighScores
        .FirstOrDefault(x => x.Difficulty.Equals(SelectedDifficulty))?.HighScoreTime;

    /// <summary>
    ///     Gets the title of the game window.
    /// </summary>
    public string Title => $"Hashiwokakero{(IsTestFieldMode ? " - Testmode" : string.Empty)}";

    /// <summary>
    ///     Gets the color of the window bar.
    /// </summary>
    public IHashiBrush? WindowColorBrush { get; private set; }

    /// <summary>
    ///     Gets or sets the selected rule for generating hints.
    /// </summary>
    public Type? SelectedRule
    {
        get => selectedRule;
        set
        {
            if (!SetProperty(ref selectedRule, value) || selectedRule == null)
            {
                return;
            }

            HintProvider.RuleInfoProvider.RuleMessage = TranslationSource.Instance[selectedRule.Name] ?? string.Empty;
            HintProvider.RuleInfoProvider.AreRulesBeingApplied = false;
            HintProvider.ResetSession();
            if (IsTestFieldMode)
            {
                SetTestSolution(TestSolutionProvider.SelectedSolutionProvider);
            }
        }
    }

    /// <summary>
    ///     Determines if the game is in cheating mode.
    /// </summary>
    public bool IsCheating
    {
        get => isCheating;
        set => SetProperty(ref isCheating, value);
    }

    /// <summary>
    ///     Determines whether the grid lines are enabled.
    /// </summary>
    public bool AreGridLinesEnabled
    {
        get => SettingsProvider.Settings.AreGridLinesEnabled;
        set
        {
            SettingsProvider.Settings.AreGridLinesEnabled = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the application is in test field mode.
    /// </summary>
    public bool IsTestFieldMode
    {
        get => isTestFieldMode;
        set
        {
            if (!SetProperty(ref isTestFieldMode, value))
            {
                return;
            }

            WindowColorBrush = isTestFieldMode
                ? brushResolver.ResolveBrush(HashiColor.TestModeBrush)
                : brushResolver.ResolveBrush(HashiColor.BasicBrush);
            OnPropertyChanged(nameof(WindowColorBrush));
            OnPropertyChanged(nameof(Title));
        }
    }

    /// <summary>
    ///     Determines if the game is generating a new Hashi puzzle.
    /// </summary>
    public bool IsGeneratingHashiPuzzle
    {
        get => isGeneratingHashiPuzzle;
        set => SetProperty(ref isGeneratingHashiPuzzle, value);
    }

    /// <summary>
    ///     Gets or sets the selected difficulty for the game.
    /// </summary>
    public DifficultyEnum SelectedDifficulty
    {
        get => selectedDifficulty;
        set
        {
            if (SetProperty(ref selectedDifficulty, value))
            {
                OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
            }
        }
    }

    /// <summary>
    ///     Creates a new game.
    /// </summary>
    public ICommand CreateNewGameCommand { get; }

    /// <summary>
    ///     Removes all bridges from the game.
    /// </summary>
    public ICommand RemoveAllBridgesCommand { get; }

    /// <summary>
    ///     Generates a hint for the game.
    /// </summary>
    public ICommand GenerateHintCommand { get; }

    /// <summary>
    ///     Handles the mouse click event on the window.
    /// </summary>
    public ICommand WindowMouseClickedCommand { get; }

    /// <summary>
    ///     Executes the undo command.
    /// </summary>
    public ICommand UndoCommand { get; }

    /// <summary>
    ///     Executes the redo command.
    /// </summary>
    public ICommand RedoCommand { get; }

    /// <summary>
    ///     Changes the language of the game.
    /// </summary>
    public ICommand ChangeLanguageCommand { get; }

    /// <summary>
    ///     Opens the generate test field window.
    /// </summary>
    public ICommand ToggleTestFieldCommand { get; }

    /// <summary>
    ///     Resets the test field to its initial state.
    /// </summary>
    public ICommand ResetTestFieldCommand { get; }

    /// <summary>
    ///   Saves the test field to a file.
    /// </summary>
    public ICommand SaveTestFieldCommand { get; }

    /// <summary>
    ///     Resets all settings to their default values.
    /// </summary>
    public ICommand ResetAllSettingsToDefaultCommand { get; }

    /// <summary>
    ///      Creates a new test field.
    /// </summary>
    public ICommand CreateTestFieldCommand { get; }

    /// <summary>
    ///      Deletes the test field.
    /// </summary>
    public ICommand DeleteTestFieldCommand { get; }

    /// <summary>
    ///    Gets the timer provider for the game.
    /// </summary>
    public ITimerProvider TimerProvider { get; }

    /// <summary>
    ///    Gets the islands provider for the game.
    /// </summary>
    public IIslandProvider IslandProvider { get; }

    /// <summary>
    ///    Gets the hint provider for the game.
    /// </summary>
    public IHintProvider HintProvider { get; }

    /// <summary>
    ///    Gets the test solution provider for the game.
    /// </summary>
    public ITestSolutionProvider TestSolutionProvider { get; }

    /// <inheritdoc />
    public ISettingsProvider SettingsProvider { get; }

    /// <summary>
    ///    Gets or sets the new rule name for the test field.
    /// </summary>
    public string NewRuleName
    {
        get => newRuleName;
        set
        {
            if (SetProperty(ref newRuleName, value))
            {
                ((RelayCommand)CreateTestFieldCommand).NotifyCanExecuteChanged();
            }
        }
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IBridgeConnectionChangedMessage)" />
    public void Receive(IBridgeConnectionChangedMessage message)
    {
        var bridgeOperationType = message.Value.BridgeOperationType;
        var sourceIsland = message.Value.SourceIsland;
        var targetIsland = message.Value.TargetIsland;

        if (bridgeOperationType == BridgeOperationTypeEnum.Add &&
            !sourceIsland.IsValidDropTarget(targetIsland))
        {
            return;
        }

        Action bridgeAction = bridgeOperationType switch
        {
            BridgeOperationTypeEnum.Add => () =>
            {
                TimerProvider.StartTimer();
                IslandProvider.AddConnection(sourceIsland, targetIsland);
            }
            ,
            BridgeOperationTypeEnum.RemoveAll => () => IslandProvider.RemoveAllConnections(sourceIsland, null),
            _ => throw new ArgumentOutOfRangeException()
        };

        bridgeAction();

        if (bridgeOperationType == BridgeOperationTypeEnum.Add)
        {
            IslandProvider.RedoHistory.Clear();
        }

        IslandProvider.RefreshIslandColors();
        IslandProvider.RemoveAllHighlights();
        IslandProvider.ClearTemporaryDropTargets();
        NotifyUndoRedoCanExecuteChanged();
        logger.Debug($"Isolated Groups: {IslandProvider.CountIsolatedIslandGroups()}");
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IAllConnectionsSetMessage)" />
    public void Receive(IAllConnectionsSetMessage message)
    {
        var actualScore = TimerProvider.Timer.Elapsed;
        TimerProvider.StopTimer();

        var caption = TranslationSource.Instance.GetRequired("MessageGameOverCaption");
        var dialogMessage = string.Format(TranslationSource.Instance.GetRequired("MessageGameOverText"), actualScore.ToString(@"hh\:mm\:ss"));

        if (IsCheating || IsTestFieldMode)
        {
            dialogWrapper.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);
            if (!IsTestFieldMode)
            {
                CreateNewGameCommand.Execute(null);
            }

            return;
        }

        //Check if highscore - when yes, write highscore to json and show message
        var currentSettingForSetDifficulty =
            SettingsProvider.Settings.HighScores.FirstOrDefault(x => x.Difficulty == SelectedDifficulty);
        var currentHighScore = currentSettingForSetDifficulty?.HighScoreTime;
        if (currentSettingForSetDifficulty != null && (currentHighScore == null || actualScore < currentHighScore))
        {
            caption = TranslationSource.Instance.GetRequired("MessageNewHighscoreCaption");
            dialogMessage += string.Format(
                TranslationSource.Instance.GetRequired("MessageNewHighscoreText").Replace(@"\n", Environment.NewLine),
                SelectedDifficulty.ToString(), actualScore.ToString(@"hh\:mm\:ss"),
                currentHighScore == null ? "-" : ((TimeSpan)currentHighScore).ToString(@"hh\:mm\:ss"));
            currentSettingForSetDifficulty.HighScoreTime = actualScore;
            SettingsProvider.SaveSettings();
            OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
        }

        dialogWrapper.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);

        CreateNewGameCommand.Execute(null);
    }

    /// <inheritdoc cref="IMainViewModel.Receive(ISetTestSolutionMessage)" />
    public void Receive(ISetTestSolutionMessage message)
    {
        SetTestSolution(message.Value);
    }

    /// <summary>
    ///   Creates a new game asynchronously.
    /// </summary>
    public async Task CreateNewGameAsync()
    {
        IsGeneratingHashiPuzzle = true;
        IsCheating = false;

        try
        {
            var solutionContainer = await hashiGenerator.GenerateHashAsync((int)SelectedDifficulty);

            HintProvider.ResetSession();
            IslandProvider.InitializeNewSolution(solutionContainer);
            TimerProvider.StopTimer();
            NotifyUndoRedoCanExecuteChanged();
        }
        catch (Exception ex)
        {
            logger.Error($"Puzzle generation failed: {ex.Message}");
            dialogWrapper.Show(
                TranslationSource.Instance["MessageGameOverCaption"] ?? "Error",
                "Puzzle generation failed. Please try again.",
                DialogButton.Ok, DialogImage.Warning);
        }
        finally
        {
            IsGeneratingHashiPuzzle = false;
        }
    }

    internal void RemoveAllBridgesExecute()
    {
        IslandProvider.RemoveAllBridges(HashiPointTypeEnum.All);
        IslandProvider.RefreshIslandColors();
        TimerProvider.StopTimer();
        NotifyUndoRedoCanExecuteChanged();

        IsCheating = false;
    }

    internal Task SetTestSolution(ISolutionProvider? solutionProvider)
    {
        if (solutionProvider is not { HashiField: not null })
        {
            return Task.CompletedTask;
        }

        IsGeneratingHashiPuzzle = true;
        IsCheating = false;

        HintProvider.ResetSession();
        IslandProvider.InitializeNewSolutionAndSetBridges(solutionProvider);
        TimerProvider.StopTimer();

        if (HintProvider.Rules.FirstOrDefault(x => x.Name.Equals(solutionProvider.Name)) is { } rule)
        {
            SelectedRule = rule;
        }

        IsGeneratingHashiPuzzle = false;

        return Task.CompletedTask;
    }

    internal void GetPotentialDropTarget(MainViewModel main, IDragDirectionChangedRequestTargetMessage message)
    {
        if (IslandProvider.GetVisibleNeighbor(message.Source, message.Direction) is not { } target ||
            target.MaxConnectionsReached)
        {
            IslandProvider.RemoveAllHighlights();
            IslandProvider.ClearTemporaryDropTargets();
            message.Reply(null);
            return;
        }

        IslandProvider.RefreshIslandColors();
        IslandProvider.RemoveAllHighlights();
        message.Source.IslandColor = brushResolver.ResolveBrush(HashiColor.GreenIslandBrush);
        target.IslandColor = brushResolver.ResolveBrush(HashiColor.GreenIslandBrush);
        IslandProvider.HighlightPathToTargetIsland(message.Source, target);

        message.Reply(target);
    }

    internal void GenerateHintCommandExecute()
    {
        IsCheating = true;
        TimerProvider.StartTimer();
        HintProvider.GenerateHint(SelectedRule!);
    }

    internal async Task ToggleTestFieldCommandExecute()
    {
        IsTestFieldMode = !IsTestFieldMode;

        if (IsTestFieldMode)
        {
            await SetTestSolution(TestSolutionProvider.SelectedSolutionProvider);
        }
        else
        {
            SelectedRule = HintProvider.Rules.First();
            await CreateNewGameAsync();
        }
    }

    internal void ChangeLanguageCommandExecute(string? culture)
    {
        if (string.IsNullOrEmpty(culture))
        {
            return;
        }

        TranslationSource.Instance.CurrentCulture = new CultureInfo(culture);
        SettingsProvider.Settings.SelectedLanguageCulture = culture;
    }

    internal void UndoCommandExecute()
    {
        IslandProvider.UndoConnection();
        IslandProvider.RefreshIslandColors();
        NotifyUndoRedoCanExecuteChanged();
    }

    internal bool UndoCommandCanExecute()
    {
        return IslandProvider.History.Any();
    }

    internal void RedoCommandExecute()
    {
        IslandProvider.RedoConnection();
        IslandProvider.RefreshIslandColors();
        NotifyUndoRedoCanExecuteChanged();
    }

    internal bool RedoCommandCanExecute()
    {
        return IslandProvider.RedoHistory.Any();
    }

    private void NotifyUndoRedoCanExecuteChanged()
    {
        ((RelayCommand)UndoCommand).NotifyCanExecuteChanged();
        ((RelayCommand)RedoCommand).NotifyCanExecuteChanged();
    }

    internal async Task ResetTestFieldCommandExecute()
    {
        await SetTestSolution(TestSolutionProvider.SelectedSolutionProvider);
    }

    internal void SaveTestFieldCommandExecute()
    {
        TestSolutionProvider.ConvertIslandsToSolutionProvider(IslandProvider.IslandsFlat);
        TestSolutionProvider.SaveTestFields();
    }

    internal void ResetAllSettingsToDefaultExecute()
    {
        if (dialogWrapper.Show(TranslationSource.Instance.GetRequired("MessageResetAllToDefaultCaption"),
                TranslationSource.Instance.GetRequired("MessageResetAllToDefaultText"), DialogButton.YesNo,
                DialogImage.Question) == DialogResult.Yes)
        {
            resourceManager.ResetSettingsAndLoadFromDefault();
            SettingsProvider.ResetSettings();
            TestSolutionProvider.ResetSettings();
            TestSolutionProvider.SelectedSolutionProvider = TestSolutionProvider.SolutionProviders.First();
        }
    }

    internal void DeleteTestFieldCommandExecute()
    {
        if (TestSolutionProvider.SelectedSolutionProvider == null)
        {
            return;
        }

        if (dialogWrapper.Show(TranslationSource.Instance.GetRequired("MessageDeleteScenarioCaption"),
                string.Format(TranslationSource.Instance.GetRequired("MessageDeleteScenarioText"),
                    TestSolutionProvider.SelectedSolutionProvider.Name),
                DialogButton.YesNo, DialogImage.Question) == DialogResult.Yes)
        {
            TestSolutionProvider.SolutionProviders.Remove(TestSolutionProvider.SelectedSolutionProvider);
            TestSolutionProvider.SelectedSolutionProvider = TestSolutionProvider.SolutionProviders.FirstOrDefault();
            TestSolutionProvider.SaveTestFields();
        }
    }

    internal void CreateTestFieldCommandExecute()
    {
        var solutionProvider = new SolutionProvider(TestSolutionProvider.HashiFieldReference, null, NewRuleName);
        TestSolutionProvider.SolutionProviders.Add(solutionProvider);
        TestSolutionProvider.SelectedSolutionProvider = solutionProvider;
        TestSolutionProvider.SaveTestFields();
    }

    internal bool CreateTestFieldCommandCanExecute()
    {
        return !string.IsNullOrEmpty(NewRuleName) && !TestSolutionProvider.SolutionProviders.Any(x => x.Name == null || x.Name.Equals(NewRuleName));
    }
}