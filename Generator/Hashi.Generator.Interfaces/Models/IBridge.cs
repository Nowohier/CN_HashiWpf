namespace Hashi.Generator.Interfaces.Models;

/// <summary>
///     Represents a bridge connecting two islands in the Hashi game.
/// </summary>
public interface IBridge
{
    /// <summary>
    ///     Gets the first island of the bridge.
    /// </summary>
    IIsland Island1 { get; }

    /// <summary>
    ///     Gets the second island of the bridge.
    /// </summary>
    IIsland Island2 { get; }

    /// <summary>
    ///     Gets the amount of bridges set between the two islands.
    /// </summary>
    int AmountBridgesSet { get; }

    /// <summary>
    ///     Creates a reverse bridge with swapped islands and applies directional bridge counts to both islands.
    /// </summary>
    /// <returns>The newly created reverse bridge.</returns>
    IBridge CreateReverseBridgeAndApplyDirections();

    /// <summary>
    ///     Adds another bridge to this bridge, incrementing bridge counts and updating directional data.
    /// </summary>
    void AddBridge();
}