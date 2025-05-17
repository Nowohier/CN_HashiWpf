using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.LinearSolver.Interfaces;
using System.Diagnostics;
using System.Drawing;

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

        // Preconfigured settings for different difficulty levels
        (int minLength, int maxLength, int minWidth, int maxWidth, int divisor, int alpha, int beta) settings =
            difficulty switch
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

        var sizeLength = random.Next(settings.minLength, settings.maxLength);
        var sizeWidth = random.Next(settings.minWidth, settings.maxWidth);
        var n = (int)Math.Round(sizeWidth * sizeLength / (double)settings.divisor);

        return await GenerateHashAsync(n, sizeLength, sizeWidth, difficulty, settings.beta, false);
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

        do
        {
            field = await CreateHashAsync(numberOfIslands, sizeLength, sizeWidth, difficulty, beta, checkDifficulty);
        }
        while (await linearSolutionSolver.SolveAsync(field) == SolverStatusEnum.Infeasible);

        var bridgeCoordinates = bridges
            .Select(x => new BridgeCoordinates(
                new Point(x.Island1.X, x.Island1.Y),
                new Point(x.Island2.X, x.Island2.Y),
                x.AmountBridgesSet))
            .ToList<IBridgeCoordinates>();

        Debug.WriteLine(string.Empty);
        Debug.WriteLine(string.Join("\nNumberOfIslands", field.Select(row => $"{{{string.Join(", ", row)}}}")));

        return solutionContainerFactory.Invoke(field, bridgeCoordinates);
    }

    private async Task<int[][]> CreateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth, int difficulty,
        int beta, bool checkDifficulty)
    {
        return await Task.Run(() =>
        {
            bridges.Clear();
            islands.Clear();

            // Initialize field with zeros
            var mainField = new int[sizeLength][];
            for (var i = 0; i < sizeLength; i++)
                mainField[i] = new int[sizeWidth];

            // Create first island at random position
            var row = random.Next(sizeLength);
            var col = random.Next(sizeWidth);
            islands.Add(islandFactory.Invoke(0, row, col));
            var edgeCount = 0;

            // Generate islands and bridges until we have enough
            while (true)
            {
                var size = islands.Count;
                for (var i = 0; i < size; i++)
                {
                    mainField = CreateIsland(mainField, islands[i]);
                    if (islands.Count == numberOfIslands) break;
                }

                if (edgeCount == bridges.Count || islands.Count == numberOfIslands) break;
                edgeCount = bridges.Count;
            }

            // Set all neighbors for each island
            foreach (var node in islands)
                node.SetAllNeighbors(mainField, islands);

            // Apply difficulty settings
            if (!checkDifficulty)
            {
                // Create bridges based on difficulty
                int alphaValue = difficulty switch
                {
                    0 or 3 or 6 => 25,
                    1 or 4 or 7 => 50,
                    2 or 5 or 8 => 75,
                    9 => 100,
                    _ => 0
                };

                mainField = CreateNewBridges(mainField, alphaValue);

                // Set beta based on difficulty range
                int betaValue = difficulty switch
                {
                    <= 2 => 20,
                    <= 5 => 15,
                    <= 8 => 10,
                    _ => 0
                };

                SetBeta(mainField, betaValue);
            }
            else
            {
                mainField = CreateNewBridges(mainField, difficulty);
                SetBeta(mainField, beta);
            }

            return mainField;
        });
    }

    private int[][] CreateNewBridges(int[][] mainField, int alpha)
    {
        var bridgesAdded = 0;
        var targetBridges = (int)(islands.Count * (alpha / 100.0));

        for (var i = 0; i < islands.Count && bridgesAdded < targetBridges; i++)
        {
            var island = islands[i];

            // Try to add a bridge downward
            if (TryAddBridgeDown(island, mainField))
            {
                bridgesAdded++;
                if (bridgesAdded >= targetBridges) break;
            }

            // Try to add a bridge rightward
            if (TryAddBridgeRight(island, mainField))
            {
                bridgesAdded++;
            }
        }

        return mainField;
    }

    private bool TryAddBridgeDown(IIsland island, int[][] mainField)
    {
        if (island.IslandDown == null ||
            DownBlockedd(island, mainField) != -1 ||
            island.AmountBridgesDown > 0 ||
            island.AmountBridgesConnectable + 1 > 7 ||
            island.IslandDown.AmountBridgesConnectable + 1 > 7)
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

        return true;
    }

    private bool TryAddBridgeRight(IIsland island, int[][] mainField)
    {
        if (island.IslandRight == null ||
            RightBlockedd(island, mainField) != -1 ||
            island.AmountBridgesRight > 0 ||
            island.AmountBridgesConnectable + 1 > 7 ||
            island.IslandRight.AmountBridgesConnectable + 1 > 7)
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

        return true;
    }

    private int[][] CreateIsland(int[][] mainField, IIsland island)
    {
        var possiblePositions = GetPossiblePositions(island, mainField);

        // Remove positions that have occupied surrounding fields
        for (var i = possiblePositions.Count - 1; i >= 0; i--)
            if (CheckSurroundingFields(possiblePositions[i].Y, possiblePositions[i].X, mainField))
                possiblePositions.RemoveAt(i);

        if (possiblePositions.Count > 0)
        {
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
        }

        return mainField;
    }

    private List<Point> GetPossiblePositions(IIsland island, int[][] mainField)
    {
        var possiblePositions = new List<Point>();
        var range = random.Next(2, 6);
        var sizeLength = mainField.Length;
        var sizeWidth = mainField[0].Length;

        // Check positions upward
        if (island.Y > 0 && !HasBridgeInDirection(island, Direction.Up))
        {
            var block = UpBlocked(island, mainField);
            for (var i = island.Y - 1; i >= 0 && i >= island.Y - range; i--)
            {
                if (mainField[i][island.X] != 0) break;
                if (mainField[i][island.X] == 0 && (block == -1 || i > block))
                    possiblePositions.Add(new Point(island.X, i));
            }
        }

        // Check positions leftward
        if (island.X > 0 && !HasBridgeInDirection(island, Direction.Left))
        {
            var block = LeftBlocked(island, mainField);
            for (var i = island.X - 1; i >= 0 && i >= island.X - range; i--)
            {
                if (mainField[island.Y][i] != 0) break;
                if (mainField[island.Y][i] == 0 && (block == -1 || i > block))
                    possiblePositions.Add(new Point(i, island.Y));
            }
        }

        // Check positions downward
        if (island.Y < sizeLength - 1 && !HasBridgeInDirection(island, Direction.Down))
        {
            var block = DownBlocked(island, mainField);
            for (var i = island.Y + 1; i <= sizeLength - 1 && i <= island.Y + range; i++)
            {
                if (mainField[i][island.X] != 0) break;
                if (mainField[i][island.X] == 0 && (block == -1 || i < block))
                    possiblePositions.Add(new Point(island.X, i));
            }
        }

        // Check positions rightward
        if (island.X < sizeWidth - 1 && !HasBridgeInDirection(island, Direction.Right))
        {
            var block = RightBlocked(island, mainField);
            for (var i = island.X + 1; i <= sizeWidth - 1 && i <= island.X + range; i++)
            {
                if (mainField[island.Y][i] != 0) break;
                if (mainField[island.Y][i] == 0 && (block == -1 || i < block))
                    possiblePositions.Add(new Point(i, island.Y));
            }
        }

        return possiblePositions;
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private bool HasBridgeInDirection(IIsland island, Direction direction)
    {
        return direction switch
        {
            Direction.Up => bridges.Any(bridge =>
                bridge.Island1.Y == island.Y &&
                bridge.Island1.X == island.X &&
                bridge.Island2.Y < bridge.Island1.Y &&
                bridge.Island2.X == bridge.Island1.X),

            Direction.Down => bridges.Any(bridge =>
                bridge.Island1.Y == island.Y &&
                bridge.Island1.X == island.X &&
                bridge.Island2.Y > bridge.Island1.Y &&
                bridge.Island2.X == bridge.Island1.X),

            Direction.Left => bridges.Any(bridge =>
                bridge.Island1.Y == island.Y &&
                bridge.Island1.X == island.X &&
                bridge.Island2.Y == bridge.Island1.Y &&
                bridge.Island2.X < bridge.Island1.X),

            Direction.Right => bridges.Any(bridge =>
                bridge.Island1.Y == island.Y &&
                bridge.Island1.X == island.X &&
                bridge.Island2.Y == bridge.Island1.Y &&
                bridge.Island2.X > bridge.Island1.X),

            _ => false
        };
    }

    private bool CheckSurroundingFields(int row, int col, int[][] mainField)
    {
        var numRows = mainField.Length;
        var numCols = mainField[0].Length;

        return (row > 0 && mainField[row - 1][col] != 0) ||                // Check up
               (row < numRows - 1 && mainField[row + 1][col] != 0) ||      // Check down
               (col > 0 && mainField[row][col - 1] != 0) ||                // Check left
               (col < numCols - 1 && mainField[row][col + 1] != 0);        // Check right
    }

    private int UpBlocked(IIsland mainIsland, int[][] mainField)
    {
        for (var row = mainIsland.Y - 1; row > 0; row--)
            for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
            {
                if (bridges.Count <= 0 || mainField[row][checkLeft] == 0) continue;

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == row &&
                        bridge.Island1.X == checkLeft &&
                        bridge.Island2.X > mainIsland.X))
                {
                    return row;
                }
            }

        return -1;
    }

    private int DownBlocked(IIsland mainIsland, int[][] mainField)
    {
        for (var row = mainIsland.Y + 1; row < mainField.Length; row++)
            for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
            {
                if (bridges.Count <= 0 || mainField[row][checkLeft] == 0) continue;

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == row &&
                        bridge.Island1.X == checkLeft &&
                        bridge.Island2.X > mainIsland.X))
                {
                    return row;
                }
            }

        return -1;
    }

    private int RightBlocked(IIsland mainIsland, int[][] mainField)
    {
        for (var col = mainIsland.X + 1; col < mainField[mainIsland.Y].Length; col++)
            for (var checkLeft = mainIsland.Y - 1; checkLeft >= 0; checkLeft--)
            {
                if (bridges.Count <= 0 || mainField[checkLeft][col] == 0) continue;

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == checkLeft &&
                        bridge.Island1.X == col &&
                        bridge.Island2.Y > mainIsland.Y))
                {
                    return col;
                }
            }

        return -1;
    }

    private int LeftBlocked(IIsland mainIsland, int[][] mainField)
    {
        for (var col = mainIsland.X - 1; col > 0; col--)
            for (var checkLeft = mainIsland.Y - 1; checkLeft >= 0; checkLeft--)
            {
                if (bridges.Count <= 0 || mainField[checkLeft][col] == 0) continue;

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == checkLeft &&
                        bridge.Island1.X == col &&
                        bridge.Island2.Y > mainIsland.Y))
                {
                    return col;
                }
            }

        return -1;
    }

    private int DownBlockedd(IIsland mainIsland, int[][] mainField)
    {
        if (mainIsland.IslandDown == null) return -1;

        for (var row = mainIsland.IslandDown.Y - 1; row > mainIsland.Y; row--)
            for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
            {
                if (mainField[row][checkLeft] == 0) continue;

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == row &&
                        bridge.Island1.X == checkLeft &&
                        bridge.Island2.X > mainIsland.X))
                {
                    return row;
                }
            }

        return -1;
    }

    private int RightBlockedd(IIsland mainIsland, int[][] mainField)
    {
        if (mainIsland.IslandRight == null) return -1;

        for (var col = mainIsland.IslandRight.X - 1; col > mainIsland.X; col--)
            for (var checkLeft = mainIsland.Y - 1; checkLeft >= 0; checkLeft--)
            {
                if (mainField[checkLeft][col] == 0) continue;

                if (bridges.Any(bridge =>
                        bridge.Island1.Y == checkLeft &&
                        bridge.Island1.X == col &&
                        bridge.Island2.Y > mainIsland.Y))
                {
                    return col;
                }
            }

        return -1;
    }

    private void SetBeta(int[][] mainField, int beta)
    {
        // Skip if beta is zero or too low
        if (beta <= 0) return;

        for (var i = bridges.Count - 1; i > 0; i -= 2)
        {
            if (random.Next(100) <= beta - 1)
            {
                bridges[i].AddBridge(mainField);
            }
        }
    }
}
