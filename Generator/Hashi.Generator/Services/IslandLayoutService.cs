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
    private readonly IBlockDetectionService blockDetectionService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IslandLayoutService" /> class.
    /// </summary>
    public IslandLayoutService(
        Func<int, int, int, IIsland> islandFactory,
        Func<IIsland, IIsland, int, IBridge> bridgeFactory,
        IBlockDetectionService blockDetectionService)
    {
        this.islandFactory = islandFactory;
        this.bridgeFactory = bridgeFactory;
        this.blockDetectionService = blockDetectionService;
    }

    /// <inheritdoc />
    public bool CreateIsland(int[][] mainField, IIsland island, List<IIsland> islands, List<IBridge> bridges)
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
        island.AmountBridgesConnectable += amountBridges;
        newIsland.AmountBridgesConnectable += amountBridges;
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
        var result = new List<Point>();
        var range = Random.Shared.Next(GeneratorConstants.MinIslandRange, GeneratorConstants.MaxIslandRange);
        var sizeLength = mainField.Length;
        var sizeWidth = mainField[0].Length;

        // Check upward direction
        if (island.Y > 0 && !blockDetectionService.HasBridgeInDirection(island, DirectionEnum.Up, bridges))
        {
            var block = blockDetectionService.GetBlocked(island, mainField, DirectionEnum.Up, bridges);
            var minY = Math.Max(0, island.Y - range);

            for (var y = island.Y - 1; y >= minY; y--)
            {
                if (mainField[y][island.X] != 0)
                {
                    break;
                }

                if (block == -1 || y > block)
                {
                    result.Add(new Point(island.X, y));
                }
            }
        }

        // Check leftward direction
        if (island.X > 0 && !blockDetectionService.HasBridgeInDirection(island, DirectionEnum.Left, bridges))
        {
            var block = blockDetectionService.GetBlocked(island, mainField, DirectionEnum.Left, bridges);
            var minX = Math.Max(0, island.X - range);

            for (var x = island.X - 1; x >= minX; x--)
            {
                if (mainField[island.Y][x] != 0)
                {
                    break;
                }

                if (block == -1 || x > block)
                {
                    result.Add(new Point(x, island.Y));
                }
            }
        }

        // Check downward direction
        if (island.Y < sizeLength - 1 &&
            !blockDetectionService.HasBridgeInDirection(island, DirectionEnum.Down, bridges))
        {
            var block = blockDetectionService.GetBlocked(island, mainField, DirectionEnum.Down, bridges);
            var maxY = Math.Min(sizeLength - 1, island.Y + range);

            for (var y = island.Y + 1; y <= maxY; y++)
            {
                if (mainField[y][island.X] != 0)
                {
                    break;
                }

                if (block == -1 || y < block)
                {
                    result.Add(new Point(island.X, y));
                }
            }
        }

        // Check rightward direction
        if (island.X < sizeWidth - 1 &&
            !blockDetectionService.HasBridgeInDirection(island, DirectionEnum.Right, bridges))
        {
            var block = blockDetectionService.GetBlocked(island, mainField, DirectionEnum.Right, bridges);
            var maxX = Math.Min(sizeWidth - 1, island.X + range);

            for (var x = island.X + 1; x <= maxX; x++)
            {
                if (mainField[island.Y][x] != 0)
                {
                    break;
                }

                if (block == -1 || x < block)
                {
                    result.Add(new Point(x, island.Y));
                }
            }
        }

        return result;
    }
}
