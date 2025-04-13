namespace Hashi.Rules.Extensions
{
    public static class RuleExtensions
    {
        ///// <summary>
        /////   Gets the connectable neighbors of the source island that do not have a connection set to the source island.
        ///// </summary>
        ///// <param name="source">The source island.</param>
        ///// <param name="allNeighbors">The visible neighbor islands.</param>
        ///// <returns>connectable neighbors of the source island that do not have a connection set to the source island.</returns>
        //public static List<IIslandViewModel> GetConnectableNeighborsWithoutConnection(this IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
        //{
        //    return allNeighbors.Where(x => !x.MaxConnectionsReached && !x.AllConnections.Any(y => y.Equals(source.Coordinates))).ToList();
        //}

        ///// <summary>
        /////   Checks if all islands are connected to the source island.
        ///// </summary>
        ///// <param name="source">The source island.</param>
        ///// <param name="allNeighbors">The visible neighbor islands.</param>
        ///// <returns>a boolean value indicating if all islands are connected to the source island.</returns>
        //public static bool AreAllNeighborsConnected(this IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
        //{
        //    return allNeighbors.All(x => x.AllConnections.Contains(source.Coordinates));
        //}

        ///// <summary>
        /////   Gets the islands connected to the source island.
        ///// </summary>
        ///// <param name="source">The source island.</param>
        ///// <param name="allNeighbors">The visible neighbor islands.</param>
        ///// <param name="amountConnections">(optional) The amount of connections per neighbor to the source island.</param>
        ///// <returns>the islands connected by one connection to the source island.</returns>
        //public static List<IIslandViewModel> GetConnectedNeighbors(this IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
        //{
        //    return amountConnections == null
        //        ? allNeighbors.Where(x => x.AllConnections.Any(y => y.Equals(source.Coordinates))).ToList()
        //        : allNeighbors
        //            .Where(x => x.AllConnections.Count(y => y.Equals(source.Coordinates)) == (int)amountConnections)
        //            .ToList();
        //}

        ///// <summary>
        /////  Gets the amount of connections to the source island from the neighbors.
        ///// </summary>
        ///// <param name="source">The source island.</param>
        ///// <param name="neighbors">The visible neighbor islands.</param>
        ///// <returns></returns>
        //public static int CountConnectionsToNeighbors(this IIslandViewModel source, IEnumerable<IIslandViewModel> neighbors)
        //{
        //    return neighbors.Sum(x => x.AllConnections.Count(y => y.Equals(source.Coordinates)));
        //}

        ///// <summary>
        ///// Checks if the remaining connections of the island are within the range of the two values.
        ///// </summary>
        ///// <param name="source">The source island.</param>
        ///// <param name="minValue">The first value.</param>
        ///// <param name="maxValue">The second value.</param>
        ///// <returns></returns>
        //public static bool AreRemainingConnectionsWithinRange(this IIslandViewModel source, int minValue, int maxValue)
        //{
        //    return source.RemainingConnections >= minValue && source.RemainingConnections <= maxValue;
        //}

        ///// <summary>
        /////   Gets the islands connected to the source island which have reached the maximum connections.
        ///// </summary>
        ///// <param name="source">The source island.</param>
        ///// <param name="allNeighbors">The visible neighbor islands.</param>
        ///// <param name="amountConnections">The amount of connections per neighbor to the source island.</param>
        ///// <returns>the islands connected to the source island which have reached the maximum connections.</returns>
        //public static List<IIslandViewModel> GetMaxedOutConnectedNeighbors(this IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
        //{
        //    return GetConnectedNeighbors(source, allNeighbors, amountConnections).Where(x => x.MaxConnectionsReached).ToList();
        //}
    }
}
