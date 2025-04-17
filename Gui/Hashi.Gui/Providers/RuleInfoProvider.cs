using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;

namespace Hashi.Gui.Providers
{
    /// <inheritdoc cref="IRuleInfoProvider"/>
    public class RuleInfoProvider(
        Func<bool?, IUpdateAllIslandColorsMessage> updateIslandColorsMessageFactory,
        Func<bool?, IRuleMessageClearedMessage> ruleMessageClearedMessageFactory)
        : ObservableObject, IRuleInfoProvider
    {
        private string ruleMessage = string.Empty;


        /// <inheritdoc />
        public bool AreRulesBeingApplied { get; set; }

        /// <inheritdoc />
        public string RuleMessage
        {
            get => ruleMessage;
            set
            {
                if (!SetProperty(ref ruleMessage, value) || ruleMessage != string.Empty) return;
                WeakReferenceMessenger.Default.Send(ruleMessageClearedMessageFactory.Invoke(null));
                WeakReferenceMessenger.Default.Send(updateIslandColorsMessageFactory.Invoke(null));
            }
        }
    }
}
