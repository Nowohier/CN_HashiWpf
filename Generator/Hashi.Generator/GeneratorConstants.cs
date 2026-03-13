namespace Hashi.Generator;

/// <summary>
///     Constants used throughout the generator.
/// </summary>
internal static class GeneratorConstants
{
    /// <summary>Maximum number of solver attempts before changing strategy.</summary>
    internal const int MaxGenerationAttempts = 50;

    /// <summary>Maximum attempts to find a rule-solvable puzzle.</summary>
    internal const int MaxRuleSolvabilityAttempts = 500;

    /// <summary>Maximum iterations per generation to prevent infinite loops.</summary>
    internal const int MaxIterationsPerGeneration = 100;

    /// <summary>Minimum island range for position generation.</summary>
    internal const int MinIslandRange = 2;

    /// <summary>Maximum island range for position generation.</summary>
    internal const int MaxIslandRange = 6;

    /// <summary>Maximum bridges per edge between two islands.</summary>
    internal const int MaxBridgesPerEdge = 2;

    /// <summary>Maximum bridges an island can have.</summary>
    internal const int MaxBridgesPerIsland = 8;

    /// <summary>Threshold for parallel processing of islands.</summary>
    internal const int ParallelProcessingThreshold = 20;

    /// <summary>Maximum connectable bridges before blocking additional bridge creation.</summary>
    internal const int MaxConnectableBridgesForAdding = 7;

    /// <summary>Search radius for blocked path calculations.</summary>
    internal const int BlockedSearchRadius = 10;

    /// <summary>Minimum number of islands before reducing complexity.</summary>
    internal const int MinIslandsBeforeReduction = 4;

    /// <summary>Minimum field dimension.</summary>
    internal const int MinFieldDimension = 5;
}
