using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hashi.Gui.JsonConverters;

/// <summary>
///     A JSON converter for <see cref="Point" /> that reads and writes the "X, Y" string format.
/// </summary>
public class PointJsonConverter : JsonConverter<Point>
{
    /// <inheritdoc />
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()
                    ?? throw new JsonException("Expected a string value for Point.");

        var parts = value.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 2
            || !int.TryParse(parts[0], out var x)
            || !int.TryParse(parts[1], out var y))
        {
            throw new JsonException($"Invalid Point format: '{value}'. Expected 'X, Y'.");
        }

        return new Point(x, y);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{value.X}, {value.Y}");
    }
}
