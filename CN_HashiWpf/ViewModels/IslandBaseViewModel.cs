using CNHashiWpf.Enums;
using CNHashiWpf.EventArgs;
using CNHashiWpf.Helpers;
using CNHashiWpf.Interfaces;
using CNHashiWpf.Messages;
using CNHashiWpf.Messages.MessageContainers;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CNHashiWpf.ViewModels
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public abstract class IslandBaseViewModel : BaseViewModel
    {
        private Point mouseDownPosition;
        private bool isDragging;
        private Brush islandColor = Brushes.LightBlue;
        private double currentDragLineX1;
        private double currentDragLineX2;
        private double currentDragLineY1;
        private double currentDragLineY2;

        protected IslandBaseViewModel(int maxConnections, Point coordinates)
        {
            MaxConnections = maxConnections;
            Coordinates = coordinates;

            DragEnterCommand = new RelayCommand<DragEventArgs>(DragEnterCommandExecute);
            DropCommand = new RelayCommand<DragEventArgs>(DropCommandExecute);
            DragOverCommand = new RelayCommand<DragEventArgs>(DragOverCommandExecute);
            DragLeaveCommand = new RelayCommand<DragEventArgs>(DragLeaveCommandExecute);
            MouseMoveCommand = new RelayCommand<MouseEventArgsWithCorrectViewBoxPosition>(MouseMoveCommandExecute);
            MouseLeftButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(MouseLeftButtonDownCommandExecute);
            MouseLeftButtonUpCommand = new RelayCommand<MouseButtonEventArgs>(MouseLeftButtonUpCommandExecute);
        }

        public double CurrentDragLineX1
        {
            get => currentDragLineX1;
            set => Set(ref currentDragLineX1, value);
        }

        public double CurrentDragLineX2
        {
            get => currentDragLineX2;
            set => Set(ref currentDragLineX2, value);
        }

        public double CurrentDragLineY1
        {
            get => currentDragLineY1;
            set => Set(ref currentDragLineY1, value);
        }

        public double CurrentDragLineY2
        {
            get => currentDragLineY2;
            set => Set(ref currentDragLineY2, value);
        }

        /// <summary>
        /// A list of all set connections for this island.
        /// </summary>
        public List<Point> AllConnections { get; } = new();

        /// <summary>
        /// Determines if the max connections have been reached
        /// </summary>
        public bool MaxConnectionsReached => AllConnections.Count >= MaxConnections;

        /// <summary>
        /// Gets the bridges on the left side.
        /// </summary>
        public List<Point> BridgesLeft => AllConnections.Where(x => x.X < Coordinates.X && x.Y == Coordinates.Y).ToList();

        /// <summary>
        /// Gets the bridges on the right side.
        /// </summary>
        public List<Point> BridgesRight => AllConnections.Where(x => x.X > Coordinates.X && x.Y == Coordinates.Y).ToList();

        /// <summary>
        /// Gets the bridges on the bottom side.
        /// </summary>
        public List<Point> BridgesUp => AllConnections.Where(x => x.X == Coordinates.X && x.Y < Coordinates.Y).ToList();

        /// <summary>
        /// Gets the bridges on the top side.
        /// </summary>
        public List<Point> BridgesDown => AllConnections.Where(x => x.X == Coordinates.X && x.Y > Coordinates.Y).ToList();

        /// <summary>
        /// Gets or sets the color of the island.
        /// </summary>
        public Brush IslandColor
        {
            get => islandColor;
            set => Set(ref islandColor, value);
        }

        /// <summary>
        /// Gets the maximum number of connections for this island.
        /// </summary>
        public int MaxConnections { get; }

        /// <summary>
        /// Gets the coordinates of the island.
        /// </summary>
        public Point Coordinates { get; }

        /// <summary>
        /// Gets the command for the left mouse button down event.
        /// </summary>
        public ICommand MouseLeftButtonDownCommand { get; }

        /// <summary>
        /// Gets the command for the left mouse button up event.
        /// </summary>
        public ICommand MouseLeftButtonUpCommand { get; }

        /// <summary>
        /// Gets the command for the drag enter event.
        /// </summary>
        public ICommand DragEnterCommand { get; }

        /// <summary>
        /// Gets the command for the drag over event.
        /// </summary>
        public ICommand DragOverCommand { get; }

        /// <summary>
        /// Gets the command for the drag leave event.
        /// </summary>
        public ICommand DragLeaveCommand { get; }

        /// <summary>
        /// Gets the command for the drop event.
        /// </summary>
        public ICommand DropCommand { get; }

        /// <summary>
        /// Gets the command for the mouse move event.
        /// </summary>
        public ICommand MouseMoveCommand { get; }

        ///// <summary>
        ///// Checks if the other island is on the same axis as this island.
        ///// </summary>
        ///// <param name="otherIsland">The other island.</param>
        ///// <returns>a boolean value indicating if the other island is on the same axis of this one or not.</returns>
        //public bool IsIslandOnSameAxis(IslandViewModel otherIsland) => (otherIsland.Coordinates.X == Coordinates.X || otherIsland.Coordinates.Y == Coordinates.Y) && otherIsland != this;

        /// <summary>
        /// Gets the connection type between two islands.
        /// </summary>
        /// <param name="targetIsland">The island to perform the check on.</param>
        /// <returns>a <see cref="ConnectionTypeEnum"/>.</returns>
        public ConnectionTypeEnum GetConnectionType(IslandViewModel targetIsland)
        {
            if (Coordinates.X == targetIsland.Coordinates.X)
            {
                return ConnectionTypeEnum.Vertical;
            }
            else if (Coordinates.Y == targetIsland.Coordinates.Y)
            {
                return ConnectionTypeEnum.Horizontal;
            }
            else
            {
                return ConnectionTypeEnum.Diagonal;
            }
        }

        /// <summary>
        /// Notifies the bridge connections.
        /// </summary>
        public void NotifyBridgeConnections()
        {
            OnPropertyChanged(nameof(BridgesLeft));
            OnPropertyChanged(nameof(BridgesRight));
            OnPropertyChanged(nameof(BridgesUp));
            OnPropertyChanged(nameof(BridgesDown));
        }

        /// <summary>
        /// Handles the drag enter event during drag-and-drop operation.
        /// </summary>
        /// <param name="e">The <see cref="DragEventArgs"/>.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
        protected virtual void DragEnterCommandExecute(DragEventArgs? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            IslandColor = Brushes.LightGreen;
        }

        /// <summary>
        /// Handles the drag over event during drag-and-drop operation.
        /// </summary>
        /// <param name="e">The <see cref="DragEventArgs"/>.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
        protected virtual void DragOverCommandExecute(DragEventArgs? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
        }

        /// <summary>
        /// Handles the drag leave event during drag-and-drop operation.
        /// </summary>
        /// <param name="e">The <see cref="DragEventArgs"/>.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
        protected virtual void DragLeaveCommandExecute(DragEventArgs? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (!e.Data.GetDataPresent(typeof(IslandViewModel)) ||
                e.Data.GetData(typeof(IslandViewModel)) is not IslandViewModel islandToConnectWith || islandToConnectWith == this)
            {
                return;
            }

            IslandColor = MaxConnectionsReached ? Brushes.Red : Brushes.LightBlue;
        }

        /// <summary>
        /// Handles the drop event during drag-and-drop operation.
        /// </summary>
        /// <param name="e">The <see cref="DragEventArgs"/>.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
        protected virtual void DropCommandExecute(DragEventArgs? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (!e.Data.GetDataPresent(typeof(IslandViewModel)) ||
                e.Data.GetData(typeof(IslandViewModel)) is not IslandViewModel islandToConnectWith || islandToConnectWith == this) return;

            if (IsValidDropTarget(islandToConnectWith))
            {
                WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(new BridgeConnectionInformationContainer(islandToConnectWith, (IslandViewModel)this, BridgeOperationType.Add)));
            }

            WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage(Brushes.LightBlue));
        }

        /// <summary>
        /// Handles the mouse move event during drag-and-drop operation.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/>.</param>
        protected virtual void MouseMoveCommandExecute(MouseEventArgsWithCorrectViewBoxPosition? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.MouseEventArgs is not { LeftButton: MouseButtonState.Pressed }) return;

            // Check if mouse is being dragged
            var currentPosition = e.MouseEventArgs.GetPosition(null);
            if (!(Math.Abs(currentPosition.X - mouseDownPosition.X) > SystemParameters.MinimumHorizontalDragDistance) &&
                !(Math.Abs(currentPosition.Y - mouseDownPosition.Y) >
                  SystemParameters.MinimumVerticalDragDistance)) return;

            if (e.MouseEventArgs.OriginalSource is not DependencyObject depObject)
            {
                return;
            }

            isDragging = true;
            CurrentDragLineX1 = e.DragStartPosition.X;
            CurrentDragLineY1 = e.DragStartPosition.Y;
            CurrentDragLineX2 = e.DragStartPosition.X;
            CurrentDragLineY2 = e.DragStartPosition.Y;
            WeakReferenceMessenger.Default.Send(new CurrentSourceIslandChangedMessage((IslandViewModel)this));

            DragDrop.AddQueryContinueDragHandler(depObject, QueryContinueDragHandler);
            DragDrop.DoDragDrop(depObject, this, DragDropEffects.Link);
            DragDrop.RemoveQueryContinueDragHandler(depObject, QueryContinueDragHandler);
        }

        /// <summary>
        /// Handles the mouse left button down event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/>.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
        protected virtual void MouseLeftButtonDownCommandExecute(MouseButtonEventArgs? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            isDragging = false;
            IslandColor = Brushes.LightGreen;
        }

        /// <summary>
        /// Handles the mouse left button up event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/>.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
        protected virtual void MouseLeftButtonUpCommandExecute(MouseButtonEventArgs? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (!isDragging)
            {
                WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(new BridgeConnectionInformationContainer((IslandViewModel)this, null, BridgeOperationType.RemoveAll)));
            }

            WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage(Brushes.LightBlue));
        }

        /// <summary>
        /// Handles the query continue drag event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void QueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
        {
            var currentPosition = CursorHelper.GetCurrentCursorPosition(((IViewBoxControl)Application.Current.MainWindow).ViewBoxControl);
            CurrentDragLineX2 = currentPosition.X;
            CurrentDragLineY2 = currentPosition.Y;

            if (e.KeyStates != DragDropKeyStates.None) return;
            isDragging = false;
            WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage(Brushes.LightBlue));
            WeakReferenceMessenger.Default.Send(new CurrentSourceIslandChangedMessage(null));
        }

        /// <summary>
        /// Checks if the drop target is valid.
        /// </summary>
        /// <param name="islandToConnectWith">The island to connect with.</param>
        /// <returns>a boolean value if drop target is valid.</returns>
        private bool IsValidDropTarget(IslandViewModel islandToConnectWith) => !MaxConnectionsReached && !islandToConnectWith.MaxConnectionsReached && GetConnectionType(islandToConnectWith) != ConnectionTypeEnum.Diagonal;
    }
}
