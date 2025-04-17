using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Gui.EventArgs;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Messages.MessageContainers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;

namespace Hashi.Gui.ViewModels;

public class IslandViewModel :
    ObservableRecipient,
    IIslandViewModel
{
    private readonly Func<IBridgeConnectionInformationContainer, IBridgeConnectionChangedMessage>
        bridgeConnectionChangedMessageFactory;

    private readonly Func<SolidColorBrush, IHashiBrush> brushFactory;

    private readonly
        Func<BridgeOperationTypeEnum, IIslandViewModel, IIslandViewModel?, IBridgeConnectionInformationContainer>
        connectionInformationContainerFactory;

    private readonly Func<IIslandViewModel, DirectionEnum, IDragDirectionChangedRequestTargetMessage>
        dragDirectionChangedRequestTargetMessageFactory;

    private readonly Func<IIsTestModeRequestMessage> isTestModeRequestMessageFactory;
    private readonly IHashiPoint mouseDownPosition;
    private readonly Func<bool?, IUpdateAllIslandColorsMessage> updateAllIslandColorsMessageFactory;
    private readonly FrameworkElement viewBoxControl;
    private DirectionEnum actualDragDirection;
    private IIslandViewModel? dropTargetIsland;
    private bool isDragging;
    private bool isHighlightHorizontalLeft;
    private bool isHighlightHorizontalRight;
    private bool isHighlightVerticalBottom;
    private bool isHighlightVerticalTop;
    private IHashiBrush islandColor;
    private Point startDragPosition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IslandViewModel" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="maxConnections">The max connections of the otherIsland.</param>
    /// <param name="viewBoxControl">The viewBox.</param>
    /// <param name="hashiPointFactory">The hashi point factory.</param>
    /// <param name="brushFactory">The brush factory.</param>
    /// <param name="updateAllIslandColorsMessageFactory">The update all island colors message factory.</param>
    /// <param name="connectionInformationContainerFactory">The bridge connection information container factory.</param>
    /// <param name="bridgeConnectionChangedMessageFactory">The bridge connection changed message factory.</param>
    /// <param name="isTestModeRequestMessageFactory">The is test mode request factory.</param>
    /// <param name="dragDirectionChangedRequestTargetMessageFactory">
    ///     The drag direction changed request target message
    ///     factory.
    /// </param>
    public IslandViewModel
    (
        int x,
        int y,
        int maxConnections,
        IViewBoxControl viewBoxControl,
        Func<int, int, HashiPointTypeEnum, IHashiPoint> hashiPointFactory,
        Func<SolidColorBrush, IHashiBrush> brushFactory,
        Func<bool?, IUpdateAllIslandColorsMessage> updateAllIslandColorsMessageFactory,
        Func<BridgeOperationTypeEnum, IIslandViewModel, IIslandViewModel?, IBridgeConnectionInformationContainer>
            connectionInformationContainerFactory,
        Func<IBridgeConnectionInformationContainer, IBridgeConnectionChangedMessage>
            bridgeConnectionChangedMessageFactory,
        Func<IIsTestModeRequestMessage> isTestModeRequestMessageFactory,
        Func<IIslandViewModel, DirectionEnum, IDragDirectionChangedRequestTargetMessage>
            dragDirectionChangedRequestTargetMessageFactory
    )
    {
        MaxConnections = maxConnections;
        Coordinates = hashiPointFactory.Invoke(x, y, HashiPointTypeEnum.Normal);
        this.viewBoxControl = (FrameworkElement)viewBoxControl.ViewBoxControl;
        this.brushFactory = brushFactory;
        this.updateAllIslandColorsMessageFactory = updateAllIslandColorsMessageFactory;
        this.connectionInformationContainerFactory = connectionInformationContainerFactory;
        this.bridgeConnectionChangedMessageFactory = bridgeConnectionChangedMessageFactory;
        this.isTestModeRequestMessageFactory = isTestModeRequestMessageFactory;
        this.dragDirectionChangedRequestTargetMessageFactory = dragDirectionChangedRequestTargetMessageFactory;

        DragEnterCommand = new RelayCommand<DragEventArgs>(DragEnterCommandExecute);
        DropCommand = new RelayCommand<DragEventArgs>(DropCommandExecute);
        DragOverCommand = new RelayCommand<DragEventArgs>(DragOverCommandExecute);
        DragLeaveCommand = new RelayCommand<DragEventArgs>(DragLeaveCommandExecute);
        MouseMoveCommand = new RelayCommand<MouseEventArgsWithCorrectViewBoxPosition>(MouseMoveCommandExecute);
        MouseLeftButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(MouseLeftButtonDownCommandExecute);
        MouseLeftButtonUpCommand = new RelayCommand<MouseButtonEventArgs>(MouseLeftButtonUpCommandExecute);
        MouseRightButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(MouseRightButtonDownCommandExecute);
        MouseRightButtonUpCommand = new RelayCommand<MouseButtonEventArgs>(MouseRightButtonUpCommandExecute);

        islandColor = brushFactory.Invoke(Brushes.LightBlue);
        mouseDownPosition = hashiPointFactory.Invoke(0, 0, HashiPointTypeEnum.Normal);
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

    /// <summary>
    ///     Gets or sets the command that is executed when the right mouse button is pressed.
    /// </summary>
    public ICommand MouseRightButtonDownCommand { get; }

    /// <summary>
    ///     Gets or sets the command that is executed when the right mouse button is released.
    /// </summary>
    public ICommand MouseRightButtonUpCommand { get; }

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
    public int MaxConnections { get; private set; }

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
        AllConnections.RemoveAll(connection =>
            sourceSonnection.X == connection.X && sourceSonnection.Y == connection.Y);
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
            ? brushFactory.Invoke(HashiColorHelper.MaxBridgesReachedBrush)
            : brushFactory.Invoke(HashiColorHelper.BasicIslandBrush);
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
        if (!MaxConnectionsReached) IslandColor = brushFactory.Invoke(HashiColorHelper.GreenIslandBrush);
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

        WeakReferenceMessenger.Default.Send(updateAllIslandColorsMessageFactory.Invoke(null));
    }

    /// <summary>
    ///     Handles the mouse move event during drag-and-drop operation.
    /// </summary>
    /// <param name="e">The <see cref="MouseEventArgs" />.</param>
    protected virtual void MouseMoveCommandExecute(MouseEventArgsWithCorrectViewBoxPosition? e)
    {
        if (e is not { MouseEventArgs.LeftButton: MouseButtonState.Pressed } || MaxConnectionsReached)
        {
            dropTargetIsland = null;
            actualDragDirection = DirectionEnum.None;
            return;
        }

        // Check if mouse is being dragged
        var currentPosition = e.MouseEventArgs.GetPosition(null);
        if (!(Math.Abs(currentPosition.X - mouseDownPosition.X) > SystemParameters.MinimumHorizontalDragDistance) &&
            !(Math.Abs(currentPosition.Y - mouseDownPosition.Y) >
              SystemParameters.MinimumVerticalDragDistance)) return;

        if (e.MouseEventArgs.OriginalSource is not DependencyObject depObject) return;

        isDragging = true;
        startDragPosition = CursorHelper.GetCurrentCursorPosition(viewBoxControl);

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
        OnPropertyChanged(nameof(MaxConnections));
        if (MaxConnectionsReached) return;
        IslandColor = brushFactory.Invoke(HashiColorHelper.GreenIslandBrush);
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
            var info = connectionInformationContainerFactory.Invoke(BridgeOperationTypeEnum.RemoveAll, this, null);
            var message = bridgeConnectionChangedMessageFactory.Invoke(info);
            WeakReferenceMessenger.Default.Send(message);

            var isTestMode = WeakReferenceMessenger.Default.Send(isTestModeRequestMessageFactory.Invoke()).Response;
            if (isTestMode)
            {
                MaxConnections = MaxConnections == 8 ? 0 : MaxConnections + 1;
                OnPropertyChanged(nameof(MaxConnections));
            }
        }

        WeakReferenceMessenger.Default.Send(updateAllIslandColorsMessageFactory.Invoke(null));
    }

    /// <summary>
    ///     Handles the mouse right button down event.
    /// </summary>
    /// <param name="e">The <see cref="MouseButtonEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void MouseRightButtonDownCommandExecute(MouseButtonEventArgs? e)
    {
    }

    /// <summary>
    ///     Handles the mouse right button up event.
    /// </summary>
    /// <param name="e">The <see cref="MouseButtonEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void MouseRightButtonUpCommandExecute(MouseButtonEventArgs? e)
    {
        if (!isDragging)
        {
            var isTestMode = WeakReferenceMessenger.Default.Send(isTestModeRequestMessageFactory.Invoke()).Response;
            if (isTestMode)
            {
                MaxConnections = MaxConnections == 0 ? 0 : MaxConnections - 1;
                OnPropertyChanged(nameof(MaxConnections));
            }
        }

        WeakReferenceMessenger.Default.Send(updateAllIslandColorsMessageFactory.Invoke(null));
    }

    private void QueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
    {
        // Calculate the difference in X and Y coordinates
        var currentPosition = CursorHelper.GetCurrentCursorPosition(viewBoxControl);
        var deltaX = currentPosition.X - startDragPosition.X;
        var deltaY = currentPosition.Y - startDragPosition.Y;

        // Determine the drag direction
        var dragDirection = GetDragDirection(deltaX, deltaY);

        if (dragDirection != actualDragDirection)
        {
            actualDragDirection = dragDirection;
            var dragDirectionMessage = dragDirectionChangedRequestTargetMessageFactory.Invoke(this, dragDirection);
            dropTargetIsland = WeakReferenceMessenger.Default.Send(dragDirectionMessage).Response;
        }

        if (e.KeyStates != DragDropKeyStates.None) return;

        isDragging = false;

        if (dropTargetIsland == null) return;

        var islandInfos =
            connectionInformationContainerFactory.Invoke(BridgeOperationTypeEnum.Add, this, dropTargetIsland);
        var addMessage = bridgeConnectionChangedMessageFactory.Invoke(islandInfos);
        WeakReferenceMessenger.Default.Send(addMessage);
        dropTargetIsland = null;
        actualDragDirection = DirectionEnum.None;
        WeakReferenceMessenger.Default.Send(updateAllIslandColorsMessageFactory.Invoke(null));
    }

    private DirectionEnum GetDragDirection(double deltaX, double deltaY)
    {
        const double threshold = 1.0; // Minimum movement to consider a direction

        // Ignore small movements
        if (Math.Abs(deltaX) < threshold && Math.Abs(deltaY) < threshold)
            return DirectionEnum.None;

        // Calculate angle in degrees
        var angle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI);

        // Normalize angle to range [0, 360)
        if (angle < 0)
            angle += 360;

        // Determine direction based on angle
        return angle switch
        {
            >= 45 and < 135 => DirectionEnum.Down,
            >= 135 and < 225 => DirectionEnum.Left,
            >= 225 and < 315 => DirectionEnum.Up,
            _ => DirectionEnum.Right
        };
    }
}