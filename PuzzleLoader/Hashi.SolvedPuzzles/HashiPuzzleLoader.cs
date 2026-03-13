using System.Text.Json;
using Hashi.SolvedPuzzles.Interfaces;

namespace Hashi.SolvedPuzzles;

/// <summary>
///     Loads a puzzle from a file.
/// </summary>
public class HashiPuzzleLoader : IHashiPuzzleLoader
{
    private static readonly string PuzzleDirectoryName = "Hashi_Puzzles";

    private static readonly string PuzzleBasePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PuzzleDirectoryName);

    /// <summary>
    ///     Loads a puzzle from a file depending on the given enum.
    /// </summary>
    /// <param name="hashiFileEnum">The hashi file enum.</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException" />
    /// <exception cref="Exception" />
    public int[][] LoadPuzzle(HashiFileEnum hashiFileEnum)
    {
        var fileName = GetHashiFileName(hashiFileEnum);

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"File {fileName} not found.");
        }

        var fileContent = File.ReadAllText(fileName).Replace("{", "[").Replace("}", "]");

        // Deserialize the JSON string to an int[][]
        var puzzle = JsonSerializer.Deserialize<int[][]>(fileContent);

        if (puzzle == null)
        {
            throw new Exception($"Failed to deserialize the puzzle from file {fileName}.");
        }

        return puzzle;
    }

    private string GetHashiFileName(HashiFileEnum hashiFileEnum)
    {
        var hashiId = hashiFileEnum.ToString();
        var parts = hashiId.Split('_');
        if (parts.Length < 3)
        {
            throw new ArgumentException($"Invalid HashiFileEnum format: {hashiId}. Expected format: Hs_NN_FFF_...");
        }

        var folderName = parts[2];
        var hashiName = $"{hashiId}.has";

        return Path.Combine(PuzzleBasePath, folderName, hashiName);
    }
}