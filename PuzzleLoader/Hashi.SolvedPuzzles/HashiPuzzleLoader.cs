using System.Text.Json;
using Hashi.Gui.Interfaces.Wrappers;
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

    private readonly IFileWrapper _fileWrapper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HashiPuzzleLoader"/> class.
    /// </summary>
    /// <param name="fileWrapper">The file wrapper used for file system operations.</param>
    public HashiPuzzleLoader(IFileWrapper fileWrapper)
    {
        _fileWrapper = fileWrapper;
    }

    /// <inheritdoc />
    public int[][] LoadPuzzle(HashiFileEnum hashiFileEnum)
    {
        var fileName = GetHashiFileName(hashiFileEnum);

        if (!_fileWrapper.Exists(fileName))
        {
            throw new FileNotFoundException($"File {fileName} not found.");
        }

        var fileContent = _fileWrapper.ReadAllText(fileName).Replace("{", "[").Replace("}", "]");

        // Deserialize the JSON string to an int[][]
        var puzzle = JsonSerializer.Deserialize<int[][]>(fileContent);

        if (puzzle == null)
        {
            throw new InvalidDataException($"Failed to deserialize the puzzle from file {fileName}.");
        }

        return puzzle;
    }

    private static string GetHashiFileName(HashiFileEnum hashiFileEnum)
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