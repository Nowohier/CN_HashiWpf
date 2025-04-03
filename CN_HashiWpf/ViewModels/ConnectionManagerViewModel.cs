using CNHashiWpf.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows;

namespace CNHashiWpf.ViewModels
{
    public class ConnectionManagerViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the islands.
        /// </summary>
        public ObservableCollection<ObservableCollection<IslandViewModel>> Islands { get; } = new();

        public void AddConnection(IslandViewModel sourceIsland, IslandViewModel targetIsland)
        {
            if (!IsValidConnection(sourceIsland, targetIsland)) return;
            if (MaxBridgesReachedBetweenSourceAndTarget(sourceIsland, targetIsland))
            {
                RemoveAllConnections(sourceIsland, targetIsland);
                return;
            }

            ManageConnections(sourceIsland, targetIsland, (island, coordinates) => island.AddConnection(coordinates));
            if (AreAllConnectionsSet())
            {
                WeakReferenceMessenger.Default.Send(new AllConnectionsSetMessage());
            }
        }

        public void RemoveConnection(IslandViewModel sourceIsland, IslandViewModel targetIsland)
        {
            ManageConnections(sourceIsland, targetIsland, (island, coordinates) => island.RemoveConnection(coordinates));
        }

        public void RemoveAllConnections(IslandViewModel sourceIsland, IslandViewModel targetIsland)
        {
            if (targetIsland == null)
            {
                foreach (var target in sourceIsland.AllConnections.Distinct().Select(GetIslandByCoordinates).ToList())
                {
                    ManageConnections(sourceIsland, target, (island, coordinates) => island.RemoveAllConnections(coordinates));
                }

                return;
            }

            ManageConnections(sourceIsland, targetIsland, (island, coordinates) => island.RemoveAllConnections(coordinates));
        }

        private bool AreAllConnectionsSet()
        {
            return Islands.All(row => row.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached));
        }

        private void ManageConnections(IslandViewModel sourceIsland, IslandViewModel targetIsland, Action<IslandViewModel, Point> connectionAction)
        {
            var sourceCoordinates = sourceIsland.Coordinates;
            var targetCoordinates = targetIsland.Coordinates;

            var islandsToConnect = GetIslandsBetween(sourceIsland, targetIsland);

            foreach (var island in islandsToConnect)
            {
                if (island.MaxConnections == 0)
                {
                    connectionAction(island, sourceCoordinates);
                    connectionAction(island, targetCoordinates);
                }

                if (island == sourceIsland)
                {
                    connectionAction(island, targetCoordinates);
                }

                if (island == targetIsland)
                {
                    connectionAction(island, sourceCoordinates);
                }
            }
        }

        private IEnumerable<IslandViewModel> GetIslandsBetween(IslandViewModel source, IslandViewModel target)
        {
            var islandsBetween = new List<IslandViewModel>();

            if (source.Coordinates.X == target.Coordinates.X)
            {
                var minY = (int)Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                var maxY = (int)Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                for (var y = minY; y <= maxY; y++)
                {
                    var island = Islands[y][(int)source.Coordinates.X];
                    islandsBetween.Add(island);
                }
            }
            else if (source.Coordinates.Y == target.Coordinates.Y)
            {
                var minX = (int)Math.Min(source.Coordinates.X, target.Coordinates.X);
                var maxX = (int)Math.Max(source.Coordinates.X, target.Coordinates.X);
                for (var x = minX; x <= maxX; x++)
                {
                    var island = Islands[(int)source.Coordinates.Y][x];
                    islandsBetween.Add(island);
                }
            }

            return islandsBetween;
        }

        private bool IsIslandInBetweenSourceAndTarget(IslandViewModel source, IslandViewModel target)
        {
            var sourceCoordinates = source.Coordinates;
            var targetCoordinates = target.Coordinates;

            if (sourceCoordinates.X == targetCoordinates.X)
            {
                var minY = (int)Math.Min(sourceCoordinates.Y, targetCoordinates.Y);
                var maxY = (int)Math.Max(sourceCoordinates.Y, targetCoordinates.Y);
                for (var y = minY + 1; y < maxY; y++)
                {
                    if (Islands[y][(int)sourceCoordinates.X].MaxConnections > 0)
                    {
                        return true;
                    }
                }
            }
            else if (sourceCoordinates.Y == targetCoordinates.Y)
            {
                var minX = (int)Math.Min(sourceCoordinates.X, targetCoordinates.X);
                var maxX = (int)Math.Max(sourceCoordinates.X, targetCoordinates.X);
                for (var x = minX + 1; x < maxX; x++)
                {
                    if (Islands[(int)sourceCoordinates.Y][x].MaxConnections > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool MaxBridgesReachedBetweenSourceAndTarget(IslandViewModel source, IslandViewModel target)
        {
            // Check if source island already has two connections to the target island
            var sourceToTargetConnections = source.AllConnections.Count(c => c == target.Coordinates);

            // Check if target island already has two connections to the source island
            var targetToSourceConnections = target.AllConnections.Count(c => c == source.Coordinates);

            // If either island has two connections to the other, the max bridges are reached
            return sourceToTargetConnections >= 2 || targetToSourceConnections >= 2;
        }

        private IslandViewModel GetIslandByCoordinates(Point coordinates)
        {
            return Islands[(int)coordinates.Y][(int)coordinates.X];
        }

        private bool IsValidConnection(IslandViewModel sourceIsland, IslandViewModel targetIsland)
        {
            // Check if the source and target coordinates are the same -> invalid
            if (sourceIsland == targetIsland)
            {
                return false;
            }

            // Check if the source and target coordinates are not on the same axis -> invalid
            if (!sourceIsland.IsIslandOnSameAxis(targetIsland))
            {
                return false;
            }

            // Check if an island is in between the source and target coordinates -> invalid
            if (IsIslandInBetweenSourceAndTarget(sourceIsland, targetIsland))
            {
                return false;
            }

            // Check if the potential connection already exists -> invalid //ToDO
            //if (DoesConnectionExist(sourceCoordinates, targetCoordinates))
            //{
            //    return false;
            //}

            // Check if the source or target island has reached its maximum connections -> invalid
            if (sourceIsland.MaxConnectionsReached || targetIsland.MaxConnectionsReached)
            {
                return false;
            }

            //// Check if the connections are crossed horizontally -> invalid
            //if (AreConnectionsCrossedHorizontally(sourceCoordinates, targetCoordinates))
            //{
            //    return false;
            //}

            //// Check if the connections are crossed vertically -> invalid
            //if (AreConnectionsCrossedVertically(sourceCoordinates, targetCoordinates))
            //{
            //    return false;
            //}

            return true;
        }
    }
}
