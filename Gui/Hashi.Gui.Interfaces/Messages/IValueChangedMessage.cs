namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents a message that contains a value of type <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">The value of type <typeparamref name="T" /></typeparam>
public interface IValueChangedMessage<out T>
{
    /// <summary>
    ///     Gets the value of the message.
    /// </summary>
    public T Value { get; }
}