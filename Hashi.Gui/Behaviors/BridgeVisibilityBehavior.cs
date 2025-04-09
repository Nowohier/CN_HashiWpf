using CommunityToolkit.Mvvm.Messaging;
using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Messages;
using Microsoft.Xaml.Behaviors;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Hashi.Gui.Behaviors
{
    /// <summary>
    ///      A behavior that controls the visibility of a bridge based on the number of connections in a Hashi game.
    /// </summary>
    public class BridgeVisibilityBehavior : Behavior<Line>, IRecipient<HintPopupClosedMessage>
    {
        private readonly DoubleAnimation fadeInAnimation = new()
        {
            From = 0.0,
            To = 1.0,
            Duration = TimeSpan.FromSeconds(0.5),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };

        private readonly ColorAnimation fadeOutAnimation = new()
        {
            From = (Color)ColorConverter.ConvertFromString("#3abf5b"),
            To = Colors.Black,
            Duration = TimeSpan.FromSeconds(0.5),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };

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

            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
            UpdateVisibility();

            WeakReferenceMessenger.Default.Register(this);
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
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

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject is not { } line || AssociatedObject.DataContext is not IIslandViewModel island) return;

            var runFadeInAnimation = BridgeType switch
            {
                BridgeTypeEnum.Horizontal => island.BridgesLeft.Count == 1 && island.BridgesRight.Count == 1 && (island.BridgesLeft.First().IsHint || island.BridgesRight.First().IsHint),
                BridgeTypeEnum.HorizontalDouble => island.BridgesLeft.Count == 2 && island.BridgesRight.Count == 2,
                BridgeTypeEnum.Vertical => island.BridgesUp.Count == 1 && island.BridgesDown.Count == 1 && (island.BridgesUp.First().IsHint || island.BridgesDown.First().IsHint),
                BridgeTypeEnum.VerticalDouble => island.BridgesUp.Count == 2 && island.BridgesDown.Count == 2,
                BridgeTypeEnum.HorizontalLeft => island.BridgesLeft.Count == 1 && island.BridgesLeft.First().IsHint,
                BridgeTypeEnum.HorizontalDoubleLeft => island.BridgesLeft.Count == 2,
                BridgeTypeEnum.HorizontalRight => island.BridgesRight.Count == 1 && island.BridgesRight.First().IsHint,
                BridgeTypeEnum.HorizontalDoubleRight => island.BridgesRight.Count == 2,
                BridgeTypeEnum.VerticalUp => island.BridgesUp.Count == 1 && island.BridgesUp.First().IsHint,
                BridgeTypeEnum.VerticalDoubleUp => island.BridgesUp.Count == 2,
                BridgeTypeEnum.VerticalDown => island.BridgesDown.Count == 1 && island.BridgesDown.First().IsHint,
                BridgeTypeEnum.VerticalDoubleDown => island.BridgesDown.Count == 2,
                _ => false
            };

            if (line.Visibility == Visibility.Visible && runFadeInAnimation)
            {
                line.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#3abf5b")!;
                line.Effect = new BlurEffect { Radius = 10 };
                line.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }
        }

        public void Receive(HintPopupClosedMessage message)
        {
            if (AssociatedObject is not { Stroke: SolidColorBrush solidColorBrush } line || line.Effect == null) return;
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, fadeOutAnimation);
            line.Effect = null;
            line.StrokeThickness = 1;
        }
    }
}
