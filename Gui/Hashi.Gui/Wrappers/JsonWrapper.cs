using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.JsonConverters;
using Hashi.Gui.ViewModels.Settings;
using Newtonsoft.Json;

namespace Hashi.Gui.Wrappers;

/// <inheritdoc cref="IJsonWrapper" />
public class JsonWrapper : IJsonWrapper
{
    private static readonly List<JsonConverter> CustomConverters =
    [
        new AbstractConverter<HighScorePerDifficultyViewModel, IHighScorePerDifficultyViewModel>(),
        new AbstractConverter<SettingsViewModel, ISettingsViewModel>(),
        new AbstractConverter<SolutionProvider, ISolutionProvider>()
    ];

    private readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
        Converters = CustomConverters
    };

    /// <inheritdoc />
    public object? DeserializeObject(string jsonString, Type objectType)
    {
        return JsonConvert.DeserializeObject(jsonString, objectType, settings);
    }

    /// <inheritdoc />
    public string SerializeObject(object? value)
    {
        return JsonConvert.SerializeObject(value, settings);
    }
}