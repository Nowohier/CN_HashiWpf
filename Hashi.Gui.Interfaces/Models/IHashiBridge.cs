using Hashi.Enums;

namespace Hashi.Gui.Interfaces.Models
{
    /// <summary>
    ///   Represents a bridge between two points in the Hashi game.
    /// </summary>
    public interface IHashiBridge
    {
        /// <summary>
        ///    Gets the action type of the bridge operation.
        /// </summary>
        BridgeOperationTypeEnum ActionType { get; }

        /// <summary>
        ///     Gets the first point.
        /// </summary>
        IHashiPoint Point1 { get; }

        /// <summary>
        ///     Gets the second point.
        /// </summary>
        IHashiPoint Point2 { get; }
    }
}
