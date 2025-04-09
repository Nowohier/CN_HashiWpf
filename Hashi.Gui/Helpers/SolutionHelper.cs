using CommunityToolkit.Mvvm.Messaging;
using Hashi.Generator.Interfaces.Models;
using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Messages;
using Hashi.Gui.Messages.MessageContainers;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Helpers
{
    /// <inheritdoc cref="ISolutionHelper"/>
    public class SolutionHelper : ISolutionHelper
    {
        /// <inheritdoc />
        public void GenerateHint(ISolutionContainer solutionContainer)
        {
            if (SetSourceCount2Neighbors2TargetsCount2Hint() || SetSourceCount4Neighbors2Hint() ||
                SetSourceCount1Neighbors1WithAvailableConnectionsHint() ||
                SetSourceCount3SourceConnectionsLessThan2Neighbors2WithAvailableConnectionsHint() || SetSourceCount2Neighbors1Hint() || SetSourceCount6SourceIsEdgeHint() || SetSourceCount5SourceIsEdgeHint())
            {
                return;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(IIslandViewModel source, IIslandViewModel target, ObservableCollection<ObservableCollection<IIslandViewModel>> islands)
        {
            var islandsBetween = new List<IIslandViewModel>();
            var connectionType = source.GetConnectionType(target);

            switch (connectionType)
            {
                case ConnectionTypeEnum.Vertical:
                    {
                        var minY = (int)Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                        var maxY = (int)Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                        for (var y = minY; y <= maxY; y++)
                        {
                            var island = islands[y][source.Coordinates.X];
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
                            var island = islands[source.Coordinates.Y][x];
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

        private bool SetSourceCount2Neighbors2TargetsCount2Hint()
        {
            //ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();
            //var potentialIslands = islands.SelectMany(x => x).Where(x => x is { MaxConnections: 2, MaxConnectionsReached: false });

            //foreach (var potentialIsland in potentialIslands)
            //{
            //    var allNeighbors = potentialIsland.GetAllVisibleNeighbors();

            //    if (allNeighbors.Count != 2)
            //    {
            //        continue;
            //    }

            //    var neighbors = allNeighbors.Where(x => x.MaxConnections == 2).ToList();
            //    if (neighbors.Count == 2 && neighbors.Count(x => !x.MaxConnectionsReached) > 1)
            //    {
            //        // Only two neighbors allowed with max connections = 2 and free connections

            //        // Set hint for the potential island
            //        foreach (var neighbor in neighbors)
            //        {
            //            if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached) continue;

            //            var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);

            //            WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));
            //        }

            //        return true;
            //    }
            //}

            return false;
        }

        private bool SetSourceCount2Neighbors1Hint()
        {
            //ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();
            //var potentialIslands = islands.SelectMany(x => x).Where(x => x is { MaxConnections: 2, MaxConnectionsReached: false });

            //foreach (var potentialIsland in potentialIslands)
            //{
            //    var allNeighbors = potentialIsland.GetAllVisibleNeighbors();

            //    if (allNeighbors.Count != 1)
            //    {
            //        continue;
            //    }

            //    var neighbor = allNeighbors.First();
            //    var missingConnections = neighbor.MaxConnections - neighbor.AllConnections.Count;
            //    for (var i = 0; i < missingConnections; i++)
            //    {
            //        if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached) break;

            //        var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);

            //        WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));
            //    }
            //}

            return false;
        }

        private bool SetSourceCount4Neighbors2Hint()
        {
            //ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();
            //var potentialIslands = islands.SelectMany(x => x).Where(x => x is { MaxConnections: 4, MaxConnectionsReached: false });

            //foreach (var potentialIsland in potentialIslands)
            //{
            //    var allNeighbors = potentialIsland.GetAllVisibleNeighbors();

            //    if (allNeighbors.Count != 2)
            //    {
            //        continue;
            //    }

            //    // Only two neighbors allowed with free connections
            //    // Set hint for the potential island
            //    foreach (var neighbor in allNeighbors)
            //    {
            //        if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached) continue;

            //        var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);
            //        WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));

            //        if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached) continue;

            //        infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);
            //        WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));
            //    }

            //    return true;
            //}

            return false;
        }

        private bool SetSourceCount1Neighbors1WithAvailableConnectionsHint()
        {
            //ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();
            //var potentialIslands = islands.SelectMany(x => x).Where(x => x is { MaxConnections: 1, MaxConnectionsReached: false });

            //foreach (var potentialIsland in potentialIslands)
            //{
            //    var allActiveNeighbors = potentialIsland.GetAllVisibleNeighbors().Where(x => !x.MaxConnectionsReached && x.MaxConnections != 1).ToList();

            //    // Only one neighbor allowed with free connections
            //    if (allActiveNeighbors.Count != 1)
            //    {
            //        continue;
            //    }

            //    // Set hint for the potential island
            //    var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, allActiveNeighbors.First());
            //    WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));

            //    return true;
            //}

            return false;
        }

        private bool SetSourceCount3SourceConnectionsLessThan2Neighbors2WithAvailableConnectionsHint()
        {
            ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();
            var potentialIslands = islands.SelectMany(x => x).Where(x => x is { MaxConnections: 3, AllConnections.Count: < 2 });

            foreach (var potentialIsland in potentialIslands)
            {
                var allNeighbors = potentialIsland.GetAllVisibleNeighbors();

                // Only one neighbor allowed with free connections
                if (allNeighbors.Count != 2)
                {
                    continue;
                }

                // Set hint for the potential island
                foreach (var neighbor in allNeighbors.Where(x => !x.MaxConnectionsReached && !x.AllConnections.Any(y => y.Equals(potentialIsland.Coordinates))))
                {
                    if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached) continue;

                    var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);
                    WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));
                }

                return true;
            }

            return false;
        }

        private bool SetSourceCount6SourceIsEdgeHint()
        {
            ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();
            var potentialIslands = islands.SelectMany(x => x).Where(x => IslandInEdge(x, islands) && x is { MaxConnections: 6, MaxConnectionsReached: false });

            foreach (var potentialIsland in potentialIslands)
            {
                var allNeighbors = potentialIsland.GetAllVisibleNeighbors();

                // Only one neighbor allowed with free connections
                if (allNeighbors.Count != 3)
                {
                    continue;
                }

                // Set hint for the potential island
                foreach (var neighbor in allNeighbors)
                {
                    if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached || neighbor.AllConnections.Count(x => potentialIsland.Coordinates.Equals(x)) == 2) continue;

                    var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);
                    WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));

                    if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached || neighbor.AllConnections.Count(x => potentialIsland.Coordinates.Equals(x)) == 2) continue;

                    infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);
                    WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));
                }

                return true;
            }

            return false;
        }

        private bool SetSourceCount5SourceIsEdgeHint()
        {
            ObservableCollection<ObservableCollection<IIslandViewModel>> islands = WeakReferenceMessenger.Default.Send<AllIslandsRequestMessage>();
            var potentialIslands = islands.SelectMany(x => x).Where(x => IslandInEdge(x, islands) && x is { MaxConnections: 5, MaxConnectionsReached: false });

            foreach (var potentialIsland in potentialIslands)
            {
                var allNeighbors = potentialIsland.GetAllVisibleNeighbors();

                // Only one neighbor allowed with free connections
                if (allNeighbors.Count != 3 || allNeighbors.All(x => x.AllConnections.Contains(potentialIsland.Coordinates)))
                {
                    continue;
                }

                // Set hint for the potential island
                foreach (var neighbor in allNeighbors)
                {
                    if (neighbor.AllConnections.Contains(potentialIsland.Coordinates))
                    {
                        continue;
                    }

                    var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);
                    WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));
                }

                return true;
            }

            return false;
        }

        private bool IslandInEdge(IIslandViewModel island, ObservableCollection<ObservableCollection<IIslandViewModel>> islands)
        {
            var x = island.Coordinates.X;
            var y = island.Coordinates.Y;
            return x == 0 || x == islands[y].Count - 1 || y == 0 || y == islands.Count - 1;
        }
    }
}
