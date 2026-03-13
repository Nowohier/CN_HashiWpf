using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Interfaces;

/// <summary>
///     Manages bridge creation and manipulation during puzzle generation.
/// </summary>
public interface IBridgeManagementService
{
    /// <summary>
    ///     Adds additional bridges based on the alpha parameter.
    /// </summary>
    void AddAdditionalBridges(int[][] mainField, int alpha, List<IIsland> islands, List<IBridge> bridges);

    /// <summary>
    ///     Adjusts bridge counts based on the beta parameter.
    /// </summary>
    void SetBeta(int[][] mainField, int beta, List<IBridge> bridges);

    /// <summary>
    ///     Builds bridge coordinates from the current bridge list.
    /// </summary>
    List<IBridgeCoordinates> BuildBridgeCoordinates(List<IBridge> bridges);
}
