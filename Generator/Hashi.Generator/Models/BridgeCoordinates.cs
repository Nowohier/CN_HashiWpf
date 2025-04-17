using Hashi.Generator.Interfaces.Models;
using Newtonsoft.Json;
using System.Drawing;

namespace Hashi.Generator.Models;

/// <inheritdoc cref="IBridgeCoordinates" />
[JsonObject(MemberSerialization.OptIn)]
public class BridgeCoordinates : IBridgeCoordinates
{
    /// <summary>
    ///     Constructor for the BridgeCoordinates class.
    /// </summary>
    /// <param name="location1">The location of the first island.</param>
    /// <param name="location2">The location of the second island.</param>
    /// <param name="amountBridges">The amount of bridges between the islands.</param>
    public BridgeCoordinates(Point location1, Point location2, int amountBridges)
    {
        Location1 = location1;
        Location2 = location2;
        AmountBridges = amountBridges;
    }

    /// <inheritdoc />
    [JsonProperty(nameof(Location1))]
    public Point Location1 { get; }

    /// <inheritdoc />
    [JsonProperty(nameof(Location2))]
    public Point Location2 { get; }

    /// <inheritdoc />
    [JsonProperty(nameof(AmountBridges))]
    public int AmountBridges { get; }
}