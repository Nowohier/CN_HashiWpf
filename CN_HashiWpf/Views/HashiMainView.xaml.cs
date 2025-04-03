using CNHashiWpf.Interfaces;
using CNHashiWpf.ViewModels;
using System.Windows;

namespace CNHashiWpf
{
    public partial class HashiMainView : IViewBoxControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashiMainView"/> class.
        /// </summary>
        public HashiMainView()
        {
            var main = new MainViewModel();
            main.CreateNewGame();
            DataContext = main;
            InitializeComponent();
            ViewBoxControl = HashiViewBox;
        }

        /// <summary>
        /// Gets the view box control.
        /// </summary>
        public FrameworkElement ViewBoxControl { get; }
    }
}