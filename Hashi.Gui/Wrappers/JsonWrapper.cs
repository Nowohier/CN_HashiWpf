using Hashi.Gui.Interfaces.Wrappers;
using Newtonsoft.Json;
using System.IO;

namespace Hashi.Gui.Wrappers
{
    /// <inheritdoc cref="IJsonWrapper"/>
    public class JsonWrapper : IJsonWrapper
    {
        private readonly JsonSerializer serializer = new();

        /// <inheritdoc />
        public object? Deserialize(TextReader reader, Type objectType)
        {
            return serializer.Deserialize(new JsonTextReader(reader), objectType);
        }

        /// <inheritdoc />
        public string SerializeObject(object? value, object formatting)
        {
            if (!(formatting is Formatting format))
            {
                throw new ArgumentException("Invalid formatting type", nameof(formatting));
            }

            return JsonConvert.SerializeObject(value, format, (JsonSerializerSettings?)null);
        }
    }
}
