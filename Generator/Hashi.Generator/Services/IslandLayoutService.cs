using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using System.Drawing;

namespace Hashi.Generator.Services;

/// <inheritdoc />
public class IslandLayoutService : IIslandLayoutService
{
    private readonly Func<int, int, int, IIsland> islandFactory;
    private readonly Func<IIsland, IIsland, int, IBridge> bridgeFactory;
    private readonly BlockDetectionService blockDetectionService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IslandLayoutService" /> class.
    /// </summary>
    public IslandLayoutService(
        Func<int, int, int, IIsland> islandFactory,
        Func<IIsland, IIsland, int, IBridge> bridgeFactory,
        BlockDetectionService blockDetectionService)
    {
        this.islandFactory = islandFactory;
        this.bridgeFactory = bridgeFactory;
        this.blockDetectionService = blockDetectionService;
    }

    /// <inheritdoc />
    public bool CreateIsland(int[][] mainField, IIsland island, IList<IIsland> islands, IList<IBridge> bridges)
    {
        var possiblePositions = GetPossiblePositions(island, mainField, bridges);
        if (possiblePositions.Count == 0)
        {
            return false;
        }

        // Eliminate positions with adjacent islands for better game patterns
        for (var i = possiblePositions.Count - 1; i >= 0; i--)
        {
            var pos = possiblePositions[i];
            if (HasAdjacentIsland(pos.Y, pos.X, mainField))
            {
                possiblePositions.RemoveAt(i);
            }
        }

        if (possiblePositions.Count == 0)
        {
            return false;
        }

        // Choose a random position and create a new island
        var randomPosition = Random.Shared.Next(possiblePositions.Count);
        var position = possiblePositions[randomPosition];
        var newIsland = islandFactory.Invoke(0, position.Y, position.X);
        islands.Add(newIsland);

        // Create a bridge between the islands
        var amountBridges = 1;
        var newBridge = bridgeFactory.Invoke(island, newIsland, amountBridges);
        bridges.Add(newBridge);
        bridges.Add(newBridge.CreateReverseBridgeAndApplyDirections());

        // Update bridge counts
        island.IncrementAmountBridgesConnectable(amountBridges);
        newIsland.IncrementAmountBridgesConnectable(amountBridges);
        mainField[island.Y][island.X] += amountBridges;
        mainField[newIsland.Y][newIsland.X] += amountBridges;

        // Update bridge direction cache based on relative position
        blockDetectionService.UpdateDirectionCache(island, newIsland);

        return true;
    }

    /// <inheritdoc />
    public bool HasAdjacentIsland(int row, int col, int[][] mainField)
    {
        var numRows = mainField.Length;
        var numCols = mainField[0].Length;

        if (row > 0 && mainField[row - 1][col] != 0)
        {
            return true;
        }

        if (row < numRows - 1 && mainField[row + 1][col] != 0)
        {
            return true;
        }

        if (col > 0 && mainField[row][col - 1] != 0)
        {
            return true;
        }

        if (col < numCols - 1 && mainField[row][col + 1] != 0)
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public int[][] InitializeField(int sizeLength, int sizeWidth)
    {
        var field = new int[sizeLength][];
        for (var i = 0; i < sizeLength; i++)
        {
            field[i] = new int[sizeWidth];
        }

        return field;
    }

    internal List<Point> GetPossiblePositions(IIsland island, int[][] mainField, IList<IBridge> bridges)
    {
        var range = Random.Shared.Next(GeneratorConstants.MinIslandRange, GeneratorConstants.MaxIslandRange);
        var result = new List<Point>();

        foreach (var direction in new[] { DirectionEnum.Up, DirectionEnum.Left, DirectionEnum.Down, DirectionEnum.Right })
        {
            result.AddRange(GetPositionsInDirection(island, mainField, bridges, direction, range));
        }

        return result;
    }

    internal IEnumerable<Point> GetPositionsInDirection(
        IIsland island, int[][] mainField, IList<IBridge> bridges, DirectionEnum direction, int range)
    {
        var sizeLength = mainField.Length;
        var sizeWidth = mainField[0].Length;

        var isVertical = direction is DirectionEnum.Up or DirectionEnum.Down;
        var isNegative = direction is DirectionEnum.Up or DirectionEnum.Left;

        var position = isVertical ? island.Y : island.X;
        var maxBound = isVertical ? sizeLength : sizeWidth;

        if (isNegative && position <= 0 || !isNegative && position >= maxBound - 1)
        {
            yield break;
        }

        if (blockDetectionService.HasBridgeInDirection(island, direction, bridges))
        {
            yield break;
        }

        var block = blockDetectionService.GetBlocked(island, mainField, direction, bridges);
        var step = isNegative ? -1 : 1;
        var limit = isNegative
            ? Math.Max(0, position - range)
            : Math.Min(maxBound - 1, position + range);

        for (var i = position + step; isNegative ? i >= limit : i <= limit; i += step)
        {
            var fieldValue = isVertical ? mainField[i][island.X] : mainField[island.Y][i];
            if (fieldValue != 0)
            {
                break;
            }

            if (block == -1 || (isNegative ? i > block : i < block))
            {
                yield return isVertical ? new Point(island.X, i) : new Point(i, island.Y);
            }
        }
    }
}
