namespace Hashi.SolvedPuzzles.Interfaces
{
    /// <summary>
    ///   Loads a puzzle from a file.
    /// </summary>
    public interface IHashiPuzzleLoader
    {
        /// <summary>
        ///     Loads a puzzle from a file depending on the given enum.
        /// </summary>
        /// <param name="hashiFileEnum">The hashi file enum.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException" />
        /// <exception cref="Exception" />
        int[][] LoadPuzzle(HashiFileEnum hashiFileEnum);
    }
}
