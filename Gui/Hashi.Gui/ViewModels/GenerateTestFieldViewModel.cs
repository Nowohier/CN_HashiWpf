using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Resources.SolutionProviders;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Messages;
using Hashi.Gui.Messaging;
using System.Windows.Input;
using System.Windows.Media;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="IGenerateTestFieldViewModel" />
public class GenerateTestFieldViewModel :
    AsyncObservableRecipient,
    IGenerateTestFieldViewModel,
    IRecipient<IBridgeConnectionChangedMessage>,
    IRecipient<IUpdateAllIslandColorsMessage>,
    IRecipient<IDropTargetIslandChangedMessage>
{
    private readonly Func<SolidColorBrush, IHashiBrush> brushFactory;
    private readonly Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory;
    private readonly IDialogWrapper dialogWrapper;
    private ISolutionProvider? selectedTestSolutionProvider;
    private bool areGridLinesEnabled = true;
    private string? testFieldName;

    public GenerateTestFieldViewModel(Func<SolidColorBrush, IHashiBrush> brushFactory,
        Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory,
        IDialogWrapper dialogWrapper,
        IIslandProvider islandProvider,
        IHintProvider hintProvider,
        ITestSolutionProvider testSolutionProvider)
    {
        this.brushFactory = brushFactory;
        this.solutionProviderFactory = solutionProviderFactory;
        this.dialogWrapper = dialogWrapper;
        IslandProvider = islandProvider;
        HintProvider = hintProvider;
        TestSolutionProvider = testSolutionProvider;

        RemoveAllBridgesCommand = new RelayCommand(RemoveAllBridgesExecute);
        GenerateHintCommand = new RelayCommand(GenerateHintCommandExecute);
        ResetTestFieldCommand = new RelayCommand(ResetTestFieldCommandExecute);
        WindowMouseClickedCommand = new RelayCommand(() => HintProvider.RuleInfoProvider.RuleMessage = string.Empty);
    }


    /// <inheritdoc />
    public IIslandProvider IslandProvider { get; }

    /// <inheritdoc />
    public IHintProvider HintProvider { get; }

    /// <inheritdoc />
    public ITestSolutionProvider TestSolutionProvider { get; }

    /// <summary>
    ///     Removes all bridges from the game.
    /// </summary>
    public ICommand RemoveAllBridgesCommand { get; }

    /// <summary>
    ///    Resets the test field to its initial state.
    /// </summary>
    public ICommand ResetTestFieldCommand { get; }

    /// <summary>
    ///     Generates a hint for the game.
    /// </summary>
    public ICommand GenerateHintCommand { get; }

    /// <summary>
    ///     Handles the mouse click event on the window.
    /// </summary>
    public ICommand WindowMouseClickedCommand { get; }

    /// <summary>
    ///    Gets or sets the selected test solution provider.
    /// </summary>
    public ISolutionProvider? SelectedTestSolutionProvider
    {
        get => selectedTestSolutionProvider;
        set
        {
            if (SetProperty(ref selectedTestSolutionProvider, value) && selectedTestSolutionProvider != null)
            {
                SetTestSolution(selectedTestSolutionProvider);
            }
        }
    }

    /// <summary>
    /// Determines whether the grid lines are enabled.
    /// </summary>
    public bool AreGridLinesEnabled
    {
        get => areGridLinesEnabled;
        set => SetProperty(ref areGridLinesEnabled, value);
    }

    /// <summary>
    ///   Gets or sets the name of the test field.
    /// </summary>
    public string? TestFieldName
    {
        get => testFieldName;
        set => SetProperty(ref testFieldName, value);
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
            BridgeOperationTypeEnum.Add => () => IslandProvider.AddConnection(sourceIsland, targetIsland),
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

    /// <inheritdoc />
    public void SetTestSolution(ISolutionProvider solutionProvider)
    {
        if (solutionProvider is not { BridgeCoordinates: not null, HashiField: not null })
        {
            return;
        }

        HintProvider.ResetSession();
        IslandProvider.InitializeNewSolutionAndSetBridges(solutionProvider);
    }

    private void RemoveAllBridgesExecute()
    {
        IslandProvider.RemoveAllBridges(HashiPointTypeEnum.All);
        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    private void GenerateHintCommandExecute()
    {
        HintProvider.GenerateHint();
    }

    private void ResetTestFieldCommandExecute()
    {
        SetTestSolution(TestSolutionProvider.SolutionProviders[1]);
        //SetTestSolution(solutionProviderFactory.Invoke(TestSolutionProvider.HashiFieldReference, [], null));
    }
}