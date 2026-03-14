using System.ComponentModel;
using System.Globalization;

namespace Hashi.Gui.Translation;

/// <summary>
///     Provides access to localized strings based on the current culture.
/// </summary>
public interface ITranslationSource : INotifyPropertyChanged
{
    /// <summary>
    ///     Gets the localized string for the specified key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string, or <c>null</c> if the key is not found.</returns>
    string? this[string key] { get; }

    /// <summary>
    ///     Gets or sets the current culture for localization.
    /// </summary>
    CultureInfo? CurrentCulture { get; set; }

    /// <summary>
    ///     Gets the localized string for the specified key, throwing if the key is missing.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the key is not found.</exception>
    string GetRequired(string key);
}
