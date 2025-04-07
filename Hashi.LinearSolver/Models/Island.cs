using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Models;
// y = y-Koordinate; x = x-Koordinate; value = Wert des Knotens; up = obere Nachbarknoten; down = untere Nachbarknoten
// right = rechter Nachbarknoten; left = linke Nachbarknoten
// remain = ist der Rest vom Wert des Knotens abgezogen der vorhandenen Knoten
// upedges, downedges, leftedges, rightedges = Anzahl an Kanten nach oben, unten, links und rechts

/// <summary>
///     Represents an island.
/// </summary>
public class Island : IIsland
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Island" /> class.
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
        UpBridges = 0;
        DownBridges = 0;
        LeftBridges = 0;
        RightBridges = 0;
        Number = number;
        LowerNeighbors = new List<IIsland>();
        UpNeighbors = new List<IIsland>();
    }

    /// <inheritdoc />
    public int Y { get; }

    /// <inheritdoc />
    public int X { get; }

    /// <inheritdoc />
    public int Value { get; set; }

    /// <inheritdoc />
    public IIsland? Up { get; private set; }

    /// <inheritdoc />
    public IIsland? Down { get; private set; }

    /// <inheritdoc />
    public IIsland? Right { get; private set; }

    /// <inheritdoc />
    public IIsland? Left { get; private set; }

    /// <inheritdoc />
    public int Number { get; }

    /// <inheritdoc />
    public int UpBridges { get; }

    /// <inheritdoc />
    public int DownBridges { get; }

    /// <inheritdoc />
    public int LeftBridges { get; }

    /// <inheritdoc />
    public int RightBridges { get; }

    /// <inheritdoc />
    public List<IIsland> LowerNeighbors { get; }

    /// <inheritdoc />
    public List<IIsland> UpNeighbors { get; }

    /// <inheritdoc />
    public void SetAllNeighbors(int[][] field, List<IIsland> islands)
    {
        for (var u = Y - 1; u >= 0; u--)
        {
            if (field[u][X] == 0) continue;
            foreach (var island in islands.Where(node => node.Y == u && node.X == X && Up == null)) Up = island;
        }

        for (var d = Y + 1; d < field.Length; d++)
        {
            if (field[d][X] == 0) continue;
            foreach (var island in islands.Where(node => node.Y == d && node.X == X && Down == null)) Down = island;
        }

        for (var l = X - 1; l >= 0; l--)
        {
            if (field[Y][l] == 0) continue;
            foreach (var island in islands.Where(node => node.Y == Y && node.X == l && Left == null)) Left = island;
        }

        for (var r = X + 1; r < field[Y].Length; r++)
        {
            if (field[Y][r] == 0) continue;
            foreach (var island in islands.Where(node => node.Y == Y && node.X == r && Right == null)) Right = island;
        }
    }

    /// <inheritdoc />
    public List<IIsland> DownBlocked(List<IIsland> island)
    {
        var islandsBlocking = new List<IIsland>();
        if (Down == null) return islandsBlocking;
        for (var row = Down.Y - 1; row > Y; row--)
        for (var checkEl = 0; checkEl < X; checkEl++)
            islandsBlocking.AddRange(island.Where(node =>
                node.X == checkEl && node.Y == row && node.Right != null && node.Right.X > X));
        return islandsBlocking;
    }

    /// <inheritdoc />
    public List<IIsland> RightBlocked(List<IIsland> island)
    {
        var islandsBlocking = new List<IIsland>();
        if (Right == null) return islandsBlocking;
        for (var column = Right.X - 1; column > X; column--)
        for (var checkEl = 0; checkEl < Y; checkEl++)
            islandsBlocking.AddRange(island.Where(node =>
                node.Y == checkEl && node.X == column && node.Down != null && node.Down.Y > Y));
        return islandsBlocking;
    }
}