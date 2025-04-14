using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Messages;
using Hashi.Gui.Messaging;
using Hashi.Gui.Translation;
using Hashi.Rules;
using NRules;
using NRules.Diagnostics;
using NRules.Fluent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;

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
    private readonly IHashiGenerator hashiGenerator;
    private bool isCheating;
    private bool isGeneratingHashiPuzzle;
    private DifficultyEnum selectedDifficulty = DifficultyEnum.Easy3;
    private Type selectedRule;
    private ISession? session;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
    /// </summary>
    /// <param name="brushFactory">The solid color brush factory.</param>
    /// <param name="dialogWrapper">The dialog wrapper.</param>
    /// <param name="hashiGenerator">The hashi generator.</param>
    /// <param name="settingsProvider">The settings provider.</param>
    /// <param name="timerProvider">The timer provider.</param>
    /// <param name="islandProvider"></param>
    public MainViewModel(
        Func<SolidColorBrush, IHashiBrush> brushFactory,
        IDialogWrapper dialogWrapper,
        IHashiGenerator hashiGenerator,
        IHashiSettingsProvider settingsProvider,
        ITimerProvider timerProvider,
        IIslandProvider islandProvider)
    {
        this.brushFactory = brushFactory;
        SettingsProvider = settingsProvider;
        TimerProvider = timerProvider;
        IslandProvider = islandProvider;
        this.dialogWrapper = dialogWrapper;
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
        WindowMouseClickedCommand = new RelayCommand(() => IslandProvider.RuleMessage = string.Empty);
        ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguageCommandExecute);

        selectedRule = Rules.First();
    }

    /// <inheritdoc />
    public ITimerProvider TimerProvider { get; }

    /// <inheritdoc />
    public IIslandProvider IslandProvider { get; }

    /// <summary>
    ///     Gets the highscore for the selected difficulty.
    /// </summary>
    public TimeSpan? HighscoreForSelectedDifficulty => SettingsProvider.Settings.HighScores
        .FirstOrDefault(x => x.Difficulty.Equals(SelectedDifficulty))?.HighScoreTime;

    /// <summary>
    ///     Determines if the game is in cheating mode.
    /// </summary>
    public bool IsCheating
    {
        get => isCheating;
        set => SetProperty(ref isCheating, value);
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
    ///     Gets the list of rules available for the game.
    /// </summary>
    public IList<Type> Rules { get; } = typeof(_1ConnectionRule1).Assembly.GetTypes()
        .Where(static x => x.Name.StartsWith('_')).ToList();

    /// <summary>
    ///     Gets or sets the selected rule for the game.
    /// </summary>
    public Type SelectedRule
    {
        get => selectedRule;
        set
        {
            if (!SetProperty(ref selectedRule, value)) return;

            IslandProvider.RuleMessage = TranslationSource.Instance[selectedRule.Name] ?? string.Empty;
            IslandProvider.AreRulesBeingApplied = false;

            if (session == null) return;
            session.Events.RhsExpressionEvaluatedEvent -= OnRhsExpressionEvaluated;
            session = null;
        }
    }

    /// <summary>
    ///     Gets or sets the selected difficulty for the game.
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

    /// <inheritdoc />
    public IHashiSettingsProvider SettingsProvider { get; }

    /// <inheritdoc />
    public async Task CreateNewGameAsync()
    {
        IsGeneratingHashiPuzzle = true;
        IsCheating = false;
        var solutionContainer = await hashiGenerator.GenerateHashAsync((int)SelectedDifficulty);

        if (session != null)
        {
            IslandProvider.AreRulesBeingApplied = false;
            session.Events.RhsExpressionEvaluatedEvent -= OnRhsExpressionEvaluated;
            session = null;
        }

        IslandProvider.InitializeNewSolution(solutionContainer);

        TimerProvider.StopTimer();
        IsGeneratingHashiPuzzle = false;
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IBridgeConnectionChangedMessage)" />
    public void Receive(IBridgeConnectionChangedMessage message)
    {
        var bridgeOperationType = message.Value.BridgeOperationType;
        var sourceIsland = message.Value.SourceIsland;
        var targetIsland = message.Value.TargetIsland;

        if (bridgeOperationType == BridgeOperationTypeEnum.Add &&
            !sourceIsland.IsValidDropTarget(targetIsland))
            return;

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

        sourceIsland.RefreshIslandColor();
        targetIsland?.RefreshIslandColor();

        IslandProvider.RemoveAllHighlights();
        IslandProvider.ClearTemporaryDropTargets();
    }

    /// <inheritdoc cref="IMainViewModel.Receive(IUpdateAllIslandColorsMessage)" />
    public void Receive(IUpdateAllIslandColorsMessage message)
    {
        IslandProvider.RefreshIslandColors();
    }

    /// <inheritdoc cref="IMainViewModel.ReceiveAsync(IAllConnectionsSetMessage,CancellationToken)" />
    public async Task ReceiveAsync(IAllConnectionsSetMessage message, CancellationToken cancellationToken)
    {
        var caption = TranslationSource.Instance["MessageGameOverCaption"]!;
        var dialogMessage = TranslationSource.Instance["MessageGameOverText"]!;
        var actualScore = TimerProvider.Timer.Elapsed;
        TimerProvider.StopTimer();

        //ToDo: Check if all islands are connected

        if (IsCheating)
        {
            dialogWrapper.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);
            await CreateNewGameAsync();
            return;
        }

        //Check if highscore - if yes, write highscore to json and show message
        var currentSettingForSetDifficulty =
            SettingsProvider.Settings.HighScores.FirstOrDefault(x => x.Difficulty == SelectedDifficulty);
        var currentHighScore = currentSettingForSetDifficulty?.HighScoreTime;
        if (currentSettingForSetDifficulty != null && (currentHighScore == null || actualScore < currentHighScore))
        {
            caption = TranslationSource.Instance["MessageNewHighscoreCaption"]!;
            dialogMessage += string.Format(
                TranslationSource.Instance["MessageNewHighscoreText"]!.Replace(@"\n", Environment.NewLine),
                SelectedDifficulty.ToString(), actualScore.ToString(@"hh\:mm\:ss"),
                currentHighScore == null ? "-" : ((TimeSpan)currentHighScore).ToString(@"hh\:mm\:ss"));
            currentSettingForSetDifficulty.HighScoreTime = actualScore;
            SettingsProvider.SaveSettings();
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
            IslandProvider.RemoveAllHighlights();
            IslandProvider.ClearTemporaryDropTargets();
            return;
        }

        targetIsland.IslandColor = brushFactory.Invoke(HashiColorHelper.GreenIslandBrush);

        IslandProvider.RemoveAllHighlights();
        IslandProvider.HighlightPathToTargetIsland(sourceIsland, targetIsland);
    }

    /// <summary>
    ///     Removes all bridges from the game.
    /// </summary>
    public void RemoveAllBridgesExecute()
    {
        IslandProvider.RemoveAllBridges();
        TimerProvider.StopTimer();

        IsCheating = false;
        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    private void GenerateHintCommandExecute()
    {
        IsCheating = true;
        TimerProvider.StartTimer();
        GenerateHint();
    }

    private void GenerateHint()
    {
        // ToDo Move to HintProvider and inject in ConnectionManager
        if (IslandProvider.AreRulesBeingApplied) return;

        IslandProvider.AreRulesBeingApplied = true;

        if (session == null)
        {
            //Load rules
            var repository = new RuleRepository();

            if (SelectedRule != typeof(_0AllRules))
                repository.Load(x => x.From(SelectedRule));
            else
                repository.Load(x => x.From(Rules));

            //Compile rules
            var factory = repository.Compile();

            //Create rules session
            session = factory.CreateSession();
            session.Events.RhsExpressionEvaluatedEvent += OnRhsExpressionEvaluated;

            session.InsertAll(IslandProvider.IslandsFlat);
            session.Insert(IslandProvider);
        }
        else
        {
            session.UpdateAll(IslandProvider.IslandsFlat);
            session.Update(IslandProvider);
        }

        var rulesFired = session.Fire();

        if (rulesFired == 0)
            dialogWrapper.Show(TranslationSource.Instance["MessageNoHintsCaption"]!,
                TranslationSource.Instance["MessageNoHintsText"]!, DialogButton.Ok, DialogImage.Information);

        IslandProvider.AreRulesBeingApplied = false;
    }

    private void OnRhsExpressionEvaluated(object? sender, RhsExpressionEventArgs e)
    {
        IslandProvider.AreRulesBeingApplied = false;
    }

    private void ChangeLanguageCommandExecute(string? culture)
    {
        if (string.IsNullOrEmpty(culture)) return;
        TranslationSource.Instance.CurrentCulture = new CultureInfo(culture);
        SettingsProvider.Settings.SelectedLanguageCulture = culture;
    }

    private void UndoCommandExecute()
    {
        IslandProvider.UndoConnection();
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
}