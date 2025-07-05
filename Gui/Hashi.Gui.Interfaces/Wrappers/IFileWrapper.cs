namespace Hashi.Gui.Interfaces.Wrappers
{
    /// <summary>
    /// Provides methods for interacting with file system operations, such as checking file existence, reading file
    /// contents, and writing text to files.
    /// </summary>
    public interface IFileWrapper
    {
        /// <summary>
        /// Determines whether the specified file or directory exists at the given path.
        /// </summary>
        /// <remarks>This method checks the existence of a file or directory at the specified path. It
        /// does not differentiate between files and directories. Ensure that the path is properly formatted and
        /// accessible to avoid exceptions.</remarks>
        /// <param name="path">The path to the file or directory to check. This must be a valid, non-null string.</param>
        /// <returns><see langword="true"/> if the file or directory exists at the specified path; otherwise, <see
        /// langword="false"/>.</returns>
        bool Exists(string path);

        /// <summary>
        /// Reads all text from the specified file.
        /// </summary>
        /// <remarks>This method reads the entire contents of the file into memory as a single string. For
        /// large files, this may result in high memory usage.</remarks>
        /// <param name="path">The path to the file to read. The path must be a valid file path and cannot be null or empty.</param>
        /// <returns>The contents of the file as a single string.</returns>
        string ReadAllText(string path);

        /// <summary>
        /// Writes the specified text to a file at the given path, overwriting the file if it already exists.
        /// </summary>
        /// <remarks>If the file specified by <paramref name="path"/> does not exist, it will be created.
        /// If the file already exists, its contents will be replaced with the specified <paramref
        /// name="content"/>.</remarks>
        /// <param name="path">The path to the file where the text will be written. Must be a valid file path.</param>
        /// <param name="content">The text content to write to the file. Can be empty but cannot be null.</param>
        void WriteAllText(string path, string content);
    }
}
