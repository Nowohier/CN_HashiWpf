using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="ILanguageViewModel" />
public class LanguageViewModel(string languageNameEnglish, string languageNameNative, string culture)
    : ObservableObject, ILanguageViewModel
{
    private string culture = culture;
    private string languageNameEnglish = languageNameEnglish;
    private string languageNameNative = languageNameNative;

    /// <inheritdoc />
    public string LanguageNameEnglish
    {
        get => languageNameEnglish;
        set => SetProperty(ref languageNameEnglish, value);
    }

    /// <inheritdoc />
    public string LanguageNameNative
    {
        get => languageNameNative;
        set => SetProperty(ref languageNameNative, value);
    }

    /// <inheritdoc />
    public string Culture
    {
        get => culture;
        set => SetProperty(ref culture, value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return LanguageNameEnglish;
    }
}