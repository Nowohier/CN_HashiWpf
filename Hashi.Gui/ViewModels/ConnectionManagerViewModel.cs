using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Enums;
using Hashi.Gui.Messages;

namespace Hashi.Gui.ViewModels
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class ConnectionManagerViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the islands.
        /// </summary>
        public ObservableCollection<ObservableCollection<IslandViewModel>> Islands { get; } = new();

        public void AddConnection(IslandViewModel? sourceIsland, IslandViewModel? targetIsland)
        {
            if (!IsValidConnection(sourceIsland, targetIsland)) return;
            if (MaxBridgesReachedBetweenSourceAndTarget(sourceIsland, targetIsland) is true)
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

        public void RemoveAllConnections(IslandViewModel? sourceIsland, IslandViewModel? targetIsland)
        {
            if (sourceIsland == null)
            {
                throw new ArgumentNullException(nameof(sourceIsland), "Source island cannot be null.");
            }

            if (targetIsland == null)
            {
                // Clears all source island connections
                foreach (var target in sourceIsland.AllConnections.Distinct().Select(GetIslandByCoordinates).ToList())
                {
                    ManageConnections(sourceIsland, target, (island, coordinates) => island.RemoveAllConnectionsMatchingCoordinates(coordinates));
                }

                return;
            }

            ManageConnections(sourceIsland, targetIsland, (island, coordinates) => island.RemoveAllConnectionsMatchingCoordinates(coordinates));
        }

        public IslandViewModel? GetPotentialTargetIsland(IslandViewModel source, IslandViewModel target)
        {
            var connectionType = source.GetConnectionType(target);

            switch (connectionType)
            {
                case ConnectionTypeEnum.Vertical:
                    {
                        if (target.Coordinates.Y > source.Coordinates.Y)
                        {
                            for (var y = (int)source.Coordinates.Y + 1; y < Islands.Count; y++)
                            {
                                var potentialTarget = Islands[y][(int)source.Coordinates.X];
                                if (IsValidConnection(source, potentialTarget))
                                {
                                    return potentialTarget;
                                }
                            }
                        }

                        if (target.Coordinates.Y < source.Coordinates.Y)
                        {
                            for (var y = (int)source.Coordinates.Y - 1; y >= 0; y--)
                            {
                                var potentialTarget = Islands[y][(int)source.Coordinates.X];
                                if (IsValidConnection(source, potentialTarget))
                                {
                                    return potentialTarget;
                                }
                            }
                        }

                        return null;
                    }
                case ConnectionTypeEnum.Horizontal:
                    {
                        if (target.Coordinates.X > source.Coordinates.X)
                        {
                            for (var x = (int)source.Coordinates.X + 1; x < Islands[(int)source.Coordinates.Y].Count; x++)
                            {
                                var potentialTarget = Islands[(int)source.Coordinates.Y][x];
                                if (IsValidConnection(source, potentialTarget))
                                {
                                    return potentialTarget;
                                }
                            }
                        }

                        if (target.Coordinates.X < source.Coordinates.X)
                        {
                            for (var x = (int)source.Coordinates.X - 1; x >= 0; x--)
                            {
                                var potentialTarget = Islands[(int)source.Coordinates.Y][x];
                                if (IsValidConnection(source, potentialTarget))
                                {
                                    return potentialTarget;
                                }
                            }
                        }

                        return null;
                    }
                case ConnectionTypeEnum.Diagonal:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Highlights the path to the target island.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        public void HighlightPathToTargetIsland(IslandViewModel source, IslandViewModel target)
        {
            var islands = GetAllIslandsInvolvedInConnection(source, target);
            var connectionType = source.GetConnectionType(target);
            foreach (var island in islands)
            {
                if (island.MaxConnections == 0)
                {
                    island.IsHighlightHorizontalLeft = connectionType == ConnectionTypeEnum.Horizontal;
                    island.IsHighlightHorizontalRight = connectionType == ConnectionTypeEnum.Horizontal;
                    island.IsHighlightVerticalTop = connectionType == ConnectionTypeEnum.Vertical;
                    island.IsHighlightVerticalBottom = connectionType == ConnectionTypeEnum.Vertical;
                }

                if (island == source)
                {
                    island.IsHighlightHorizontalLeft = connectionType == ConnectionTypeEnum.Horizontal && target.Coordinates.X < source.Coordinates.X;
                    island.IsHighlightHorizontalRight = connectionType == ConnectionTypeEnum.Horizontal && target.Coordinates.X > source.Coordinates.X;
                    island.IsHighlightVerticalTop = connectionType == ConnectionTypeEnum.Vertical && target.Coordinates.Y < source.Coordinates.Y;
                    island.IsHighlightVerticalBottom = connectionType == ConnectionTypeEnum.Vertical && target.Coordinates.Y > source.Coordinates.Y;
                }

                if (island == target)
                {
                    island.IsHighlightHorizontalLeft = connectionType == ConnectionTypeEnum.Horizontal && source.Coordinates.X < target.Coordinates.X;
                    island.IsHighlightHorizontalRight = connectionType == ConnectionTypeEnum.Horizontal && source.Coordinates.X > target.Coordinates.X;
                    island.IsHighlightVerticalTop = connectionType == ConnectionTypeEnum.Vertical && source.Coordinates.Y < target.Coordinates.Y;
                    island.IsHighlightVerticalBottom = connectionType == ConnectionTypeEnum.Vertical && source.Coordinates.Y > target.Coordinates.Y;
                }
            }
        }

        public void RemoveAllHighlights()
        {
            foreach (var row in Islands)
            {
                foreach (var island in row)
                {
                    island.IsHighlightHorizontalLeft = false;
                    island.IsHighlightHorizontalRight = false;
                    island.IsHighlightVerticalTop = false;
                    island.IsHighlightVerticalBottom = false;
                }
            }
        }

        public void RemoveAllPotentialIslandCoordinates()
        {
            foreach (var row in Islands)
            {
                foreach (var island in row)
                {
                    island.PotentialTargetIslandCoordinates = null;
                }
            }
        }

        public IEnumerable<IslandViewModel> GetAllIslandsInvolvedInConnection(IslandViewModel source, IslandViewModel target)
        {
            var islandsBetween = new List<IslandViewModel>();
            var connectionType = source.GetConnectionType(target);

            switch (connectionType)
            {
                case ConnectionTypeEnum.Vertical:
                    {
                        var minY = (int)Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                        var maxY = (int)Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                        for (var y = minY; y <= maxY; y++)
                        {
                            var island = Islands[y][(int)source.Coordinates.X];
                            islandsBetween.Add(island);
                        }

                        break;
                    }
                case ConnectionTypeEnum.Horizontal:
                    {
                        var minX = (int)Math.Min(source.Coordinates.X, target.Coordinates.X);
                        var maxX = (int)Math.Max(source.Coordinates.X, target.Coordinates.X);
                        for (var x = minX; x <= maxX; x++)
                        {
                            var island = Islands[(int)source.Coordinates.Y][x];
                            islandsBetween.Add(island);
                        }

                        break;
                    }
                case ConnectionTypeEnum.Diagonal:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return islandsBetween;
        }

        /// <summary>
        /// Checks if the drop target is valid.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns>a boolean value if drop target is valid.</returns>
        public bool IsValidDropTarget(IslandViewModel? source, IslandViewModel? target) => source != null && target != null && !source.MaxConnectionsReached && !target.MaxConnectionsReached && source.GetConnectionType(target) != ConnectionTypeEnum.Diagonal;

        private bool AreAllConnectionsSet() => Islands.All(row => row.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached));

        private void ManageConnections(IslandViewModel? source, IslandViewModel? target, Action<IslandViewModel, Point> connectionAction)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Source island cannot be null.");
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "Target island cannot be null.");
            }

            if (source.GetConnectionType(target) == ConnectionTypeEnum.Diagonal)
            {
                throw new InvalidOperationException("Diagonal connections are not allowed.");
            }

            var sourceCoordinates = source.Coordinates;
            var targetCoordinates = target.Coordinates;

            var islandsToConnect = GetAllIslandsInvolvedInConnection(source, target);

            foreach (var island in islandsToConnect)
            {
                if (island.MaxConnections == 0)
                {
                    connectionAction(island, sourceCoordinates);
                    connectionAction(island, targetCoordinates);
                }

                if (island == source)
                {
                    connectionAction(island, targetCoordinates);
                }

                if (island == target)
                {
                    connectionAction(island, sourceCoordinates);
                }
            }
        }

        /// <summary>
        /// Checks if the maximum number of bridges has been reached between the source or target islands.
        /// </summary>
        /// <param name="source">The source Island.</param>
        /// <param name="target">The target island.</param>
        /// <returns>a boolean value indicating if max bridges have been reached.</returns>
        private bool? MaxBridgesReachedBetweenSourceAndTarget(IslandViewModel? source, IslandViewModel? target)
        {
            return source == null || target == null ? null : source.AllConnections.Count(c => c == target.Coordinates) >= 2 || target.AllConnections.Count(c => c == source.Coordinates) >= 2;
        }

        /// <summary>
        /// Gets an island by coordinates.
        /// </summary>
        /// <param name="coordinates">The island coordinates.</param>
        /// <returns>an island.</returns>
        private IslandViewModel GetIslandByCoordinates(Point coordinates)
        {
            return Islands[(int)coordinates.Y][(int)coordinates.X];
        }

        /// <summary>
        /// Checks if the potential connection between the source and target islands is valid.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns></returns>
        private bool IsValidConnection(IslandViewModel? source, IslandViewModel? target)
        {
            // Check if the source and/or target islands are null -> invalid
            if (source == null || target == null)
            {
                return false;
            }

            // Check if the source and target coordinates are the same -> invalid
            if (source == target)
            {
                return false;
            }

            // Check if the source and target coordinates are not on the same axis -> invalid
            if (source.GetConnectionType(target) == ConnectionTypeEnum.Diagonal)
            {
                return false;
            }

            // Check if an island is in between the source and target coordinates -> invalid
            if (IsIslandInBetweenSourceAndTarget(source, target))
            {
                return false;
            }

            // Check if the source or target island has reached its maximum connections -> invalid
            if (source.MaxConnectionsReached || target.MaxConnectionsReached)
            {
                return false;
            }

            // Check if potential connection would collide with existing connections
            if (WouldConnectionCollide(source, target))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if an island with MaxConnections > 0 is in between the source and target coordinates.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns>a boolean value indicating if if an island with MaxConnections > 0 is in between the source and target coordinates.</returns>
        private bool IsIslandInBetweenSourceAndTarget(IslandViewModel source, IslandViewModel target)
        {
            var connectionType = source.GetConnectionType(target);

            switch (connectionType)
            {
                case ConnectionTypeEnum.Vertical:
                    {
                        var minY = (int)Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                        var maxY = (int)Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                        for (var y = minY + 1; y < maxY; y++)
                        {
                            if (Islands[y][(int)source.Coordinates.X].MaxConnections > 0)
                            {
                                return true;
                            }
                        }

                        break;
                    }
                case ConnectionTypeEnum.Horizontal:
                    {
                        var minX = (int)Math.Min(source.Coordinates.X, target.Coordinates.X);
                        var maxX = (int)Math.Max(source.Coordinates.X, target.Coordinates.X);
                        for (var x = minX + 1; x < maxX; x++)
                        {
                            if (Islands[(int)source.Coordinates.Y][x].MaxConnections > 0)
                            {
                                return true;
                            }
                        }

                        break;
                    }
                case ConnectionTypeEnum.Diagonal:
                default:
                    throw new InvalidOperationException("Invalid connection type. Diagonal connections are not allowed here.");
            }

            return false;
        }

        /// <summary>
        /// Checks if the potential connection between the source and target islands would collide with existing connections.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns>a boolean value indicating if the connection would collide.</returns>
        private bool WouldConnectionCollide(IslandViewModel source, IslandViewModel target)
        {
            var connectionType = source.GetConnectionType(target);
            var islands = GetAllIslandsInvolvedInConnection(source, target).Where(x => x.MaxConnections == 0);

            return connectionType switch
            {
                ConnectionTypeEnum.Horizontal => islands.Any(island =>
                    island.BridgesUp.Count > 0 || island.BridgesDown.Count > 0),
                ConnectionTypeEnum.Vertical => islands.Any(island =>
                    island.BridgesLeft.Count > 0 || island.BridgesRight.Count > 0),
                ConnectionTypeEnum.Diagonal => throw new InvalidOperationException("Invalid connection type. Diagonal connections are not allowed here."),
                _ => true
            };
        }
    }
}
