using CNHashiWpf.Messages;
using CNHashiWpf.Messages.MessageContainers;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Input;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace CNHashiWpf.ViewModels
{
    public class IslandViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IslandViewModel"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="maxConnections">The max connections of the island.</param>
        public IslandViewModel(int x, int y, int maxConnections)
        {
            Coordinates = new Point(x, y);
            MaxConnections = maxConnections;

            DragEnterCommand = new RelayCommand<DragEventArgs>(DragEnterCommandExecute);
            DropCommand = new RelayCommand<DragEventArgs>(DropCommandExecute);
            DragOverCommand = new RelayCommand<DragEventArgs>(DragOverCommandExecute);
            MouseMoveCommand = new RelayCommand<MouseEventArgs>(MouseMoveCommandExecute);
        }

        public ICommand DragEnterCommand { get; }

        public ICommand DragOverCommand { get; }

        public ICommand DropCommand { get; }

        public ICommand MouseMoveCommand { get; }

        public int MaxConnections { get; }

        public Point Coordinates { get; }

        public List<Point> AllConnections { get; } = new();

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
        /// Adds a connection.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        public void AddConnection(Point connection)
        {
            AllConnections.Add(connection);
            NotifyBridgeConnections();
        }

        /// <summary>
        /// Removes a connection.
        /// </summary>
        /// <param name="connection">The connection to remove.</param>
        public void RemoveConnection(Point connection)
        {
            AllConnections.Remove(connection);
            NotifyBridgeConnections();
        }

        public void DragEnterCommandExecute(DragEventArgs? e)
        {

        }

        public void DragOverCommandExecute(DragEventArgs? e)
        {

        }

        public void DropCommandExecute(DragEventArgs? e)
        {
            if (e == null || !e.Data.GetDataPresent(typeof(IslandViewModel)) ||
                e.Data.GetData(typeof(IslandViewModel)) is not IslandViewModel islandToConnectWith) return;

            if (islandToConnectWith == this) return;

            WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(new BridgeConnectionInformationContainer(islandToConnectWith, this)));
        }

        public void MouseMoveCommandExecute(MouseEventArgs? e)
        {
            if (e is not { LeftButton: MouseButtonState.Pressed }) return;

            DragDrop.DoDragDrop((DependencyObject)e.OriginalSource, this, DragDropEffects.Link);
        }

        private void NotifyBridgeConnections()
        {
            OnPropertyChanged(nameof(BridgesLeft));
            OnPropertyChanged(nameof(BridgesRight));
            OnPropertyChanged(nameof(BridgesUp));
            OnPropertyChanged(nameof(BridgesDown));
        }
    }
}
