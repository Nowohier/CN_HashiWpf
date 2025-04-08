using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Models
{
    /// <inheritdoc cref="ISolutionContainer"/>
    public class SolutionContainer : ISolutionContainer
    {
        public SolutionContainer(IReadOnlyList<int[]> hashiField, List<IBridgeCoordinates> bridgeCoordinates)
        {
            HashiField = hashiField;
            BridgeCoordinates = bridgeCoordinates;
        }

        /// <inheritdoc/>
        public IReadOnlyList<int[]> HashiField { get; }

        /// <inheritdoc/>
        public List<IBridgeCoordinates> BridgeCoordinates { get; }
    }
}
