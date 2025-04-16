using Hashi.Enums;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;
using Hashi.Rules;
using Hashi.Rules.Extensions;
using NRules;
using NRules.Diagnostics;
using NRules.RuleModel;

namespace Hashi.Gui.Providers
{
    /// <inheritdoc cref="IHintProvider"/>
    public class HintProvider : IHintProvider
    {
        private readonly IIslandProvider islandProvider;
        private readonly IDialogWrapper dialogWrapper;
        private readonly IRuleRepository ruleRepository;
        private ISession? session;

        /// <summary>
        /// Initializes a new instance of the <see cref="HintProvider"/> class.
        /// </summary>
        /// <param name="islandProvider">The island provider.</param>
        /// <param name="dialogWrapper">The dialog wrapper.</param>
        /// <param name="ruleRepository">The rule repository.</param>
        /// <param name="ruleInfoProvider">The rule info provider.</param>
        public HintProvider(IIslandProvider islandProvider, IDialogWrapper dialogWrapper, IRuleRepository ruleRepository, IRuleInfoProvider ruleInfoProvider)
        {
            RuleInfoProvider = ruleInfoProvider;
            this.islandProvider = islandProvider;
            this.dialogWrapper = dialogWrapper;
            this.ruleRepository = ruleRepository;

        }

        /// <inheritdoc />
        public IRuleInfoProvider RuleInfoProvider { get; }

        /// <inheritdoc />
        public IList<Type> Rules { get; } = typeof(_1ConnectionRule1).Assembly.GetTypes()
            .Where(static x => x.Name.StartsWith('_')).ToList();

        /// <inheritdoc />
        public void ResetSession()
        {
            if (session == null) return;
            RuleInfoProvider.AreRulesBeingApplied = false;
            session.Events.RhsExpressionEvaluatedEvent -= OnRhsExpressionEvaluated;
            session = null;
        }

        /// <inheritdoc />
        public void GenerateHint(Type selectedRule)
        {
            ArgumentNullException.ThrowIfNull(selectedRule);

            if (RuleInfoProvider.AreRulesBeingApplied) return;

            RuleInfoProvider.AreRulesBeingApplied = true;

            if (session == null)
            {
                //Compile rules
                var factory = selectedRule != typeof(_0AllRules) ? ruleRepository.CompileOne(selectedRule.FullName!) : ruleRepository.Compile();

                //Create rules session
                session = factory.CreateSession();
                session.Events.RhsExpressionEvaluatedEvent += OnRhsExpressionEvaluated;
                session.InsertAll(islandProvider.IslandsFlat);
            }
            else
            {
                session.UpdateAll(islandProvider.IslandsFlat);
            }

            var rulesFired = session.Fire();

            if (rulesFired == 0)
                dialogWrapper.Show(TranslationSource.Instance["MessageNoHintsCaption"]!,
                    TranslationSource.Instance["MessageNoHintsText"]!, DialogButton.Ok, DialogImage.Information);

            RuleInfoProvider.AreRulesBeingApplied = false;
        }

        private void OnRhsExpressionEvaluated(object? sender, RhsExpressionEventArgs e)
        {
            RuleInfoProvider.AreRulesBeingApplied = false;
        }
    }
}
