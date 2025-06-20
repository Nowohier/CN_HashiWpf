using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.LinearSolver;
using Hashi.LinearSolver.Interfaces;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Hashi.Generator;

/// <summary>
///     Generates a Hashi field.
/// </summary>
public class HashiGenerator : IHashiGenerator
{
    private readonly Func<int, int, int, IIsland> islandFactory;
    private readonly Func<IIsland, IIsland, int, IBridge> bridgeFactory;
    private readonly Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider> solutionContainerFactory;
    private readonly ILinearSolutionSolverWithIterativ linearSolutionSolver;
    private readonly List<IBridge> bridges = [];
    private readonly List<IIsland> islands = [];
    private readonly Random random = new();

    // Cache structures to reduce redundant calculations
    private readonly Dictionary<(int, int), int> blockCache = new(64);
    private readonly Dictionary<(IIsland, Direction), bool> bridgeDirectionCache = new(64);

    /// <summary>
    /// Constructor for HashiGenerator.
    /// </summary>
    public HashiGenerator(
        Func<int, int, int, IIsland> islandFactory,
        Func<IIsland, IIsland, int, IBridge> bridgeFactory,
        Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider> solutionContainerFactory,
        ILinearSolutionSolverWithIterativ linearSolutionSolverWithIterativ)
    {
        this.islandFactory = islandFactory;
        this.bridgeFactory = bridgeFactory;
        this.solutionContainerFactory = solutionContainerFactory;
        linearSolutionSolver = linearSolutionSolverWithIterativ;
    }

    /// <inheritdoc />
    public async Task<ISolutionProvider> GenerateHashAsync(int difficulty = -1, int amountNodes = 10, int width = 0,
        int length = 0, int alpha = 0, int beta = 0)
    {
        if (difficulty >= 0)
        {
            return await GenerateWithDifficultyAsync(difficulty);
        }

        return await GenerateHashAsync(amountNodes, length, width, alpha, beta, true);
    }

    /// <summary>
    ///     Gets the bridges of the Hashi field.
    /// </summary>
    /// <returns>a list of bridge models.</returns>
    public List<IBridge> GetBridges() => bridges;

    private async Task<ISolutionProvider> GenerateWithDifficultyAsync(int difficulty)
    {
        if (difficulty < 0 || difficulty > 9)
            throw new ArgumentException("Invalid difficulty level.");

        // Preconfigured settings - using array lookup for better performance than switch
        var settings = GetDifficultySettings(difficulty);

        var sizeLength = random.Next(settings.minLength, settings.maxLength);
        var sizeWidth = random.Next(settings.minWidth, settings.maxWidth);
        var n = (int)Math.Round(sizeWidth * sizeLength / (double)settings.divisor);

        return await GenerateHashAsync(n, sizeLength, sizeWidth, difficulty, settings.beta, false);
    }

