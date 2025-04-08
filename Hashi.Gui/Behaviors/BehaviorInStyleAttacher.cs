using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Windows;

namespace Hashi.Gui.Behaviors
{
    /// <summary>
    ///     This class is used to attach behaviors to styles. It is used in the XAML files to attach behaviors to styles.
    /// </summary>
    public static class BehaviorInStyleAttacher
    {
        /// <summary>
        ///       The Behaviors attached property. This property is used to attach behaviors to styles.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached(
                "Behaviors",
                typeof(IEnumerable),
                typeof(BehaviorInStyleAttacher),
                new UIPropertyMetadata(null, OnBehaviorsChanged));

        /// <summary>
        ///       Gets the Behaviors attached property. This property is used to attach behaviors to styles.
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static IEnumerable GetBehaviors(DependencyObject dependencyObject)
        {
            return (IEnumerable)dependencyObject.GetValue(BehaviorsProperty);
        }

        /// <summary>
        ///      Sets the Behaviors attached property. This property is used to attach behaviors to styles.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetBehaviors(
            DependencyObject dependencyObject, IEnumerable value)
        {
            dependencyObject.SetValue(BehaviorsProperty, value);
        }

        /// <summary>
        ///       This method is called when the Behaviors property is changed. It is used to attach behaviors to styles.
        /// </summary>
        /// <param name="depObj">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnBehaviorsChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IEnumerable == false)
                return;

            var newBehaviorCollection = e.NewValue as IEnumerable;

            var behaviorCollection = Interaction.GetBehaviors(depObj);
            behaviorCollection.Clear();
            if (newBehaviorCollection == null) return;

            foreach (Behavior behavior in newBehaviorCollection)
            {
                // you need to make a copy of behavior in order to attach it to several controls
                var copy = behavior.Clone() as Behavior;
                behaviorCollection.Add(copy!);
            }
        }
    }
}
