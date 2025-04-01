namespace CNHashiGenerator.Models
{
    /// <summary>
    /// Represents a node in the V3 game.
    /// </summary>
    public class Island
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Island"/> class.
        /// </summary>
        /// <param name="amountBridgesConnectable">The amountBridgesConnectable.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        public Island(int amountBridgesConnectable, int y, int x)
        {
            Y = y;
            X = x;
            AmountBridgesConnectable = amountBridgesConnectable;
            AmountBridgesUp = 0;
            AmountBridgesDown = 0;
            AmountBridgesLeft = 0;
            AmountBridgesRight = 0;
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
        /// Gets the amount of bridges connectable.
        /// </summary>
        public int AmountBridgesConnectable { get; set; }

        /// <summary>
        /// Gets the amount of bridges up.
        /// </summary>
        public int AmountBridgesUp { get; set; }

        /// <summary>
        /// Gets the amount of bridges down.
        /// </summary>
        public int AmountBridgesDown { get; set; }

        /// <summary>
        /// Gets the amount of bridges left.
        /// </summary>
        public int AmountBridgesLeft { get; set; }

        /// <summary>
        /// Gets the amount of bridges right.
        /// </summary>
        public int AmountBridgesRight { get; set; }

        /// <summary>
        /// Gets the upper neighbor.
        /// </summary>
        public Island? IslandUp { get; private set; }

        /// <summary>
        /// Gets the lower neighbor.
        /// </summary>
        public Island? IslandDown { get; private set; }

        /// <summary>
        /// Gets the left neighbor.
        /// </summary>
        public Island? IslandLeft { get; private set; }

        /// <summary>
        /// Gets the right neighbor.
        /// </summary>
        public Island? IslandRight { get; private set; }

        /// <summary>
        /// Set all neighbors of the island.
        /// </summary>
        /// <param name="field">The field array.</param>
        /// <param name="islands">The list of islands.</param>
        public void SetAllNeighbors(int[][] field, List<Island> islands)
        {
            for (var u = Y - 1; u >= 0; u--)
            {
                if (field[u][X] == 0) continue;
                foreach (var island in islands.Where(island => island.Y == u && island.X == X && IslandUp == null))
                {
                    IslandUp = island;
                    break;
                }
            }
            for (var d = Y + 1; d < field.Length; d++)
            {
                if (field[d][X] == 0) continue;
                foreach (var island in islands.Where(island => island.Y == d && island.X == X && IslandDown == null))
                {
                    IslandDown = island;
                    break;
                }
            }
            for (var l = X - 1; l >= 0; l--)
            {
                if (field[Y][l] == 0) continue;
                foreach (var island in islands.Where(island => island.Y == Y && island.X == l && IslandLeft == null))
                {
                    IslandLeft = island;
                    break;
                }
            }
            for (var r = X + 1; r < field[Y].Length; r++)
            {
                if (field[Y][r] == 0) continue;
                foreach (var island in islands.Where(island => island.Y == Y && island.X == r && IslandRight == null))
                {
                    IslandRight = island;
                    break;
                }
            }
        }
    }
}
