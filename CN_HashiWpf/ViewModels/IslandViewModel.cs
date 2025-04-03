using System.Windows;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace CNHashiWpf.ViewModels
{
    public class IslandViewModel : IslandBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IslandViewModel"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="maxConnections">The max connections of the otherIsland.</param>
        public IslandViewModel(int x, int y, int maxConnections)
        : base(maxConnections, new Point(x, y))
        {

        }

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

        /// <summary>
        /// Removes a connection.
        /// </summary>
        /// <param name="connection">The connection to remove.</param>
        public void RemoveAllConnections(Point connection)
        {
            AllConnections.RemoveAll(x => x == connection);
            NotifyBridgeConnections();
        }
    }
}
