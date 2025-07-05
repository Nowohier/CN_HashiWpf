using Hashi.Gui.Interfaces.Wrappers;
using System.IO;

namespace Hashi.Gui.Wrappers
{
    /// <inheritdoc cref="IDirectoryWrapper" />
    public class DirectoryWrapper : IDirectoryWrapper
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
