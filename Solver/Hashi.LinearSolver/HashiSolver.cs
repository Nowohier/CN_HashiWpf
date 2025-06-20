using Google.OrTools.Sat;
using Hashi.Enums;
using System.Diagnostics;

namespace Hashi.LinearSolver
{
    public class Island
    {
        public int Id { get; }
        public int Row { get; }
        public int Col { get; }
        public int BridgesRequired { get; }
        public List<int> Neighbors { get; } = new();

        public Island(int id, int row, int col, int bridgesRequired)
        {
            Id = id;
            Row = row;
            Col = col;
            BridgesRequired = bridgesRequired;
        }
    }

    public class Edge
    {
        public int Id;
        public int IslandA;
        public int IslandB;
        public Edge(int id, int a, int b) { Id = id; IslandA = a; IslandB = b; }
    }

    public class HashiSolver
    {
        /// <summary>
        /// Benchmarks the Hashi puzzle solver using the given files. This method iterates through each file, solves the puzzle, and logs the time taken for each solution.
        /// </summary>
        /// <param name="files">A collection of file paths.</param>
        /// <param name="lazy">Use lazy or non lazy solving.</param>
        public static void Benchmark(string[] files, bool lazy = true)
        {
            // ToDO: Fix this
            //var times = new List<double>();

            //foreach (var file in files)
            //{
            //    try
            //    {
            //        var time = lazy ? SolveLazy(file) : Solve(file);
            //        times.Add(time);
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.WriteLine($"Error processing file {file}: {e}");
            //    }
            //}

            //var avgTime = times.Count == 0 ? 0.0 : times.Average();
            //Debug.WriteLine($"Average time: {avgTime}s");
        }

        /// <summary>
        /// Solves the Hashi puzzle from the given file using the Google OR-Tools CP-SAT solver. This method is considerably slower than the
        /// lazy version, as it does not use lazy constraints to iteratively refine the solution.
        /// </summary>
        /// <param name="file">The file to read from</param>
        /// <returns>A double value representing the seconds taken to solve the puzzle.</returns>
        public static double Solve(string file)
        {
            var (islands, intersections) = ReadFile(file);
            var n = islands.Count;

            // Build edge maps
            var edgeMap = new Dictionary<int, Edge>();
            var revEdgeMap = new Dictionary<(int, int), int>();
            var e = 0;
            for (var i = 0; i < n; i++)
            {
                foreach (var j in islands[i].Neighbors)
                {
                    if (i < j)
                    {
                        e++;
                        edgeMap[e] = new Edge(e, i, j);
                        revEdgeMap[(i, j)] = e;
                    }
                }
            }
            var m = edgeMap.Count;

            var model = new CpModel();
            // x[e, l]: x[e,1] (at least 1 bridge), x[e,2] (at least 2 bridges)
            var x = new IntVar[m + 1, 3]; // 1-based, l=1,2
            for (var edgeId = 1; edgeId <= m; edgeId++)
            {
                x[edgeId, 1] = model.NewBoolVar($"x_{edgeId}_1");
                x[edgeId, 2] = model.NewBoolVar($"x_{edgeId}_2");
                // x[e,2] <= x[e,1]
                model.Add(x[edgeId, 2] <= x[edgeId, 1]);
            }

            // 1) Correct bridge count
            for (var i = 0; i < n; i++)
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

            // 2) No intersections
            foreach (var (a, b, c, d) in intersections)
            {
                var e1 = a < b ? revEdgeMap[(a, b)] : revEdgeMap[(b, a)];
                var e2 = c < d ? revEdgeMap[(c, d)] : revEdgeMap[(d, c)];
                model.Add(x[e1, 1] + x[e2, 1] <= 1);
            }

            // 3) Connectivity (polynomial encoding)
            // y[e, l]: y[e, l] == 1 iff edge e can be reached from source edge in <= l-1 steps
            var y = new IntVar[m + 1, m + 1];
            for (var edgeId = 1; edgeId <= m; edgeId++)
            {
                for (var l = 1; l <= m; l++)
                {
                    y[edgeId, l] = model.NewBoolVar($"y_{edgeId}_{l}");
                }
            }

            // Exactly one source edge
            var y1 = new List<IntVar>();
            for (var edgeId = 1; edgeId <= m; edgeId++)
            {
                y1.Add(y[edgeId, 1]);
            }

            model.Add(LinearExpr.Sum(y1) == 1);

            // y[e, l] <= y[e, l+1]
            for (var edgeId = 1; edgeId <= m; edgeId++)
            {
                for (var l = 1; l < m; l++)
                {
                    model.Add(y[edgeId, l] <= y[edgeId, l + 1]);
                }
            }

            // x[e,1] == y[e,m]
            for (var edgeId = 1; edgeId <= m; edgeId++)
            {
                model.Add(x[edgeId, 1] == y[edgeId, m]);
            }

            // Reachability propagation
            for (var edgeId = 1; edgeId <= m; edgeId++)
            {
                var (i1, i2) = (edgeMap[edgeId].IslandA, edgeMap[edgeId].IslandB);
                for (var l = 1; l < m; l++)
                {
                    var neighbors = new List<IntVar>();
                    foreach (var j in islands[i1].Neighbors)
                    {
                        var neighborEdge = i1 < j ? revEdgeMap[(i1, j)] : revEdgeMap[(j, i1)];
                        neighbors.Add(y[neighborEdge, l]);
                    }
                    foreach (var j in islands[i2].Neighbors)
                    {
                        var neighborEdge = i2 < j ? revEdgeMap[(i2, j)] : revEdgeMap[(j, i2)];
                        neighbors.Add(y[neighborEdge, l]);
                    }
                    var sumExpr = LinearExpr.Sum(neighbors) + y[edgeId, l];
                    model.Add(y[edgeId, l + 1] <= sumExpr);
                }
            }

            // Solve
            var solver = new CpSolver();
            var watch = Stopwatch.StartNew();
            var status = solver.Solve(model);
            watch.Stop();

            if (status == CpSolverStatus.Optimal)
            {
                Debug.WriteLine("Solution found:");
                PrettyPrint(islands, x, solver, edgeMap);
            }
            else
            {
                Debug.WriteLine("Problem is unsatisfiable.");
            }

            return Math.Round(watch.Elapsed.TotalSeconds, 3);
        }

