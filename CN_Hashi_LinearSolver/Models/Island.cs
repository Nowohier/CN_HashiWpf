namespace CNHashiLinearSolver.Models
{
    // y = y-Koordinate; x = x-Koordinate; value = Wert des Knotens; up = obere Nachbarknoten; down = untere Nachbarknoten
    // right = rechter Nachbarknoten; left = linke Nachbarknoten
    // remain = ist der Rest vom Wert des Knotens abgezogen der vorhandenen Knoten
    // upedges, downedges, leftedges, rightedges = Anzahl an Kanten nach oben, unten, links und rechts


    /// <summary>
    /// Represents an island.
    /// </summary>
    public class Island
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Island"/> class.
        /// </summary>
        /// <param name="value">The actual value of the island.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="number">The island number.</param>
        public Island(int value, int y, int x, int number)
        {
            Y = y;
            X = x;
            Value = value;
            UpEdges = 0;
            DownEdges = 0;
            LeftEdges = 0;
            RightEdges = 0;
            Number = number;
            LowerNeighbors = new List<Island>();
            UpNeighbors = new List<Island>();
        }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the actual value of the island.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets the upper neighbor.
        /// </summary>
        public Island Up { get; private set; }

        /// <summary>
        /// Gets the lower neighbor.
        /// </summary>
        public Island Down { get; private set; }

        /// <summary>
        /// Gets the right neighbor.
        /// </summary>
        public Island Right { get; private set; }

        /// <summary>
        /// Gets the left neighbor.
        /// </summary>
        public Island Left { get; private set; }

        /// <summary>
        /// Gets the island number.
        /// </summary>
        public int Number { get; }

        public int UpEdges { get; private set; }
        public int DownEdges { get; private set; }
        public int LeftEdges { get; private set; }
        public int RightEdges { get; private set; }

        /// <summary>
        /// Gets the lower neighbors.
        /// </summary>
        public List<Island> LowerNeighbors { get; }

        /// <summary>
        /// Gets the upper neighbors.
        /// </summary>
        public List<Island> UpNeighbors { get; }

        /// <summary>
        /// Adds an edge to the island.
        /// </summary>
        /// <param name="field">The array of fields.</param>
        /// <param name="islands">The list of islands.</param>
        public void SetAllNeighbors(int[][] field, List<Island> islands)
        {
            for (var u = Y - 1; u >= 0; u--)
            {
                if (field[u][X] == 0) continue;
                foreach (var island in islands.Where(node => node.Y == u && node.X == X && Up == null))
                {
                    Up = island;
                }
            }
            for (var d = Y + 1; d < field.Length; d++)
            {
                if (field[d][X] == 0) continue;
                foreach (var island in islands.Where(node => node.Y == d && node.X == X && Down == null))
                {
                    Down = island;
                }
            }
            for (var l = X - 1; l >= 0; l--)
            {
                if (field[Y][l] == 0) continue;
                foreach (var island in islands.Where(node => node.Y == Y && node.X == l && Left == null))
                {
                    Left = island;
                }
            }
            for (var r = X + 1; r < field[Y].Length; r++)
            {
                if (field[Y][r] == 0) continue;
                foreach (var island in islands.Where(node => node.Y == Y && node.X == r && Right == null))
                {
                    Right = island;
                }
            }
        }

        /// <summary>
        /// Returns all islands between this island and the lower neighbor.
        /// </summary>
        /// <param name="island">A list of all islands.</param>
        /// <returns>all islands between this island and the lower neighbor.</returns>
        public List<Island> DownBlocked(List<Island> island)
        {
            var islandsBlocking = new List<Island>();
            if (Down == null) return islandsBlocking;
            for (var row = Down.Y - 1; row > Y; row--)
            {
                for (var checkEl = 0; checkEl < X; checkEl++)
                {
                    islandsBlocking.AddRange(island.Where(node => node.X == checkEl && node.Y == row && node.Right != null && node.Right.X > X));
                }
            }
            return islandsBlocking;
        }

        /// <summary>
        /// Returns all islands between this island and the right neighbor.
        /// </summary>
        /// <param name="island">A list of all islands.</param>
        /// <returns>all islands between this island and the right neighbor.</returns>
        public List<Island> RightBlocked(List<Island> island)
        {
            var islandsBlocking = new List<Island>();
            if (Right == null) return islandsBlocking;
            for (var column = Right.X - 1; column > X; column--)
            {
                for (var checkEl = 0; checkEl < Y; checkEl++)
                {
                    islandsBlocking.AddRange(island.Where(node => node.Y == checkEl && node.X == column && node.Down != null && node.Down.Y > Y));
                }
            }
            return islandsBlocking;
        }
    }
}
