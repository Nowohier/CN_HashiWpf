using System.Diagnostics;
using System.IO;

namespace CNHashiWpf.Models.V2
{
    public class V2Main
    {
        private static readonly Random Random = new();
        private const int StepPerCycle = 100;

        public V2Main()
        {
            //string path = ParseToPath(args);
            //if (path == null) return;

            var grid = GenerateTillFull(10, 10);
            DrawGrid(grid);
            //SaveGrid(grid, path);
        }

        //private static string ParseToPath(string[] args)
        //{
        //    // Implementiere die Argument-Parsing-Logik hier
        //    return "puzzles/puzzle.csv";
        //}

        private static List<List<V2Island>> Generate(int width, int height)
        {
            var grid = new List<List<V2Island>>();
            for (int i = 0; i < width; i++)
            {
                var row = new List<V2Island>();
                for (int j = 0; j < height; j++)
                {
                    row.Add(new V2Island(i, j));
                }
                grid.Add(row);
            }

            var islands = new List<V2Island>();
            islands.Add(grid[Random.Next(width)][Random.Next(height)]);
            islands[0].MakeIsland(0);

            bool isDeadEnd = false;
            while (true)
            {
                for (int i = 0; i < StepPerCycle; i++)
                {
                    if (islands.Count == 0)
                    {
                        isDeadEnd = true;
                        break;
                    }

                    var currentNode = islands[Random.Next(islands.Count)];
                    int direction = GetRandomDirection(grid, currentNode.X, currentNode.Y);
                    if (direction == -1)
                    {
                        islands.Remove(currentNode);
                        continue;
                    }

                    int thickness = GetRandomBridgeThickness(grid, currentNode.X, currentNode.Y);
                    int length = GetRandomBridgeLength(grid, currentNode.X, currentNode.Y, direction);
                    var dirVector = DirectionToVector(direction);
                    int x = currentNode.X;
                    int y = currentNode.Y;
                    var lastNode = grid[x + dirVector.Item1 * (length + 1)][y + dirVector.Item2 * (length + 1)];

                    bool adjacentIslandFound = false;
                    for (int dir = 0; dir < 2; dir++)
                    {
                        var vector = DirectionToVector(dir);
                        if (IsInGrid(lastNode.X + vector.Item1, lastNode.Y + vector.Item2, grid.Count, grid[0].Count) &&
                            grid[lastNode.X + vector.Item1][lastNode.Y + vector.Item2].NType == 1)
                        {
                            adjacentIslandFound = true;
                        }
                        if (IsInGrid(lastNode.X - vector.Item1, lastNode.Y - vector.Item2, grid.Count, grid[0].Count) &&
                            grid[lastNode.X - vector.Item1][lastNode.Y - vector.Item2].NType == 1)
                        {
                            adjacentIslandFound = true;
                        }
                    }

                    if (adjacentIslandFound) continue;

                    for (int j = 0; j < length; j++)
                    {
                        grid[x + dirVector.Item1 * (j + 1)][y + dirVector.Item2 * (j + 1)].MakeBridge(thickness, direction % 2 == 0);
                    }
                    lastNode.MakeIsland(thickness);
                    islands.Add(lastNode);
                    currentNode.ICount += thickness;
                }

                //if (isDeadEnd || Console.ReadLine().ToLower() == "n") break;
                if (isDeadEnd) break;
            }
            return grid;
        }

        private static int GetRandomDirection(List<List<V2Island>> grid, int x, int y)
        {
            var possibleDirections = new List<int>();
            if (x > 1 && grid[x - 1][y].NType == 0 && grid[x - 2][y].NType == 0) possibleDirections.Add(0);
            if (y > 1 && grid[x][y - 1].NType == 0 && grid[x][y - 2].NType == 0) possibleDirections.Add(1);
            if (x < grid.Count - 2 && grid[x + 1][y].NType == 0 && grid[x + 2][y].NType == 0) possibleDirections.Add(2);
            if (y < grid[0].Count - 2 && grid[x][y + 1].NType == 0 && grid[x][y + 2].NType == 0) possibleDirections.Add(3);
            if (possibleDirections.Count == 0) return -1;
            return possibleDirections[Random.Next(possibleDirections.Count)];
        }

        private static int GetRandomBridgeThickness(List<List<V2Island>> grid, int x, int y)
        {
            if (8 - grid[x][y].ICount > 1) return Random.Next(1, 3);
            return 1;
        }

        private static int GetRandomBridgeLength(List<List<V2Island>> grid, int x, int y, int direction)
        {
            var dirVector = DirectionToVector(direction);
            int maxLength = 1;
            int checkX = x + dirVector.Item1 * (maxLength + 2);
            int checkY = y + dirVector.Item2 * (maxLength + 2);
            while (IsInGrid(checkX, checkY, grid.Count, grid[0].Count) && grid[checkX][checkY].NType == 0)
            {
                maxLength++;
                checkX += dirVector.Item1;
                checkY += dirVector.Item2;
            }
            return Random.Next(1, maxLength + 1);
        }

        private static (int, int) DirectionToVector(int direction)
        {
            return direction switch
            {
                0 => (-1, 0),
                1 => (0, -1),
                2 => (1, 0),
                3 => (0, 1),
                _ => (0, 0)
            };
        }

        private static bool IsInGrid(int x, int y, int width, int height)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        private static bool CheckIfGridFull(List<List<V2Island>> grid)
        {
            int width = grid.Count;
            int height = grid[0].Count;
            if (grid[0].TrueForAll(node => node.NType == 0)) return false;
            if (grid[width - 1].TrueForAll(node => node.NType == 0)) return false;
            if (grid.TrueForAll(row => row[0].NType == 0)) return false;
            if (grid.TrueForAll(row => row[height - 1].NType == 0)) return false;
            return true;
        }

        private static List<List<V2Island>> GenerateTillFull(int width, int height)
        {
            List<List<V2Island>> grid;
            do
            {
                grid = Generate(width, height);
            } while (!CheckIfGridFull(grid));
            return grid;
        }

        private static void DrawGrid(List<List<V2Island>> grid)
        {
            foreach (var row in grid)
            {
                foreach (var node in row)
                {
                    Debug.Write(node.NType + "  ");
                }
                Debug.WriteLine(string.Empty);
            }
        }

        private static void SaveGrid(List<List<V2Island>> grid, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                int width = grid.Count;
                int height = grid[0].Count;
                writer.WriteLine($"{width};;{height};;{string.Join("", grid.SelectMany(row => row.Select(node => node.NType)))};;{string.Join("", grid.SelectMany(row => row.Select(node => node.NType)))}");
            }
        }
    }
}
