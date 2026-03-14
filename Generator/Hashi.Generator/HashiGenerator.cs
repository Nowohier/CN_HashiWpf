using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.LinearSolver.Interfaces;
using Hashi.Logging.Interfaces;
using System.Diagnostics;

namespace Hashi.Generator;

/// <summary>
///     Generates a Hashi field.
/// </summary>
public class HashiGenerator : IHashiGenerator
{
    private readonly Func<int, int, int, IIsland> islandFactory;
    private readonly Func<int[][], List<IBridgeCoordinates>, ISolutionProvider> solutionContainerFactory;
    private readonly IHashiSolver hashiSolver;
    private readonly IRuleSolvabilityValidator ruleSolvabilityValidator;
    private readonly IDifficultySettingsProvider difficultySettingsProvider;
    private readonly IBlockDetectionService blockDetectionService;
    private readonly IIslandLayoutService islandLayoutService;
    private readonly IBridgeManagementService bridgeManagementService;
    private readonly ILogger logger;

    /// <summary>
    ///     Constructor for HashiGenerator.
    /// </summary>
    public HashiGenerator(
        Func<int, int, int, IIsland> islandFactory,
        Func<int[][], List<IBridgeCoordinates>, ISolutionProvider> solutionContainerFactory,
        IHashiSolver hashiSolver,
        IRuleSolvabilityValidator ruleSolvabilityValidator,
        IDifficultySettingsProvider difficultySettingsProvider,
        IBlockDetectionService blockDetectionService,
        IIslandLayoutService islandLayoutService,
        IBridgeManagementService bridgeManagementService,
        ILoggerFactory loggerFactory)
    {
        this.islandFactory = islandFactory;
        this.solutionContainerFactory = solutionContainerFactory;
        this.hashiSolver = hashiSolver;
        this.ruleSolvabilityValidator = ruleSolvabilityValidator;
        this.difficultySettingsProvider = difficultySettingsProvider;
        this.blockDetectionService = blockDetectionService;
        this.islandLayoutService = islandLayoutService;
        this.bridgeManagementService = bridgeManagementService;
        this.logger = loggerFactory.CreateLogger<HashiGenerator>();
    }

    /// <inheritdoc />
    public async Task<ISolutionProvider> GenerateHashAsync(int difficulty = -1, int amountNodes = 10, int width = 0,
        int length = 0, int alpha = 0, int beta = 0)
    {
        logger.Info(
            $"Starting hash generation - difficulty: {difficulty}, nodes: {amountNodes}, size: {width}x{length}");

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

        var settings = difficultySettingsProvider.GetDifficultySettings(difficulty);

        var sizeLength = Random.Shared.Next(settings.MinLength, settings.MaxLength);
        var sizeWidth = Random.Shared.Next(settings.MinWidth, settings.MaxWidth);
        var n = (int)Math.Round(sizeWidth * sizeLength / (double)settings.Divisor);

        return await GenerateHashAsync(n, sizeLength, sizeWidth, difficulty, settings.Beta, false);
    }

