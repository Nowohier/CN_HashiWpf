using Hashi.Generator.Interfaces.Models;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Hashi.Generator.Models;

/// <inheritdoc cref="IBridgeCoordinates" />
public class BridgeCoordinates : IBridgeCoordinates
{
    /// <summary>
    ///     Constructor for the BridgeCoordinates class.
    /// </summary>
    /// <param name="location1">The location of the first island.</param>
    /// <param name="location2">The location of the second island.</param>
    /// <param name="amountBridges">The amount of bridges between the islands.</param>
    [JsonConstructor]
    public BridgeCoordinates(Point location1, Point location2, int amountBridges)
    {
        Location1 = location1;
        Location2 = location2;
        AmountBridges = amountBridges;
    }

    /// <inheritdoc />
    [JsonPropertyName("Location1")]
    public Point Location1 { get; }

    /// <inheritdoc />
    [JsonPropertyName("Location2")]
    public Point Location2 { get; }

    /// <inheritdoc />
    [JsonPropertyName("AmountBridges")]
    public int AmountBridges { get; }
}
