namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Represents a view model for a language.
/// </summary>
public interface ILanguageViewModel
{
    /// <summary>
    ///     The culture of the language.
    /// </summary>
    string Culture { get; set; }

    /// <summary>
    /// Gets or sets the name of the language in English.
    /// </summary>
    string LanguageNameEnglish { get; set; }

    /// <summary>
    /// Gets or sets the native name of the language.
    /// </summary>
    string LanguageNameNative { get; set; }
}