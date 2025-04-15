using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Gui.Interfaces.Resources.SolutionProviders;
using Newtonsoft.Json;
using System.Drawing;

namespace Hashi.Gui.Resources.SolutionProviders
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TestSolutionProvider : ITestSolutionProvider
    {
        /// <inheritdoc />
        public IReadOnlyList<int[]> HashiFieldReference =>
        [
            [0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0]
        ];

        /// <inheritdoc />
        [JsonProperty(nameof(SolutionProviders))]
        public List<ISolutionProvider> SolutionProviders =>
        [
            None,
            _1ConnectionsRule1,
            _3ConnectionsRule1,
            _4ConnectionsRule1,
            _5ConnectionsRule1,
            _5ConnectionsRule2,
            _6ConnectionsRule1,
            _8ConnectionsRule1,
            _7ConnectionsRule1,
            _9GeneralRule1,
            _9GeneralRule2
        ];

        private static readonly ISolutionProvider None = new SolutionProvider(null!, null!, nameof(None));

        private static readonly ISolutionProvider _1ConnectionsRule1 = new SolutionProvider(
            [
                [0, 1, 0, 0, 2, 0],
                [0, 0, 4, 0, 0, 0],
                [0, 0, 0, 3, 0, 1],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 1, 0, 0, 0],
                [0, 0, 0, 0, 0, 0]
            ],
            [],
            nameof(_1ConnectionsRule1)
        );

        private static readonly ISolutionProvider _3ConnectionsRule1 = new SolutionProvider(
            [
                [3, 0, 0, 3, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 1, 0, 0, 0, 0],
                [0, 0, 0, 3, 0, 3],
                [0, 0, 0, 0, 0, 0],
                [0, 3, 0, 0, 0, 3]
            ],
            [],
            nameof(_3ConnectionsRule1)
        );

        private static readonly ISolutionProvider _4ConnectionsRule1 = new SolutionProvider(
            [
                [4, 0, 5, 0, 3, 0],
                [0, 3, 0, 0, 0, 0],
                [2, 0, 0, 0, 4, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 4, 0, 2, 0, 0],
                [0, 0, 0, 0, 3, 0]
            ],
            [],
            nameof(_4ConnectionsRule1)
        );

        private static readonly ISolutionProvider _5ConnectionsRule1 = new SolutionProvider(
            [
                [3, 0, 0, 4, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 3, 0, 5, 0, 2],
                [5, 0, 2, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [3, 0, 5, 0, 0, 2]
            ],
            [],
            nameof(_5ConnectionsRule1)
        );

        private static readonly ISolutionProvider _5ConnectionsRule2 = new SolutionProvider(
            [
                [1, 0, 0, 4, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 3, 0, 5, 0, 1],
                [5, 0, 3, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [3, 0, 5, 0, 0, 1]
            ],
            [
                new BridgeCoordinates(new Point(0, 0), new Point(0, 3), 1),
                new BridgeCoordinates(new Point(5, 2), new Point(3, 2), 1),
                new BridgeCoordinates(new Point(5, 5), new Point(2, 5), 2),
            ],
            nameof(_5ConnectionsRule2)
        );

        private static readonly ISolutionProvider _6ConnectionsRule1 = new SolutionProvider(
            [
                [3, 0, 6, 0, 0, 2],
                [0, 0, 0, 3, 0, 0],
                [0, 0, 2, 0, 0, 0],
                [2, 0, 0, 6, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 4, 0, 0]
            ],
            [],
            nameof(_6ConnectionsRule1)
        );

        private static readonly ISolutionProvider _7ConnectionsRule1 = new SolutionProvider(
            [
                [0, 0, 3, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [2, 0, 7, 0, 4, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 5, 0, 0, 0]
            ],
            [],
            nameof(_7ConnectionsRule1)
        );

        private static readonly ISolutionProvider _8ConnectionsRule1 = new SolutionProvider(
            [
                [0, 0, 3, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [4, 0, 8, 0, 0, 5],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 2, 0, 0, 0],
                [0, 0, 0, 0, 0, 0]
            ],
            [],
            nameof(_8ConnectionsRule1)
        );

        private static readonly ISolutionProvider _9GeneralRule1 = new SolutionProvider(
            [
                [0, 2, 0, 0, 3, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 1, 0, 4, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 2, 0]
            ],
            [],
            nameof(_9GeneralRule1)
        );

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
