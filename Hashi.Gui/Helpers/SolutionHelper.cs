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
        public void GenerateHint(ISolutionContainer solutionContainer, ObservableCollection<ObservableCollection<IIslandViewModel>> islands)
        {
            if (islands.SelectMany(x => x).SelectMany(x => x.AllConnections).Any(x => x.IsHint))
            {
                // A hint is already visible
                return;
            }
            var set = SetSourceCount2Neighbors2TargetsCount2Hint(islands);
            if (set)
            {
                var islandsWithHints = islands.SelectMany(x => x).Where(x => x.AllConnections.Any(y => y.IsHint));
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

        private bool SetSourceCount2Neighbors2TargetsCount2Hint(ObservableCollection<ObservableCollection<IIslandViewModel>> islands)
        {
            var potentialIslands = islands.SelectMany(x => x).Where(x => x is { MaxConnections: 2, MaxConnectionsReached: false });

            foreach (var potentialIsland in potentialIslands)
            {
                var allNeighbors = potentialIsland.GetAllVisibleNeighbors(islands);

                if (allNeighbors.Count != 2)
                {
                    continue;
                }

                var neighbors = allNeighbors.Where(x => x.MaxConnections == 2).ToList();
                if (neighbors.Count == 2 && neighbors.Count(x => !x.MaxConnectionsReached) > 1)
                {
                    // Only two neighbors allowed with max connections = 2 and free connections

                    // Set hint for the potential island
                    foreach (var neighbor in neighbors)
                    {
                        if (potentialIsland.MaxConnectionsReached || neighbor.MaxConnectionsReached) continue;

                        var infoContainer = new BridgeConnectionInformationContainer(BridgeOperationTypeEnum.Add, potentialIsland, neighbor);

                        WeakReferenceMessenger.Default.Send(new BridgeConnectionChangedMessage(infoContainer));
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
