using Hashi.Gui.Interfaces.Providers;

namespace Hashi.Generator.Simulation;

/// <summary>
///     A lightweight simulation implementation of <see cref="IRuleInfoProvider" /> used for rule-based solvability
///     validation.
/// </summary>
internal class SimulationRuleInfoProvider : IRuleInfoProvider
{
    /// <inheritdoc />
    public string RuleMessage { get; set; } = string.Empty;

    /// <inheritdoc />
    public bool AreRulesBeingApplied { get; set; }
}
