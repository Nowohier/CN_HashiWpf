namespace Hashi.Gui.Interfaces.Providers
{
    /// <summary>
    /// Provides information about the rules being applied in the game.
    /// </summary>
    public interface IRuleInfoProvider
    {
        /// <summary>
        /// The rule message to be displayed.
        /// </summary>
        string RuleMessage { get; set; }

        /// <summary>
        /// Determines if the rules are being applied.
        /// </summary>
        bool AreRulesBeingApplied { get; set; }
    }
}
