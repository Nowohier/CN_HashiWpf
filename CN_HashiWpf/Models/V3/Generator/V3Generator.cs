using CN_HashiWpf.Models.V3.LinearSolution;
using Google.OrTools.Sat;
using System.Diagnostics;
using System.Windows;

namespace CNHashiWpf.Models.V3.Generator
{
    public class V3Generator
    {
        private readonly List<V3Island> nodes = new();
        private readonly List<V3Bridge> edges = new();

        public V3Generator(int n, int sizeLength, int sizeWidth, int difficulty, int beta, bool checkDiff)
        {
            var solvableTestHashi = new[]
            {
                new[] { 2, 2, 5, 1, 2, 3, 0, 0, 0, 0, 0, 0, 3, 3, 0, 3 },
                new[] { 4, 0, 8, 0, 0, 0, 4, 1, 0, 0, 3, 0, 0, 2, 2, 0 },
                new[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 4 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 3, 0, 0, 8, 0, 0, 8, 6, 2, 0, 4, 8, 4 },
                new[] { 0, 0, 0, 1, 4, 0, 7, 1, 0, 2, 0, 0, 0, 2, 0, 2 },
                new[] { 0, 0, 0, 2, 5, 1, 4, 2, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 4, 0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 6, 3 },
                new[] { 2, 0, 6, 1, 1, 0, 4, 0, 0, 1, 2, 1, 0, 2, 5, 2 },
                new[] { 2, 0, 7, 0, 0, 0, 4, 0, 3, 4, 6, 3, 0, 3, 4, 1 },
                new[] { 1, 2, 6, 0, 0, 0, 0, 0, 0, 0, 4, 2, 0, 1, 0, 0 },
                new[] { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 5, 2 },
                new[] { 0, 2, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 7, 0, 6, 4 },
                new[] { 4, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 2, 0, 0 },
                new[] { 5, 2, 4, 0, 0, 0, 4, 3, 0, 0, 0, 1, 2, 0, 5, 2 },
                new[] { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 2, 0, 3, 2 }
            };
            var field = CreateHash(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
            var linear = new V3LinearSolutionWithIterativ();
            while (linear.Solve(field) == CpSolverStatus.Infeasible)
            {
                field = CreateHash(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
            }

            Debug.WriteLine(string.Empty);
            Debug.WriteLine(string.Join("\n", field.Select(row => $"{{{string.Join(", ", row)}}}")));
        }

        public int[][] CreateHash(int n, int sizeLength, int sizeWidth, int difficulty, int beta, bool checkDiff)
        {
            edges.Clear();
            nodes.Clear();

            var mainField = new int[sizeLength][];
            for (var i = 0; i < sizeLength; i++)
            {
                mainField[i] = new int[sizeWidth];
            }

            var rand = new Random();
            var row = rand.Next(sizeLength);
            var col = rand.Next(sizeWidth);
            nodes.Add(new V3Island(0, row, col));
            var edgeCount = 0;

            while (true)
            {
                var size = nodes.Count;
                for (var i = 0; i < size; i++)
                {
                    mainField = CreateNode(mainField, nodes[i]);
                    if (nodes.Count == n)
                    {
                        break;
                    }
                }
                if (edgeCount == edges.Count || nodes.Count == n)
                {
                    break;
                }
                edgeCount = edges.Count;
            }

            foreach (var node in nodes)
            {
                node.SetAllNeighbors(mainField, nodes);
            }

            if (!checkDiff)
            {
                mainField = difficulty switch
                {
                    0 => CreateNewEdges(mainField, 25),
                    3 => CreateNewEdges(mainField, 25),
                    6 => CreateNewEdges(mainField, 25),
                    1 => CreateNewEdges(mainField, 50),
                    4 => CreateNewEdges(mainField, 50),
                    7 => CreateNewEdges(mainField, 50),
                    2 => CreateNewEdges(mainField, 75),
                    5 => CreateNewEdges(mainField, 75),
                    8 => CreateNewEdges(mainField, 75),
                    9 => CreateNewEdges(mainField, 100),
                    _ => mainField
                };

                switch (difficulty)
                {
                    case >= 0 and <= 2:
                        SetBeta(mainField, 20);
                        break;
                    case >= 3 and <= 5:
                        SetBeta(mainField, 15);
                        break;
                    case >= 6 and <= 8:
                        SetBeta(mainField, 10);
                        break;
                    case 9:
                        SetBeta(mainField, 0);
                        break;
                }
            }
            else
            {
                mainField = CreateNewEdges(mainField, difficulty);
                SetBeta(mainField, beta);
            }

            return mainField;
        }

        public int[][] CreateNewEdges(int[][] mainField, int alpha)
        {
            var i = 0;
            var j = 0;

            while (i < nodes.Count)
            {
                var node = nodes[i];
                if (node.Down != null && DownBlockedd(node, mainField) == -1 && node.DownEdges == 0)
                {
                    if (node.Value + 1 <= 7 && node.Down.Value + 1 <= 7)
                    {
                        var v3Edge = new V3Bridge(node, node.Down, 1);
                        edges.Add(v3Edge);
                        edges.Add(v3Edge.AddOtherSide());
                        node.Value += 1;
                        node.Down.Value += 1;
                        mainField[node.Y][node.X] += 1;
                        mainField[node.Down.Y][node.X] += 1;
                        j++;
                    }
                }
                if (j >= nodes.Count * (alpha / 100.0))
                {
                    break;
                }
                if (node.Right != null && RightBlockedd(node, mainField) == -1 && node.RightEdges == 0)
                {
                    if (node.Value + 1 <= 7 && node.Right.Value + 1 <= 7)
                    {
                        var v3Edge = new V3Bridge(node, node.Right, 1);
                        edges.Add(v3Edge);
                        edges.Add(v3Edge.AddOtherSide());
                        node.Value += 1;
                        node.Right.Value += 1;
                        mainField[node.Y][node.X] += 1;
                        mainField[node.Right.Y][node.X] += 1;
                        j++;
                    }
                }
                if (j >= nodes.Count * (alpha / 100.0))
                {
                    break;
                }
                i++;
            }

            return mainField;
        }

        public int[][] CreateNode(int[][] mainField, V3Island island)
        {
            var rand = new Random();
            var possibles = new List<Point>();
            var range = rand.Next(2, 6);

            if (island.Y != 0)
            {
                var block = UpBlocked(island, mainField);
                var check = false;
                foreach (var edge in edges)
                {
                    if (edge.Node1.Y == island.Y && edge.Node1.X == island.X && edge.Node2.Y < edge.Node1.Y && edge.Node2.X == edge.Node1.X)
                    {
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    for (var i = island.Y - 1; i >= 0 && i >= island.Y - range; i--)
                    {
                        if (mainField[i][island.X] != 0)
                        {
                            break;
                        }
                        if (mainField[i][island.X] == 0 && (block == -1 || i > block))
                        {
                            possibles.Add(new Point(island.X, i));
                        }
                    }
                }
            }

            if (island.X != 0)
            {
                var block = LeftBlocked(island, mainField);
                var check = edges.Any(edge => edge.Node1.Y == island.Y && edge.Node1.X == island.X && edge.Node2.Y == edge.Node1.Y && edge.Node2.X <= edge.Node1.X);
                if (!check)
                {
                    for (var i = island.X - 1; i >= 0 && i >= island.X - range; i--)
                    {
                        if (mainField[island.Y][i] != 0)
                        {
                            break;
                        }
                        if (mainField[island.Y][i] == 0 && (block == -1 || i > block))
                        {
                            possibles.Add(new Point(i, island.Y));
                        }
                    }
                }
            }

            if (island.Y != mainField.Length - 1)
            {
                var block = DownBlocked(island, mainField);
                var check = edges.Any(edge => edge.Node1.Y == island.Y && edge.Node1.X == island.X && edge.Node2.Y > edge.Node1.Y && edge.Node2.X == edge.Node1.X);
                if (!check)
                {
                    for (var i = island.Y + 1; i <= mainField.Length - 1 && i <= island.Y + range; i++)
                    {
                        if (mainField[i][island.X] != 0)
                        {
                            break;
                        }
                        if (mainField[i][island.X] == 0 && (block == -1 || i < block))
                        {
                            possibles.Add(new Point(island.X, i));
                        }
                    }
                }
            }

            if (island.X != mainField[0].Length - 1)
            {
                var block = RightBlocked(island, mainField);
                var check = edges.Any(edge => edge.Node1.Y == island.Y && edge.Node1.X == island.X && edge.Node2.Y == edge.Node1.Y && edge.Node2.X > edge.Node1.X);
                if (!check)
                {
                    for (var i = island.X + 1; i <= mainField[island.Y].Length - 1 && i <= island.X + range; i++)
                    {
                        if (mainField[island.Y][i] != 0)
                        {
                            break;
                        }
                        if (mainField[island.Y][i] == 0 && (block == -1 || i < block))
                        {
                            possibles.Add(new Point(i, island.Y));
                        }
                    }
                }
            }

            for (var i = possibles.Count - 1; i >= 0; i--)
            {
                if (CheckSurroundingFields(Convert.ToInt32(possibles[i].Y), Convert.ToInt32(possibles[i].X), mainField))
                {
                    possibles.RemoveAt(i);
                }
            }

            if (possibles.Count > 0)
            {
                var randomPosition = rand.Next(possibles.Count);
                var newNode = new V3Island(0, Convert.ToInt32(possibles[randomPosition].Y), Convert.ToInt32(possibles[randomPosition].X));
                nodes.Add(newNode);
                var edgeNumber = 1;
                var v3Edge = new V3Bridge(island, newNode, edgeNumber);
                edges.Add(v3Edge);
                edges.Add(v3Edge.AddOtherSide());
                island.Value += edgeNumber;
                newNode.Value += edgeNumber;
                mainField[island.Y][island.X] += edgeNumber;
                mainField[newNode.Y][newNode.X] += edgeNumber;
            }

            return mainField;
        }

        public bool CheckSurroundingFields(int row, int col, int[][] mainField)
        {
            var numRows = mainField.Length;
            var numCols = mainField[0].Length;

            if (row - 1 >= 0 && mainField[row - 1][col] != 0)
            {
                return true;
            }
            if (row + 1 < numRows && mainField[row + 1][col] != 0)
            {
                return true;
            }
            if (col - 1 >= 0 && mainField[row][col - 1] != 0)
            {
                return true;
            }
            return col + 1 < numCols && mainField[row][col + 1] != 0;
        }

        public int UpBlocked(V3Island mainIsland, int[][] mainField)
        {
            for (var row = mainIsland.Y - 1; row > 0; row--)
            {
                for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
                {
                    if (edges.Count <= 0 || mainField[row][checkLeft] == 0) continue;
                    if (edges.Any(edge => edge.Node1.Y == row && edge.Node1.X == checkLeft && edge.Node2.X > mainIsland.X))
                    {
                        return row;
                    }
                }
            }
            return -1;
        }

        public int DownBlocked(V3Island mainIsland, int[][] mainField)
        {
            for (var row = mainIsland.Y + 1; row < mainField.Length; row++)
            {
                for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
                {
                    if (edges.Count <= 0 || mainField[row][checkLeft] == 0) continue;
                    if (edges.Any(edge => edge.Node1.Y == row && edge.Node1.X == checkLeft && edge.Node2.X > mainIsland.X))
                    {
                        return row;
                    }
                }
            }
            return -1;
        }

        public int RightBlocked(V3Island mainIsland, int[][] mainField)
        {
            for (var col = mainIsland.X + 1; col < mainField[mainIsland.Y].Length; col++)
            {
                for (var checkLeft = mainIsland.Y - 1; checkLeft >= 0; checkLeft--)
                {
                    if (edges.Count <= 0 || mainField[checkLeft][col] == 0) continue;
                    if (edges.Any(edge => edge.Node1.Y == checkLeft && edge.Node1.X == col && edge.Node2.Y > mainIsland.Y))
                    {
                        return col;
                    }
                }
            }
            return -1;
        }

        public int LeftBlocked(V3Island mainIsland, int[][] mainField)
        {
            for (var col = mainIsland.X - 1; col > 0; col--)
            {
                for (var checkLeft = mainIsland.Y - 1; checkLeft >= 0; checkLeft--)
                {
                    if (edges.Count <= 0 || mainField[checkLeft][col] == 0) continue;
                    if (edges.Any(edge => edge.Node1.Y == checkLeft && edge.Node1.X == col && edge.Node2.Y > mainIsland.Y))
                    {
                        return col;
                    }
                }
            }
            return -1;
        }

        public int DownBlockedd(V3Island mainIsland, int[][] mainField)
        {
            for (var row = mainIsland.Down.Y - 1; row > mainIsland.Y; row--)
            {
                for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
                {
                    if (mainField[row][checkLeft] == 0) continue;
                    if (edges.Any(edge => edge.Node1.Y == row && edge.Node1.X == checkLeft && edge.Node2.X > mainIsland.X))
                    {
                        return row;
                    }
                }
            }
            return -1;
        }

        public int RightBlockedd(V3Island mainIsland, int[][] mainField)
        {
            for (var col = mainIsland.Right.X - 1; col > mainIsland.X; col--)
            {
                for (var checkLeft = mainIsland.Y - 1; checkLeft >= 0; checkLeft--)
                {
                    if (mainField[checkLeft][col] == 0) continue;
                    if (edges.Any(edge => edge.Node1.Y == checkLeft && edge.Node1.X == col && edge.Node2.Y > mainIsland.Y))
                    {
                        return col;
                    }
                }
            }
            return -1;
        }

        public void SetBeta(int[][] mainField, int beta)
        {
            var rdm = new Random();
            for (var i = edges.Count - 1; i > 0; i -= 2)
            {
                var random = rdm.Next(100);
                if (random <= beta - 1)
                {
                    edges[i].ActualEdge(mainField);
                }
            }
        }

        public List<V3Bridge> GetEdges()
        {
            return edges;
        }

        public List<V3Island> GetNodes()
        {
            return nodes;
        }
    }
}
