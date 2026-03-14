using Hashi.Gui.Interfaces.Wrappers;
using System.IO;

namespace Hashi.Gui.Wrappers
{
    /// <inheritdoc cref="IDirectoryWrapper" />
    public class DirectoryWrapper : IDirectoryWrapper
    {
        /// <inheritdoc />
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }
    }
}
