using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Interfaces.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Hashi.Gui.Messaging;

/// <summary>
///     An abstract base class for recipients that can handle asynchronous messages.
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public abstract class AsyncObservableRecipient : ObservableRecipient
{
    // Cache reflection results.
    private static readonly MethodInfo? RegisterMethodInfo = typeof(IMessengerExtensions).GetMethod(
        nameof(IMessengerExtensions.Register), BindingFlags.Public | BindingFlags.Static, [typeof(object)]);

    private static readonly MethodInfo? UnregisterMethodInfo =
        typeof(IMessenger).GetMethod("Unregister", [typeof(object)]);

    /// <inheritdoc />
    protected override void OnActivated()
    {
        base.OnActivated();
        RegisterAsyncHandlers();
    }

    /// <inheritdoc />
    protected override void OnDeactivated()
    {
        UnregisterAsyncHandlers();
        base.OnDeactivated();
    }

    private void RegisterAsyncHandlers()
    {
        var asyncRecipientInterfaces = GetType().GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRecipient<>));

        foreach (var iFace in asyncRecipientInterfaces)
        {
            var messageType = iFace.GetGenericArguments()[0];
            if (RegisterMethodInfo != null)
            {
                var registerMethod = RegisterMethodInfo.MakeGenericMethod(messageType);
                registerMethod.Invoke(null, [Messenger, this]);
            }
        }
    }

    private void UnregisterAsyncHandlers()
    {
        var interfaces = GetType().GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRecipient<>));

        foreach (var iFace in interfaces)
        {
            var messageType = iFace.GetGenericArguments().First();
            if (UnregisterMethodInfo != null)
            {
                var genericMethod = UnregisterMethodInfo.MakeGenericMethod(messageType);
                genericMethod.Invoke(Messenger, [this]);
            }
        }
    }
}