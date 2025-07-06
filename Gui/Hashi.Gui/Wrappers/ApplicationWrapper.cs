using Hashi.Gui.Interfaces.Wrappers;
using System.Windows;

namespace Hashi.Gui.Wrappers
{
    /// <inheritdoc cref="IApplicationWrapper"/>
    public class ApplicationWrapper : IApplicationWrapper
    {
        public object? GetCurrentApplication()
        {
            return Application.Current;
        }

        public object? GetApplicationResource(string resourceName)
        {
            return Application.Current?.Resources[resourceName];
        }
    }
}
