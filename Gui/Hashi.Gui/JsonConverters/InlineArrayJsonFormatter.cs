using System.Text.RegularExpressions;

namespace Hashi.Gui.JsonConverters;

/// <summary>
///     A utility that formats JSON strings to collapse array elements onto single lines.
/// </summary>
public static partial class InlineArrayJsonFormatter
{
    /// <summary>
    ///     Formats a JSON string by collapsing array elements onto single lines with space separation.
    /// </summary>
    /// <param name="json">The indented JSON string to format.</param>
    /// <returns>The formatted JSON string with inline arrays.</returns>
    public static string FormatInlineArrays(string json)
    {
        return InlineArrayRegex().Replace(json, match =>
        {
            var inner = match.Groups[1].Value;
            var elements = inner.Split(',')
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e));
            return $"[{string.Join(", ", elements)}]";
        });
    }

    [GeneratedRegex(@"\[\s*\n\s*([\d,\s\n]+?)\s*\]", RegexOptions.Singleline)]
    private static partial Regex InlineArrayRegex();
}
