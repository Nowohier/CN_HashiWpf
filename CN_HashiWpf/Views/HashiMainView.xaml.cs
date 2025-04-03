using CNHashiWpf.ViewModels;

namespace CNHashiWpf
{
    public partial class HashiMainView
    {
        public HashiMainView()
        {
            var main = new MainViewModel();
            main.CreateNewGame();
            DataContext = main;
            InitializeComponent();
        }
    }
}