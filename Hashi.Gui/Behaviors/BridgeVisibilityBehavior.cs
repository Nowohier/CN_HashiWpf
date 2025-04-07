using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Shapes;

namespace Hashi.Gui.Behaviors
{
    /// <summary>
    ///      A behavior that controls the visibility of a bridge based on the number of connections in a Hashi game.
    /// </summary>
    public class BridgeVisibilityBehavior : Behavior<Line>
    {
        /// <summary>
        ///       Identifies the <see cref="AllConnections"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllConnectionsProperty =
            DependencyProperty.Register(nameof(AllConnections), typeof(ObservableCollection<IHashiPoint>), typeof(BridgeVisibilityBehavior), new PropertyMetadata(OnAllConnectionsChanged));

        /// <summary>
        ///      Identifies the <see cref="BridgeType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BridgeTypeProperty =
            DependencyProperty.Register(nameof(BridgeType), typeof(BridgeTypeEnum), typeof(BridgeVisibilityBehavior), new PropertyMetadata(BridgeTypeEnum.None, OnPropertiesChanged));

        /// <summary>
        ///     Gets or sets the collection of all connections for the island.
        /// </summary>
        public ObservableCollection<IHashiPoint> AllConnections
        {
            get => (ObservableCollection<IHashiPoint>)GetValue(AllConnectionsProperty);
            set => SetValue(AllConnectionsProperty, value);
        }

        /// <summary>
        ///     Gets or sets the type of the bridge.
        /// </summary>
        public BridgeTypeEnum BridgeType
        {
            get => (BridgeTypeEnum)GetValue(BridgeTypeProperty);
            set => SetValue(BridgeTypeProperty, value);
        }

        /// <summary>
        ///     Called when the <see cref="AllConnections"/> property changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/>.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnAllConnectionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not BridgeVisibilityBehavior behavior) return;
            if (e.OldValue is ObservableCollection<IHashiPoint> oldCollection)
            {
                oldCollection.CollectionChanged -= behavior.OnCollectionChanged!;
            }

            if (e.NewValue is ObservableCollection<IHashiPoint> newCollection)
            {
                newCollection.CollectionChanged += behavior.OnCollectionChanged!;
            }

            behavior.UpdateVisibility();
        }

        /// <summary>
        ///     Called when the collection of connections changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateVisibility();
        }

        /// <summary>
        ///     Called when the <see cref="BridgeType"/> property changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/>.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BridgeVisibilityBehavior behavior)
            {
                behavior.UpdateVisibility();
            }
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateVisibility();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AllConnections.CollectionChanged -= OnCollectionChanged!;
        }

        /// <summary>
        ///     Updates the visibility of the bridge based on the number of connections.
        /// </summary>
        private void UpdateVisibility()
        {
            if (AssociatedObject is not { } line || AssociatedObject.DataContext is not IIslandViewModel island) return;

            line.Visibility = BridgeType switch
            {
                BridgeTypeEnum.Horizontal => island.BridgesLeft.Count == 1 && island.BridgesRight.Count == 1 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.HorizontalDouble => island.BridgesLeft.Count == 2 && island.BridgesRight.Count == 2 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.Vertical => island.BridgesUp.Count == 1 && island.BridgesDown.Count == 1 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.VerticalDouble => island.BridgesUp.Count == 2 && island.BridgesDown.Count == 2 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.HorizontalLeft => island.BridgesLeft.Count == 1 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.HorizontalDoubleLeft => island.BridgesLeft.Count == 2 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.HorizontalRight => island.BridgesRight.Count == 1 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.HorizontalDoubleRight => island.BridgesRight.Count == 2 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.VerticalUp => island.BridgesUp.Count == 1 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.VerticalDoubleUp => island.BridgesUp.Count == 2 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.VerticalDown => island.BridgesDown.Count == 1 ? Visibility.Visible : Visibility.Hidden,
                BridgeTypeEnum.VerticalDoubleDown => island.BridgesDown.Count == 2 ? Visibility.Visible : Visibility.Hidden,
                _ => Visibility.Hidden
            };
        }
    }
}
