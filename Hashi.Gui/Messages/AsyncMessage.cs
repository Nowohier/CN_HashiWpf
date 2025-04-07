using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IAsyncMessage"/>
    public abstract class AsyncMessage : CollectionRequestMessage<Task>, IAsyncMessage
    {
        private readonly CancellationTokenSource? cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncMessage"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected AsyncMessage(CancellationToken cancellationToken = default)
        {
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        /// <inheritdoc />
        public CancellationToken CancellationToken => cancellationTokenSource?.Token ?? default;
    }

    /// <inheritdoc cref="IAsyncMessage{T}"/>
    public class AsyncMessage<T> : AsyncMessage, IAsyncMessage<T>
    {
        /// <inheritdoc />
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncMessage{T}"/> class with the specified value.
        /// </summary>
        /// <param name="value">The value of type T.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public AsyncMessage(T value, CancellationToken cancellationToken = default) : base(cancellationToken)
        {
            Value = value;
        }
    }
}
