using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace CNHashiWpf.ViewModels
{
    public class IslandViewModel : BaseViewModel
    {
        public IslandViewModel(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;

            DragEnterCommand = new RelayCommand<object>(DragEnterCommandExecute, DragEnterCommandCanExecute);
            DropCommand = new RelayCommand<object>(DropCommandExecute, DropCommandCanExecute);
        }

        public ICommand DragEnterCommand { get; }

        public ICommand DropCommand { get; }

        public int Value { get; set; }

        public int Y { get; set; }

        public int X { get; set; }

        public void DragEnterCommandExecute(object? eventArgs)
        {

        }

        public bool DragEnterCommandCanExecute(object? eventArgs)
        {
            return true;
        }

        public void DropCommandExecute(object? eventArgs)
        {

        }

        public bool DropCommandCanExecute(object? eventArgs)
        {
            return true;
        }
    }
}
