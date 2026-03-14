using FluentAssertions;
using Hashi.Gui.Messages;

namespace Hashi.Gui.Test.Messages;

/// <summary>
/// Unit tests for <see cref="AsyncMessage"/> and <see cref="AsyncMessage{T}"/> classes.
/// </summary>
[TestFixture]
public class AsyncMessageTests
{
    #region AsyncMessage Tests

    [Test]
    public void CancellationToken_WhenDefaultCancellation_ShouldReturnNonDefaultToken()
    {
        // Arrange & Act
        var message = new TestAsyncMessage();

        // Assert — linked token source creates a new token, so it should not be CancellationToken.None
        message.CancellationToken.Should().NotBe(CancellationToken.None);
    }

    [Test]
    public void CancellationToken_WhenCustomToken_ShouldReturnLinkedToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var message = new TestAsyncMessage(cts.Token);

        // Assert — linked token should not be cancelled initially
        message.CancellationToken.CanBeCanceled.Should().BeTrue();
        message.CancellationToken.IsCancellationRequested.Should().BeFalse();

        // When the source is cancelled, the linked token should also be cancelled
        cts.Cancel();
        message.CancellationToken.IsCancellationRequested.Should().BeTrue();
    }

    #endregion

    #region AsyncMessage<T> Tests

    [Test]
    public void Constructor_WhenValueProvided_ShouldSetValue()
    {
        // Arrange & Act
        var message = new AsyncMessage<string>("test-value");

        // Assert
        message.Value.Should().Be("test-value");
    }

    [Test]
    public void Value_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Arrange
        var expected = 42;

        // Act
        var message = new AsyncMessage<int>(expected);

        // Assert
        message.Value.Should().Be(expected);
    }

    #endregion

    #region Test Types

    /// <summary>
    /// Concrete subclass of AsyncMessage for testing the abstract base class.
    /// </summary>
    private class TestAsyncMessage : AsyncMessage
    {
        public TestAsyncMessage(CancellationToken cancellationToken = default) : base(cancellationToken)
        {
        }
    }

    #endregion
}
