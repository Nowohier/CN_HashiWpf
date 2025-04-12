using Autofac;
using Hashi.SolvedPuzzles.Interfaces;

namespace Hashi.SolvedPuzzles;

/// <inheritdoc />
// ReSharper disable once UnusedMember.Global
public class AutoFacHashiSolvedPuzzlesModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HashiPuzzleLoader>().As<IHashiPuzzleLoader>().SingleInstance();
    }
}