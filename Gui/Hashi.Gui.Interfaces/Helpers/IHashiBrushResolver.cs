using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Interfaces.Helpers
{
    /// <summary>
    /// Defines a mechanism for resolving brushes based on a specified color.
    /// </summary>
    /// <remarks>Implementations of this interface provide a way to map a <see cref="HashiColor"/> to an <see
    /// cref="IHashiBrush"/>. This can be used for rendering or styling purposes where brushes are required.</remarks>
    public interface IHashiBrushResolver
    {
        /// <summary>
        /// Resolves a brush corresponding to the specified color.
        /// </summary>
        /// <remarks>This method is typically used to retrieve a brush for rendering purposes based on a
        /// predefined color. Ensure that the <paramref name="color"/> parameter is valid and supported by the
        /// implementation.</remarks>
        /// <param name="color">The color for which to resolve a brush. Must be a valid <see cref="HashiColor"/> value.</param>
        /// <returns>An <see cref="IHashiBrush"/> instance representing the brush for the specified color,  or <see
        /// langword="null"/> if no brush is available for the given color.</returns>
        IHashiBrush ResolveBrush(HashiColor color);
    }
}
