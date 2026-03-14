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
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        ArgumentOutOfRangeException.ThrowIfNegative(amountBridgesConnectable);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(amountBridgesConnectable, 8);

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
    public int AmountBridgesConnectable { get; private set; }

    /// <inheritdoc />
    public int AmountBridgesUp { get; private set; }

    /// <inheritdoc />
    public int AmountBridgesDown { get; private set; }

    /// <inheritdoc />
    public int AmountBridgesLeft { get; private set; }

    /// <inheritdoc />
    public int AmountBridgesRight { get; private set; }

    /// <inheritdoc />
    public void IncrementAmountBridgesConnectable(int delta) => AmountBridgesConnectable += delta;

    /// <inheritdoc />
    public void IncrementAmountBridgesUp(int delta) => AmountBridgesUp += delta;

    /// <inheritdoc />
    public void IncrementAmountBridgesDown(int delta) => AmountBridgesDown += delta;

    /// <inheritdoc />
    public void IncrementAmountBridgesLeft(int delta) => AmountBridgesLeft += delta;

    /// <inheritdoc />
    public void IncrementAmountBridgesRight(int delta) => AmountBridgesRight += delta;

    /// <inheritdoc />
    public void SetAmountBridgesUp(int value) => AmountBridgesUp = value;

    /// <inheritdoc />
    public void SetAmountBridgesDown(int value) => AmountBridgesDown = value;

    /// <inheritdoc />
    public void SetAmountBridgesLeft(int value) => AmountBridgesLeft = value;

    /// <inheritdoc />
    public void SetAmountBridgesRight(int value) => AmountBridgesRight = value;

    /// <inheritdoc />
    public IIsland? IslandUp { get; private set; }

    /// <inheritdoc />
    public IIsland? IslandDown { get; private set; }

    /// <inheritdoc />
    public IIsland? IslandLeft { get; private set; }

    /// <inheritdoc />
    public IIsland? IslandRight { get; private set; }

    /// <inheritdoc />
    public void SetAllNeighbors(int[][] field, IReadOnlyList<IIsland> islands)
    {
        var islandLookup = new Dictionary<(int Y, int X), IIsland>(islands.Count);
        foreach (var island in islands)
        {
            islandLookup.TryAdd((island.Y, island.X), island);
        }

        SetAllNeighbors(field, islandLookup);
    }

    /// <inheritdoc />
    public void SetAllNeighbors(int[][] field, Dictionary<(int Y, int X), IIsland> islandLookup)
    {
        // Look up
        for (var u = Y - 1; u >= 0; u--)
        {
            if (field[u][X] == 0)
            {
                continue;
            }

            if (IslandUp == null && islandLookup.TryGetValue((u, X), out var upIsland))
            {
                IslandUp = upIsland;
            }
            break;
        }

        // Look down
        for (var d = Y + 1; d < field.Length; d++)
        {
            if (field[d][X] == 0)
            {
                continue;
            }

            if (IslandDown == null && islandLookup.TryGetValue((d, X), out var downIsland))
            {
                IslandDown = downIsland;
            }
            break;
        }

        // Look left
        for (var l = X - 1; l >= 0; l--)
        {
            if (field[Y][l] == 0)
            {
                continue;
            }

            if (IslandLeft == null && islandLookup.TryGetValue((Y, l), out var leftIsland))
            {
                IslandLeft = leftIsland;
            }
            break;
        }

        // Look right
        for (var r = X + 1; r < field[Y].Length; r++)
        {
            if (field[Y][r] == 0)
            {
                continue;
            }

            if (IslandRight == null && islandLookup.TryGetValue((Y, r), out var rightIsland))
            {
                IslandRight = rightIsland;
            }
            break;
        }
    }
}