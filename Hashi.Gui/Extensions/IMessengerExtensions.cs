using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Messages;
using System.Diagnostics.CodeAnalysis;

namespace Hashi.Gui.Extensions;

/// <summary>
///     Extensions for the IMessenger interface to support asynchronous messaging.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
public static partial class IMessengerExtensions
{
    /// <summary>
    ///     Registers an asynchronous recipient for a specific message type.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="messenger">The messenger.</param>
    /// <param name="recipient">The recipient.</param>
    public static void Register<TMessage>(this IMessenger messenger, IAsyncRecipient<TMessage> recipient)
        where TMessage : class, IAsyncMessage
    {
        messenger.Register<TMessage>(recipient, (r, message) =>
        {
            if (r is IAsyncRecipient<TMessage> asyncRecipient)
            {
                var task = asyncRecipient.ReceiveAsync(message, message.CancellationToken);
                message.Reply(task);
            }
        });
    }

    /// <summary>
    ///     Sends an asynchronous message and waits for all responses.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="messenger">The messenger.</param>
    /// <param name="message">The message.</param>
    /// <returns>a Task.</returns>
    public static Task SendAsync<TMessage>(this IMessenger messenger, TMessage message)
        where TMessage : AsyncMessage
    {
        messenger.Send(message);
        return Task.WhenAll(message.Responses);
    }

    /// <summary>
    ///     Sends an asynchronous message of type TMessage and waits for all responses.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="messenger">The messenger.</param>
    /// <returns>a Task.</returns>
    public static Task SendAsync<TMessage>(this IMessenger messenger)
        where TMessage : AsyncMessage, new()
    {
        return messenger.SendAsync(new TMessage());
    }
}