using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hashi.Gui.ViewModels
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The <see cref="INotifyPropertyChanged.PropertyChanged" /> event is raised when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property value and raises the <see cref="PropertyChanged" /> event if the value has changed.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="storage">The storage field.</param>
        /// <param name="value">The changed value.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>a boolean value if property was changed and notified.</returns>
        public virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}
