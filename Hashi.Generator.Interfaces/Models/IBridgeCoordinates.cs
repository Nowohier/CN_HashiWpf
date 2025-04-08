using System.Drawing;

namespace Hashi.Generator.Interfaces.Models
{
    /// <summary>
    /// Contains the coordinates of the two islands and the amount of bridges between them.
    /// </summary>
    public interface IBridgeCoordinates
    {
        /// <summary>
        /// Coordinates of the first island.
        /// </summary>
        Point Location1 { get; }

        /// <summary>
        /// Coordinates of the second island.
        /// </summary>
        Point Location2 { get; }

        /// <summary>
        /// Amount of bridges between the two islands.
        /// </summary>
        int AmountBridges { get; }

        /// <summary>
        /// Checks if the bridge is horizontal or vertical.
        /// </summary>
        bool IsHorizontal { get; }

        /// <summary>
        /// Checks if the bridge is vertical or horizontal.
        /// </summary>
        bool IsVertical { get; }
    }
}
