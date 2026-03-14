using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.JsonConverters;
using Hashi.Gui.ViewModels.Settings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hashi.Gui.Wrappers;

/// <inheritdoc cref="IJsonWrapper" />
public class JsonWrapper : IJsonWrapper
{
    private readonly JsonSerializerOptions settings = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new PointJsonConverter(),
            new AbstractConverter<HighScorePerDifficultyViewModel, IHighScorePerDifficultyViewModel>(),
            new AbstractConverter<SettingsViewModel, ISettingsViewModel>(),
            new AbstractConverter<SolutionProvider, ISolutionProvider>(),
            new AbstractConverter<BridgeCoordinates, IBridgeCoordinates>()
        }
    };

    /// <inheritdoc />
    public object? DeserializeObject(string jsonString, Type objectType)
    {
        return JsonSerializer.Deserialize(jsonString, objectType, settings);
    }

    /// <inheritdoc />
    public string SerializeObject(object? value)
    {
        return JsonSerializer.Serialize(value, settings);
    }

    /// <inheritdoc />
    public string SerializeWithCustomIndenting(object obj)
    {
        var json = JsonSerializer.Serialize(obj, settings);
        return InlineArrayJsonFormatter.FormatInlineArrays(json);
    }
}
