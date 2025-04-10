namespace Hashi.Generator.Interfaces.Models;

/// <summary>
///     Represents a solution container for the Hashi game.
/// </summary>
public interface ISolutionContainer
{
    /// <summary>
    ///     The Hashi field represented as a 2D array of integers.
    /// </summary>
    IReadOnlyList<int[]> HashiField { get; }

    /// <summary>
    ///     A list of bridge coordinates, each containing the coordinates of two islands and the number of bridges between
    ///     them.
    /// </summary>
    List<IBridgeCoordinates> BridgeCoordinates { get; }
}