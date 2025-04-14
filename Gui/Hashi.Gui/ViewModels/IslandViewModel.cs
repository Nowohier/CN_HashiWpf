using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Gui.EventArgs;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;
using Hashi.Gui.Messages;
using Hashi.Gui.Messages.MessageContainers;
using Hashi.Gui.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hashi.Gui.ViewModels;

public class IslandViewModel : ObservableRecipient, IIslandViewModel
{
    private readonly IIslandProvider islandProvider;
    private readonly IHashiPoint mouseDownPosition = new HashiPoint(0, 0);
    private IIslandViewModel? dropTargetIsland;
    private bool isDragging;
    private bool isHighlightHorizontalLeft;
    private bool isHighlightHorizontalRight;
    private bool isHighlightVerticalBottom;
    private bool isHighlightVerticalTop;
    private IHashiBrush islandColor = new HashiBrush(Brushes.LightBlue);

    /// <summary>
    ///     Initializes a new instance of the <see cref="IslandViewModel" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="maxConnections">The max connections of the otherIsland.</param>
    /// <param name="islandProvider">The island provider.</param>
    public IslandViewModel(int x, int y, int maxConnections, IIslandProvider islandProvider)
    {
        this.islandProvider = islandProvider;
        MaxConnections = maxConnections;
        Coordinates = new HashiPoint(x, y);

        DragEnterCommand = new RelayCommand<DragEventArgs>(DragEnterCommandExecute);
        DropCommand = new RelayCommand<DragEventArgs>(DropCommandExecute);
        DragOverCommand = new RelayCommand<DragEventArgs>(DragOverCommandExecute);
        DragLeaveCommand = new RelayCommand<DragEventArgs>(DragLeaveCommandExecute);
        MouseMoveCommand = new RelayCommand<MouseEventArgsWithCorrectViewBoxPosition>(MouseMoveCommandExecute);
        MouseLeftButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(MouseLeftButtonDownCommandExecute);
        MouseLeftButtonUpCommand = new RelayCommand<MouseButtonEventArgs>(MouseLeftButtonUpCommandExecute);
    }

    /// <summary>
    ///     Gets or sets the command that is executed when the left mouse button is pressed.
    /// </summary>
    public ICommand MouseLeftButtonDownCommand { get; }

    /// <summary>
    ///     Gets or sets the command that is executed when the left mouse button is released.
    /// </summary>
    public ICommand MouseLeftButtonUpCommand { get; }

    /// <summary>
    ///     Gets or sets the command that is executed when the drag enter event occurs.
    /// </summary>
    public ICommand DragEnterCommand { get; }

    /// <summary>
    ///     Gets or sets the command that is executed when the drag over event occurs.
    /// </summary>
    public ICommand DragOverCommand { get; }

    /// <summary>
    ///     Gets or sets the command that is executed when the drag leave event occurs.
    /// </summary>
    public ICommand DragLeaveCommand { get; }

    /// <summary>
    ///     Gets or sets the command that is executed when the drop event occurs.
    /// </summary>
    public ICommand DropCommand { get; }

    /// <summary>
    ///     Gets or sets the command that is executed when the mouse moves.
    /// </summary>
    public ICommand MouseMoveCommand { get; }

    /// <inheritdoc />
    public ObservableCollection<IHashiPoint> AllConnections { get; } = new();

    /// <inheritdoc />
    public bool MaxConnectionsReached => AllConnections.Count >= MaxConnections;

    /// <inheritdoc />
    public List<IHashiPoint> BridgesLeft =>
        AllConnections.Where(x => x.X < Coordinates.X && x.Y == Coordinates.Y).ToList();

    /// <inheritdoc />
    public List<IHashiPoint> BridgesRight =>
        AllConnections.Where(x => x.X > Coordinates.X && x.Y == Coordinates.Y).ToList();

    /// <inheritdoc />
    public List<IHashiPoint> BridgesUp =>
        AllConnections.Where(x => x.X == Coordinates.X && x.Y < Coordinates.Y).ToList();

    /// <inheritdoc />
    public List<IHashiPoint> BridgesDown =>
        AllConnections.Where(x => x.X == Coordinates.X && x.Y > Coordinates.Y).ToList();

