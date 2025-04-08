using Hashi.Generator.Interfaces.Models;
using System.Drawing;

namespace Hashi.Generator.Models
{
    /// <inheritdoc cref="IBridgeCoordinates"/>
    public class BridgeCoordinates : IBridgeCoordinates
    {
        /// <summary>
        /// Constructor for the BridgeCoordinates class.
        /// </summary>
        /// <param name="location1">The location of the first island.</param>
        /// <param name="location2">The location of the second island.</param>
        /// <param name="amountBridges">The amount of bridges between the islands.</param>
        public BridgeCoordinates(Point location1, Point location2, int amountBridges)
        {
            Location1 = new Point();
            Location2 = new Point();
            AmountBridges = amountBridges;
        }

        /// <inheritdoc />
        public Point Location1 { get; }

        /// <inheritdoc />
        public Point Location2 { get; }

        /// <inheritdoc />
        public int AmountBridges { get; }

        /// <inheritdoc />
        public bool IsHorizontal => Location1.Y == Location2.Y;

        /// <inheritdoc />
        public bool IsVertical => Location1.X == Location2.X;
    }
}
