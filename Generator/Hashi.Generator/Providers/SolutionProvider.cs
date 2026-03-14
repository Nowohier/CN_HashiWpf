using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using System.Text.Json.Serialization;

namespace Hashi.Generator.Providers;

/// <inheritdoc cref="ISolutionProvider" />
public class SolutionProvider : ISolutionProvider
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SolutionProvider" /> class.
    /// </summary>
    /// <param name="hashiField">The hashi field.</param>
    /// <param name="bridgeCoordinates">The bridge coordinates.</param>
    /// <param name="name">The name of the solution.</param>
    [JsonConstructor]
    public SolutionProvider(
        IReadOnlyList<int[]>? hashiField,
        IReadOnlyList<IBridgeCoordinates>? bridgeCoordinates,
        string? name = null)
    {
        HashiField = hashiField;
        BridgeCoordinates = bridgeCoordinates;
        Name = name ?? string.Empty;
    }

    /// <inheritdoc />
    [JsonPropertyName("HashiField")]
    public IReadOnlyList<int[]>? HashiField { get; }

    /// <inheritdoc />
    [JsonPropertyName("BridgeCoordinates")]
    public IReadOnlyList<IBridgeCoordinates>? BridgeCoordinates { get; }

    /// <inheritdoc />
    [JsonPropertyName("Name")]
    public string? Name { get; set; }
}
