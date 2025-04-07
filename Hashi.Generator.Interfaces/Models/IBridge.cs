namespace Hashi.Generator.Interfaces.Models;

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
    ///     Adds the other side of the bridge.
    /// </summary>
    /// <returns>the added bridge.</returns>
    IBridge AddOtherSide();

    /// <summary>
    ///     Adds another bridge to this bridge.
    /// </summary>
    /// <param name="mainField">The main field array.</param>
    void AddBridge(int[][] mainField);
}