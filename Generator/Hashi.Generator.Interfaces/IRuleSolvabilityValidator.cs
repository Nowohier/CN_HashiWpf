using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Interfaces;

/// <summary>
///     Validates whether a generated Hashi puzzle is fully solvable using only the hint rule engine.
/// </summary>
public interface IRuleSolvabilityValidator
{
    /// <summary>
    ///     Determines whether the given puzzle field is completely solvable by iteratively applying the hint rules.
    /// </summary>
    /// <param name="field">The puzzle field as a 2D array where each cell value represents the bridge count of an island (0 = empty).</param>
    /// <param name="bridgeCoordinates">The known solution bridges to validate against.</param>
    /// <returns><c>true</c> if the puzzle can be solved entirely via hint rules; otherwise, <c>false</c>.</returns>
    Task<bool> IsFullySolvableByRules(int[][] field, IList<IBridgeCoordinates> bridgeCoordinates);
}
