using CNHashiGenerator;
using CNHashiWpf.Enums;
using CNHashiWpf.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace CNHashiWpf.ViewModels
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class MainViewModel : BaseViewModel, IRecipient<BridgeConnectionChangedMessage>
    {
        private readonly HashiGenerator hashiGenerator = new();

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register(this);
        }

        /// <summary>
        /// Gets the islands.
        /// </summary>
        public ObservableCollection<ObservableCollection<IslandViewModel>> Islands { get; } = new();

        /// <summary>
        /// Creates a new game.
        /// </summary>
        /// <param name="difficulty">The difficulty setting (0-9).</param>
        public void CreateNewGame(int difficulty)
        {
            var result = hashiGenerator.GenerateHash(difficulty);
            DrawGame(result);
        }

        /// <summary>
        /// Draws the game.
        /// </summary>
        /// <param name="mainArray"></param>
        private void DrawGame(IReadOnlyList<int[]> mainArray)
        {
            Islands.Clear();
            for (var row = 0; row < mainArray.Count; row++)
            {
                var rowCollection = new ObservableCollection<IslandViewModel>();
                for (var column = 0; column < mainArray[0].Length; column++)
                {
                    rowCollection.Add(new IslandViewModel(column, row, mainArray[row][column]));
                }
                Islands.Add(rowCollection);
            }
        }


        public void Receive(BridgeConnectionChangedMessage message)
        {
            var connectionInfos = message.Value;

            if (connectionInfos.IsDiagonalConnection)
            {
                return;
            }

            var sourceIsland = connectionInfos.SourceIsland;
            var targetIsland = connectionInfos.TargetIsland;

            Action bridgeAction = connectionInfos.BridgeOperationType switch
            {
                BridgeOperationType.Add => () => AddConnection(sourceIsland, targetIsland),
                BridgeOperationType.Remove => () => RemoveConnection(sourceIsland, targetIsland),
                _ => throw new ArgumentOutOfRangeException()
            };

            bridgeAction();
        }

        private void AddConnection(IslandViewModel sourceIsland, IslandViewModel targetIsland)
        {
            if (IsValidConnection(sourceIsland, targetIsland))
            {
                var islandsToConnect = GetIslandsBetween(sourceIsland, targetIsland);

                foreach (var island in islandsToConnect)
                {
                    if (island.MaxConnections == 0)
                    {
                        island.AddConnection(sourceIsland.Coordinates);
                        island.AddConnection(targetIsland.Coordinates);
                    }

                    if (island == sourceIsland)
                    {
                        island.AddConnection(targetIsland.Coordinates);
                    }

                    if (island == targetIsland)
                    {
                        island.AddConnection(sourceIsland.Coordinates);
                    }
                }
            }
        }

        private void RemoveConnection(IslandViewModel sourceIsland, IslandViewModel targetIsland)
        {

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

        private bool IsValidConnection(IslandViewModel sourceIsland, IslandViewModel targetIsland)
        {
            var sourceCoordinates = sourceIsland.Coordinates;
            var targetCoordinates = targetIsland.Coordinates;

            // Check if the source and target coordinates are the same -> invalid
            if (sourceCoordinates == targetCoordinates)
            {
                return false;
            }

            // Check if the source and target coordinates are not on the same axis -> invalid
            if (sourceCoordinates.X != targetCoordinates.X && sourceCoordinates.Y != targetCoordinates.Y)
            {
                return false;
            }

            // Check if an island is in between the source and target coordinates -> invalid
            if (IsIslandInBetween(sourceCoordinates, targetCoordinates))
            {
                return false;
            }

            // Check if the potential connection already exists -> invalid //ToDO
            //if (DoesConnectionExist(sourceCoordinates, targetCoordinates))
            //{
            //    return false;
            //}

            // Check if the source island has reached its maximum connections -> invalid
            if (sourceIsland.AllConnections.Count >= sourceIsland.MaxConnections)
            {
                return false;
            }

            // Check if the target island has reached its maximum connections -> invalid
            if (targetIsland.AllConnections.Count >= targetIsland.MaxConnections)
            {
                return false;
            }

            // Check if the connections are crossed horizontally -> invalid
            if (AreConnectionsCrossedHorizontally(sourceCoordinates, targetCoordinates))
            {
                return false;
            }

            // Check if the connections are crossed vertically -> invalid
            if (AreConnectionsCrossedVertically(sourceCoordinates, targetCoordinates))
            {
                return false;
            }

            return true;
        }

        private bool IsIslandInBetween(Point sourceCoordinates, Point targetCoordinates)
        {
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

        private bool DoesConnectionExist(Point sourceCoordinates, Point targetCoordinates)
        {
            var sourceIsland = GetIslandByCoordinates(sourceCoordinates);
            return sourceIsland.AllConnections.Contains(targetCoordinates);
        }

        private IslandViewModel GetIslandByCoordinates(Point coordinates)
        {
            return Islands[(int)coordinates.Y][(int)coordinates.X];
        }

        private bool AreConnectionsCrossedHorizontally(Point sourceCoordinates, Point targetCoordinates)
        {
            var minY = (int)Math.Min(sourceCoordinates.Y, targetCoordinates.Y);
            var maxY = (int)Math.Max(sourceCoordinates.Y, targetCoordinates.Y);

            for (var y = minY; y <= maxY; y++)
            {
                var row = Islands[y];
                foreach (var island in row)
                {
                    foreach (var connection in island.AllConnections)
                    {
                        if (connection.Y == y && connection.X > Math.Min(sourceCoordinates.X, targetCoordinates.X) && connection.X < Math.Max(sourceCoordinates.X, targetCoordinates.X))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool AreConnectionsCrossedVertically(Point sourceCoordinates, Point targetCoordinates)
        {
            var minX = (int)Math.Min(sourceCoordinates.X, targetCoordinates.X);
            var maxX = (int)Math.Max(sourceCoordinates.X, targetCoordinates.X);

            for (var x = minX; x <= maxX; x++)
            {
                foreach (var row in Islands)
                {
                    var island = row[x];
                    foreach (var connection in island.AllConnections)
                    {
                        if (connection.X == x && connection.Y > Math.Min(sourceCoordinates.Y, targetCoordinates.Y) && connection.Y < Math.Max(sourceCoordinates.Y, targetCoordinates.Y))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
