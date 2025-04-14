using NRules;
using NRules.RuleModel;

namespace Hashi.Rules.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IRuleRepository"/>.
    /// </summary>
    public static class HashiRuleRepositoryExtensions
    {
        /// <summary>
        /// Compiles all rules in the repository into a session factory.
        /// Use <see cref="RuleCompiler"/> explicitly if only need to compile a subset of rules.
        /// </summary>
        /// <param name="repository">Rule repository.</param>
        /// <param name="ruleName">The rule name.</param>
        /// <returns>Session factory.</returns>
        /// <seealso cref="RuleCompiler"/>
        public static ISessionFactory CompileOne(this IRuleRepository repository, string ruleName)
        {
            var compiler = new RuleCompiler();
            if (repository.GetRules().FirstOrDefault(x => x.Name.Equals(ruleName)) is not { } ruleDefinition)
            {
                throw new ArgumentException($"Rule {ruleName} not found in the repository.");
            }

            ISessionFactory factory = compiler.Compile([ruleDefinition], CancellationToken.None);
            return factory;
        }
    }
}
