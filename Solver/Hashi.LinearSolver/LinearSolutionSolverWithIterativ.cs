using Google.OrTools.Sat;
using Hashi.Enums;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using System.Diagnostics;

namespace Hashi.LinearSolver;

/// <inheritdoc cref="ILinearSolutionSolverWithIterativ" />
public class LinearSolutionSolverWithIterativ : ILinearSolutionSolverWithIterativ
{
    private readonly Func<IIsland, IIsland, IBridge> bridgeFactory;
    private readonly Func<int, int, int, int, IBridgePair> bridgePairFactory;
    private readonly Func<IList<int>, IList<int>, IHelper> helperFactory;
    private readonly Func<int, int, int, int, IIsland> islandFactory;
    private char[][] fieldChars = { };

    public LinearSolutionSolverWithIterativ(
        Func<IIsland, IIsland, IBridge> bridgeFactory,
        Func<int, int, int, int, IBridgePair> bridgePairFactory,
        Func<int, int, int, int, IIsland> islandFactory,
        Func<IList<int>, IList<int>, IHelper> helperFactory)
    {
        this.bridgeFactory = bridgeFactory;
        this.bridgePairFactory = bridgePairFactory;
        this.islandFactory = islandFactory;
        this.helperFactory = helperFactory;
    }

    /// <inheritdoc />
    public async Task<SolverStatusEnum> SolveAsync(int[][] mainField)
    {
        return await Task.Run(() =>
        {
            var islands = new List<IIsland>();
            var bridges = new List<IBridge>();
            var delta = new List<IBridgePair>();

            var solver = new CpSolver();
            var model = new CpModel();

            // Create all islands (nodes)
            int numberOfNodes = 0;
            for (int row = 0; row < mainField.Length; row++)
                for (int col = 0; col < mainField[row].Length; col++)
                {
                    if (mainField[row][col] == 0) continue;
                    islands.Add(islandFactory.Invoke(mainField[row][col], row, col, numberOfNodes++));
                }

            // Set neighbors for each island
            foreach (var island in islands)
                island.SetAllNeighbors(mainField, islands);

            // Set lower and upper neighbors for each island
            foreach (var island in islands)
            {
                if (island.Up != null) island.LowerNeighbors.Add(island.Up);
                if (island.Left != null) island.LowerNeighbors.Add(island.Left);
                if (island.Right != null) island.UpNeighbors.Add(island.Right);
                if (island.Down != null) island.UpNeighbors.Add(island.Down);
            }

            // Deduplicate bridges using a HashSet
            var bridgeKeys = new HashSet<(int, int, int, int)>();
            foreach (var island in islands)
            {
                foreach (var neighbor in island.LowerNeighbors)
                {
                    var key = (island.X, island.Y, neighbor.X, neighbor.Y);
                    var keyRev = (neighbor.X, neighbor.Y, island.X, island.Y);
                    if (bridgeKeys.Add(key) && bridgeKeys.Add(keyRev))
                        bridges.Add(bridgeFactory.Invoke(island, neighbor));
                }
                foreach (var neighbor in island.UpNeighbors)
                {
                    var key = (island.X, island.Y, neighbor.X, neighbor.Y);
                    var keyRev = (neighbor.X, neighbor.Y, island.X, island.Y);
                    if (bridgeKeys.Add(key) && bridgeKeys.Add(keyRev))
                        bridges.Add(bridgeFactory.Invoke(island, neighbor));
                }
            }

            // Build a dictionary for fast bridge index lookup
            var bridgeIndex = new Dictionary<(int, int), int>(bridges.Count);
            for (int i = 0; i < bridges.Count; i++)
                bridgeIndex[(bridges[i].Island1.Number, bridges[i].Island2.Number)] = i;

            // Find all possible crossing bridge pairs (delta)
            var deltaSet = new HashSet<(int, int, int, int)>();
            foreach (var island in islands)
            {
                if (island.Down != null)
                {
                    var potentialBlockingIslandsDown = island.DownBlocked(islands);
                    foreach (var islandDown in potentialBlockingIslandsDown)
                    {
                        if (islandDown.Right != null)
                        {
                            var pairKey = (island.Number, island.Down.Number, islandDown.Number, islandDown.Right.Number);
                            var pairKeyRev = (islandDown.Number, islandDown.Right.Number, island.Number, island.Down.Number);
                            if (deltaSet.Add(pairKey) && deltaSet.Add(pairKeyRev))
                                delta.Add(bridgePairFactory.Invoke(island.Number, island.Down.Number, islandDown.Number, islandDown.Right.Number));
                        }
                    }
                }
                if (island.Right != null)
                {
                    var potentialBlockingIslandsRight = island.RightBlocked(islands);
                    foreach (var islandRight in potentialBlockingIslandsRight)
                    {
                        if (islandRight.Down != null)
                        {
                            var pairKey = (island.Number, island.Right.Number, islandRight.Number, islandRight.Down.Number);
                            var pairKeyRev = (islandRight.Number, islandRight.Down.Number, island.Number, island.Right.Number);
                            if (deltaSet.Add(pairKey) && deltaSet.Add(pairKeyRev))
                                delta.Add(bridgePairFactory.Invoke(island.Number, island.Right.Number, islandRight.Number, islandRight.Down.Number));
                        }
                    }
                }
            }

            var x = new IntVar[bridges.Count];
            var y = new IntVar[bridges.Count];

            // Each x variable represents the number of bridges between two islands (0, 1, or 2)
            for (int i = 0; i < x.Length; i++)
                x[i] = model.NewIntVar(0, 2, $"{bridges[i].Island1.Y}/{bridges[i].Island1.X},{bridges[i].Island2.Y}/{bridges[i].Island2.X}");

            // Each y variable indicates if a bridge exists (0 or 1)
            for (int i = 0; i < y.Length; i++)
                y[i] = model.NewIntVar(0, 1, $"{bridges[i].Island1.Y}/{bridges[i].Island1.X},{bridges[i].Island2.Y}/{bridges[i].Island2.X}");

            // Add constraints for each island: sum of incoming and outgoing bridges equals island value
            foreach (var island in islands)
            {
                var positionBridgesLow = new List<int>();
                foreach (var islandLow in island.LowerNeighbors)
                {
                    if (bridgeIndex.TryGetValue((islandLow.Number, island.Number), out var idx))
                        positionBridgesLow.Add(idx);
                }
                var positionBridgesUp = new List<int>();
                foreach (var islandUp in island.UpNeighbors)
                {
                    if (bridgeIndex.TryGetValue((island.Number, islandUp.Number), out var idx))
                        positionBridgesUp.Add(idx);
                }
                var lowIntVars = LinearExpr.Sum(positionBridgesLow.Count > 0 ? positionBridgesLow.Select(low => x[low]) : new IntVar[0]);
                var upIntVars = LinearExpr.Sum(positionBridgesUp.Count > 0 ? positionBridgesUp.Select(up => x[up]) : new IntVar[0]);
                model.Add(upIntVars + lowIntVars == island.Value);
            }

            // Add constraints for bridge variables
            for (int i = 0; i < bridges.Count; i++)
            {
                model.Add(y[i] <= x[i]);
                var times = model.NewIntVar(0, 2, string.Empty);
                model.Add(y[i] * 2 == times);
                model.Add(x[i] <= times);
            }

            // Add constraints for crossing bridges (no two crossing bridges at the same time)
            foreach (var bridgePair in delta)
            {
                if (bridgeIndex.TryGetValue((bridgePair.Bridge1[0], bridgePair.Bridge1[1]), out var first) &&
                    bridgeIndex.TryGetValue((bridgePair.Bridge2[0], bridgePair.Bridge2[1]), out var second))
                {
                    model.Add(y[first] + y[second] <= 1);
                }
            }

            // Solve the model
            var status = solver.Solve(model);
            if (!status.Equals(CpSolverStatus.Optimal)) return (SolverStatusEnum)(int)status;

            var solution = solver.Response.Solution;
            var values = new long[x.Length];
            for (int i = 0; i < x.Length; i++) values[i] = solution[i];

            var allComp = FindComponents(bridges, islands.Count, values);

            // Ensure the solution is a single connected component
            while (allComp.Count > 1)
            {
                foreach (var comp in allComp)
                {
                    var constraints = new ILiteral[comp.Bridges.Count];
                    for (int j = 0; j < comp.Bridges.Count; j++)
                    {
                        constraints[j] = model.NewBoolVar($"constraint_{comp.Bridges[j]}");
                        model.Add(x[comp.Bridges[j]] != values[comp.Bridges[j]]).OnlyEnforceIf(constraints[j]);
                    }
                    model.AddBoolOr(constraints);
                }

                status = solver.Solve(model);
                if (!status.Equals(CpSolverStatus.Optimal)) return (SolverStatusEnum)(int)status;

                solution = solver.Response.Solution;
                for (int i = 0; i < x.Length; i++) values[i] = solution[i];
                allComp = FindComponents(bridges, islands.Count, values);
#if DEBUG
                Debug.WriteLine(allComp.Count);
#endif
            }

            // Prepare fieldChars for debug output
            fieldChars = new char[mainField.Length][];
            for (int i = 0; i < fieldChars.Length; i++)
            {
                fieldChars[i] = new char[mainField[0].Length];
                Array.Fill(fieldChars[i], '0');
            }

            foreach (var node in islands)
                fieldChars[node.Y][node.X] = (char)(node.Value + 48);

#if DEBUG
            int iteration = 1;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0) continue;
                Debug.WriteLine(
                    $"{iteration++}. Edge: From node ({bridges[i].Island1.Y}/{bridges[i].Island1.X}) value {bridges[i].Island1.Value} to node ({bridges[i].Island2.Y}/{bridges[i].Island2.X}) value {bridges[i].Island2.Value} with bridge count: {values[i]}.\n");
                PrintBridges(bridges[i], values[i]);
            }
            Debug.WriteLine(string.Join("\n", fieldChars.Select(row => "{" + string.Join(", ", row) + "}")));
#endif

            return (SolverStatusEnum)(int)status;
        });
    }

    /// <summary>
    /// Finds all connected components in the solution graph.
    /// </summary>
    private List<IHelper> FindComponents(List<IBridge> bridges, int amountIslands, long[] value)
    {
        var components = new List<IHelper>();
        var checkedIslands = new bool[amountIslands];
        var graph = new List<IHelper>(amountIslands);
        for (int i = 0; i < amountIslands; i++)
            graph.Add(helperFactory.Invoke(new List<int>(), new List<int>()));

        for (int i = 0; i < bridges.Count; i++)
        {
            if (value[i] == 0) continue;
            graph[bridges[i].Island1.Number].Islands.Add(bridges[i].Island2.Number);
            graph[bridges[i].Island2.Number].Islands.Add(bridges[i].Island1.Number);
            graph[bridges[i].Island1.Number].Bridges.Add(i);
        }

        for (int islandIndex = 0; islandIndex < amountIslands; islandIndex++)
        {
            if (checkedIslands[islandIndex]) continue;
            var component = helperFactory.Invoke(new List<int>(), new List<int>());
            FindConnectedComponents(graph, islandIndex, checkedIslands, component);
            components.Add(component);
        }

        return components;
    }

    /// <summary>
    /// Prints the bridges on the field for debug output.
    /// </summary>
    private void PrintBridges(IBridge bridge, long edgeNumber)
    {
        if (bridge.Island1.X < bridge.Island2.X)
            for (int i = bridge.Island1.X + 1; i < bridge.Island2.X; i++)
                fieldChars[bridge.Island1.Y][i] = edgeNumber switch
                {
                    1 => '-',
                    2 => '=',
                    _ => fieldChars[bridge.Island1.Y][i]
                };
        if (bridge.Island1.X > bridge.Island2.X)
            for (int i = bridge.Island2.X + 1; i < bridge.Island1.X; i++)
                fieldChars[bridge.Island1.Y][i] = edgeNumber switch
                {
                    1 => '-',
                    2 => '=',
                    _ => fieldChars[bridge.Island1.Y][i]
                };
        if (bridge.Island1.Y < bridge.Island2.Y)
            for (int i = bridge.Island1.Y + 1; i < bridge.Island2.Y; i++)
                fieldChars[i][bridge.Island1.X] = edgeNumber switch
                {
                    1 => '|',
                    2 => '"',
                    _ => fieldChars[i][bridge.Island1.X]
                };
        if (bridge.Island1.Y > bridge.Island2.Y)
            for (int i = bridge.Island2.Y + 1; i < bridge.Island1.Y; i++)
                fieldChars[i][bridge.Island1.X] = edgeNumber switch
                {
                    1 => '|',
                    2 => '"',
                    _ => fieldChars[i][bridge.Island1.X]
                };
    }

    /// <summary>
    /// Depth-first search to find all connected components.
    /// </summary>
    private void FindConnectedComponents(IReadOnlyList<IHelper> graph, int islandIndex, IList<bool> checkedIslands, IHelper component)
    {
        checkedIslands[islandIndex] = true;
        component.Islands.Add(islandIndex);
        for (int i = 0; i < graph[islandIndex].Bridges.Count; i++)
            component.Bridges.Add(graph[islandIndex].Bridges[i]);

        for (int i = 0; i < graph[islandIndex].Islands.Count; i++)
        {
            int neighbor = graph[islandIndex].Islands[i];
            if (!checkedIslands[neighbor])
                FindConnectedComponents(graph, neighbor, checkedIslands, component);
        }
    }
}
