using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Hashi.Gui.Translation
{
    /// <summary>
    ///   Provides a singleton instance for accessing localized strings.
    /// </summary>
    public class TranslationSource : INotifyPropertyChanged
    {
        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly ResourceManager resManager = Language.Resources.ResourceManager;
        private CultureInfo? currentCulture;

        /// <summary>
        ///    Initializes a new instance of the <see cref="TranslationSource" /> class.
        /// </summary>
        public static TranslationSource Instance { get; } = new();


        /// <summary>
        ///   Gets the localized string for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>the localized string.</returns>
        public string? this[string key] => resManager.GetString(key, currentCulture);

        /// <summary>
        ///   Gets or sets the current culture for localization.
        /// </summary>
        public CultureInfo? CurrentCulture
        {
            get => currentCulture;
            set
            {
                if (Equals(currentCulture, value)) return;
                currentCulture = value;
                var @event = PropertyChanged;
                @event?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
        }
    }
}