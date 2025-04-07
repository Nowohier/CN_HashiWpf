using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Gui.Enums;
using Hashi.LinearSolver.Interfaces;
using System.Diagnostics;
using System.Drawing;

namespace Hashi.Generator;

/// <summary>
///     Generates a Hashi field.
/// </summary>
public class HashiGenerator : IHashiGenerator
{
    private readonly Func<IIsland, IIsland, int, IBridge> bridgeFactory;
    private readonly List<IBridge> bridges = new();
    private readonly Func<int, int, int, IIsland> islandFactory;
    private readonly List<IIsland> islands = new();
    private readonly ILinearSolutionSolverWithIterativ linearSolutionSolverWithIterativ;
    private readonly Random random = new();

    public HashiGenerator(Func<int, int, int, IIsland> islandFactory,
        Func<IIsland, IIsland, int, IBridge> bridgeFactory,
        ILinearSolutionSolverWithIterativ linearSolutionSolverWithIterativ)
    {
        this.islandFactory = islandFactory;
        this.bridgeFactory = bridgeFactory;
        this.linearSolutionSolverWithIterativ = linearSolutionSolverWithIterativ;
    }

    public async Task<int[][]> GenerateHashAsync(int difficulty = -1, int amountNodes = 10, int width = 0, int length = 0, int alpha = 0,
        int beta = 0)
    {
        // ToDo: remove

        if (difficulty >= 0)
        {
            if (difficulty == 0)
            {
                var sizeLength = random.Next(5, 10);
                var sizeWidth = random.Next(5, 10);
                var n = (int)Math.Round(sizeWidth * sizeLength / 4.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 0, 0, false);
            }

            if (difficulty == 1)
            {
                var sizeLength = random.Next(14, 16);
                var sizeWidth = random.Next(14, 16);
                var n = (int)Math.Round(sizeWidth * sizeLength / 4.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 1, 0, false);
            }

            if (difficulty == 2)
            {
                var sizeLength = random.Next(10, 16);
                var sizeWidth = random.Next(10, 16);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 2, 0, false);
            }

            if (difficulty == 3)
            {
                var sizeLength = random.Next(11, 18);
                var sizeWidth = random.Next(11, 18);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 3, 0, false);
            }

