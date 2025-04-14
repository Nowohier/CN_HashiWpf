using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;

namespace Hashi.Generator.Providers;

/// <inheritdoc cref="ISolutionProvider" />
public class SolutionProvider(IReadOnlyList<int[]> hashiField, List<IBridgeCoordinates> bridgeCoordinates) : ISolutionProvider
{
    /// <inheritdoc />
    public IReadOnlyList<int[]> HashiField { get; } = hashiField;

    /// <inheritdoc />
    public List<IBridgeCoordinates> BridgeCoordinates { get; } = bridgeCoordinates;
}