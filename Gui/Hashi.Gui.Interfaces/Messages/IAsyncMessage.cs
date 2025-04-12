using System.Diagnostics.CodeAnalysis;

namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents an asynchronous message that can be sent and received.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IAsyncMessage
{
    /// <summary>
    ///     Gets the cancellation token for this message.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    ///     Gets the collection of responses to this message.
    /// </summary>
    IReadOnlyCollection<Task> Responses { get; }

    /// <summary>
    ///     Replies to the message with a task.
    /// </summary>
    /// <param name="response">The response.</param>
    void Reply(Task response);
}

/// <summary>
///     Represents an asynchronous message that carries a value of type T.
/// </summary>
/// <typeparam name="T">The type T.</typeparam>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IAsyncMessage<out T> : IAsyncMessage
{
    /// <summary>
    ///     Gets the value of type T associated with this message.
    /// </summary>
    T Value { get; }
}