using System.ComponentModel;
using System.Globalization;
using System.Resources;
using Hashi.Gui.Language;

namespace Hashi.Gui.Translation;

/// <summary>
///     Provides a singleton instance for accessing localized strings.
/// </summary>
public class TranslationSource : ITranslationSource
{
    private readonly ResourceManager resManager = Resources.ResourceManager;
    private CultureInfo? currentCulture;

    /// <summary>
    ///     Gets the singleton instance used for XAML bindings.
    /// </summary>
    public static TranslationSource Instance { get; } = new();


    /// <inheritdoc />
    public string? this[string key] => resManager.GetString(key, currentCulture);

    /// <inheritdoc />
    public string GetRequired(string key) =>
        this[key] ?? throw new InvalidOperationException($"Missing translation: {key}");

    /// <inheritdoc />
    public CultureInfo? CurrentCulture
    {
        get => currentCulture;
        set
        {
            if (Equals(currentCulture, value))
            {
                return;
            }

            currentCulture = value;
            var @event = PropertyChanged;
            @event?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
}