        /// <summary>
        /// Converts the given 2D array of integers into a list of islands and a list of intersections.
        /// </summary>
        /// <param name="data">The 2D array.</param>
        /// <returns>A list of islands and a list of intersections.</returns>
        public static (List<Island>, List<(int, int, int, int)>) ConvertData(int[][] data)
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

            return BuildIslandsAndIntersections(grid, rows, columns);
        }

        /// <summary>
        /// Reads a Hashi puzzle from a file and converts it into a list of islands and intersections.
        /// </summary>
        /// <param name="file">The file path.</param>
        /// <returns>A list of islands and a list of intersections.</returns>
        public static (List<Island>, List<(int, int, int, int)>) ReadFile(string file)
        {
            var lines = File.ReadAllLines(file);
            var header = lines[0].Split(' ').Select(int.Parse).ToArray();
            int rows = header[0], columns = header[1]; //, islands = header[2];
            var grid = new int[rows, columns];
            for (var r = 0; r < rows; r++)
            {
                var rowVals = lines[r + 1]
                    .Split([' '], StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                for (var c = 0; c < columns; c++)
                {
                    grid[r, c] = rowVals[c];
                }
            }
            return BuildIslandsAndIntersections(grid, rows, columns);
        }

        /// <summary>
        /// Pretty prints the solution of the Hashi puzzle in a human-readable format. This method constructs a grid representation of the puzzle, displaying islands and bridges based on the solution provided by the solver.
        /// </summary>
        /// <param name="islands">The islands.</param>
        /// <param name="x">x is a 2D array of boolean decision variables representing the number of bridges between islands.</param>
        /// <param name="solver">The <see cref="CpSolver"/> instance.</param>
        /// <param name="edgeMap">A dictionary with <see cref="Edge"/> mapping information.</param>
        public static void PrettyPrint(List<Island> islands, IntVar[,] x, CpSolver solver, Dictionary<int, Edge> edgeMap)
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
                        bridgeChar = numBridges == 2 ? "=" : "-";
                    else
                        bridgeChar = numBridges == 2 ? "‖" : "|";

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

            for (var r = 0; r < 2 * rows + 1; r++)
            {
                for (var c = 0; c < 2 * cols + 1; c++)
                {
                    Debug.Write(grid[r, c]);
                }

                Debug.WriteLine("");
            }
        }

        /// <summary>
        /// Solves the Hashi puzzle using lazy constraints. This method iteratively refines the solution by adding cuts for disconnected components until a connected solution is found or declared infeasible.
        /// </summary>
        /// <param name="data">The 2D array of integer data representing the hashi field.</param>
        /// <returns>The state after trying to resolve the puzzle.</returns>
        public static SolverStatusEnum SolveLazy(int[][] data)
        {
            var (islands, intersections) = ConvertData(data);
            return SolveLazy(islands, intersections);
        }

        /// <summary>
        /// Solves the Hashi puzzle from the given file using lazy constraints. This method iteratively refines the solution by adding cuts for disconnected components until a connected solution is found or declared infeasible.
        /// </summary>
        /// <param name="file">The file path.</param>
        /// <returns>The state after trying to resolve the puzzle.</returns>
        public static SolverStatusEnum SolveLazy(string file)
        {
            var (islands, intersections) = HashiSolver.ReadFile(file);
            return SolveLazy(islands, intersections);
        }

