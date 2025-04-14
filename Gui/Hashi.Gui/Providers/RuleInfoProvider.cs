using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Messages;

namespace Hashi.Gui.Providers
{
    /// <inheritdoc cref="IRuleInfoProvider"/>
    public class RuleInfoProvider(IIslandProvider islandProvider) : ObservableRecipient, IRuleInfoProvider
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
                WeakReferenceMessenger.Default.Send(new HintPopupClosedMessage());
                islandProvider.RefreshIslandColors();
            }
        }
    }
}
