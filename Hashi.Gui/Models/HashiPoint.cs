using Hashi.Gui.Interfaces.Models;
using System.Windows;

namespace Hashi.Gui.Models
{
    /// <inheritdoc />
    public class HashiPoint : IHashiPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashiPoint"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public HashiPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <inheritdoc />
        public double X { get; }

        /// <inheritdoc />
        public double Y { get; }

        public static implicit operator Point(HashiPoint hashiPoint)
        {
            return new Point(hashiPoint.X, hashiPoint.Y);
        }

        public static implicit operator HashiPoint(Point point)
        {
            return new HashiPoint(point.X, point.Y);
        }
    }
}
