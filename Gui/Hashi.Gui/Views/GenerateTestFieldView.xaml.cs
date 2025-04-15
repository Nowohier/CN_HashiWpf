using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;

namespace Hashi.Gui.Views
{
    public partial class GenerateTestFieldView : IGenerateTestFieldView
    {
        public GenerateTestFieldView(IMainViewModel mainViewModel)
        {
            DataContext = mainViewModel;
            InitializeComponent();
            ViewBoxControl = HashiViewBox;
        }

        /// <summary>
        ///     Gets the view box control.
        /// </summary>
        public object ViewBoxControl { get; }
    }
}
