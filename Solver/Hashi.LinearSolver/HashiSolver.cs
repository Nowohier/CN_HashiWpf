using Google.OrTools.Sat;
using Hashi.Enums;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.Logging.Interfaces;
using System.Diagnostics;

namespace Hashi.LinearSolver
{
    /// <summary>
    /// Represents a solver for the Hashiwokakero (Hashi) puzzle using Google OR-Tools CP-SAT solver.
    /// </summary>
    public class HashiSolver : IHashiSolver
    {
        private readonly Func<int, int, int, int, IIsland> islandFactory;
        private readonly Func<int, int, int, IEdge> edgeFactory;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashiSolver"/> class with the specified island and edge factories.
        /// This constructor allows for custom creation of islands and edges, which can be useful for testing or extending functionality.
        /// </summary>
        /// <param name="islandFactory">The island factory.</param>
        /// <param name="edgeFactory">The edge factory.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public HashiSolver(Func<int, int, int, int, IIsland> islandFactory, Func<int, int, int, IEdge> edgeFactory, ILoggerFactory loggerFactory)
        {
            this.islandFactory = islandFactory;
            this.edgeFactory = edgeFactory;
            this.logger = loggerFactory.CreateLogger<HashiSolver>();
        }

        /// <inheritdoc />
        public Task<(List<IIsland>, List<(int, int, int, int)>)> ConvertData(int[][] data)
        {
            var rows = data.Length;
            var columns = data[0].Length;
            var grid = new int[rows, columns];
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    grid[r, c] = data[r][c];
                }
            }

