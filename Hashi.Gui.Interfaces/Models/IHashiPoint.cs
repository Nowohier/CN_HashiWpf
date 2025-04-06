namespace Hashi.Gui.Interfaces.Models
{
    /// <summary>
    /// A point containing x and y coordinates.
    /// </summary>
    public interface IHashiPoint
    {
        /// <summary>
        /// Gets the x coordinate of the point.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the y coordinate of the point.
        /// </summary>
        public double Y { get; }
    }
}
