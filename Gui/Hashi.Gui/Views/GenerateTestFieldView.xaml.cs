using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Views;

namespace Hashi.Gui.Views
{
    public partial class GenerateTestFieldView : IWindow<IGenerateTestFieldViewModel>
    {
        public GenerateTestFieldView(IGenerateTestFieldViewModel generateTestFieldViewModel)
        {
            DataContext = generateTestFieldViewModel;
            InitializeComponent();
            ViewBoxControl = HashiViewBox;
        }

        /// <summary>
        ///     Gets the view box control.
        /// </summary>
        public object ViewBoxControl { get; }
    }
}
