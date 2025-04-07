using Newtonsoft.Json;

namespace Hashi.Gui.JsonConverters
{
    /// <summary>
    ///   A JSON converter that converts between a real type and an abstract type.
    /// </summary>
    /// <typeparam name="TReal">The real type.</typeparam>
    /// <typeparam name="TAbstract">The abstract type.</typeparam>
    public class AbstractConverter<TReal, TAbstract>
        : JsonConverter where TReal : TAbstract
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
            => objectType == typeof(TAbstract);

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type type, object? value, JsonSerializer jser)
            => jser.Deserialize<TReal>(reader);

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer jser)
            => jser.Serialize(writer, value);
    }
}
