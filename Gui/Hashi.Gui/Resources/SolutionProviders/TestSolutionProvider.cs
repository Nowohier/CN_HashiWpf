using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Gui.Interfaces.Resources.SolutionProviders;
using System.Drawing;

namespace Hashi.Gui.Resources.SolutionProviders
{
    public class TestSolutionProvider : ITestSolutionProvider
    {
        /// <inheritdoc />
        public List<ISolutionProvider> SolutionProviders => [None, _9GeneralRule2];

        private static readonly ISolutionProvider None = new SolutionProvider(null!, null!, nameof(None));

        private static readonly ISolutionProvider _9GeneralRule2 = new SolutionProvider(
            [
                [1, 0, 3, 0, 0, 3],
                [0, 0, 0, 0, 0, 0],
                [2, 0, 5, 0, 2, 0],
                [0, 0, 0, 2, 0, 4],
                [3, 0, 2, 0, 1, 0],
                [0, 1, 0, 3, 0, 3]
            ],
            [
                new BridgeCoordinates(new Point(2, 0), new Point(5, 0), 2),
                new BridgeCoordinates(new Point(5, 0), new Point(5, 3), 1),
                new BridgeCoordinates(new Point(3, 3), new Point(5, 3), 2),
                new BridgeCoordinates(new Point(5, 3), new Point(5, 5), 1),
                new BridgeCoordinates(new Point(5, 5), new Point(3, 5), 2),
                new BridgeCoordinates(new Point(3, 5), new Point(1, 5), 1)
            ],
            nameof(_9GeneralRule2)
        );
    }
}
