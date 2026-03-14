using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Services;

/// <inheritdoc />
public class BlockDetectionService : IBlockDetectionService
{
    private readonly Dictionary<(int X, int Y, DirectionEnum dir, bool isBetween), int> blockCache = new(64);
    private readonly Dictionary<(IIsland, DirectionEnum), bool> bridgeDirectionCache = new(64);

    /// <inheritdoc />
    public int GetBlocked(IIsland mainIsland, int[][] mainField, DirectionEnum direction, IList<IBridge> bridges)
    {
        var key = (mainIsland.X, mainIsland.Y, direction, false);
        if (blockCache.TryGetValue(key, out var value))
        {
            return value;
        }

        var result = CalculateBlocked(mainIsland, mainField, direction, bridges);
        blockCache[key] = result;
        return result;
    }

    /// <inheritdoc />
    public int GetDownBlockedBetween(IIsland mainIsland, int[][] mainField, IList<IBridge> bridges)
    {
        if (mainIsland.IslandDown == null)
        {
            return -1;
        }

        return GetBlockedBetween(mainIsland, mainField, DirectionEnum.Down, bridges);
    }

    /// <inheritdoc />
    public int GetRightBlockedBetween(IIsland mainIsland, int[][] mainField, IList<IBridge> bridges)
    {
        if (mainIsland.IslandRight == null)
        {
            return -1;
        }

        return GetBlockedBetween(mainIsland, mainField, DirectionEnum.Right, bridges);
    }

    /// <inheritdoc />
    public bool HasBridgeInDirection(IIsland island, DirectionEnum direction, IList<IBridge> bridges)
    {
        if (bridgeDirectionCache.TryGetValue((island, direction), out var cached))
        {
            return cached;
        }

        var result = direction switch
        {
            DirectionEnum.Up => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.Y < island.Y && b.Island2.X == island.X),

            DirectionEnum.Down => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.Y > island.Y && b.Island2.X == island.X),

            DirectionEnum.Left => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.X < island.X && b.Island2.Y == island.Y),

            DirectionEnum.Right => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.X > island.X && b.Island2.Y == island.Y),

            _ => false
        };

        bridgeDirectionCache[(island, direction)] = result;
        return result;
    }

    /// <summary>
    ///     Updates the bridge direction cache when creating new bridges.
    /// </summary>
    public void UpdateDirectionCache(IIsland island1, IIsland island2)
    {
        if (island1.X == island2.X)
        {
            if (island1.Y > island2.Y)
            {
                bridgeDirectionCache[(island1, DirectionEnum.Up)] = true;
                bridgeDirectionCache[(island2, DirectionEnum.Down)] = true;
            }
            else
            {
                bridgeDirectionCache[(island1, DirectionEnum.Down)] = true;
                bridgeDirectionCache[(island2, DirectionEnum.Up)] = true;
            }
        }
        else
        {
            if (island1.X > island2.X)
            {
                bridgeDirectionCache[(island1, DirectionEnum.Left)] = true;
                bridgeDirectionCache[(island2, DirectionEnum.Right)] = true;
            }
            else
            {
                bridgeDirectionCache[(island1, DirectionEnum.Right)] = true;
                bridgeDirectionCache[(island2, DirectionEnum.Left)] = true;
            }
        }
    }

    /// <inheritdoc />
    public void ClearCaches()
    {
        blockCache.Clear();
        bridgeDirectionCache.Clear();
    }

    private int CalculateBlocked(IIsland mainIsland, int[][] mainField, DirectionEnum direction, IList<IBridge> bridges)
    {
        if (bridges.Count == 0)
        {
            return -1;
        }

        return direction switch
        {
            DirectionEnum.Up => CalculateVerticalBlocked(mainIsland, mainField, mainIsland.Y - 1,
                Math.Max(1, mainIsland.Y - GeneratorConstants.BlockedSearchRadius), -1, bridges),
            DirectionEnum.Down => CalculateVerticalBlocked(mainIsland, mainField, mainIsland.Y + 1,
                Math.Min(mainField.Length - 1, mainIsland.Y + GeneratorConstants.BlockedSearchRadius), 1, bridges),
            DirectionEnum.Left => CalculateHorizontalBlocked(mainIsland, mainField, mainIsland.X - 1,
                Math.Max(1, mainIsland.X - GeneratorConstants.BlockedSearchRadius), -1, bridges),
            DirectionEnum.Right => CalculateHorizontalBlocked(mainIsland, mainField, mainIsland.X + 1,
                Math.Min(mainField[mainIsland.Y].Length - 1, mainIsland.X + GeneratorConstants.BlockedSearchRadius), 1,
                bridges),
            _ => -1
        };
    }

    private static int CalculateVerticalBlocked(IIsland mainIsland, int[][] mainField, int start, int limit, int step,
        IList<IBridge> bridges)
    {
        for (var row = start; step > 0 ? row <= limit : row >= limit; row += step)
        {
            for (var x = mainIsland.X - 1; x >= 0; x--)
            {
                if (mainField[row][x] == 0)
                {
                    continue;
                }

                foreach (var bridge in bridges)
                {
                    if (bridge.Island1.Y == row &&
                        bridge.Island1.X == x &&
                        bridge.Island2.X > mainIsland.X)
                    {
                        return row;
                    }
                }
            }
        }

        return -1;
    }

    private static int CalculateHorizontalBlocked(IIsland mainIsland, int[][] mainField, int start, int limit,
        int step, IList<IBridge> bridges)
    {
        for (var col = start; step > 0 ? col <= limit : col >= limit; col += step)
        {
            for (var y = mainIsland.Y - 1; y >= 0; y--)
            {
                if (mainField[y][col] == 0)
                {
                    continue;
                }

                foreach (var bridge in bridges)
                {
                    if (bridge.Island1.Y == y &&
                        bridge.Island1.X == col &&
                        bridge.Island2.Y > mainIsland.Y)
                    {
                        return col;
                    }
                }
            }
        }

        return -1;
    }

    private int GetBlockedBetween(IIsland mainIsland, int[][] mainField, DirectionEnum direction,
        IList<IBridge> bridges)
    {
        var key = (mainIsland.X, mainIsland.Y, direction, true);
        if (blockCache.TryGetValue(key, out var value))
        {
            return value;
        }

        var result = direction == DirectionEnum.Down
            ? CalculateVerticalBlockedBetween(mainIsland, mainField, bridges)
            : CalculateHorizontalBlockedBetween(mainIsland, mainField, bridges);

        blockCache[key] = result;
        return result;
    }

    private static int CalculateVerticalBlockedBetween(IIsland mainIsland, int[][] mainField, IList<IBridge> bridges)
    {
        return CalculateVerticalBlocked(mainIsland, mainField, mainIsland.Y + 1, mainIsland.IslandDown!.Y - 1, 1,
            bridges);
    }

    private static int CalculateHorizontalBlockedBetween(IIsland mainIsland, int[][] mainField, IList<IBridge> bridges)
    {
        return CalculateHorizontalBlocked(mainIsland, mainField, mainIsland.X + 1, mainIsland.IslandRight!.X - 1, 1,
            bridges);
    }
}
