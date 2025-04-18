using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.JsonConverters;
using Hashi.Gui.ViewModels.Settings;
using Newtonsoft.Json;
using System.IO;

namespace Hashi.Gui.Wrappers;

/// <inheritdoc cref="IJsonWrapper" />
public class JsonWrapper : IJsonWrapper
{
    private static readonly List<JsonConverter> CustomConverters =
    [
        new AbstractConverter<HighScorePerDifficultyViewModel, IHighScorePerDifficultyViewModel>(),
        new AbstractConverter<SettingsViewModel, ISettingsViewModel>(),
        new AbstractConverter<SolutionProvider, ISolutionProvider>(),
        new AbstractConverter<BridgeCoordinates, IBridgeCoordinates>()
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

    /// <inheritdoc />
    public string SerializeWithCustomIndenting(object obj)
    {
        // Ignores the indenting on arrays

        using var sw = new StringWriter();
        using JsonWriter writer = new HashiJsonTextWriter(sw);

        writer.Formatting = Formatting.Indented;

        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None
        };

        CustomConverters.ForEach(converter => serializer.Converters.Add(converter));

        serializer.Serialize(writer, obj);
        return sw.ToString();
    }
}