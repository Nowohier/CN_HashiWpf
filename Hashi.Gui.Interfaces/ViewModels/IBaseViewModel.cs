using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hashi.Gui.Interfaces.ViewModels
{
    public interface IBaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null);

        /// <summary>
        /// Sets the value of a property and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="storage">The variable to store the new value in.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The property name. If left empty, all properties will be notified.</param>
        /// <returns>a boolean value if the property has been updated or not.</returns>
        public bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = "");
    }
}
