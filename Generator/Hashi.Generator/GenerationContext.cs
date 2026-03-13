using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator;

/// <summary>
///     Holds the mutable state for a single puzzle generation run.
/// </summary>
internal class GenerationContext
{
    internal List<IBridge> Bridges { get; } = [];
    internal List<IIsland> Islands { get; } = [];
    internal int[][] Field { get; set; } = [];

    internal void Clear()
    {
        Bridges.Clear();
        Islands.Clear();
        Field = [];
    }
}