    /// <inheritdoc />
    public IHashiBrush IslandColor
    {
        get => islandColor;
        set => SetProperty(ref islandColor, value);
    }

    /// <inheritdoc />
    public bool IsHighlightHorizontalLeft
    {
        get => isHighlightHorizontalLeft;
        set => SetProperty(ref isHighlightHorizontalLeft, value);
    }

    /// <inheritdoc />
    public bool IsHighlightHorizontalRight
    {
        get => isHighlightHorizontalRight;
        set => SetProperty(ref isHighlightHorizontalRight, value);
    }

    /// <inheritdoc />
    public bool IsHighlightVerticalTop
    {
        get => isHighlightVerticalTop;
        set => SetProperty(ref isHighlightVerticalTop, value);
    }

    /// <inheritdoc />
    public bool IsHighlightVerticalBottom
    {
        get => isHighlightVerticalBottom;
        set => SetProperty(ref isHighlightVerticalBottom, value);
    }

    /// <inheritdoc />
    public int MaxConnections { get; }

    /// <inheritdoc />
    public int RemainingConnections => MaxConnections - AllConnections.Count;

    /// <inheritdoc />
    public IHashiPoint Coordinates { get; }

    /// <inheritdoc />
    public ConnectionTypeEnum GetConnectionType(IIslandViewModel targetIsland)
    {
        if (Coordinates.X == targetIsland.Coordinates.X) return ConnectionTypeEnum.Vertical;
        return Coordinates.Y == targetIsland.Coordinates.Y
            ? ConnectionTypeEnum.Horizontal
            : ConnectionTypeEnum.Diagonal;
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetAllVisibleNeighbors()
    {
        return islandProvider.GetAllVisibleNeighbors(this);
    }

    /// <inheritdoc />
    public IIslandViewModel? GetVisibleNeighbor(IIslandViewModel potentialTargetIsland)
    {
        return islandProvider.GetVisibleNeighbor(this, potentialTargetIsland);
    }

    /// <inheritdoc />
    public void NotifyBridgeConnections()
    {
        OnPropertyChanged(nameof(BridgesLeft));
        OnPropertyChanged(nameof(BridgesRight));
        OnPropertyChanged(nameof(BridgesUp));
        OnPropertyChanged(nameof(BridgesDown));
    }

    /// <inheritdoc />
    public void AddConnection(IHashiPoint connection)
    {
        AllConnections.Add(connection);
        NotifyBridgeConnections();
    }

    /// <inheritdoc />
    public void RemoveAllConnectionsMatchingCoordinates(IHashiPoint sourceSonnection)
    {
        AllConnections.RemoveAll(connection => sourceSonnection.X == connection.X && sourceSonnection.Y == connection.Y);
        NotifyBridgeConnections();
    }

    /// <inheritdoc />
    public bool IsValidDropTarget(IIslandViewModel? target)
    {
        return target != null && MaxConnections > 0 && target.MaxConnections > 0 &&
               !MaxConnectionsReached && !target.MaxConnectionsReached &&
               GetConnectionType(target) != ConnectionTypeEnum.Diagonal;
    }

    /// <inheritdoc />
    public bool? MaxBridgesReachedToTarget(IIslandViewModel? target)
    {
        return target == null
            ? null
            : AllConnections.Count(c => c.Equals(target.Coordinates)) >= 2 ||
              target.AllConnections.Count(c => c.Equals(Coordinates)) >= 2;
    }

    /// <inheritdoc />
    public void ResetDropTarget()
    {
        dropTargetIsland = null;
    }

    /// <inheritdoc />
    public void RefreshIslandColor()
    {
        IslandColor = MaxConnectionsReached
            ? new HashiBrush(HashiColorHelper.MaxBridgesReachedBrush)
            : new HashiBrush(HashiColorHelper.BasicIslandBrush);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"Island, Coordinate (X = {Coordinates.X}, Y = {Coordinates.Y}) | MaxConnections: {MaxConnections}, MaxReached: {MaxConnectionsReached}, BridgesCount: (Up {BridgesUp.Count}, Down {BridgesDown.Count}, Left {BridgesLeft.Count}, Right {BridgesRight.Count}";
    }

    /// <summary>
    ///     Handles the drag enter event during drag-and-drop operation.
    /// </summary>
    /// <param name="e">The <see cref="DragEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void DragEnterCommandExecute(DragEventArgs? e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));

        IslandColor = new HashiBrush(HashiColorHelper.GreenIslandBrush);
    }

