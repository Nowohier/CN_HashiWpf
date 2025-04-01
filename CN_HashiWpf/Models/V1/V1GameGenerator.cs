namespace CNHashiWpf.Models.V1
{
    /// <summary>
    /// Generates the game
    /// </summary>
    public class V1GameGenerator
    {
        /// <summary>
        /// The islands in the game.
        /// </summary>
        public List<V1Island> Islands { get; set; } = new();

        /// <summary>
        /// The bridges in the game.
        /// </summary>
        public List<V1Bridge> Bridges { get; set; } = new();

        /// <summary>
        /// The amount of columns in the game grid.
        /// </summary>
        public int AmountColumns => Islands.Max(i => i.X) + 1;

        /// <summary>
        /// The amount of rows in the game grid.
        /// </summary>
        public int AmountRows => Islands.Max(i => i.Y) + 1;

        /// <summary>
        /// Adds the islands to the game
        /// </summary>
        public void AddIslands()
        {
            // ToDo: Dynamic generation
            AddIsland(1, 1, 2);
            AddIsland(1, 2, 1);
            AddIsland(2, 1, 1);
            AddIsland(2, 2, 2);
        }

        /// <summary>
        /// Adds a bridge between two islands.
        /// </summary>
        /// <param name="island1">The first island to connect to.</param>
        /// <param name="island2">The second island to connect to.</param>
        /// <returns>a boolean value if bridge has been added.</returns>
        public bool AddBridge(V1Island island1, V1Island island2)
        {
            //ToDo: Implement bridge rules
            if (island1.BridgesConnected >= island1.BridgesNeeded || island2.BridgesConnected >= island2.BridgesNeeded)
                return false;

            var existingBridge = Bridges.Find(b => b.Island1 == island1 && b.Island2 == island2 || b.Island1 == island2 && b.Island2 == island1);
            if (existingBridge != null)
            {
                if (existingBridge.Count < 2)
                {
                    existingBridge.Count++;
                    island1.BridgesConnected++;
                    island2.BridgesConnected++;
                    return true;
                }
                return false;
            }

            Bridges.Add(new V1Bridge(island1, island2));
            island1.BridgesConnected++;
            island2.BridgesConnected++;
            return true;
        }

        /// <summary>
        /// Checks if the game is complete
        /// </summary>
        /// <returns></returns>
        public bool IsGameComplete()
        {
            //ToDo: Implement game complete check
            foreach (var island in Islands)
            {
                if (island.BridgesConnected != island.BridgesNeeded)
                    return false;
            }
            return true;
        }

        private void AddIsland(int x, int y, int bridgesNeeded)
        {
            Islands.Add(new V1Island(x, y, bridgesNeeded));
        }
    }
}