using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Enums;
using Hashi.Gui.EventArgs;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;
using Hashi.Gui.Messages;
using Hashi.Gui.Messages.MessageContainers;
using Hashi.Gui.Models;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Hashi.Gui.ViewModels;

public class IslandViewModel : BaseViewModel, IIslandViewModel
{
    private readonly IHashiPoint mouseDownPosition = new HashiPoint(0, 0);
    private int count;
    private bool isDragging;
    private bool isHighlightHorizontalLeft;
    private bool isHighlightHorizontalRight;
    private bool isHighlightVerticalBottom;
    private bool isHighlightVerticalTop;
    private IHashiBrush islandColor = new HashiBrush(Brushes.LightBlue);
    private IHashiPoint? potentialTargetIslandCoordinates;

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

    public IHashiPoint? PotentialTargetIslandCoordinates
    {
        get => potentialTargetIslandCoordinates;
        set => Set(ref potentialTargetIslandCoordinates, value);
    }

    /// <inheritdoc />
    public List<IHashiPoint> AllConnections { get; } = new();

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
        set => Set(ref islandColor, value);
    }

    /// <inheritdoc />
    public bool IsHighlightHorizontalLeft
    {
        get => isHighlightHorizontalLeft;
        set => Set(ref isHighlightHorizontalLeft, value);
    }

    /// <inheritdoc />
    public bool IsHighlightHorizontalRight
    {
        get => isHighlightHorizontalRight;
        set => Set(ref isHighlightHorizontalRight, value);
    }

    /// <inheritdoc />
    public bool IsHighlightVerticalTop
    {
        get => isHighlightVerticalTop;
        set => Set(ref isHighlightVerticalTop, value);
    }

    /// <inheritdoc />
    public bool IsHighlightVerticalBottom
    {
        get => isHighlightVerticalBottom;
        set => Set(ref isHighlightVerticalBottom, value);
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
        if (Coordinates.X == targetIsland.Coordinates.X)
            return ConnectionTypeEnum.Vertical;
        if (Coordinates.Y == targetIsland.Coordinates.Y)
            return ConnectionTypeEnum.Horizontal;
        return ConnectionTypeEnum.Diagonal;
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

    /// <summary>
    ///     Handles the drag enter event during drag-and-drop operation.
    /// </summary>
    /// <param name="e">The <see cref="DragEventArgs" />.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
    protected virtual void DragEnterCommandExecute(DragEventArgs? e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));

        IslandColor = new HashiBrush(HashiColors.GreenIslandBrush);
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

        IslandColor = MaxConnectionsReached
            ? new HashiBrush(HashiColors.MaxBridgesReachedBrush)
            : new HashiBrush(HashiColors.BasicIslandBrush);
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
        WeakReferenceMessenger.Default.Send(new CurrentSourceIslandChangedMessage(this));

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
        IslandColor = new HashiBrush(HashiColors.GreenIslandBrush);
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
                new BridgeConnectionInformationContainer(this, BridgeOperationTypeEnum.RemoveAll)));

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
                potentialIsland.Coordinates != PotentialTargetIslandCoordinates)
            {
                Debug.WriteLine(count++);
                PotentialTargetIslandCoordinates = potentialIsland.Coordinates;
                WeakReferenceMessenger.Default.Send(new PotentialTargetIslandChangedMessage(potentialIsland));
            }

        if (e.KeyStates != DragDropKeyStates.None) return;

        isDragging = false;
        WeakReferenceMessenger.Default.Send(
            new BridgeConnectionChangedMessage(
                new BridgeConnectionInformationContainer(this, BridgeOperationTypeEnum.Add)));
        WeakReferenceMessenger.Default.Send(new PotentialTargetIslandChangedMessage(null));
        WeakReferenceMessenger.Default.Send(new CurrentSourceIslandChangedMessage(null));
        WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage());
    }
}