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
        public static void Benchmark(string[] files, bool lazy = true)
        {
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

        public static double Solve(string file)
        {
            var (islands, intersections) = ReadFile(file);
            int n = islands.Count;

            // Build edge maps
            var edgeMap = new Dictionary<int, Edge>();
            var revEdgeMap = new Dictionary<(int, int), int>();
            int e = 0;
            for (int i = 0; i < n; i++)
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
            int m = edgeMap.Count;

            var model = new CpModel();
            // x[e, l]: x[e,1] (at least 1 bridge), x[e,2] (at least 2 bridges)
            var x = new IntVar[m + 1, 3]; // 1-based, l=1,2
            for (int edgeId = 1; edgeId <= m; edgeId++)
            {
                x[edgeId, 1] = model.NewBoolVar($"x_{edgeId}_1");
                x[edgeId, 2] = model.NewBoolVar($"x_{edgeId}_2");
                // x[e,2] <= x[e,1]
                model.Add(x[edgeId, 2] <= x[edgeId, 1]);
            }

            // 1) Correct bridge count
            for (int i = 0; i < n; i++)
            {
                var sum = new List<LinearExpr>();
                foreach (var j in islands[i].Neighbors)
                {
                    int edgeId = i < j ? revEdgeMap[(i, j)] : revEdgeMap[(j, i)];
                    sum.Add(x[edgeId, 1]);
                    sum.Add(x[edgeId, 2]);
                }
                model.Add(LinearExpr.Sum(sum) == islands[i].BridgesRequired);
            }

            // 2) No intersections
            foreach (var (a, b, c, d) in intersections)
            {
                int e1 = a < b ? revEdgeMap[(a, b)] : revEdgeMap[(b, a)];
                int e2 = c < d ? revEdgeMap[(c, d)] : revEdgeMap[(d, c)];
                model.Add(x[e1, 1] + x[e2, 1] <= 1);
            }

            // 3) Connectivity (polynomial encoding)
            // y[e, l]: y[e, l] == 1 iff edge e can be reached from source edge in <= l-1 steps
            var y = new IntVar[m + 1, m + 1];
            for (int edgeId = 1; edgeId <= m; edgeId++)
                for (int l = 1; l <= m; l++)
                    y[edgeId, l] = model.NewBoolVar($"y_{edgeId}_{l}");

            // Exactly one source edge
            var y1 = new List<IntVar>();
            for (int edgeId = 1; edgeId <= m; edgeId++)
                y1.Add(y[edgeId, 1]);
            model.Add(LinearExpr.Sum(y1) == 1);

            // y[e, l] <= y[e, l+1]
            for (int edgeId = 1; edgeId <= m; edgeId++)
                for (int l = 1; l < m; l++)
                    model.Add(y[edgeId, l] <= y[edgeId, l + 1]);

            // x[e,1] == y[e,m]
            for (int edgeId = 1; edgeId <= m; edgeId++)
                model.Add(x[edgeId, 1] == y[edgeId, m]);

            // Reachability propagation
            for (int edgeId = 1; edgeId <= m; edgeId++)
            {
                var (i1, i2) = (edgeMap[edgeId].IslandA, edgeMap[edgeId].IslandB);
                for (int l = 1; l < m; l++)
                {
                    var neighbors = new List<IntVar>();
                    foreach (var j in islands[i1].Neighbors)
                    {
                        int neighborEdge = i1 < j ? revEdgeMap[(i1, j)] : revEdgeMap[(j, i1)];
                        neighbors.Add(y[neighborEdge, l]);
                    }
                    foreach (var j in islands[i2].Neighbors)
                    {
                        int neighborEdge = i2 < j ? revEdgeMap[(i2, j)] : revEdgeMap[(j, i2)];
                        neighbors.Add(y[neighborEdge, l]);
                    }
                    var sumExpr = LinearExpr.Sum(neighbors) + y[edgeId, l];
                    model.Add(y[edgeId, l + 1] <= sumExpr);
                }
            }

            // Solve
            var solver = new CpSolver();
            var watch = System.Diagnostics.Stopwatch.StartNew();
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

        public static (List<Island>, List<(int, int, int, int)>) ConvertData(int[][] data)
        {
            var rows = data.Length;
            var columns = data[0].Length;

            var grid = new int[rows, columns];
            var islandMap = new Dictionary<(int, int), int>();
            var islands = new List<Island>();
            var intersections = new List<(int, int, int, int)>();

            var currId = 0;
            for (var r = 0; r < rows; r++)
            {
                var rowVals = data[r];

                for (var c = 0; c < columns; c++)
                {
                    grid[r, c] = rowVals[c];
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

        public static (List<Island>, List<(int, int, int, int)>) ReadFile(string file)
        {
            var lines = File.ReadAllLines(file);
            var header = lines[0].Split(' ').Select(int.Parse).ToArray();
            int nrows = header[0], ncols = header[1], nislands = header[2];

            var grid = new int[nrows, ncols];
            var islandMap = new Dictionary<(int, int), int>();
            var islands = new List<Island>();
            var intersections = new List<(int, int, int, int)>();

            int currId = 0;
            for (int r = 0; r < nrows; r++)
            {
                //var rowVals = lines[r + 1].Split(' ').Select(int.Parse).ToArray();
                var rowVals = lines[r + 1]
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();

                for (int c = 0; c < ncols; c++)
                {
                    grid[r, c] = rowVals[c];
                    if (grid[r, c] > 0)
                    {
                        islands.Add(new Island(currId, r, c, grid[r, c]));
                        islandMap[(r, c)] = currId;
                        currId++;
                    }
                }
            }

            // Find neighbors
            for (int idx = 0; idx < islands.Count; idx++)
            {
                var island = islands[idx];
                foreach (var (dr, dc) in new[] { (0, -1), (0, 1), (1, 0), (-1, 0) })
                {
                    int r = island.Row + dr, c = island.Col + dc;
                    while (r >= 0 && r < nrows && c >= 0 && c < ncols)
                    {
                        if (islandMap.TryGetValue((r, c), out int neighborId))
                        {
                            island.Neighbors.Add(neighborId);
                            break;
                        }
                        r += dr; c += dc;
                    }
                }
            }

            // Find intersections
            for (int r = 0; r < nrows; r++)
            {
                for (int c = 0; c < ncols; c++)
                {
                    if (!islandMap.ContainsKey((r, c)))
                    {
                        var neighIds = new List<int>();
                        foreach (var (dr, dc) in new[] { (0, -1), (0, 1), (1, 0), (-1, 0) })
                        {
                            int rr = r + dr, cc = c + dc;
                            while (rr >= 0 && rr < nrows && cc >= 0 && cc < ncols)
                            {
                                if (islandMap.TryGetValue((rr, cc), out int neighborId))
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

        public static void PrettyPrint(List<Island> islands, IntVar[,] x, CpSolver solver, Dictionary<int, Edge> edgeMap)
        {
            int rows = islands.Max(island => island.Row) + 1;
            int cols = islands.Max(island => island.Col) + 1;
            var grid = new string[2 * rows + 1, 2 * cols + 1];
            for (int r = 0; r < 2 * rows + 1; r++)
                for (int c = 0; c < 2 * cols + 1; c++)
                    grid[r, c] = " ";

            foreach (var island in islands)
                grid[2 * island.Row + 1, 2 * island.Col + 1] = island.BridgesRequired.ToString();

            foreach (var edge in edgeMap.Values)
            {
                int val1 = (int)solver.Value(x[edge.Id, 1]);
                int val2 = (int)solver.Value(x[edge.Id, 2]);
                if (val1 > 0)
                {
                    int numBridges = val1 + val2;
                    var a = islands[edge.IslandA];
                    var b = islands[edge.IslandB];
                    string bridgeChar;
                    if (a.Row == b.Row)
                        bridgeChar = numBridges == 2 ? "=" : "-";
                    else
                        bridgeChar = numBridges == 2 ? "‖" : "|";

                    if (a.Row == b.Row)
                    {
                        int startC = Math.Min(2 * a.Col + 1, 2 * b.Col + 1);
                        int endC = Math.Max(2 * a.Col + 1, 2 * b.Col + 1);
                        for (int c = startC + 1; c < endC; c++)
                            grid[2 * a.Row + 1, c] = bridgeChar;
                    }
                    else
                    {
                        int startR = Math.Min(2 * a.Row + 1, 2 * b.Row + 1);
                        int endR = Math.Max(2 * a.Row + 1, 2 * b.Row + 1);
                        for (int r = startR + 1; r < endR; r++)
                            grid[r, 2 * a.Col + 1] = bridgeChar;
                    }
                }
            }

            for (int r = 0; r < 2 * rows + 1; r++)
            {
                for (int c = 0; c < 2 * cols + 1; c++)
                    Debug.Write(grid[r, c]);
                Debug.WriteLine("");
            }
        }

        public static SolverStatusEnum SolveLazy(string file)
        {
            var (islands, intersections) = HashiSolver.ReadFile(file);
            return SolveLazy(islands, intersections);
        }

        public static SolverStatusEnum SolveLazy(List<Island> islands, List<(int, int, int, int)> intersections)
        {
            int n = islands.Count;

            // Build edge maps
            var edgeMap = new Dictionary<int, (int, int)>();
            var revEdgeMap = new Dictionary<(int, int), int>();
            int e = 0;
            for (int i = 0; i < n; i++)
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
            int m = edgeMap.Count;

            var model = new CpModel();
            // x[e, l]: x[e,1] (at least 1 bridge), x[e,2] (at least 2 bridges)
            var x = new IntVar[m + 1, 3]; // 1-based, l=1,2
            for (int edgeId = 1; edgeId <= m; edgeId++)
            {
                x[edgeId, 1] = model.NewBoolVar($"x_{edgeId}_1");
                x[edgeId, 2] = model.NewBoolVar($"x_{edgeId}_2");
                model.Add(x[edgeId, 2] <= x[edgeId, 1]);
            }

            // 1) Correct bridge count
            for (int i = 0; i < n; i++)
            {
                var sum = new List<LinearExpr>();
                foreach (var j in islands[i].Neighbors)
                {
                    int edgeId = i < j ? revEdgeMap[(i, j)] : revEdgeMap[(j, i)];
                    sum.Add(x[edgeId, 1]);
                    sum.Add(x[edgeId, 2]);
                }
                model.Add(LinearExpr.Sum(sum) == islands[i].BridgesRequired);
            }

            // 2) No intersections
            foreach (var (a, b, c, d) in intersections)
            {
                int e1 = a < b ? revEdgeMap[(a, b)] : revEdgeMap[(b, a)];
                int e2 = c < d ? revEdgeMap[(c, d)] : revEdgeMap[(d, c)];
                model.Add(x[e1, 1] + x[e2, 1] <= 1);
            }

            var solver = new CpSolver();
            var watch = Stopwatch.StartNew();

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
                var xsol = new int[m + 1, 3];
                for (int edgeId = 1; edgeId <= m; edgeId++)
                {
                    xsol[edgeId, 1] = (int)solver.Value(x[edgeId, 1]);
                    xsol[edgeId, 2] = (int)solver.Value(x[edgeId, 2]);
                }

                // Check connectivity and get cuts
                var cuts = FindComponentCuts(islands, xsol, edgeMap, revEdgeMap);

                // If only one component, solution is connected
                if (cuts.Count == 1 && cuts[0].Count == 0)
                {
                    Debug.WriteLine($"Solution found ({Math.Round(watch.Elapsed.TotalSeconds, 3)})");
                    //PrettyPrint(islands, x, solver, edgeMap.ToDictionary(kv => kv.Key, kv => new Edge(kv.Key, kv.Value.Item1, kv.Value.Item2)));
                    watch.Stop();
                    return SolverStatusEnum.Optimal;
                }

                // Add a cut for each disconnected component
                foreach (var cut in cuts)
                {
                    if (cut.Count > 0)
                    {
                        var cutVars = cut.Select(eid => x[eid, 1]).ToArray();
                        model.Add(LinearExpr.Sum(cutVars) >= 1);
                    }
                }
            }
        }

        // Returns a list of edge sets, one for each component cut (edges that, if added, would connect the component)
        private static List<List<int>> FindComponentCuts(
            List<Island> islands,
            int[,] xsol,
            Dictionary<int, (int, int)> edgeMap,
            Dictionary<(int, int), int> revEdgeMap)
        {
            int n = islands.Count;
            int m = edgeMap.Count;

            // Build adjacency list from solution
            var adj = new Dictionary<int, List<int>>();
            for (int i = 0; i < n; i++)
                adj[i] = new List<int>();
            for (int e = 1; e <= m; e++)
            {
                if (xsol[e, 1] > 0)
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
                int start = Enumerable.Range(0, n).First(i => !allVisited.Contains(i));
                var component = new HashSet<int>();
                var queue = new Queue<int>();
                queue.Enqueue(start);
                component.Add(start);

                while (queue.Count > 0)
                {
                    int current = queue.Dequeue();
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
                            int edgeId = i < j ? revEdgeMap[(i, j)] : revEdgeMap[(j, i)];
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