    /// <summary>
    ///     Generates a Hashi field.
    /// </summary>
    internal async Task<ISolutionProvider> GenerateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth,
        int difficulty, int beta, bool checkDifficulty)
    {
        int[][] field;
        var attempts = 0;
        var ruleAttempts = 0;
        var context = new GenerationContext();

        while (true)
        {
            blockDetectionService.ClearCaches();

            field = await CreateHashAsync(context, numberOfIslands, sizeLength, sizeWidth, difficulty, beta, checkDifficulty);
            attempts++;
            ruleAttempts++;

            if (await hashiSolver.SolveLazy(field) == SolverStatusEnum.Infeasible)
            {
                if (attempts >= GeneratorConstants.MaxGenerationAttempts)
                {
                    attempts = 0;
                    beta = Math.Max(0, beta - 5);
                }

                continue;
            }

            if (await ruleSolvabilityValidator.IsFullySolvableByRules(field))
            {
                logger.Info($"Rule-solvable puzzle found after {ruleAttempts} attempt(s)");
                break;
            }

            logger.Debug($"Puzzle rejected by rule validator (attempt {ruleAttempts})");

            if (ruleAttempts >= GeneratorConstants.MaxRuleSolvabilityAttempts)
            {
                logger.Warn(
                    $"Could not find rule-solvable puzzle after {GeneratorConstants.MaxRuleSolvabilityAttempts} attempts. Reducing complexity.");
                ruleAttempts = 0;
                numberOfIslands = Math.Max(GeneratorConstants.MinIslandsBeforeReduction, numberOfIslands - 2);
                sizeLength = Math.Max(GeneratorConstants.MinFieldDimension, sizeLength - 1);
                sizeWidth = Math.Max(GeneratorConstants.MinFieldDimension, sizeWidth - 1);
            }

            attempts = 0;
        }

        var bridgeCoordinates = bridgeManagementService.BuildBridgeCoordinates(context.Bridges);

        if (Debugger.IsAttached)
        {
            logger.Debug($"Number of islands: {context.Islands.Count}");
            logger.Debug("Generated field:");
            foreach (var row in field)
            {
                logger.Debug($"{{{string.Join(", ", row)}}}");
            }
        }

        return solutionContainerFactory.Invoke(field, bridgeCoordinates);
    }

    internal async Task<int[][]> CreateHashAsync(GenerationContext context, int numberOfIslands, int sizeLength, int sizeWidth, int difficulty,
        int beta, bool checkDifficulty)
    {
        return await Task.Run(() =>
        {
            context.Clear();

            var mainField = islandLayoutService.InitializeField(sizeLength, sizeWidth);
            context.Field = mainField;

            // Create first island at random position
            var row = Random.Shared.Next(sizeLength);
            var col = Random.Shared.Next(sizeWidth);
            context.Islands.Add(islandFactory.Invoke(0, row, col));
            var edgeCount = 0;
            var iterationCount = 0;

            // Generate islands and bridges until we have enough
            while (iterationCount++ < GeneratorConstants.MaxIterationsPerGeneration)
            {
                var size = context.Islands.Count;
                var islandsAdded = false;

                for (var i = 0; i < size; i++)
                {
                    if (islandLayoutService.CreateIsland(mainField, context.Islands[i], context.Islands,
                            context.Bridges))
                    {
                        islandsAdded = true;
                    }

                    if (context.Islands.Count >= numberOfIslands)
                    {
                        break;
                    }
                }

                if ((!islandsAdded && edgeCount == context.Bridges.Count) ||
                    context.Islands.Count >= numberOfIslands)
                {
                    break;
                }

                edgeCount = context.Bridges.Count;
            }

            // Set all neighbors for each island
            var islandLookup = new Dictionary<(int Y, int X), IIsland>(context.Islands.Count);
            foreach (var island in context.Islands)
            {
                islandLookup.TryAdd((island.Y, island.X), island);
            }

            if (context.Islands.Count > GeneratorConstants.ParallelProcessingThreshold)
            {
                Parallel.ForEach(context.Islands, node => { node.SetAllNeighbors(mainField, islandLookup); });
            }
            else
            {
                foreach (var node in context.Islands)
                    node.SetAllNeighbors(mainField, islandLookup);
            }

            // Apply difficulty settings
            if (!checkDifficulty)
            {
                var alphaValue = difficultySettingsProvider.GetAlphaForDifficulty(difficulty);
                bridgeManagementService.AddAdditionalBridges(mainField, alphaValue, context.Islands, context.Bridges);

                var betaValue = difficultySettingsProvider.GetBetaForDifficulty(difficulty);
                bridgeManagementService.SetBeta(mainField, betaValue, context.Bridges);
            }
            else
            {
                bridgeManagementService.AddAdditionalBridges(mainField, difficulty, context.Islands, context.Bridges);
                bridgeManagementService.SetBeta(mainField, beta, context.Bridges);
            }

            return mainField;
        });
    }
}
