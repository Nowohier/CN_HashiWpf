using Hashi.Gui.Interfaces.Wrappers;
using System.IO;

namespace Hashi.Gui.Wrappers
{
    /// <inheritdoc cref="IFileWrapper" />
    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}
