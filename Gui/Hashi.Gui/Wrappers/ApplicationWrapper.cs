using Hashi.Gui.Interfaces.Wrappers;
using System.Windows;

namespace Hashi.Gui.Wrappers
{
    /// <inheritdoc cref="IApplicationWrapper"/>
    public class ApplicationWrapper : IApplicationWrapper
    {
        /// <inheritdoc />
        public object? GetCurrentApplication()
        {
            return Application.Current;
        }

        /// <inheritdoc />
        public object? GetApplicationResource(string resourceName)
        {
            return Application.Current?.Resources[resourceName];
        }
    }
}