        /// <summary>
        /// Solves the Hashi puzzle using lazy constraints. This method iteratively refines the solution by adding cuts for disconnected components until a connected solution is found or declared infeasible.
        /// </summary>
        /// <param name="islands">A list of Hashi islands.</param>
        /// <param name="intersections">A list of intersections.</param>
        /// <returns></returns>
        public static SolverStatusEnum SolveLazy(List<Island> islands, List<(int, int, int, int)> intersections)
        {
            var n = islands.Count;

            // Build edge maps
            var edgeMap = new Dictionary<int, (int, int)>();
            var revEdgeMap = new Dictionary<(int, int), int>();
            var e = 0;
            for (var i = 0; i < n; i++)
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
            var m = edgeMap.Count;

            var model = new CpModel();
            var x = new IntVar[m + 1, 3]; // 1-based, l=1,2
            for (var edgeId = 1; edgeId <= m; edgeId++)
            {
                x[edgeId, 1] = model.NewBoolVar($"x_{edgeId}_1");
                x[edgeId, 2] = model.NewBoolVar($"x_{edgeId}_2");
                model.Add(x[edgeId, 2] <= x[edgeId, 1]);
            }

            // 1) Correct bridge count
            for (var i = 0; i < n; i++)
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

            // 2) No intersections
            foreach (var (a, b, c, d) in intersections)
            {
                var e1 = a < b ? revEdgeMap[(a, b)] : revEdgeMap[(b, a)];
                var e2 = c < d ? revEdgeMap[(c, d)] : revEdgeMap[(d, c)];
                model.Add(x[e1, 1] + x[e2, 1] <= 1);
            }

            var solver = new CpSolver();
            var watch = Stopwatch.StartNew();

            // Track added cuts to avoid duplicates
            var addedCuts = new HashSet<string>();

            while (true)
            {
                var status = solver.Solve(model);
                if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible)
                {
                    Debug.WriteLine($"Problem is unsatisfiable ({Math.Round(watch.Elapsed.TotalSeconds, 3)}).");
                    watch.Stop();
                    return SolverStatusEnum.Infeasible;
                }

                // Extract solution
                var xSol = new int[m + 1, 3];
                for (var edgeId = 1; edgeId <= m; edgeId++)
                {
                    xSol[edgeId, 1] = (int)solver.Value(x[edgeId, 1]);
                    xSol[edgeId, 2] = (int)solver.Value(x[edgeId, 2]);
                }

                // Check connectivity and get cuts
                var cuts = FindComponentCuts(islands, xSol, edgeMap, revEdgeMap);

                // If only one component, solution is connected
                if (cuts is [{ Count: 0 }])
                {
                    Debug.WriteLine($"Solution found ({Math.Round(watch.Elapsed.TotalSeconds, 3)})");
                    watch.Stop();
                    PrettyPrint(islands, x, solver, edgeMap.Select(kvp => new KeyValuePair<int, Edge>(kvp.Key, new Edge(kvp.Key, kvp.Value.Item1, kvp.Value.Item2))).ToDictionary());
                    return SolverStatusEnum.Optimal;
                }

                // Add a cut for each disconnected component, but only if not already added
                var newCutAdded = false;
                foreach (var cut in cuts)
                {
                    if (cut.Count > 0)
                    {
                        // Use a sorted string as a unique key for the cut
                        var cutKey = string.Join(",", cut.OrderBy(xInt => xInt));
                        if (addedCuts.Add(cutKey))
                        {
                            var cutVars = cut.Select(eid => x[eid, 1]).ToArray();
                            model.Add(LinearExpr.Sum(cutVars) >= 1);
                            newCutAdded = true;
                        }
                    }
                }

                // If no new cuts were added, but still disconnected, declare infeasible (should not happen)
                if (!newCutAdded)
                {
                    Debug.WriteLine("No new cuts could be added, but solution is still disconnected. Declaring infeasible.");
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
        /// <returns>>A list of islands and a list of intersections.</returns>
        private static (List<Island>, List<(int, int, int, int)>) BuildIslandsAndIntersections(
        int[,] grid, int rows, int columns)
        {
            var islandMap = new Dictionary<(int, int), int>();
            var islands = new List<Island>();
            var intersections = new List<(int, int, int, int)>();

            var currId = 0;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    if (grid[r, c] > 0)
                    {
                        islands.Add(new Island(currId, r, c, grid[r, c]));
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
                            intersections.Add((neighIds[0], neighIds[1], neighIds[2], neighIds[3]));
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
        /// <param name="edgeMap"></param>
        /// <param name="revEdgeMap"></param>
        /// <returns>The returned value is a list of integer lists. Each inner list contains the edge IDs that, if at least one is activated (i.e., a bridge is built), would connect that component to the rest of the puzzle.</returns>
        private static List<List<int>> FindComponentCuts(
            List<Island> islands,
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
