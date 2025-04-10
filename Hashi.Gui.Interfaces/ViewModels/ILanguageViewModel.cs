namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Represents a view model for a language.
/// </summary>
public interface ILanguageViewModel
{
    /// <summary>
    ///     The english name of the language.
    /// </summary>
    string LanguageNameEnglish { get; set; }

    /// <summary>
    ///     The native name of the language.
    /// </summary>
    string LanguageNameNative { get; set; }

    /// <summary>
    ///     The culture of the language.
    /// </summary>
    string Culture { get; set; }
}