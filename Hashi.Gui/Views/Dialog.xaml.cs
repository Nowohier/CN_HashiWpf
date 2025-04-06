using Hashi.Gui.Enums;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces;
using Hashi.Gui.ViewModels;

namespace Hashi.Gui.Views
{
    public partial class Dialog : ICloseable, IDialogResult
    {

        public static DialogResult Show(string caption, string message, DialogButton button = DialogButton.Ok, DialogImage image = DialogImage.None)
        {
            var res = DialogResult.Cancel;
            //Always run dialog in an sta thread
            GeneralHelper.StartStaTask(() =>
            {
                var window = new Dialog(caption, message, button, image);
                window.ShowDialog();
                res = window.DialogResult;
            }).GetAwaiter().GetResult();

            return res;
        }

        public new DialogResult DialogResult { get; set; }

        private Dialog(string caption, string message, DialogButton button = DialogButton.Ok, DialogImage image = DialogImage.None)
        {
            DataContext = new DialogViewModel(caption, message, button, image);
            InitializeComponent();
        }
    }
}
