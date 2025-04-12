using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.JsonConverters;
using Hashi.Gui.ViewModels;
using Newtonsoft.Json;

namespace Hashi.Gui.Wrappers;

/// <inheritdoc cref="IJsonWrapper" />
public class JsonWrapper : IJsonWrapper
{
    private readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
        Converters =
        {
            new AbstractConverter<HighScorePerDifficultyViewModel, IHighScorePerDifficultyViewModel>(),
            new AbstractConverter<SettingsViewModel, ISettingsViewModel>()
        }
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