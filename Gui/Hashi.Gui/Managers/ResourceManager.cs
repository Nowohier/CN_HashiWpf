using Hashi.Gui.Interfaces.Managers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Logging.Interfaces;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hashi.Gui.Managers
{
    /// <inheritdoc cref="IResourceManager"/>
    public class ResourceManager(IPathProvider pathProvider, ILogger logger) : IResourceManager
    {
        private readonly string[] embeddedResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        internal const string ErrorMessage = "Failed to create directory '{0}': {1}";

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
            if (!Directory.Exists(directoryPath)) return;
            Directory.Delete(directoryPath, true);
        }

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (Directory.Exists(directoryPath)) return;
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (ArgumentException ex)
            {
                var message = string.Format(ErrorMessage, directoryPath, ex.Message);
                logger.Error(message);
                throw new ArgumentException(message, ex);
            }
        }

        private void EnsureFileExists(string filePath, string embeddedFileName)
        {
            if (File.Exists(filePath) || !TryGetEmbeddedResourceName(embeddedFileName, out var resourceName)) return;

            var content = GetEmbeddedResource(resourceName);
            File.WriteAllText(filePath, content);
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
