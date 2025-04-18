using Newtonsoft.Json;
using System.IO;

namespace Hashi.Gui.JsonConverters;

/// <summary>
///     A custom JSON text writer that formats the output to ignore array indenting.
/// </summary>
/// <param name="writer">The TextWriter to use.</param>
public class HashiJsonTextWriter(TextWriter writer) : JsonTextWriter(writer)
{
    /// <inheritdoc />
    protected override void WriteIndent()
    {
        if (WriteState != WriteState.Array)
            base.WriteIndent();
        else
            WriteIndentSpace();
    }
}