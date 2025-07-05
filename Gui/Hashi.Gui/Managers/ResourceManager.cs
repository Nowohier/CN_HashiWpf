using Hashi.Gui.Interfaces.Managers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Wrappers;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hashi.Gui.Managers
{
    /// <inheritdoc cref="IResourceManager"/>
    public class ResourceManager(
        IPathProvider pathProvider,
        IFileWrapper fileWrapper,
        IDirectoryWrapper directoryWrapper) : IResourceManager
    {
        private readonly string[] embeddedResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

        /// <inheritdoc />
        public void PrepareUi()
        {
            EnsureDirectoryExists(pathProvider.SettingsDirectoryPath);
            EnsureFileExists(pathProvider.HashiSettingsFilePath, pathProvider.HashiSettingsFileName);
            EnsureFileExists(pathProvider.HashiTestFieldsFilePath, pathProvider.HashiTestFieldsFileName);
        }

        /// <inheritdoc />
        public void ResetSettingsAndLoadFromDefault()
        {
            RemoveSettingsDirectory(pathProvider.SettingsDirectoryPath);
            PrepareUi();
        }

        private void RemoveSettingsDirectory(string directoryPath)
        {
            if (!directoryWrapper.Exists(directoryPath)) return;
            directoryWrapper.Delete(directoryPath, true);
        }

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (directoryWrapper.Exists(directoryPath)) return;
            directoryWrapper.CreateDirectory(directoryPath);
        }

        private void EnsureFileExists(string filePath, string embeddedFileName)
        {
            if (fileWrapper.Exists(filePath) ||
                !TryGetEmbeddedResourceName(embeddedFileName, out var resourceName)) return;

            var content = GetEmbeddedResource(resourceName);
            fileWrapper.WriteAllText(filePath, content);
        }

        private bool TryGetEmbeddedResourceName(string fileName, out string resourceName)
        {
            if (embeddedResourceNames.FirstOrDefault(x => x.Contains(fileName)) is { } value)
            {
                resourceName = value;
                return true;
            }

            resourceName = string.Empty;
            return false;
        }

        private string GetEmbeddedResource(string resourceFileName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFileName)
                               ?? throw new FileNotFoundException($"Embedded resource '{resourceFileName}' not found.");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}