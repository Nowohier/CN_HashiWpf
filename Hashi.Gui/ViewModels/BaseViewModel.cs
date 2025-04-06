using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Hashi.Gui.ViewModels
{
    /// <summary>
    /// Base class for all view models.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            CommandManager.InvalidateRequerySuggested();
            return true;
        }
    }
}
