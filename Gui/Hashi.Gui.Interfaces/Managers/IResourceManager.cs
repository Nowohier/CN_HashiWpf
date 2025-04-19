namespace Hashi.Gui.Interfaces.Managers
{
    /// <summary>
    ///   Interface for the setup manager.
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        ///   Prepares the UI for first use by creating necessary directories and files.
        /// </summary>
        void PrepareUi();

        /// <summary>
        ///  Resets the settings and loads them from the default settings files.
        /// </summary>
        void ResetSettingsAndLoadFromDefault();
    }
}
