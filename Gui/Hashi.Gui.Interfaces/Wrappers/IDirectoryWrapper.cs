namespace Hashi.Gui.Interfaces.Wrappers
{
    /// <summary>
    /// Provides methods for interacting with directories in the file system.
    /// </summary>
    /// <remarks>This interface abstracts common directory operations, such as checking for the existence of a
    /// directory and creating new directories. It is useful for scenarios where directory operations need to be mocked
    /// or tested in isolation.</remarks>
    public interface IDirectoryWrapper
    {
        /// <summary>
        /// Determines whether the specified file or directory exists at the given path.
        /// </summary>
        /// <remarks>This method checks the existence of a file or directory without distinguishing
        /// between the two. Use additional methods to determine whether the path refers to a file or
        /// directory.</remarks>
        /// <param name="path">The path to the file or directory to check. This can be an absolute or relative path.</param>
        /// <returns><see langword="true"/> if the file or directory exists at the specified path; otherwise, <see
        /// langword="false"/>. </returns>
        bool Exists(string path);

        /// <summary>
        /// Creates a new directory at the specified path.
        /// </summary>
        /// <remarks>If the directory already exists, this method does nothing. Ensure the caller has
        /// appropriate permissions to create directories at the specified path.</remarks>
        /// <param name="path">The path where the directory should be created. Must be a valid file system path.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Deletes the specified file or directory at the given path.
        /// </summary>
        /// <remarks>Use this method with caution, especially when <paramref name="recursive"/> is set to
        /// <see langword="true"/>,  as it can delete large amounts of data. Ensure the <paramref name="path"/> is
        /// correct to avoid unintended deletions.</remarks>
        /// <param name="path">The path of the file or directory to delete. Must be a valid path and cannot be null or empty.</param>
        /// <param name="recursive">A value indicating whether to delete directories and their contents recursively. If <see langword="true"/>,
        /// all subdirectories and files within the directory will be deleted. If <see langword="false"/>, only the
        /// specified directory will be deleted, and an exception will be thrown if the directory is not empty.</param>
        void Delete(string path, bool recursive);
    }
}
