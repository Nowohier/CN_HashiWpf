namespace CNHashiWpf.Models.V3.Generator
{
    /// <summary>
    /// Represents an edge between two nodes.
    /// </summary>
    public class V3Bridge
    {
        public V3Bridge(V3Island node1, V3Island node2, int count)
        {
            Node1 = node1;
            Node2 = node2;
            Count = count;
        }

        public V3Island Node1 { get; }
        public V3Island Node2 { get; }
        public int Count { get; private set; }

        /// <summary>
        /// Adds the other side of the edge.
        /// </summary>
        /// <returns></returns>
        public V3Bridge AddOtherSide()
        {
            var otherSideV3Edge = new V3Bridge(Node2, Node1, Count);
            if (Node1.X == Node2.X && Node1.Y > Node2.Y)
            {
                Node1.UpEdges += Count;
                Node2.DownEdges += Count;
            }
            if (Node1.X == Node2.X && Node1.Y < Node2.Y)
            {
                Node1.DownEdges += Count;
                Node2.UpEdges += Count;
            }
            if (Node1.X > Node2.X && Node1.Y == Node2.Y)
            {
                Node1.LeftEdges += Count;
                Node2.RightEdges += Count;
            }
            if (Node1.X < Node2.X && Node1.Y == Node2.Y)
            {
                Node1.RightEdges += Count;
                Node2.LeftEdges += Count;
            }
            return otherSideV3Edge;
        }

        /// <summary>
        /// Adds another edge to this edge.
        /// </summary>
        /// <param name="mainField">The main field array.</param>
        public void ActualEdge(int[][] mainField)
        {
            Count++;
            if (Node1.Up == Node2)
            {
                Node1.UpEdges = Count;
                Node2.DownEdges = Count;
            }
            if (Node1.Down == Node2)
            {
                Node1.DownEdges = Count;
                Node2.UpEdges = Count;
            }
            if (Node1.Left == Node2)
            {
                Node1.LeftEdges = Count;
                Node2.RightEdges = Count;
            }
            if (Node1.Right == Node2)
            {
                Node1.RightEdges = Count;
                Node2.LeftEdges = Count;
            }
            Node1.Value += 1;
            Node2.Value += 1;
            mainField[Node1.Y][Node1.X]++;
            mainField[Node2.Y][Node2.X]++;
        }
    }
}
