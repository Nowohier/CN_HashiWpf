using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Newtonsoft.Json;

namespace Hashi.Generator.Providers;

/// <inheritdoc cref="ISolutionProvider" />
[JsonObject(MemberSerialization.OptIn)]
public class SolutionProvider(
    IReadOnlyList<int[]>? hashiField,
    List<IBridgeCoordinates>? bridgeCoordinates,
    string? name = null) : ISolutionProvider
{
    /// <inheritdoc />
    [JsonProperty(nameof(HashiField))]
    public IReadOnlyList<int[]>? HashiField { get; } = hashiField;

    /// <inheritdoc />
    [JsonProperty(nameof(BridgeCoordinates))]
    public List<IBridgeCoordinates>? BridgeCoordinates { get; } = bridgeCoordinates;

    /// <inheritdoc />
    [JsonProperty(nameof(Name))]
    public string? Name { get; } = name ?? string.Empty;
}