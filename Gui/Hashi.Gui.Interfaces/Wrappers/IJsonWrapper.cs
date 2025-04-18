namespace Hashi.Gui.Interfaces.Wrappers;

/// <summary>
///     Interface for a JSON wrapper.
/// </summary>
public interface IJsonWrapper
{
    /// <summary>
    ///     Deserializes the JSON structure contained by the specified JsonReader
    ///     into an instance of the specified type.
    /// </summary>
    /// <param name="jsonString">The json string.</param>
    /// <param name="objectType">The <see cref="Type" /> of object being deserialized.</param>
    /// <returns>The instance of <paramref name="objectType" /> being deserialized.</returns>
    object? DeserializeObject(string jsonString, Type objectType);

    /// <summary>
    ///     Serializes the specified object to a JSON string using formatting.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>
    ///     A JSON string representation of the object.
    /// </returns>
    string SerializeObject(object? value);

    /// <summary>
    ///     Serializes the specified object to a JSON string using hashi formatting.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>a formatted json string.</returns>
    string SerializeWithCustomIndenting(object obj);
}