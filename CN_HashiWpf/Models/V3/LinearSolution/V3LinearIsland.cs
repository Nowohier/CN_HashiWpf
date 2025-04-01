namespace CNHashiWpf.Models.V3.LinearSolution
{
    // y = y-Koordinate; x = x-Koordinate; value = Wert des Knotens; up = obere Nachbarknoten; down = untere Nachbarknoten
    // right = rechter Nachbarknoten; left = linke Nachbarknoten
    // remain = ist der Rest vom Wert des Knotens abgezogen der vorhandenen Knoten
    // upedges, downedges, leftedges, rightedges = Anzahl an Kanten nach oben, unten, links und rechts
    public class V3LinearIsland
    {
        public V3LinearIsland(int value, int y, int x, int number)
        {
            Y = y;
            X = x;
            Value = value;
            UpEdges = 0;
            DownEdges = 0;
            LeftEdges = 0;
            RightEdges = 0;
            Number = number;
            LowerNeighbours = new List<V3LinearIsland>();
            UpNeighbours = new List<V3LinearIsland>();
        }

        public V3LinearIsland(int value, int y, int x)
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
        public V3LinearIsland Up { get; private set; }
        public V3LinearIsland Down { get; private set; }
        public V3LinearIsland Right { get; private set; }
        public V3LinearIsland Left { get; private set; }
        public int Number { get; }
        public int UpEdges { get; private set; }
        public int DownEdges { get; private set; }
        public int LeftEdges { get; private set; }
        public int RightEdges { get; private set; }

        public List<V3LinearIsland> LowerNeighbours { get; }
        public List<V3LinearIsland> UpNeighbours { get; }

        // Setzt die Nachbarn des Knotens
        public void SetAllNeighbors(int[][] field, List<V3LinearIsland> nodes)
        {
            for (var u = Y - 1; u >= 0; u--)
            {
                if (field[u][X] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == u && node.X == X && Up == null))
                {
                    Up = node;
                }
            }
            for (var d = Y + 1; d < field.Length; d++)
            {
                if (field[d][X] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == d && node.X == X && Down == null))
                {
                    Down = node;
                }
            }
            for (var l = X - 1; l >= 0; l--)
            {
                if (field[Y][l] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == Y && node.X == l && Left == null))
                {
                    Left = node;
                }
            }
            for (var r = X + 1; r < field[Y].Length; r++)
            {
                if (field[Y][r] == 0) continue;
                foreach (var node in nodes.Where(node => node.Y == Y && node.X == r && Right == null))
                {
                    Right = node;
                }
            }
        }

        // Gibt alle Knoten zurück, die sich zwischen dem mainNode und dem unteren Nachbarknoten befinden
        public List<V3LinearIsland> DownBlocked(List<V3LinearIsland> nodes)
        {
            var nodesBlocking = new List<V3LinearIsland>();
            if (Down == null) return nodesBlocking;
            for (var row = Down.Y - 1; row > Y; row--)
            {
                for (var checkEl = 0; checkEl < X; checkEl++)
                {
                    nodesBlocking.AddRange(nodes.Where(node => node.X == checkEl && node.Y == row && node.Right != null && node.Right.X > X));
                }
            }
            return nodesBlocking;
        }

        // Gibt alle Knoten zurück, die sich zwischen dem mainNode und dem rechten Nachbarknoten befinden
        public List<V3LinearIsland> RightBlocked(List<V3LinearIsland> nodes)
        {
            var nodesBlocking = new List<V3LinearIsland>();
            if (Right == null) return nodesBlocking;
            for (var column = Right.X - 1; column > X; column--)
            {
                for (var checkEl = 0; checkEl < Y; checkEl++)
                {
                    nodesBlocking.AddRange(nodes.Where(node => node.Y == checkEl && node.X == column && node.Down != null && node.Down.Y > Y));
                }
            }
            return nodesBlocking;
        }
    }
}