    /// <summary>
    ///     Handles the drag over event during drag-and-drop operation.
    /// </summary>
    /// <param name="e">The <see cref="DragEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void DragOverCommandExecute(DragEventArgs? e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
    }

    /// <summary>
    ///     Handles the drag leave event during drag-and-drop operation.
    /// </summary>
    /// <param name="e">The <see cref="DragEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void DragLeaveCommandExecute(DragEventArgs? e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));

        if (!e.Data.GetDataPresent(typeof(IslandViewModel)) ||
            e.Data.GetData(typeof(IslandViewModel)) is not IslandViewModel islandToConnectWith ||
            islandToConnectWith == this)
            return;

        RefreshIslandColor();
    }

    /// <summary>
    ///     Handles the drop event during drag-and-drop operation.
    /// </summary>
    /// <param name="e">The <see cref="DragEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void DropCommandExecute(DragEventArgs? e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));

        if (!e.Data.GetDataPresent(typeof(IslandViewModel)) ||
            e.Data.GetData(typeof(IslandViewModel)) is not IslandViewModel islandToConnectWith ||
            islandToConnectWith == this) return;

        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    /// <summary>
    ///     Handles the mouse move event during drag-and-drop operation.
    /// </summary>
    /// <param name="e">The <see cref="MouseEventArgs" />.</param>
    protected virtual void MouseMoveCommandExecute(MouseEventArgsWithCorrectViewBoxPosition? e)
    {
        if (e is not { MouseEventArgs.LeftButton: MouseButtonState.Pressed }) return;

        // Check if mouse is being dragged
        var currentPosition = e.MouseEventArgs.GetPosition(null);
        if (!(Math.Abs(currentPosition.X - mouseDownPosition.X) > SystemParameters.MinimumHorizontalDragDistance) &&
            !(Math.Abs(currentPosition.Y - mouseDownPosition.Y) >
              SystemParameters.MinimumVerticalDragDistance)) return;

        if (e.MouseEventArgs.OriginalSource is not DependencyObject depObject) return;

        isDragging = true;

        DragDrop.AddQueryContinueDragHandler(depObject, QueryContinueDragHandler);
        DragDrop.DoDragDrop(depObject, this, DragDropEffects.Link);
        DragDrop.RemoveQueryContinueDragHandler(depObject, QueryContinueDragHandler);
    }

    /// <summary>
    ///     Handles the mouse left button down event.
    /// </summary>
    /// <param name="e">The <see cref="MouseButtonEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void MouseLeftButtonDownCommandExecute(MouseButtonEventArgs? e)
    {
        isDragging = false;
        IslandColor = new HashiBrush(HashiColorHelper.GreenIslandBrush);
    }

    /// <summary>
    ///     Handles the mouse left button up event.
    /// </summary>
    /// <param name="e">The <see cref="MouseButtonEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void MouseLeftButtonUpCommandExecute(MouseButtonEventArgs? e)
    {
        if (!isDragging)
            WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(
                new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.RemoveAll, this)));

        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    private void QueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
    {
        var window = Application.Current.MainWindow as IViewBoxControl;

        if (window?.ViewBoxControl is not FrameworkElement viewBox)
            throw new InvalidOperationException("ViewBoxControl is not available.");

        var currentPosition = CursorHelper.GetCurrentCursorPosition(viewBox);

        if (VisualTreeHelper.HitTest(viewBox, currentPosition)?.VisualHit is FrameworkElement element)
            if (element.DataContext != this && element.DataContext is IslandViewModel potentialIsland &&
                potentialIsland != dropTargetIsland)
            {
                dropTargetIsland = potentialIsland;
                WeakReferenceMessenger.Default.Send(new DropTargetIslandChangedMessage(
                    new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.None, this, dropTargetIsland)));
            }

        if (e.KeyStates != DragDropKeyStates.None) return;

        isDragging = false;

        if (dropTargetIsland == null) return;

        var potentialTarget = GetVisibleNeighbor(dropTargetIsland);

        WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(
            new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, this, potentialTarget)));
        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }
}