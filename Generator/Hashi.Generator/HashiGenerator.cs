using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.LinearSolver.Interfaces;
using Hashi.Logging.Interfaces;
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
    private readonly IHashiSolver hashiSolver;
    private readonly IRuleSolvabilityValidator ruleSolvabilityValidator;
    private readonly ILogger logger;
    private readonly List<IBridge> bridges = [];
    private readonly List<IIsland> islands = [];
    private readonly Random random = new();

    // Cache structures to reduce redundant calculations
    private readonly Dictionary<(int X, int Y, Direction dir), int> blockCache = new(64);
    private readonly Dictionary<(IIsland, Direction), bool> bridgeDirectionCache = new(64);

    /// <summary>
    /// Constructor for HashiGenerator.
    /// </summary>
    public HashiGenerator(
        Func<int, int, int, IIsland> islandFactory,
        Func<IIsland, IIsland, int, IBridge> bridgeFactory,
        Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider> solutionContainerFactory,
        IHashiSolver hashiSolver,
        IRuleSolvabilityValidator ruleSolvabilityValidator,
        ILoggerFactory loggerFactory)
    {
        this.islandFactory = islandFactory;
        this.bridgeFactory = bridgeFactory;
        this.solutionContainerFactory = solutionContainerFactory;
        this.hashiSolver = hashiSolver;
        this.ruleSolvabilityValidator = ruleSolvabilityValidator;
        this.logger = loggerFactory.CreateLogger<HashiGenerator>();
    }

    /// <inheritdoc />
    public async Task<ISolutionProvider> GenerateHashAsync(int difficulty = -1, int amountNodes = 10, int width = 0,
        int length = 0, int alpha = 0, int beta = 0)
    {
        logger.Info($"Starting hash generation - difficulty: {difficulty}, nodes: {amountNodes}, size: {width}x{length}");

        if (difficulty >= 0)
        {
            return await GenerateWithDifficultyAsync(difficulty);
        }

        return await GenerateHashAsync(amountNodes, length, width, alpha, beta, true);
    }

    internal async Task<ISolutionProvider> GenerateWithDifficultyAsync(int difficulty)
    {
        if (difficulty < 0 || difficulty > 9)
        {
            throw new ArgumentException("Invalid difficulty level.");
        }

        // Preconfigured settings - using array lookup for better performance than switch
        var settings = GetDifficultySettings(difficulty);

        var sizeLength = random.Next(settings.minLength, settings.maxLength);
        var sizeWidth = random.Next(settings.minWidth, settings.maxWidth);
        var n = (int)Math.Round(sizeWidth * sizeLength / (double)settings.divisor);

        return await GenerateHashAsync(n, sizeLength, sizeWidth, difficulty, settings.beta, false);
    }

    // Static settings lookup for better performance
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (int minLength, int maxLength, int minWidth, int maxWidth, int divisor, int alpha, int beta) GetDifficultySettings(int difficulty)
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
    internal async Task<ISolutionProvider> GenerateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth,
        int difficulty, int beta, bool checkDifficulty)
    {
        int[][] field;
        var attempts = 0;
        const int maxAttempts = 50; // Maximum number of attempts before changing strategy
        const int maxRuleAttempts = 500; // Maximum attempts to find a rule-solvable puzzle
        var ruleAttempts = 0;

        while (true)
        {
            // Clear caches with each new attempt
            blockCache.Clear();
            bridgeDirectionCache.Clear();

            field = await CreateHashAsync(numberOfIslands, sizeLength, sizeWidth, difficulty, beta, checkDifficulty);
            attempts++;
            ruleAttempts++;

            // Check if the puzzle is solvable by the CP-SAT solver
            if (await hashiSolver.SolveLazy(field) == SolverStatusEnum.Infeasible)
            {
                // If solver keeps failing, try a different generation strategy
                if (attempts >= maxAttempts)
                {
                    attempts = 0;
                    beta = Math.Max(0, beta - 5);
                }

                continue;
            }

            // Build bridge coordinates from current solution
            var candidateBridgeCoordinates = BuildBridgeCoordinates();

            // Validate that the puzzle is fully solvable by hint rules
            if (await ruleSolvabilityValidator.IsFullySolvableByRules(field, candidateBridgeCoordinates))
            {
                logger.Info($"Rule-solvable puzzle found after {ruleAttempts} attempt(s)");
                break;
            }

            logger.Debug($"Puzzle rejected by rule validator (attempt {ruleAttempts})");

            // Gradually simplify if too many attempts fail rule validation
            if (ruleAttempts >= maxRuleAttempts)
            {
                logger.Warn(
                    $"Could not find rule-solvable puzzle after {maxRuleAttempts} attempts. Reducing complexity.");
                ruleAttempts = 0;
                numberOfIslands = Math.Max(4, numberOfIslands - 2);
                sizeLength = Math.Max(5, sizeLength - 1);
                sizeWidth = Math.Max(5, sizeWidth - 1);
            }

            // Reset solver attempt counter for the next generation
            attempts = 0;
        }

        var bridgeCoordinates = BuildBridgeCoordinates();

        if (Debugger.IsAttached)
        {
            logger.Debug($"Number of islands: {islands.Count}");
            logger.Debug("Generated field:");
            foreach (var row in field)
            {
                logger.Debug($"{{{string.Join(", ", row)}}}");
            }
        }

        return solutionContainerFactory.Invoke(field, bridgeCoordinates);
    }

    /// <summary>
    ///     Builds the bridge coordinates from the current bridge list.
    /// </summary>
    /// <returns>A list of bridge coordinates.</returns>
    internal List<IBridgeCoordinates> BuildBridgeCoordinates()
    {
        var bridgeCoordinates = new List<IBridgeCoordinates>(bridges.Count);
        foreach (var bridge in bridges)
        {
            bridgeCoordinates.Add(new BridgeCoordinates(
                new Point(bridge.Island1.X, bridge.Island1.Y),
                new Point(bridge.Island2.X, bridge.Island2.Y),
                bridge.AmountBridgesSet));
        }

        return bridgeCoordinates;
    }

    internal async Task<int[][]> CreateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth, int difficulty,
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
                    {
                        break;
                    }
                }

                if ((!islandsAdded && edgeCount == bridges.Count) || islands.Count >= numberOfIslands)
                {
                    break;
                }

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
                var alphaValue = GetAlphaForDifficulty(difficulty);
                AddAdditionalBridges(mainField, alphaValue);

                // Set beta based on difficulty range
                var betaValue = GetBetaForDifficulty(difficulty);
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
    internal static int[][] InitializeField(int sizeLength, int sizeWidth)
    {
        var field = new int[sizeLength][];
        for (var i = 0; i < sizeLength; i++)
        {
            field[i] = new int[sizeWidth]; // Default values are already 0
        }

        return field;
    }

    // Alpha lookup with inline optimization
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetAlphaForDifficulty(int difficulty)
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
    internal static int GetBetaForDifficulty(int difficulty)
    {
        if (difficulty <= 2)
        {
            return 20;
        }

        if (difficulty <= 5)
        {
            return 15;
        }

        if (difficulty <= 8)
        {
            return 10;
        }

        return 0;
    }

    // Add bridges based on alpha parameter
    internal void AddAdditionalBridges(int[][] mainField, int alpha)
    {
        var bridgesAdded = 0;
        var targetBridges = (int)(islands.Count * (alpha / 100.0));

        // Create an index array for random access without shuffling the original island list
        var islandIndices = new int[islands.Count];
        for (var i = 0; i < islandIndices.Length; i++)
        {
            islandIndices[i] = i;
        }

        // Fisher-Yates shuffle for better randomization
        ShuffleArray(islandIndices);

        foreach (var idx in islandIndices)
        {
            if (bridgesAdded >= targetBridges)
            {
                break;
            }

            var island = islands[idx];

            // Try to add bridges with early exit on success
            if (TryAddBridgeDown(island, mainField))
            {
                bridgesAdded++;
                if (bridgesAdded >= targetBridges)
                {
                    break;
                }
            }

            if (TryAddBridgeRight(island, mainField))
            {
                bridgesAdded++;
            }
        }
    }

    // Efficient Fisher-Yates shuffle
    internal void ShuffleArray<T>(T[] array)
    {
        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    internal bool TryAddBridgeDown(IIsland island, int[][] mainField)
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

    internal bool TryAddBridgeRight(IIsland island, int[][] mainField)
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

    internal bool CreateIsland(int[][] mainField, IIsland island)
    {
        var possiblePositions = GetPossiblePositions(island, mainField);
        if (possiblePositions.Count == 0)
        {
            return false;
        }

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

        if (possiblePositions.Count == 0)
        {
            return false;
        }

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
    internal void UpdateDirectionCache(IIsland island1, IIsland island2)
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
    internal List<Point> GetPossiblePositions(IIsland island, int[][] mainField)
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
        if (island.X > 0 && !HasBridgeInDirection(island, Direction.Left))
        {
            var block = GetLeftBlocked(island, mainField);
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
        if (island.Y < sizeLength - 1 && !HasBridgeInDirection(island, Direction.Down))
        {
            var block = GetDownBlocked(island, mainField);
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
        if (island.X < sizeWidth - 1 && !HasBridgeInDirection(island, Direction.Right))
        {
            var block = GetRightBlocked(island, mainField);
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

    internal enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Use the cache for direction checks before resorting to slower LINQ operations
    internal bool HasBridgeInDirection(IIsland island, Direction direction)
    {
        if (bridgeDirectionCache.TryGetValue((island, direction), out var cached))
        {
            return cached;
        }

        // Only perform LINQ if not in cache
        var result = direction switch
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
    internal bool HasAdjacentIsland(int row, int col, int[][] mainField)
    {
        var numRows = mainField.Length;
        var numCols = mainField[0].Length;

        // Check in all four directions with boundary checks
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

    // Unified cache lookup for blocked direction checks
    internal int GetBlocked(IIsland mainIsland, int[][] mainField, Direction direction)
    {
        var key = (mainIsland.X, mainIsland.Y, direction);
        if (blockCache.TryGetValue(key, out var value))
        {
            return value;
        }

        var result = CalculateBlocked(mainIsland, mainField, direction);
        blockCache[key] = result;
        return result;
    }

    internal int CalculateBlocked(IIsland mainIsland, int[][] mainField, Direction direction)
    {
        if (bridges.Count == 0)
        {
            return -1;
        }

        return direction switch
        {
            Direction.Up => CalculateVerticalBlocked(mainIsland, mainField, mainIsland.Y - 1, Math.Max(1, mainIsland.Y - 10), -1),
            Direction.Down => CalculateVerticalBlocked(mainIsland, mainField, mainIsland.Y + 1, Math.Min(mainField.Length - 1, mainIsland.Y + 10), 1),
            Direction.Left => CalculateHorizontalBlocked(mainIsland, mainField, mainIsland.X - 1, Math.Max(1, mainIsland.X - 10), -1),
            Direction.Right => CalculateHorizontalBlocked(mainIsland, mainField, mainIsland.X + 1, Math.Min(mainField[mainIsland.Y].Length - 1, mainIsland.X + 10), 1),
            _ => -1
        };
    }

    private int CalculateVerticalBlocked(IIsland mainIsland, int[][] mainField, int start, int limit, int step)
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

    private int CalculateHorizontalBlocked(IIsland mainIsland, int[][] mainField, int start, int limit, int step)
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

    // Convenience wrappers for callers
    internal int GetUpBlocked(IIsland mainIsland, int[][] mainField) => GetBlocked(mainIsland, mainField, Direction.Up);
    internal int GetDownBlocked(IIsland mainIsland, int[][] mainField) => GetBlocked(mainIsland, mainField, Direction.Down);
    internal int GetLeftBlocked(IIsland mainIsland, int[][] mainField) => GetBlocked(mainIsland, mainField, Direction.Left);
    internal int GetRightBlocked(IIsland mainIsland, int[][] mainField) => GetBlocked(mainIsland, mainField, Direction.Right);

    internal int GetDownBlockedd(IIsland mainIsland, int[][] mainField)
    {
        if (mainIsland.IslandDown == null)
        {
            return -1;
        }

        return GetBlockedBetween(mainIsland, mainField, Direction.Down);
    }

    internal int GetRightBlockedd(IIsland mainIsland, int[][] mainField)
    {
        if (mainIsland.IslandRight == null)
        {
            return -1;
        }

        return GetBlockedBetween(mainIsland, mainField, Direction.Right);
    }

    // Unified between-islands blocking check
    private int GetBlockedBetween(IIsland mainIsland, int[][] mainField, Direction direction)
    {
        // Use distinct cache keys for between-island checks by adding offset to direction enum
        var cacheDir = direction == Direction.Down ? (Direction)10 : (Direction)11;
        var key = (mainIsland.X, mainIsland.Y, cacheDir);
        if (blockCache.TryGetValue(key, out var value))
        {
            return value;
        }

        var result = direction == Direction.Down
            ? CalculateVerticalBlockedBetween(mainIsland, mainField)
            : CalculateHorizontalBlockedBetween(mainIsland, mainField);

        blockCache[key] = result;
        return result;
    }

    private int CalculateVerticalBlockedBetween(IIsland mainIsland, int[][] mainField)
    {
        for (var row = mainIsland.Y + 1; row < mainIsland.IslandDown!.Y; row++)
        {
            for (var x = mainIsland.X - 1; x >= 0; x--)
            {
                if (mainField[row][x] == 0)
                {
                    continue;
                }

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == row &&
                        bridge.Island1.X == x &&
                        bridge.Island2.X > mainIsland.X))
                {
                    return row;
                }
            }
        }

        return -1;
    }

    private int CalculateHorizontalBlockedBetween(IIsland mainIsland, int[][] mainField)
    {
        for (var col = mainIsland.X + 1; col < mainIsland.IslandRight!.X; col++)
        {
            for (var y = mainIsland.Y - 1; y >= 0; y--)
            {
                if (mainField[y][col] == 0)
                {
                    continue;
                }

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == y &&
                        bridge.Island1.X == col &&
                        bridge.Island2.Y > mainIsland.Y))
                {
                    return col;
                }
            }
        }

        return -1;
    }

    internal void SetBeta(int[][] mainField, int beta)
    {
        // Skip if beta is zero or too low
        if (beta <= 0)
        {
            return;
        }

        // Calculate exact number of bridges to add for better efficiency
        var bridgesToAdd = (int)Math.Ceiling(bridges.Count * 0.5 * (beta / 100.0));
        if (bridgesToAdd <= 0)
        {
            return;
        }

        // Get candidate bridges (every other one to avoid duplicates)
        var candidates = new List<int>(bridges.Count / 2);
        for (var i = bridges.Count - 1; i > 0; i -= 2)
        {
            candidates.Add(i);
        }

        // Shuffle candidates for better distribution
        ShuffleList(candidates);

        // Add additional bridges up to the calculated count
        for (var i = 0; i < Math.Min(bridgesToAdd, candidates.Count); i++)
        {
            bridges[candidates[i]].AddBridge(mainField);
        }
    }

    /// Fisher-Yates shuffle for lists
    internal void ShuffleList(List<int> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