            if (difficulty == 4)
            {
                var sizeLength = random.Next(10, 18);
                var sizeWidth = random.Next(10, 18);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 4, 0, false);
            }

            if (difficulty == 5)
            {
                var sizeLength = random.Next(13, 18);
                var sizeWidth = random.Next(13, 18);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 5, 0, false);
            }

            if (difficulty == 6)
            {
                var sizeLength = random.Next(15, 20);
                var sizeWidth = random.Next(15, 20);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 6, 0, false);
            }

            if (difficulty == 7)
            {
                var sizeLength = random.Next(14, 20);
                var sizeWidth = random.Next(14, 20);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 7, 0, false);
            }

            if (difficulty == 8)
            {
                var sizeLength = random.Next(16, 31);
                var sizeWidth = random.Next(16, 31);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 8, 0, false);
            }

            if (difficulty == 9)
            {
                var sizeLength = random.Next(20, 31);
                var sizeWidth = random.Next(20, 31);
                var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                return await GenerateHashAsync(n, sizeLength, sizeWidth, 9, 0, false);
            }

            throw new ArgumentException("Invalid difficulty level.");
        }

        return await GenerateHashAsync(amountNodes, length, width, alpha, beta, true);
    }

    /// <summary>
    ///     Gets the bridges of the Hashi field.
    /// </summary>
    /// <returns>a list of bridge models.</returns>
    public List<IBridge> GetBridges()
    {
        return bridges;
    }

    /// <summary>
    ///     Gets the islands of the Hashi field.
    /// </summary>
    /// <returns>a list of island models.</returns>
    public List<IIsland> GetIslands()
    {
        return islands;
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
    private async Task<int[][]> GenerateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth, int difficulty, int beta,
        bool checkDifficulty)
    {
        var field = await CreateHashAsync(numberOfIslands, sizeLength, sizeWidth, difficulty, beta, checkDifficulty);

        while (await linearSolutionSolverWithIterativ.SolveAsync(field) == SolverStatusEnum.Infeasible)
            field = await CreateHashAsync(numberOfIslands, sizeLength, sizeWidth, difficulty, beta, checkDifficulty);

        Debug.WriteLine(string.Empty);
        Debug.WriteLine(string.Join("\nNumberOfIslands", field.Select(row => $"{{{string.Join(", ", row)}}}")));

        return field;
    }

    private async Task<int[][]> CreateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth, int difficulty, int beta,
        bool checkDifficulty)
    {
        var task = Task.Run(() =>
        {
            bridges.Clear();
            islands.Clear();

            var mainField = new int[sizeLength][];
            for (var i = 0; i < sizeLength; i++) mainField[i] = new int[sizeWidth];

            var row = random.Next(sizeLength);
            var col = random.Next(sizeWidth);
            islands.Add(islandFactory.Invoke(0, row, col));
            var edgeCount = 0;

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

            foreach (var node in islands) node.SetAllNeighbors(mainField, islands);

            if (!checkDifficulty)
            {
                mainField = difficulty switch
                {
                    0 => CreateNewBridges(mainField, 25),
                    3 => CreateNewBridges(mainField, 25),
                    6 => CreateNewBridges(mainField, 25),
                    1 => CreateNewBridges(mainField, 50),
                    4 => CreateNewBridges(mainField, 50),
                    7 => CreateNewBridges(mainField, 50),
                    2 => CreateNewBridges(mainField, 75),
                    5 => CreateNewBridges(mainField, 75),
                    8 => CreateNewBridges(mainField, 75),
                    9 => CreateNewBridges(mainField, 100),
                    _ => mainField
                };

                switch (difficulty)
                {
                    case >= 0 and <= 2:
                        SetBeta(mainField, 20);
                        break;
                    case >= 3 and <= 5:
                        SetBeta(mainField, 15);
                        break;
                    case >= 6 and <= 8:
                        SetBeta(mainField, 10);
                        break;
                    case 9:
                        SetBeta(mainField, 0);
                        break;
                }
            }
            else
            {
                mainField = CreateNewBridges(mainField, difficulty);
                SetBeta(mainField, beta);
            }

            return mainField;
        });

        return await task;
    }

    private int[][] CreateNewBridges(int[][] mainField, int alpha)
    {
        var i = 0;
        var j = 0;

        while (i < islands.Count)
        {
            var island = islands[i];
            if (island.IslandDown != null && DownBlockedd(island, mainField) == -1 && island.AmountBridgesDown == 0)
                if (island.AmountBridgesConnectable + 1 <= 7 && island.IslandDown.AmountBridgesConnectable + 1 <= 7)
                {
                    var v3Edge = bridgeFactory.Invoke(island, island.IslandDown, 1);
                    bridges.Add(v3Edge);
                    bridges.Add(v3Edge.AddOtherSide());
                    island.AmountBridgesConnectable += 1;
                    island.IslandDown.AmountBridgesConnectable += 1;
                    mainField[island.Y][island.X] += 1;
                    mainField[island.IslandDown.Y][island.X] += 1;
                    j++;
                }

            if (j >= islands.Count * (alpha / 100.0)) break;
            if (island.IslandRight != null && RightBlockedd(island, mainField) == -1 && island.AmountBridgesRight == 0)
                if (island.AmountBridgesConnectable + 1 <= 7 && island.IslandRight.AmountBridgesConnectable + 1 <= 7)
                {
                    var v3Edge = bridgeFactory.Invoke(island, island.IslandRight, 1);
                    bridges.Add(v3Edge);
                    bridges.Add(v3Edge.AddOtherSide());
                    island.AmountBridgesConnectable += 1;
                    island.IslandRight.AmountBridgesConnectable += 1;
                    mainField[island.Y][island.X] += 1;
                    mainField[island.IslandRight.Y][island.X] += 1;
                    j++;
                }

            if (j >= islands.Count * (alpha / 100.0)) break;
            i++;
        }

        return mainField;
    }

    private int[][] CreateIsland(int[][] mainField, IIsland island)
    {
        var possiblePositions = new List<Point>();
        var range = random.Next(2, 6);

        if (island.Y != 0)
        {
            var block = UpBlocked(island, mainField);
            var check = false;
            foreach (var bridge in bridges)
                if (bridge.Island1.Y == island.Y && bridge.Island1.X == island.X &&
                    bridge.Island2.Y < bridge.Island1.Y && bridge.Island2.X == bridge.Island1.X)
                {
                    check = true;
                    break;
                }

            if (!check)
                for (var i = island.Y - 1; i >= 0 && i >= island.Y - range; i--)
                {
                    if (mainField[i][island.X] != 0) break;
                    if (mainField[i][island.X] == 0 && (block == -1 || i > block))
                        possiblePositions.Add(new Point(island.X, i));
                }
        }

        if (island.X != 0)
        {
            var block = LeftBlocked(island, mainField);
            var check = bridges.Any(bridge =>
                bridge.Island1.Y == island.Y && bridge.Island1.X == island.X && bridge.Island2.Y == bridge.Island1.Y &&
                bridge.Island2.X <= bridge.Island1.X);
            if (!check)
                for (var i = island.X - 1; i >= 0 && i >= island.X - range; i--)
                {
                    if (mainField[island.Y][i] != 0) break;
                    if (mainField[island.Y][i] == 0 && (block == -1 || i > block))
                        possiblePositions.Add(new Point(i, island.Y));
                }
        }

        if (island.Y != mainField.Length - 1)
        {
            var block = DownBlocked(island, mainField);
            var check = bridges.Any(bridge =>
                bridge.Island1.Y == island.Y && bridge.Island1.X == island.X && bridge.Island2.Y > bridge.Island1.Y &&
                bridge.Island2.X == bridge.Island1.X);
            if (!check)
                for (var i = island.Y + 1; i <= mainField.Length - 1 && i <= island.Y + range; i++)
                {
                    if (mainField[i][island.X] != 0) break;
                    if (mainField[i][island.X] == 0 && (block == -1 || i < block))
                        possiblePositions.Add(new Point(island.X, i));
                }
        }

        if (island.X != mainField[0].Length - 1)
        {
            var block = RightBlocked(island, mainField);
            var check = bridges.Any(bridge =>
                bridge.Island1.Y == island.Y && bridge.Island1.X == island.X && bridge.Island2.Y == bridge.Island1.Y &&
                bridge.Island2.X > bridge.Island1.X);
            if (!check)
                for (var i = island.X + 1; i <= mainField[island.Y].Length - 1 && i <= island.X + range; i++)
                {
                    if (mainField[island.Y][i] != 0) break;
                    if (mainField[island.Y][i] == 0 && (block == -1 || i < block))
                        possiblePositions.Add(new Point(i, island.Y));
                }
        }

        for (var i = possiblePositions.Count - 1; i >= 0; i--)
            if (CheckSurroundingFields(Convert.ToInt32(possiblePositions[i].Y), Convert.ToInt32(possiblePositions[i].X),
                    mainField))
                possiblePositions.RemoveAt(i);

        if (possiblePositions.Count > 0)
        {
            var randomPosition = random.Next(possiblePositions.Count);
            var newIsland = islandFactory.Invoke(0, Convert.ToInt32(possiblePositions[randomPosition].Y),
                Convert.ToInt32(possiblePositions[randomPosition].X));
            islands.Add(newIsland);
            var amountBridges = 1;
            var newBridge = bridgeFactory.Invoke(island, newIsland, amountBridges);
            bridges.Add(newBridge);
            bridges.Add(newBridge.AddOtherSide());
            island.AmountBridgesConnectable += amountBridges;
            newIsland.AmountBridgesConnectable += amountBridges;
            mainField[island.Y][island.X] += amountBridges;
            mainField[newIsland.Y][newIsland.X] += amountBridges;
        }

        return mainField;
    }

    private bool CheckSurroundingFields(int row, int col, int[][] mainField)
    {
        var numRows = mainField.Length;
        var numCols = mainField[0].Length;

        if (row - 1 >= 0 && mainField[row - 1][col] != 0) return true;
        if (row + 1 < numRows && mainField[row + 1][col] != 0) return true;
        if (col - 1 >= 0 && mainField[row][col - 1] != 0) return true;
        return col + 1 < numCols && mainField[row][col + 1] != 0;
    }

    private int UpBlocked(IIsland mainIsland, int[][] mainField)
    {
        for (var row = mainIsland.Y - 1; row > 0; row--)
            for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
            {
                if (bridges.Count <= 0 || mainField[row][checkLeft] == 0) continue;
                if (bridges.Any(bridge =>
                        bridge.Island1.Y == row && bridge.Island1.X == checkLeft && bridge.Island2.X > mainIsland.X))
                    return row;
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
                        bridge.Island1.Y == row && bridge.Island1.X == checkLeft && bridge.Island2.X > mainIsland.X))
                    return row;
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
                        bridge.Island1.Y == checkLeft && bridge.Island1.X == col && bridge.Island2.Y > mainIsland.Y))
                    return col;
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
                        bridge.Island1.Y == checkLeft && bridge.Island1.X == col && bridge.Island2.Y > mainIsland.Y))
                    return col;
            }

        return -1;
    }

    private int DownBlockedd(IIsland mainIsland, int[][] mainField)
    {
        for (var row = mainIsland.IslandDown.Y - 1; row > mainIsland.Y; row--)
            for (var checkLeft = mainIsland.X - 1; checkLeft >= 0; checkLeft--)
            {
                if (mainField[row][checkLeft] == 0) continue;
                if (bridges.Any(bridge =>
                        bridge.Island1.Y == row && bridge.Island1.X == checkLeft && bridge.Island2.X > mainIsland.X))
                    return row;
            }

        return -1;
    }

    private int RightBlockedd(IIsland mainIsland, int[][] mainField)
    {
        for (var col = mainIsland.IslandRight.X - 1; col > mainIsland.X; col--)
            for (var checkLeft = mainIsland.Y - 1; checkLeft >= 0; checkLeft--)
            {
                if (mainField[checkLeft][col] == 0) continue;
                if (bridges.Any(bridge =>
                        bridge.Island1.Y == checkLeft && bridge.Island1.X == col && bridge.Island2.Y > mainIsland.Y))
                    return col;
            }

        return -1;
    }

    private void SetBeta(int[][] mainField, int beta)
    {
        for (var i = bridges.Count - 1; i > 0; i -= 2)
            if (random.Next(100) <= beta - 1)
                bridges[i].AddBridge(mainField);
    }
}