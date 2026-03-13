using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Models;

/// <inheritdoc cref="IIsland" />
public class Island : IIsland
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Island" /> class.
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

    /// <inheritdoc />
    public int Y { get; }

    /// <inheritdoc />
    public int X { get; }

    /// <inheritdoc />
    public int AmountBridgesConnectable { get; set; }

    /// <inheritdoc />
    public int AmountBridgesUp { get; set; }

    /// <inheritdoc />
    public int AmountBridgesDown { get; set; }

    /// <inheritdoc />
    public int AmountBridgesLeft { get; set; }

    /// <inheritdoc />
    public int AmountBridgesRight { get; set; }

    /// <inheritdoc />
    public IIsland? IslandUp { get; private set; }

    /// <inheritdoc />
    public IIsland? IslandDown { get; private set; }

    /// <inheritdoc />
    public IIsland? IslandLeft { get; private set; }

    /// <inheritdoc />
    public IIsland? IslandRight { get; private set; }

    /// <inheritdoc />
    public void SetAllNeighbors(int[][] field, List<IIsland> islands)
    {
        for (var u = Y - 1; u >= 0; u--)
        {
            if (field[u][X] == 0)
            {
                continue;
            }

            foreach (var island in islands.Where(island => island.Y == u && island.X == X && IslandUp == null))
            {
                IslandUp = island;
                break;
            }
        }

        for (var d = Y + 1; d < field.Length; d++)
        {
            if (field[d][X] == 0)
            {
                continue;
            }

            foreach (var island in islands.Where(island => island.Y == d && island.X == X && IslandDown == null))
            {
                IslandDown = island;
                break;
            }
        }

        for (var l = X - 1; l >= 0; l--)
        {
            if (field[Y][l] == 0)
            {
                continue;
            }

            foreach (var island in islands.Where(island => island.Y == Y && island.X == l && IslandLeft == null))
            {
                IslandLeft = island;
                break;
            }
        }

        for (var r = X + 1; r < field[Y].Length; r++)
        {
            if (field[Y][r] == 0)
            {
                continue;
            }

            foreach (var island in islands.Where(island => island.Y == Y && island.X == r && IslandRight == null))
            {
                IslandRight = island;
                break;
            }
        }
    }
}