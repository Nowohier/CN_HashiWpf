using System.Windows;
using Hashi.Gui.Interfaces;

namespace Hashi.Gui.Views
{
    public partial class HashiMainView : IViewBoxControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashiMainView"/> class.
        /// </summary>
        public HashiMainView()
        {
            InitializeComponent();
            ViewBoxControl = HashiViewBox;
        }

        /// <summary>
        /// Gets the view box control.
        /// </summary>
        public FrameworkElement ViewBoxControl { get; }

        /// <summary>
        /// Handles the Loaded event of the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var scaledWidth = (int)SystemParameters.PrimaryScreenWidth * 0.9;
            var scaledHeight = (int)SystemParameters.PrimaryScreenHeight * 0.9;

            MaxWidth = scaledWidth;
            MaxHeight = scaledHeight;
        }
    }
}