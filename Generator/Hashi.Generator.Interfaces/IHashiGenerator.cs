using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;

namespace Hashi.Generator.Interfaces;

/// <summary>
///     Interface for generating a Hashi field.
/// </summary>
public interface IHashiGenerator
{
    /// <summary>
    ///     Generates a Hashi field.
    /// </summary>
    /// <param name="difficulty">The difficulty setting (0-9).</param>
    /// <param name="amountNodes">The amount of nodes.</param>
    /// <param name="width">The width of the hashi field.</param>
    /// <param name="length">The length of the hashi field.</param>
    /// <param name="alpha">The alpha value.</param>
    /// <param name="beta">The beta value.</param>
    /// <returns>a hashi field array and a list of bridges.</returns>
    Task<ISolutionProvider> GenerateHashAsync(int difficulty = -1, int amountNodes = 10, int width = 0, int length = 0,
        int alpha = 0,
        int beta = 0);

    /// <summary>
    ///     Gets the bridges of the Hashi field.
    /// </summary>
    /// <returns>a list of bridge models.</returns>
    List<IBridge> GetBridges();

    /// <summary>
    ///     Gets the islands of the Hashi field.
    /// </summary>
    /// <returns>a list of island models.</returns>
    List<IIsland> GetIslands();
}