            return Task.FromResult(BuildIslandsAndIntersections(grid, rows, columns));
        }

        /// <inheritdoc />
        public async Task<(List<IIsland>, List<(int, int, int, int)>)> ReadFile(string file)
        {
            var lines = await File.ReadAllLinesAsync(file);
            var header = lines[0].Split(' ').Select(int.Parse).ToArray();
            int rows = header[0], columns = header[1]; //, islands = header[2];
            var grid = new int[rows, columns];
            for (var r = 0; r < rows; r++)
            {
                var rowVals = lines[r + 1]
                    .Split((char[])[' '], StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                for (var c = 0; c < columns; c++)
                {
                    grid[r, c] = rowVals[c];
                }
            }
            return BuildIslandsAndIntersections(grid, rows, columns);
        }

        /// <inheritdoc />
        public Task PrettyPrint(List<IIsland> islands, IntVar[,] x, CpSolver solver, Dictionary<int, IEdge> edgeMap)
        {
            var rows = islands.Max(island => island.Row) + 1;
            var cols = islands.Max(island => island.Col) + 1;
            var grid = new string[2 * rows + 1, 2 * cols + 1];
            for (var r = 0; r < 2 * rows + 1; r++)
            {
                for (var c = 0; c < 2 * cols + 1; c++)
                {
                    grid[r, c] = " ";
                }
            }

            foreach (var island in islands)
                grid[2 * island.Row + 1, 2 * island.Col + 1] = island.BridgesRequired.ToString();

            foreach (var edge in edgeMap.Values)
            {
                var val1 = (int)solver.Value(x[edge.Id, 1]);
                var val2 = (int)solver.Value(x[edge.Id, 2]);
                if (val1 > 0)
                {
                    var numBridges = val1 + val2;
                    var a = islands[edge.IslandA];
                    var b = islands[edge.IslandB];
                    string bridgeChar;
                    if (a.Row == b.Row)
                    {
                        bridgeChar = numBridges == 2 ? "=" : "-";
                    }
                    else
                    {
                        bridgeChar = numBridges == 2 ? "‖" : "|";
                    }

                    if (a.Row == b.Row)
                    {
                        var startC = Math.Min(2 * a.Col + 1, 2 * b.Col + 1);
                        var endC = Math.Max(2 * a.Col + 1, 2 * b.Col + 1);
                        for (var c = startC + 1; c < endC; c++)
                        {
                            grid[2 * a.Row + 1, c] = bridgeChar;
                        }
                    }
                    else
                    {
                        var startR = Math.Min(2 * a.Row + 1, 2 * b.Row + 1);
                        var endR = Math.Max(2 * a.Row + 1, 2 * b.Row + 1);
                        for (var r = startR + 1; r < endR; r++)
                        {
                            grid[r, 2 * a.Col + 1] = bridgeChar;
                        }
                    }
                }
            }

            logger.Debug("Grid visualization:");
            for (var r = 0; r < 2 * rows + 1; r++)
            {
                var lineParts = new string[2 * cols + 1];
                for (var c = 0; c < 2 * cols + 1; c++)
                {
                    lineParts[c] = grid[r, c];
                }
                logger.Debug(string.Join("", lineParts));
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<SolverStatusEnum> SolveLazy(int[][] data, bool prettyPrint = true)
        {
            logger.Info($"Starting solver with {data.Length}x{data[0].Length} grid");
            var (islands, intersections) = await ConvertData(data);
            return await SolveLazy(islands, intersections, prettyPrint);
        }

        /// <inheritdoc />
        public async Task<SolverStatusEnum> SolveLazy(string file, bool prettyPrint = true)
        {
            logger.Info($"Reading puzzle from file: {file}");
            var (islands, intersections) = await ReadFile(file);
            return await SolveLazy(islands, intersections, prettyPrint);
        }

        /// <inheritdoc />
        public async Task<SolverStatusEnum> SolveLazy(List<IIsland> islands, List<(int, int, int, int)> intersections, bool prettyPrint = true)
        {
            var n = islands.Count;
            logger.Info($"Starting solving with {n} islands and {intersections.Count} intersections");

            var (edgeMap, revEdgeMap) = BuildEdgeMaps(islands);
            var m = edgeMap.Count;

            var model = new CpModel();
            var x = CreateBridgeVariables(model, m);

            AddBridgeCountConstraints(model, x, islands, revEdgeMap);
            AddIntersectionConstraints(model, x, intersections, revEdgeMap);

            return await SolveWithLazyCuts(model, x, islands, edgeMap, revEdgeMap, m, prettyPrint);
        }

        private static (Dictionary<int, (int, int)> edgeMap, Dictionary<(int, int), int> revEdgeMap) BuildEdgeMaps(
            List<IIsland> islands)
        {
            var edgeMap = new Dictionary<int, (int, int)>();
            var revEdgeMap = new Dictionary<(int, int), int>();
            var e = 0;
            for (var i = 0; i < islands.Count; i++)
            {
                foreach (var j in islands[i].Neighbors)
                {
                    if (i < j)
                    {
                        e++;
                        edgeMap[e] = (i, j);
                        revEdgeMap[(i, j)] = e;
                    }
                }
            }

            return (edgeMap, revEdgeMap);
        }

        private static IntVar[,] CreateBridgeVariables(CpModel model, int edgeCount)
        {
            var x = new IntVar[edgeCount + 1, 3];
            for (var edgeId = 1; edgeId <= edgeCount; edgeId++)
            {
                x[edgeId, 1] = model.NewBoolVar($"x_{edgeId}_1");
                x[edgeId, 2] = model.NewBoolVar($"x_{edgeId}_2");
                model.Add(x[edgeId, 2] <= x[edgeId, 1]);
            }

            return x;
        }

        private static void AddBridgeCountConstraints(CpModel model, IntVar[,] x, List<IIsland> islands,
            Dictionary<(int, int), int> revEdgeMap)
        {
            for (var i = 0; i < islands.Count; i++)
            {
                var sum = new List<LinearExpr>();
                foreach (var j in islands[i].Neighbors)
                {
                    var edgeId = i < j ? revEdgeMap[(i, j)] : revEdgeMap[(j, i)];
                    sum.Add(x[edgeId, 1]);
                    sum.Add(x[edgeId, 2]);
                }

                model.Add(LinearExpr.Sum(sum) == islands[i].BridgesRequired);
            }
        }

        private static void AddIntersectionConstraints(CpModel model, IntVar[,] x,
            List<(int, int, int, int)> intersections, Dictionary<(int, int), int> revEdgeMap)
        {
            foreach (var (a, b, c, d) in intersections)
            {
                var e1 = a < b ? revEdgeMap[(a, b)] : revEdgeMap[(b, a)];
                var e2 = c < d ? revEdgeMap[(c, d)] : revEdgeMap[(d, c)];
                model.Add(x[e1, 1] + x[e2, 1] <= 1);
            }
        }

        private async Task<SolverStatusEnum> SolveWithLazyCuts(CpModel model, IntVar[,] x, List<IIsland> islands,
            Dictionary<int, (int, int)> edgeMap, Dictionary<(int, int), int> revEdgeMap, int m, bool prettyPrint)
        {
            var solver = new CpSolver();
            var watch = Stopwatch.StartNew();
            var addedCuts = new HashSet<string>();

            while (true)
            {
                var status = solver.Solve(model);
                if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible)
                {
                    watch.Stop();
                    return SolverStatusEnum.Infeasible;
                }

                var xSol = new int[m + 1, 3];
                for (var edgeId = 1; edgeId <= m; edgeId++)
                {
                    xSol[edgeId, 1] = (int)solver.Value(x[edgeId, 1]);
                    xSol[edgeId, 2] = (int)solver.Value(x[edgeId, 2]);
                }

                var cuts = FindComponentCuts(islands, xSol, edgeMap, revEdgeMap);

                if (cuts is [{ Count: 0 }])
                {
                    logger.Info($"Solution found ({Math.Round(watch.Elapsed.TotalSeconds, 3)} seconds)");
                    watch.Stop();
                    if (prettyPrint)
                    {
                        await PrettyPrint(islands, x, solver,
                            edgeMap.Select(kvp =>
                                    new KeyValuePair<int, IEdge>(kvp.Key,
                                        edgeFactory.Invoke(kvp.Key, kvp.Value.Item1, kvp.Value.Item2)))
                                .ToDictionary());
                    }

                    return SolverStatusEnum.Optimal;
                }

                var newCutAdded = false;
                foreach (var cut in cuts)
                {
                    if (cut.Count > 0)
                    {
                        var cutKey = string.Join(",", cut.OrderBy(xInt => xInt));
                        if (addedCuts.Add(cutKey))
                        {
                            var cutVars = cut.Select(eid => x[eid, 1]).ToArray();
                            model.Add(LinearExpr.Sum(cutVars) >= 1);
                            newCutAdded = true;
                        }
                    }
                }

                if (!newCutAdded)
                {
                    logger.Warn(
                        "No new cuts could be added, but solution is still disconnected. Declaring infeasible.");
                    watch.Stop();
                    return SolverStatusEnum.Infeasible;
                }
            }
        }

        /// <summary>
        /// Builds the islands and intersections from the given grid.
        /// </summary>
        /// <param name="grid">The integer grid.</param>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <returns>A list of islands and a list of intersections.</returns>
        private (List<IIsland>, List<(int, int, int, int)>) BuildIslandsAndIntersections(
        int[,] grid, int rows, int columns)
        {
            var islandMap = new Dictionary<(int, int), int>();
            var islands = new List<IIsland>();
            var intersections = new List<(int, int, int, int)>();

            var currId = 0;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    if (grid[r, c] > 0)
                    {
                        islands.Add(islandFactory.Invoke(currId, r, c, grid[r, c]));
                        islandMap[(r, c)] = currId;
                        currId++;
                    }
                }
            }

            // Find neighbors
            for (var idx = 0; idx < islands.Count; idx++)
            {
                var island = islands[idx];
                foreach (var (dr, dc) in new[] { (0, -1), (0, 1), (1, 0), (-1, 0) })
                {
                    int r = island.Row + dr, c = island.Col + dc;
                    while (r >= 0 && r < rows && c >= 0 && c < columns)
                    {
                        if (islandMap.TryGetValue((r, c), out var neighborId))
                        {
                            island.Neighbors.Add(neighborId);
                            break;
                        }
                        r += dr; c += dc;
                    }
                }
            }

            // Find intersections
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    if (!islandMap.ContainsKey((r, c)))
                    {
                        var neighIds = new List<int>();
                        foreach (var (dr, dc) in new[] { (0, -1), (0, 1), (1, 0), (-1, 0) })
                        {
                            int rr = r + dr, cc = c + dc;
                            while (rr >= 0 && rr < rows && cc >= 0 && cc < columns)
                            {
                                if (islandMap.TryGetValue((rr, cc), out var neighborId))
                                {
                                    neighIds.Add(neighborId);
                                    break;
                                }
                                rr += dr; cc += dc;
                            }
                        }
                        if (neighIds.Count == 4)
                        {
                            intersections.Add((neighIds[0], neighIds[1], neighIds[2], neighIds[3]));
                        }
                    }
                }
            }

            return (islands, intersections);
        }

        /// <summary>
        /// Finds the component cuts in the solution.
        /// </summary>
        /// <param name="islands">The islands.</param>
        /// <param name="xSol">xSol is a 2D array holding the current integer solution for bridge variables, extracted from the solver, and is used for connectivity checks and cut generation in the lazy constraint loop.</param>
        /// <param name="edgeMap">Maps edge IDs to their endpoint island index pairs (islandA, islandB).</param>
        /// <param name="revEdgeMap">Maps island index pairs to their edge ID for reverse lookup.</param>
        /// <returns>The returned value is a list of integer lists. Each inner list contains the edge IDs that, if at least one is activated (i.e., a bridge is built), would connect that component to the rest of the puzzle.</returns>
        private List<List<int>> FindComponentCuts(
            List<IIsland> islands,
            int[,] xSol,
            Dictionary<int, (int, int)> edgeMap,
            Dictionary<(int, int), int> revEdgeMap)
        {
            var n = islands.Count;
            var m = edgeMap.Count;

            // Build adjacency list from solution
            var adj = new Dictionary<int, List<int>>();
            for (var i = 0; i < n; i++)
            {
                adj[i] = new List<int>();
            }

            for (var e = 1; e <= m; e++)
            {
                if (xSol[e, 1] > 0)
                {
                    var (i, j) = edgeMap[e];
                    adj[i].Add(j);
                    adj[j].Add(i);
                }
            }

            // Find all connected components
            var allVisited = new HashSet<int>();
            var componentCuts = new List<List<int>>();

            while (allVisited.Count < n)
            {
                // Start new component from first unvisited island
                var start = Enumerable.Range(0, n).First(i => !allVisited.Contains(i));
                var component = new HashSet<int>();
                var queue = new Queue<int>();
                queue.Enqueue(start);
                component.Add(start);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    foreach (var neighbor in adj[current])
                    {
                        if (!component.Contains(neighbor))
                        {
                            component.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }

                // Find cut edges for this component
                var cutEdges = new List<int>();
                foreach (var i in component)
                {
                    foreach (var j in islands[i].Neighbors)
                    {
                        if (!component.Contains(j))
                        {
                            var edgeId = i < j ? revEdgeMap[(i, j)] : revEdgeMap[(j, i)];
                            cutEdges.Add(edgeId);
                        }
                    }
                }

                componentCuts.Add(cutEdges);
                foreach (var v in component)
                    allVisited.Add(v);
            }

            return componentCuts;
        }
    }
}
