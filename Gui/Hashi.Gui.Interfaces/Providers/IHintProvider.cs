namespace Hashi.Gui.Interfaces.Providers
{
    /// <summary>
    ///   Provides hints for the game.
    /// </summary>
    public interface IHintProvider
    {
        /// <summary>
        ///    Gets the rule info provider for the game.
        /// </summary>
        IRuleInfoProvider RuleInfoProvider { get; }

        /// <summary>
        ///     Gets the list of rules available for the game.
        /// </summary>
        IList<Type> Rules { get; }

        /// <summary>
        ///    Resets the session and removes all hints.
        /// </summary>
        void ResetSession();

        /// <summary>
        ///    Generates a hint based on the selected rule.
        /// </summary>
        /// <param name="selectedRule">The selected rule.</param>
        void GenerateHint(Type selectedRule);
    }
}
