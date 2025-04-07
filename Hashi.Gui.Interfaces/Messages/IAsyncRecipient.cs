namespace Hashi.Gui.Interfaces.Messages
{
    /// <summary>
    /// Represents an asynchronous recipient that can receive messages of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public interface IAsyncRecipient<in TMessage>
        where TMessage : IAsyncMessage
    {
        /// <summary>
        /// Receives a message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>a Task.</returns>
        Task ReceiveAsync(TMessage message, CancellationToken cancellationToken);
    }
}
