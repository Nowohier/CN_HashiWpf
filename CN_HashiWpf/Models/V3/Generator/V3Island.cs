namespace CNHashiWpf.Models.V3.Generator
{
    /// <summary>
    /// Represents a node in the V3 game.
    /// </summary>
    public class V3Island
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="V3Island"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        public V3Island(int value, int y, int x)
        {
            Y = y;
            X = x;
            Value = value;
            UpEdges = 0;
            DownEdges = 0;
            LeftEdges = 0;
            RightEdges = 0;
        }

        public int Y { get; }
        public int X { get; }
        public int Value { get; set; }
        public int UpEdges { get; set; }
        public int DownEdges { get; set; }
        public int LeftEdges { get; set; }
        public int RightEdges { get; set; }
        public V3Island? Up { get; private set; }
        public V3Island? Down { get; private set; }
        public V3Island? Left { get; private set; }
        public V3Island? Right { get; private set; }

        /// <summary>
        /// Set all neighbors of the node
        /// </summary>
        /// <param name="field">The field array.</param>
        /// <param name="nodes">The list of nodes.</param>
        public void SetAllNeighbors(int[][] field, List<V3Island> nodes)
        {
            for (var u = Y - 1; u >= 0; u--)
            {
                if (field[u][X] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == u && node.X == X && Up == null))
                {
                    Up = node;
                    break;
                }
            }
            for (var d = Y + 1; d < field.Length; d++)
            {
                if (field[d][X] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == d && node.X == X && Down == null))
                {
                    Down = node;
                    break;
                }
            }
            for (var l = X - 1; l >= 0; l--)
            {
                if (field[Y][l] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == Y && node.X == l && Left == null))
                {
                    Left = node;
                    break;
                }
            }
            for (var r = X + 1; r < field[Y].Length; r++)
            {
                if (field[Y][r] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == Y && node.X == r && Right == null))
                {
                    Right = node;
                    break;
                }
            }
        }
    }
}
