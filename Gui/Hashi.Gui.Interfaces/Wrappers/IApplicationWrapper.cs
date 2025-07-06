namespace Hashi.Gui.Interfaces.Wrappers
{
    /// <summary>
    /// Provides methods for interacting with the current application and its resources.
    /// </summary>
    /// <remarks>This interface is designed to abstract application-level operations, such as retrieving the
    /// current application instance or accessing application-specific resources. Implementations may vary depending on
    /// the application framework or environment.</remarks>
    public interface IApplicationWrapper
    {
        /// <summary>
        /// Retrieves the current application instance associated with the runtime environment.
        /// </summary>
        /// <remarks>The returned object may vary depending on the runtime context. Callers should verify
        /// the type and state of the object before performing operations on it.</remarks>
        /// <returns>An object representing the current application instance, or <see langword="null"/> if no application is
        /// available.</returns>
        object? GetCurrentApplication();

        /// <summary>
        /// Retrieves an application resource by its name.
        /// </summary>
        /// <remarks>This method searches for a resource within the application's resource collection.  If
        /// the resource does not exist, the method returns <see langword="null"/>.</remarks>
        /// <param name="resourceName">The name of the resource to retrieve. Cannot be null or empty.</param>
        /// <returns>The resource object if found; otherwise, <see langword="null"/>.</returns>
        object? GetApplicationResource(string resourceName);
    }
}
