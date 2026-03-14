using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hashi.Gui.JsonConverters;

/// <summary>
///     A JSON converter that converts between a real type and an abstract type.
/// </summary>
/// <typeparam name="TReal">The real type.</typeparam>
/// <typeparam name="TAbstract">The abstract type.</typeparam>
public class AbstractConverter<TReal, TAbstract>
    : JsonConverter<TAbstract> where TReal : TAbstract
{
    /// <inheritdoc />
    public override TAbstract? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<TReal>(ref reader, options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TAbstract value, JsonSerializerOptions options)
    {
        // Create a copy of options without this converter to avoid infinite recursion
        var optionsCopy = new JsonSerializerOptions(options);
        optionsCopy.Converters.Remove(this);

        JsonSerializer.Serialize(writer, value, typeof(TReal), optionsCopy);
    }
}
