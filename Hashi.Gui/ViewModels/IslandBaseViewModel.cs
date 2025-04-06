using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Enums;
using Hashi.Gui.EventArgs;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces;
using Hashi.Gui.Messages;
using Hashi.Gui.Messages.MessageContainers;

namespace Hashi.Gui.ViewModels
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public abstract class IslandBaseViewModel : BaseViewModel, IIslandBaseViewModel
    {
        private Point mouseDownPosition;
        private bool isDragging;
        private Brush islandColor = Brushes.LightBlue;
        private bool isHighlightHorizontalLeft;
        private bool isHighlightHorizontalRight;
        private bool isHighlightVerticalTop;
        private bool isHighlightVerticalBottom;
        private int count;
        private Point? potentialTargetIslandCoordinates;

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

        public Point? PotentialTargetIslandCoordinates
        {
            get => potentialTargetIslandCoordinates;
            set => Set(ref potentialTargetIslandCoordinates, value);
        }

        /// <inheritdoc/>
        public List<Point> AllConnections { get; } = new();

        /// <inheritdoc/>
        public bool MaxConnectionsReached => AllConnections.Count >= MaxConnections;

        /// <inheritdoc/>
        public List<Point> BridgesLeft => AllConnections.Where(x => x.X < Coordinates.X && x.Y == Coordinates.Y).ToList();

        /// <inheritdoc/>
        public List<Point> BridgesRight => AllConnections.Where(x => x.X > Coordinates.X && x.Y == Coordinates.Y).ToList();

        /// <inheritdoc/>
        public List<Point> BridgesUp => AllConnections.Where(x => x.X == Coordinates.X && x.Y < Coordinates.Y).ToList();

        /// <inheritdoc/>
        public List<Point> BridgesDown => AllConnections.Where(x => x.X == Coordinates.X && x.Y > Coordinates.Y).ToList();

        /// <inheritdoc/>
        public Brush IslandColor
        {
            get => islandColor;
            set => Set(ref islandColor, value);
        }

        /// <inheritdoc/>
        public bool IsHighlightHorizontalLeft
        {
            get => isHighlightHorizontalLeft;
            set => Set(ref isHighlightHorizontalLeft, value);
        }

        /// <inheritdoc/>
        public bool IsHighlightHorizontalRight
        {
            get => isHighlightHorizontalRight;
            set => Set(ref isHighlightHorizontalRight, value);
        }

        /// <inheritdoc/>
        public bool IsHighlightVerticalTop
        {
            get => isHighlightVerticalTop;
            set => Set(ref isHighlightVerticalTop, value);
        }

        /// <inheritdoc/>
        public bool IsHighlightVerticalBottom
        {
            get => isHighlightVerticalBottom;
            set => Set(ref isHighlightVerticalBottom, value);
        }

        /// <inheritdoc/>
        public int MaxConnections { get; }

        /// <inheritdoc/>
        public Point Coordinates { get; }

        /// <inheritdoc/>
        public ICommand MouseLeftButtonDownCommand { get; }

        /// <inheritdoc/>
        public ICommand MouseLeftButtonUpCommand { get; }

        /// <inheritdoc/>
        public ICommand DragEnterCommand { get; }

        /// <inheritdoc/>
        public ICommand DragOverCommand { get; }

        /// <inheritdoc/>
        public ICommand DragLeaveCommand { get; }

        /// <inheritdoc/>
        public ICommand DropCommand { get; }

        /// <inheritdoc/>
        public ICommand MouseMoveCommand { get; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

            IslandColor = HashiColors.GreenIslandBrush;
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

            IslandColor = MaxConnectionsReached ? HashiColors.MaxBridgesReachedBrush : HashiColors.BasicIslandBrush;
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

            WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage(HashiColors.BasicIslandBrush));
        }

        /// <summary>
        /// Handles the mouse move event during drag-and-drop operation.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/>.</param>
        protected virtual void MouseMoveCommandExecute(MouseEventArgsWithCorrectViewBoxPosition? e)
        {
            if (e is not { MouseEventArgs.LeftButton: MouseButtonState.Pressed }) return;

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
            isDragging = false;
            IslandColor = HashiColors.GreenIslandBrush;
        }

        /// <summary>
        /// Handles the mouse left button up event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/>.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if e is null.</exception>
        protected virtual void MouseLeftButtonUpCommandExecute(MouseButtonEventArgs? e)
        {
            if (!isDragging)
            {
                WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(new BridgeConnectionInformationContainer((IslandViewModel)this, BridgeOperationType.RemoveAll)));
            }

            WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage(HashiColors.BasicIslandBrush));
        }

        /// <summary>
        /// Handles the query continue drag event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void QueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
        {
            var window = Application.Current.MainWindow as IViewBoxControl;
            var viewBox = window?.ViewBoxControl;

            if (viewBox == null)
            {
                throw new InvalidOperationException("ViewBoxControl is not available.");
            }

            var currentPosition = CursorHelper.GetCurrentCursorPosition(viewBox);

            if (VisualTreeHelper.HitTest(viewBox, currentPosition)?.VisualHit is FrameworkElement element)
            {
                if (element.DataContext != this && element.DataContext is IslandViewModel potentialIsland && potentialIsland.Coordinates != PotentialTargetIslandCoordinates)
                {
                    Debug.WriteLine(count++);
                    PotentialTargetIslandCoordinates = potentialIsland.Coordinates;
                    WeakReferenceMessenger.Default.Send(new PotentialTargetIslandChangedMessage(potentialIsland));
                }
            }

            if (e.KeyStates != DragDropKeyStates.None) return;

            isDragging = false;
            WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(new BridgeConnectionInformationContainer((IslandViewModel)this, BridgeOperationType.Add)));
            WeakReferenceMessenger.Default.Send(new PotentialTargetIslandChangedMessage(null));
            WeakReferenceMessenger.Default.Send(new CurrentSourceIslandChangedMessage(null));
            WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage(Brushes.LightBlue));
        }
    }
}
