using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Messages;
using Hashi.Gui.ViewModels;

namespace Hashi.Gui.Providers
{
    /// <inheritdoc cref="IRuleInfoProvider"/>
    public class RuleInfoProvider(IIslandProvider islandProvider) : NotifyPropertyChangedBase, IRuleInfoProvider
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
                WeakReferenceMessenger.Default.Send(new RuleMessageClearedMessage());
                islandProvider.RefreshIslandColors();
            }
        }
    }
}
