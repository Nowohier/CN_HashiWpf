using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Models
{
    /// <inheritdoc cref="IHashiBridge"/>
    public class HashiBridge : IHashiBridge
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HashiBridge" /> class.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        public HashiBridge(BridgeOperationTypeEnum actionType, IHashiPoint point1, IHashiPoint point2)
        {
            ActionType = actionType;
            Point1 = point1;
            Point2 = point2;
        }

        /// <inheritdoc />
        public BridgeOperationTypeEnum ActionType { get; }

        /// <inheritdoc />
        public IHashiPoint Point1 { get; }

        /// <inheritdoc />
        public IHashiPoint Point2 { get; }
    }
}
