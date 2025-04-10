using Hashi.Gui.Translation;
using System.Windows.Data;

namespace Hashi.Gui.Localization
{
    /// <summary>
    ///   A binding class for localization.
    /// </summary>
    public class Loc : Binding
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Loc" /> class.
        /// </summary>
        /// <param name="name">The name of the binding property.</param>
        public Loc(string name) : base($"[{name}]")
        {
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}
