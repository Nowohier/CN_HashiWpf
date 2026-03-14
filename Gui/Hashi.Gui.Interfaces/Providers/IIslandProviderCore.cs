namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides the shared grid-navigation and connection management logic
///     used by both the WPF <see cref="IIslandProvider" /> and the simulation provider.
///     Composite of <see cref="INeighborFinder" />, <see cref="IConnectionValidator" />,
///     and <see cref="IIslandGroupCounter" />.
/// </summary>
public interface IIslandProviderCore : INeighborFinder, IConnectionValidator, IIslandGroupCounter
{
}
