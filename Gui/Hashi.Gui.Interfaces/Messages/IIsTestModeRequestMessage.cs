namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents a message that requests the test mode status.
/// </summary>
public interface IIsTestModeRequestMessage
{
    /// <summary>
    ///     The response to the request message.
    /// </summary>
    bool Response { get; }

    /// <summary>
    ///     Replies to the request message with the given response.
    /// </summary>
    /// <param name="response">The response.</param>
    void Reply(bool response);
}