    // Static settings lookup for better performance
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int minLength, int maxLength, int minWidth, int maxWidth, int divisor, int alpha, int beta) GetDifficultySettings(int difficulty)
    {
        // Create once and reuse for better performance
        return difficulty switch
        {
            0 => (5, 10, 5, 10, 4, 25, 20),
            1 => (14, 16, 14, 16, 4, 50, 20),
            2 => (10, 16, 10, 16, 3, 75, 20),
            3 => (11, 18, 11, 18, 3, 25, 15),
            4 => (10, 18, 10, 18, 3, 50, 15),
            5 => (13, 18, 13, 18, 3, 75, 15),
            6 => (15, 20, 15, 20, 3, 25, 10),
            7 => (14, 20, 14, 20, 3, 50, 10),
            8 => (16, 31, 16, 31, 3, 75, 10),
            9 => (20, 31, 20, 31, 3, 100, 0),
            _ => throw new ArgumentException("Invalid difficulty level.")
        };
    }

    /// <summary>
    ///     Generates a Hashi field.
    /// </summary>
    /// <param name="numberOfIslands">The amount of islands to be created.</param>
    /// <param name="sizeLength">The length of the main field array.</param>
    /// <param name="sizeWidth">The width of the main field array.</param>
    /// <param name="difficulty">The difficulty (0-9).</param>
    /// <param name="beta">The beta value.</param>
    /// <param name="checkDifficulty">Determines if the difficulty should be checked.</param>
    /// <returns>a valid hashi field array with one possible solution.</returns>
    private async Task<ISolutionProvider> GenerateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth,
        int difficulty, int beta, bool checkDifficulty)
    {
        int[][] field;
        int attempts = 0;
        const int maxAttempts = 5; // Maximum number of attempts before changing strategy

        do
        {
            // Clear caches with each new attempt
            blockCache.Clear();
            bridgeDirectionCache.Clear();

            field = await CreateHashAsync(numberOfIslands, sizeLength, sizeWidth, difficulty, beta, checkDifficulty);
            attempts++;

            // If solver keeps failing, try a different generation strategy instead of reducing field size
            if (attempts >= maxAttempts)
            {
                // Reset attempt counter but vary the generation parameters slightly
                attempts = 0;
                beta = Math.Max(0, beta - 5); // Reduce bridge complexity slightly
            }
        } while (GetHashiSolveStatus(field) == SolverStatusEnum.Infeasible);
        //while (await linearSolutionSolver.SolveAsync(field) == SolverStatusEnum.Infeasible);

        // Optimize bridge coordinate creation with direct list allocation
        var bridgeCoordinates = new List<IBridgeCoordinates>(bridges.Count);
        foreach (var bridge in bridges)
        {
            bridgeCoordinates.Add(new BridgeCoordinates(
                new Point(bridge.Island1.X, bridge.Island1.Y),
                new Point(bridge.Island2.X, bridge.Island2.Y),
                bridge.AmountBridgesSet));
        }

        if (Debugger.IsAttached)
        {
            Debug.WriteLine(string.Empty);
            Debug.WriteLine($"Number of islands: {islands.Count}");
            Debug.WriteLine(string.Join("\n", field.Select(row => $"{{{string.Join(", ", row)}}}")));
        }

        return solutionContainerFactory.Invoke(field, bridgeCoordinates);
    }

    private SolverStatusEnum GetHashiSolveStatus(int[][] field)
    {
        var convertedData = HashiSolver.ConvertData(field);
        return HashiSolver.SolveLazy(convertedData.Item1, convertedData.Item2);
    }

    private async Task<int[][]> CreateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth, int difficulty,
        int beta, bool checkDifficulty)
    {
        return await Task.Run(() =>
        {
            bridges.Clear();
            islands.Clear();

            // Initialize field with zeros (using more efficient array initialization)
            var mainField = InitializeField(sizeLength, sizeWidth);

            // Create first island at random position
            var row = random.Next(sizeLength);
            var col = random.Next(sizeWidth);
            islands.Add(islandFactory.Invoke(0, row, col));
            var edgeCount = 0;
            var iterationCount = 0;
            const int maxIterations = 100; // Prevent infinite loops

            // Generate islands and bridges until we have enough
            while (iterationCount++ < maxIterations)
            {
                var size = islands.Count;
                var islandsAdded = false;

                // Create islands from existing ones
                for (var i = 0; i < size; i++)
                {
                    if (CreateIsland(mainField, islands[i]))
                    {
                        islandsAdded = true;
                    }

                    if (islands.Count >= numberOfIslands)
                        break;
                }

                if ((!islandsAdded && edgeCount == bridges.Count) || islands.Count >= numberOfIslands)
                    break;

                edgeCount = bridges.Count;
            }

            // Set all neighbors for each island - performance improvement with Parallel.ForEach for large grids
            if (islands.Count > 20) // Only parallelize for larger grids
            {
                Parallel.ForEach(islands, node =>
                {
                    node.SetAllNeighbors(mainField, islands);
                });
            }
            else
            {
                foreach (var node in islands)
                    node.SetAllNeighbors(mainField, islands);
            }

            // Apply difficulty settings
            if (!checkDifficulty)
            {
                // Apply bridges based on difficulty
                int alphaValue = GetAlphaForDifficulty(difficulty);
                AddAdditionalBridges(mainField, alphaValue);

                // Set beta based on difficulty range
                int betaValue = GetBetaForDifficulty(difficulty);
                SetBeta(mainField, betaValue);
            }
            else
            {
                AddAdditionalBridges(mainField, difficulty);
                SetBeta(mainField, beta);
            }

            return mainField;
        });
    }

    // Optimized field initialization
    private static int[][] InitializeField(int sizeLength, int sizeWidth)
    {
        var field = new int[sizeLength][];
        for (var i = 0; i < sizeLength; i++)
            field[i] = new int[sizeWidth]; // Default values are already 0
        return field;
    }

    // Alpha lookup with inline optimization
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetAlphaForDifficulty(int difficulty)
    {
        return difficulty switch
        {
            0 or 3 or 6 => 25,
            1 or 4 or 7 => 50,
            2 or 5 or 8 => 75,
            9 => 100,
            _ => 0
        };
    }

    // Beta lookup with inline optimization
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBetaForDifficulty(int difficulty)
    {
        if (difficulty <= 2) return 20;
        if (difficulty <= 5) return 15;
        if (difficulty <= 8) return 10;
        return 0;
    }

    // Add bridges based on alpha parameter
    private void AddAdditionalBridges(int[][] mainField, int alpha)
    {
        var bridgesAdded = 0;
        var targetBridges = (int)(islands.Count * (alpha / 100.0));

        // Create an index array for random access without shuffling the original island list
        var islandIndices = new int[islands.Count];
        for (int i = 0; i < islandIndices.Length; i++)
            islandIndices[i] = i;

        // Fisher-Yates shuffle for better randomization
        ShuffleArray(islandIndices);

        foreach (var idx in islandIndices)
        {
            if (bridgesAdded >= targetBridges) break;

            var island = islands[idx];

            // Try to add bridges with early exit on success
            if (TryAddBridgeDown(island, mainField))
            {
                bridgesAdded++;
                if (bridgesAdded >= targetBridges) break;
            }

            if (TryAddBridgeRight(island, mainField))
            {
                bridgesAdded++;
            }
        }
    }

    // Efficient Fisher-Yates shuffle
    private void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    private bool TryAddBridgeDown(IIsland island, int[][] mainField)
    {
        // Short-circuit evaluation with most likely conditions first
        if (island.IslandDown == null ||
            island.AmountBridgesDown > 0 ||
            island.AmountBridgesConnectable + 1 > 7 ||
            island.IslandDown.AmountBridgesConnectable + 1 > 7 ||
            GetDownBlockedd(island, mainField) != -1)
        {
            return false;
        }

        var bridge = bridgeFactory.Invoke(island, island.IslandDown, 1);
        bridges.Add(bridge);
        bridges.Add(bridge.AddOtherSide());

        island.AmountBridgesConnectable += 1;
        island.IslandDown.AmountBridgesConnectable += 1;
        mainField[island.Y][island.X] += 1;
        mainField[island.IslandDown.Y][island.X] += 1;

        // Update direction cache
        bridgeDirectionCache[(island, Direction.Down)] = true;

        return true;
    }

    private bool TryAddBridgeRight(IIsland island, int[][] mainField)
    {
        // Short-circuit evaluation with most likely conditions first
        if (island.IslandRight == null ||
            island.AmountBridgesRight > 0 ||
            island.AmountBridgesConnectable + 1 > 7 ||
            island.IslandRight.AmountBridgesConnectable + 1 > 7 ||
            GetRightBlockedd(island, mainField) != -1)
        {
            return false;
        }

        var bridge = bridgeFactory.Invoke(island, island.IslandRight, 1);
        bridges.Add(bridge);
        bridges.Add(bridge.AddOtherSide());

        island.AmountBridgesConnectable += 1;
        island.IslandRight.AmountBridgesConnectable += 1;
        mainField[island.Y][island.X] += 1;
        mainField[island.IslandRight.Y][island.X] += 1;

        // Update direction cache
        bridgeDirectionCache[(island, Direction.Right)] = true;

        return true;
    }

    private bool CreateIsland(int[][] mainField, IIsland island)
    {
        var possiblePositions = GetPossiblePositions(island, mainField);
        if (possiblePositions.Count == 0) return false;

        // Eliminate positions with adjacent islands for better game patterns
        // Iterating backwards for efficient removal
        for (var i = possiblePositions.Count - 1; i >= 0; i--)
        {
            var pos = possiblePositions[i];
            if (HasAdjacentIsland(pos.Y, pos.X, mainField))
            {
                possiblePositions.RemoveAt(i);
            }
        }

        if (possiblePositions.Count == 0) return false;

        // Choose a random position and create a new island
        var randomPosition = random.Next(possiblePositions.Count);
        var position = possiblePositions[randomPosition];
        var newIsland = islandFactory.Invoke(0, position.Y, position.X);
        islands.Add(newIsland);

        // Create a bridge between the islands
        var amountBridges = 1;
        var newBridge = bridgeFactory.Invoke(island, newIsland, amountBridges);
        bridges.Add(newBridge);
        bridges.Add(newBridge.AddOtherSide());

        // Update bridge counts
        island.AmountBridgesConnectable += amountBridges;
        newIsland.AmountBridgesConnectable += amountBridges;
        mainField[island.Y][island.X] += amountBridges;
        mainField[newIsland.Y][newIsland.X] += amountBridges;

        // Update bridge direction cache based on relative position
        UpdateDirectionCache(island, newIsland);

        return true;
    }

    // Update the cache when creating new bridges
    private void UpdateDirectionCache(IIsland island1, IIsland island2)
    {
        if (island1.X == island2.X)
        {
            // Vertical bridge
            if (island1.Y > island2.Y)
            {
                bridgeDirectionCache[(island1, Direction.Up)] = true;
                bridgeDirectionCache[(island2, Direction.Down)] = true;
            }
            else
            {
                bridgeDirectionCache[(island1, Direction.Down)] = true;
                bridgeDirectionCache[(island2, Direction.Up)] = true;
            }
        }
        else
        {
            // Horizontal bridge
            if (island1.X > island2.X)
            {
                bridgeDirectionCache[(island1, Direction.Left)] = true;
                bridgeDirectionCache[(island2, Direction.Right)] = true;
            }
            else
            {
                bridgeDirectionCache[(island1, Direction.Right)] = true;
                bridgeDirectionCache[(island2, Direction.Left)] = true;
            }
        }
    }

    // Optimized method using boundary checks to reduce redundant operations
    private List<Point> GetPossiblePositions(IIsland island, int[][] mainField)
    {
        var result = new List<Point>();
        var range = random.Next(2, 6);
        var sizeLength = mainField.Length;
        var sizeWidth = mainField[0].Length;

        // Check upward direction
        if (island.Y > 0 && !HasBridgeInDirection(island, Direction.Up))
        {
            var block = GetUpBlocked(island, mainField);
            // Optimized range calculations to avoid unnecessary iterations
            var minY = Math.Max(0, island.Y - range);

            for (var y = island.Y - 1; y >= minY; y--)
            {
                if (mainField[y][island.X] != 0) break;
                if (block == -1 || y > block)
                {
                    result.Add(new Point(island.X, y));
                }
            }
        }

        // Check leftward direction
        if (island.X > 0 && !HasBridgeInDirection(island, Direction.Left))
        {
            var block = GetLeftBlocked(island, mainField);
            var minX = Math.Max(0, island.X - range);

            for (var x = island.X - 1; x >= minX; x--)
            {
                if (mainField[island.Y][x] != 0) break;
                if (block == -1 || x > block)
                {
                    result.Add(new Point(x, island.Y));
                }
            }
        }

        // Check downward direction
        if (island.Y < sizeLength - 1 && !HasBridgeInDirection(island, Direction.Down))
        {
            var block = GetDownBlocked(island, mainField);
            var maxY = Math.Min(sizeLength - 1, island.Y + range);

            for (var y = island.Y + 1; y <= maxY; y++)
            {
                if (mainField[y][island.X] != 0) break;
                if (block == -1 || y < block)
                {
                    result.Add(new Point(island.X, y));
                }
            }
        }

        // Check rightward direction
        if (island.X < sizeWidth - 1 && !HasBridgeInDirection(island, Direction.Right))
        {
            var block = GetRightBlocked(island, mainField);
            var maxX = Math.Min(sizeWidth - 1, island.X + range);

            for (var x = island.X + 1; x <= maxX; x++)
            {
                if (mainField[island.Y][x] != 0) break;
                if (block == -1 || x < block)
                {
                    result.Add(new Point(x, island.Y));
                }
            }
        }

        return result;
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Use the cache for direction checks before resorting to slower LINQ operations
    private bool HasBridgeInDirection(IIsland island, Direction direction)
    {
        if (bridgeDirectionCache.TryGetValue((island, direction), out bool cached))
            return cached;

        // Only perform LINQ if not in cache
        bool result = direction switch
        {
            Direction.Up => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.Y < island.Y && b.Island2.X == island.X),

            Direction.Down => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.Y > island.Y && b.Island2.X == island.X),

            Direction.Left => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.X < island.X && b.Island2.Y == island.Y),

            Direction.Right => bridges.Any(b =>
                b.Island1.Y == island.Y && b.Island1.X == island.X &&
                b.Island2.X > island.X && b.Island2.Y == island.Y),

            _ => false
        };

        // Update cache with result
        bridgeDirectionCache[(island, direction)] = result;
        return result;
    }

    // Optimized version with early returns
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool HasAdjacentIsland(int row, int col, int[][] mainField)
    {
        var numRows = mainField.Length;
        var numCols = mainField[0].Length;

        // Check in all four directions with boundary checks
        if (row > 0 && mainField[row - 1][col] != 0) return true;
        if (row < numRows - 1 && mainField[row + 1][col] != 0) return true;
        if (col > 0 && mainField[row][col - 1] != 0) return true;
        if (col < numCols - 1 && mainField[row][col + 1] != 0) return true;

        return false;
    }

    // Use cache for blocked direction checks
    private int GetUpBlocked(IIsland mainIsland, int[][] mainField)
    {
        var key = (mainIsland.X, -mainIsland.Y); // Negative Y for up direction
        if (blockCache.TryGetValue(key, out int value))
            return value;

        int result = CalculateUpBlocked(mainIsland, mainField);
        blockCache[key] = result;
        return result;
    }

    private int CalculateUpBlocked(IIsland mainIsland, int[][] mainField)
    {
        // Early exit if no bridges
        if (bridges.Count == 0) return -1;

        // Use array bounds to optimize loop
        int minRow = Math.Max(1, mainIsland.Y - 10); // Limit search depth for performance

        for (var row = mainIsland.Y - 1; row >= minRow; row--)
        {
            for (var x = mainIsland.X - 1; x >= 0; x--)
            {
                if (mainField[row][x] == 0) continue;

                // Fast path: check if any horizontal bridge crosses our path
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

    private int GetDownBlocked(IIsland mainIsland, int[][] mainField)
    {
        var key = (mainIsland.X, mainIsland.Y);
        if (blockCache.TryGetValue(key, out int value))
            return value;

        int result = CalculateDownBlocked(mainIsland, mainField);
        blockCache[key] = result;
        return result;
    }

    private int CalculateDownBlocked(IIsland mainIsland, int[][] mainField)
    {
        if (bridges.Count == 0) return -1;

        int maxRow = Math.Min(mainField.Length - 1, mainIsland.Y + 10);

        for (var row = mainIsland.Y + 1; row <= maxRow; row++)
        {
            for (var x = mainIsland.X - 1; x >= 0; x--)
            {
                if (mainField[row][x] == 0) continue;

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

    private int GetRightBlocked(IIsland mainIsland, int[][] mainField)
    {
        var key = (mainIsland.X, mainIsland.Y + 1000); // Offset for unique key
        if (blockCache.TryGetValue(key, out int value))
            return value;

        int result = CalculateRightBlocked(mainIsland, mainField);
        blockCache[key] = result;
        return result;
    }

    private int CalculateRightBlocked(IIsland mainIsland, int[][] mainField)
    {
        if (bridges.Count == 0) return -1;

        int maxCol = Math.Min(mainField[mainIsland.Y].Length - 1, mainIsland.X + 10);

        for (var col = mainIsland.X + 1; col <= maxCol; col++)
        {
            for (var y = mainIsland.Y - 1; y >= 0; y--)
            {
                if (mainField[y][col] == 0) continue;

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

    private int GetLeftBlocked(IIsland mainIsland, int[][] mainField)
    {
        var key = (-mainIsland.X, mainIsland.Y + 1000); // Negative X for left direction
        if (blockCache.TryGetValue(key, out int value))
            return value;

        int result = CalculateLeftBlocked(mainIsland, mainField);
        blockCache[key] = result;
        return result;
    }

    private int CalculateLeftBlocked(IIsland mainIsland, int[][] mainField)
    {
        if (bridges.Count == 0) return -1;

        int minCol = Math.Max(1, mainIsland.X - 10);

        for (var col = mainIsland.X - 1; col >= minCol; col--)
        {
            for (var y = mainIsland.Y - 1; y >= 0; y--)
            {
                if (mainField[y][col] == 0) continue;

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

    private int GetDownBlockedd(IIsland mainIsland, int[][] mainField)
    {
        if (mainIsland.IslandDown == null) return -1;

        var key = (mainIsland.X, mainIsland.Y + 2000); // Unique key with offset
        if (blockCache.TryGetValue(key, out int value))
            return value;

        int result = -1;

        // Search only in the range between the islands for better performance
        for (var row = mainIsland.Y + 1; row < mainIsland.IslandDown.Y; row++)
        {
            bool foundBlocking = false;
            for (var x = mainIsland.X - 1; x >= 0; x--)
            {
                if (mainField[row][x] == 0) continue;

                if (bridges.Any(bridge =>
                    bridge.Island1.Y == row &&
                    bridge.Island1.X == x &&
                    bridge.Island2.X > mainIsland.X))
                {
                    result = row;
                    foundBlocking = true;
                    break;
                }
            }

            if (foundBlocking) break;
        }

        blockCache[key] = result;
        return result;
    }

    private int GetRightBlockedd(IIsland mainIsland, int[][] mainField)
    {
        if (mainIsland.IslandRight == null) return -1;

        var key = (mainIsland.X, mainIsland.Y + 3000); // Unique key with offset
        if (blockCache.TryGetValue(key, out int value))
            return value;

        int result = -1;

        // Search only in the range between the islands
        for (var col = mainIsland.X + 1; col < mainIsland.IslandRight.X; col++)
        {
            bool foundBlocking = false;
            for (var y = mainIsland.Y - 1; y >= 0; y--)
            {
                if (mainField[y][col] == 0) continue;

                if (bridges.Any(bridge =>
                    bridge.Island1.Y == y &&
                    bridge.Island1.X == col &&
                    bridge.Island2.Y > mainIsland.Y))
                {
                    result = col;
                    foundBlocking = true;
                    break;
                }
            }

            if (foundBlocking) break;
        }

        blockCache[key] = result;
        return result;
    }

    private void SetBeta(int[][] mainField, int beta)
    {
        // Skip if beta is zero or too low
        if (beta <= 0) return;

        // Calculate exact number of bridges to add for better efficiency
        int bridgesToAdd = (int)Math.Ceiling(bridges.Count * 0.5 * (beta / 100.0));
        if (bridgesToAdd <= 0) return;

        // Get candidate bridges (every other one to avoid duplicates)
        var candidates = new List<int>(bridges.Count / 2);
        for (var i = bridges.Count - 1; i > 0; i -= 2)
            candidates.Add(i);

        // Shuffle candidates for better distribution
        ShuffleList(candidates);

        // Add additional bridges up to the calculated count
        for (int i = 0; i < Math.Min(bridgesToAdd, candidates.Count); i++)
        {
            bridges[candidates[i]].AddBridge(mainField);
        }
    }

    /// Fisher-Yates shuffle for lists
    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

