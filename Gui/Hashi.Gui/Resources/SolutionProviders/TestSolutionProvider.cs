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
        [JsonProperty(nameof(SolutionProviders))]
        public List<ISolutionProvider> SolutionProviders => [
            new SolutionProvider(null!, null!, "None"),
            new SolutionProvider(
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
                "_9GeneralRule2"
            )
        ];

        //private static readonly ISolutionProvider None = new SolutionProvider(null!, null!, nameof(None));

        //private static readonly ISolutionProvider _9GeneralRule2 = new SolutionProvider(
        //    [
        //        [1, 0, 3, 0, 0, 3],
        //        [0, 0, 0, 0, 0, 0],
        //        [2, 0, 5, 0, 2, 0],
        //        [0, 0, 0, 2, 0, 4],
        //        [3, 0, 2, 0, 1, 0],
        //        [0, 1, 0, 3, 0, 3]
        //    ],
        //    [
        //        new BridgeCoordinates(new Point(2, 0), new Point(5, 0), 2),
        //        new BridgeCoordinates(new Point(5, 0), new Point(5, 3), 1),
        //        new BridgeCoordinates(new Point(3, 3), new Point(5, 3), 2),
        //        new BridgeCoordinates(new Point(5, 3), new Point(5, 5), 1),
        //        new BridgeCoordinates(new Point(5, 5), new Point(3, 5), 2),
        //        new BridgeCoordinates(new Point(3, 5), new Point(1, 5), 1)
        //    ],
        //    nameof(_9GeneralRule2)
        //);
    }
}
