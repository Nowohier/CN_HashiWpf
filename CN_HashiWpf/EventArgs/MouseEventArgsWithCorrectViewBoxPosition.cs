using System.Windows;
using System.Windows.Input;

namespace CNHashiWpf.EventArgs
{
    /// <summary>
    /// Represents the mouse event arguments with the correct drag start position.
    /// </summary>
    public class MouseEventArgsWithCorrectViewBoxPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEventArgsWithCorrectViewBoxPosition"/> class.
        /// </summary>
        /// <param name="mouseEventArgs">The normal <see cref="MouseEventArgs"/>.</param>
        /// <param name="dragStartPosition">The correct drag start position.</param>
        public MouseEventArgsWithCorrectViewBoxPosition(MouseEventArgs mouseEventArgs, Point dragStartPosition)
        {
            MouseEventArgs = mouseEventArgs;
            DragStartPosition = dragStartPosition;
        }

        /// <summary>
        /// Gets the drag start position.
        /// </summary>
        public Point DragStartPosition { get; }

        /// <summary>
        /// Gets the mouse event arguments.
        /// </summary>
        public MouseEventArgs MouseEventArgs { get; }

    }
}
