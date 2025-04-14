using Autofac;
using NRules.Integration.Autofac;

namespace Hashi.Rules
{
    /// <inheritdoc />
    public class AutoFacRulesModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterRuleRepository(r => r.AssemblyOf<_1ConnectionRule1>());
        }
    }
}
