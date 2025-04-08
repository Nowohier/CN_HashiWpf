using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Enums;
using Hashi.Gui.EventArgs;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;
using Hashi.Gui.Messages;
using Hashi.Gui.Messages.MessageContainers;
using Hashi.Gui.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Hashi.Gui.ViewModels;

public class IslandViewModel : ObservableRecipient, IIslandViewModel
{
    private readonly IHashiPoint mouseDownPosition = new HashiPoint(0, 0);
    private bool isDragging;
    private bool isHighlightHorizontalLeft;
    private bool isHighlightHorizontalRight;
    private bool isHighlightVerticalBottom;
    private bool isHighlightVerticalTop;
    private IHashiBrush islandColor = new HashiBrush(Brushes.LightBlue);
    private IIslandViewModel? dropTargetIsland;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IslandViewModel" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="maxConnections">The max connections of the otherIsland.</param>
    public IslandViewModel(int x, int y, int maxConnections)
    {
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
    public IHashiPoint Coordinates { get; }

    /// <inheritdoc />
    public ICommand MouseLeftButtonDownCommand { get; }

    /// <inheritdoc />
    public ICommand MouseLeftButtonUpCommand { get; }

    /// <inheritdoc />
    public ICommand DragEnterCommand { get; }

    /// <inheritdoc />
    public ICommand DragOverCommand { get; }

    /// <inheritdoc />
    public ICommand DragLeaveCommand { get; }

    /// <inheritdoc />
    public ICommand DropCommand { get; }

    /// <inheritdoc />
    public ICommand MouseMoveCommand { get; }

    /// <inheritdoc />
    public ConnectionTypeEnum GetConnectionType(IIslandViewModel targetIsland)
    {
        if (Coordinates.X == targetIsland.Coordinates.X) return ConnectionTypeEnum.Vertical;
        return Coordinates.Y == targetIsland.Coordinates.Y ? ConnectionTypeEnum.Horizontal : ConnectionTypeEnum.Diagonal;
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
    public void RemoveAllConnectionsMatchingCoordinates(IHashiPoint connection)
    {
        AllConnections.RemoveAll(x => x == connection);
        NotifyBridgeConnections();
    }

    /// <inheritdoc />
    public IIslandViewModel? GetVisibleNeighbor(IIslandViewModel potentialTargetIsland)
    {
        ArgumentNullException.ThrowIfNull(potentialTargetIsland);

        if (this == potentialTargetIsland)
        {
            throw new ArgumentException("Potential target island is identical to source.");
        }

        if (Coordinates.Y == potentialTargetIsland.Coordinates.Y && Coordinates.X > potentialTargetIsland.Coordinates.X)
            return GetVisibleNeighbor(DirectionEnum.Left);
        if (Coordinates.Y == potentialTargetIsland.Coordinates.Y && Coordinates.X < potentialTargetIsland.Coordinates.X)
            return GetVisibleNeighbor(DirectionEnum.Right);
        if (Coordinates.X == potentialTargetIsland.Coordinates.X && Coordinates.Y > potentialTargetIsland.Coordinates.Y)
            return GetVisibleNeighbor(DirectionEnum.Up);
        if (Coordinates.X == potentialTargetIsland.Coordinates.X && Coordinates.Y < potentialTargetIsland.Coordinates.Y)
            return GetVisibleNeighbor(DirectionEnum.Down);
        return null;
    }

    /// <inheritdoc />
    public IIslandViewModel? GetVisibleNeighbor(DirectionEnum direction)
    {
        return direction switch
        {
            DirectionEnum.Up => CheckDirection(0, -1, ConnectionTypeEnum.Vertical),
            DirectionEnum.Down => CheckDirection(0, 1, ConnectionTypeEnum.Vertical),
            DirectionEnum.Left => CheckDirection(-1, 0, ConnectionTypeEnum.Horizontal),
            DirectionEnum.Right => CheckDirection(1, 0, ConnectionTypeEnum.Horizontal),
            _ => null
        };
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetAllVisibleNeighbors()
    {
        var neighbors = new List<IIslandViewModel>();

        if (CheckDirection(0, -1, ConnectionTypeEnum.Vertical) is { } above)
            neighbors.Add(above);

        if (CheckDirection(0, 1, ConnectionTypeEnum.Vertical) is { } below)
            neighbors.Add(below);

        if (CheckDirection(-1, 0, ConnectionTypeEnum.Horizontal) is { } left)
            neighbors.Add(left);

        if (CheckDirection(1, 0, ConnectionTypeEnum.Horizontal) is { } right)
            neighbors.Add(right);

        return neighbors;
    }

    /// <inheritdoc />
    public void ResetDropTarget()
    {
        dropTargetIsland = null;
    }

    /// <inheritdoc />
    public void CheckIslandColor()
    {
        IslandColor = MaxConnectionsReached
            ? new HashiBrush(HashiColorHelper.MaxBridgesReachedBrush)
            : new HashiBrush(HashiColorHelper.BasicIslandBrush);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Island, {Coordinates} | MaxConnections: {MaxConnections}, MaxConnectionsReached: {MaxConnectionsReached}, BridgesCount: (Up {BridgesUp.Count}, Down {BridgesDown.Count}, Left {BridgesLeft.Count}, Right {BridgesRight.Count}";
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

        CheckIslandColor();
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
        {
            WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.RemoveAll, this)));
        }

        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    /// <summary>
    ///     Handles the query continue drag event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
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
                WeakReferenceMessenger.Default.Send(new DropTargetIslandChangedMessage(new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.None, this, dropTargetIsland)));
            }

        if (e.KeyStates != DragDropKeyStates.None) return;

        isDragging = false;

        if (dropTargetIsland == null)
        {
            return;
        }

        var potentialTarget = GetVisibleNeighbor(dropTargetIsland);

        WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, this, potentialTarget)));
        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }

    private IIslandViewModel? CheckDirection(int dx, int dy, ConnectionTypeEnum connectionType)
    {
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();

        if (connectionType == ConnectionTypeEnum.Diagonal)
        {
            return null;
        }

        var currentX = Coordinates.X + dx;
        var currentY = Coordinates.Y + dy;

        while (currentY >= 0 && currentY < islands.Count && currentX >= 0 && currentX < islands[currentY].Count)
        {
            var neighbor = islands[currentY][currentX];
            if (IsCollidingConnection(neighbor, connectionType))
            {
                break;
            }

            if (neighbor.MaxConnections > 0)
            {
                return neighbor;
            }

            currentX += dx;
            currentY += dy;
        }

        return null;
    }

    /// <summary>
    ///     Checks if the potential connection between the source and target islands would collide with existing connections.
    /// </summary>
    /// <param name="target">The target island.</param>
    /// <param name="connectionType">The connection type.</param>
    /// <returns>a boolean value indicating if the connection would collide.</returns>
    private bool IsCollidingConnection(IIslandViewModel target, ConnectionTypeEnum connectionType)
    {
        if (target.MaxConnections > 0)
        {
            return false;
        }

        return connectionType switch
        {
            ConnectionTypeEnum.Vertical => target.BridgesLeft.Count > 0 || target.BridgesRight.Count > 0,
            ConnectionTypeEnum.Horizontal => target.BridgesUp.Count > 0 || target.BridgesDown.Count > 0,
            _ => throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType, "Invalid connection type.")
        };
    }
}