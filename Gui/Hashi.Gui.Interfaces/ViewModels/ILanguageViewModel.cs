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
}