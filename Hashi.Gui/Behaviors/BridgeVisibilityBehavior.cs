using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Gui.Helpers;
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

namespace Hashi.Gui.Behaviors;

/// <summary>
///     A behavior that controls the visibility of a bridge based on the number of connections in a Hashi game.
/// </summary>
public class BridgeVisibilityBehavior : Behavior<Line>, IRecipient<HintPopupClosedMessage>
{
    /// <summary>
    ///     Identifies the <see cref="AllConnections" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllConnectionsProperty =
        DependencyProperty.Register(nameof(AllConnections), typeof(ObservableCollection<IHashiPoint>),
            typeof(BridgeVisibilityBehavior), new PropertyMetadata(OnAllConnectionsChanged));

    /// <summary>
    ///     Identifies the <see cref="BridgeType" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty BridgeTypeProperty =
        DependencyProperty.Register(nameof(BridgeType), typeof(BridgeTypeEnum), typeof(BridgeVisibilityBehavior),
            new PropertyMetadata(BridgeTypeEnum.None, OnPropertiesChanged));

    private readonly DoubleAnimation fadeInAnimation = new()
    {
        From = 0.0,
        To = 1.0,
        Duration = TimeSpan.FromSeconds(0.5),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
    };

    private readonly ColorAnimation fadeOutAnimation = new()
    {
        From = HashiColorHelper.IntenseGreenBrush.Color,
        To = Colors.Black,
        Duration = TimeSpan.FromSeconds(0.5),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
    };

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
    ///     Receives the <see cref="HintPopupClosedMessage" />.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Receive(HintPopupClosedMessage message)
    {
        if (AssociatedObject is not { Stroke: SolidColorBrush solidColorBrush } line) return;

        if (AssociatedObject.DataContext is not IIslandViewModel island)
            throw new InvalidOperationException(
                $"{nameof(BridgeVisibilityBehavior)}: DataContext must be of type IIslandViewModel.");

        if (line.Effect == null) return;

        foreach (var connection in island.AllConnections) connection.IsHint = false;

        solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, fadeOutAnimation);
        line.Effect = null;
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        WeakReferenceMessenger.Default.Register(this);
        AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        UpdateVisibility();
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
            BridgeTypeEnum.Horizontal => island.BridgesLeft.Count == 1 && island.BridgesRight.Count == 1
                ? Visibility.Visible
                : Visibility.Hidden,
            BridgeTypeEnum.HorizontalDouble => island.BridgesLeft.Count == 2 && island.BridgesRight.Count == 2
                ? Visibility.Visible
                : Visibility.Hidden,
            BridgeTypeEnum.Vertical => island.BridgesUp.Count == 1 && island.BridgesDown.Count == 1
                ? Visibility.Visible
                : Visibility.Hidden,
            BridgeTypeEnum.VerticalDouble => island.BridgesUp.Count == 2 && island.BridgesDown.Count == 2
                ? Visibility.Visible
                : Visibility.Hidden,
            BridgeTypeEnum.HorizontalLeft => island.BridgesLeft.Count == 1 ? Visibility.Visible : Visibility.Hidden,
            BridgeTypeEnum.HorizontalDoubleLeft => island.BridgesLeft.Count == 2
                ? Visibility.Visible
                : Visibility.Hidden,
            BridgeTypeEnum.HorizontalRight => island.BridgesRight.Count == 1 ? Visibility.Visible : Visibility.Hidden,
            BridgeTypeEnum.HorizontalDoubleRight => island.BridgesRight.Count == 2
                ? Visibility.Visible
                : Visibility.Hidden,
            BridgeTypeEnum.VerticalUp => island.BridgesUp.Count == 1 ? Visibility.Visible : Visibility.Hidden,
            BridgeTypeEnum.VerticalDoubleUp => island.BridgesUp.Count == 2 ? Visibility.Visible : Visibility.Hidden,
            BridgeTypeEnum.VerticalDown => island.BridgesDown.Count == 1 ? Visibility.Visible : Visibility.Hidden,
            BridgeTypeEnum.VerticalDoubleDown => island.BridgesDown.Count == 2 ? Visibility.Visible : Visibility.Hidden,
            _ => Visibility.Hidden
        };
    }

    private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (AssociatedObject is not { Visibility: Visibility.Visible } line) return;
        if (AssociatedObject.DataContext is not IIslandViewModel island)
            throw new InvalidOperationException(
                $"{nameof(BridgeVisibilityBehavior)}: DataContext must be of type IIslandViewModel.");

        var runFadeInAnimation = BridgeType switch
        {
            BridgeTypeEnum.Horizontal => island.BridgesLeft.Count == 1 && island.BridgesRight.Count == 1 &&
                                         (island.BridgesLeft.First().IsHint || island.BridgesRight.First().IsHint),
            BridgeTypeEnum.HorizontalDouble => island.BridgesLeft.Count == 2 && island.BridgesRight.Count == 2 &&
                                               (island.BridgesLeft.Any(x => x.IsHint) ||
                                                island.BridgesRight.Any(x => x.IsHint)),
            BridgeTypeEnum.Vertical => island.BridgesUp.Count == 1 && island.BridgesDown.Count == 1 &&
                                       (island.BridgesUp.First().IsHint || island.BridgesDown.First().IsHint),
            BridgeTypeEnum.VerticalDouble => island.BridgesUp.Count == 2 && island.BridgesDown.Count == 2 &&
                                             (island.BridgesUp.Any(x => x.IsHint) ||
                                              island.BridgesDown.Any(x => x.IsHint)),
            BridgeTypeEnum.HorizontalLeft => island.BridgesLeft.Count == 1 && island.BridgesLeft.First().IsHint,
            BridgeTypeEnum.HorizontalDoubleLeft => island.BridgesLeft.Count == 2 &&
                                                   island.BridgesLeft.Any(x => x.IsHint),
            BridgeTypeEnum.HorizontalRight => island.BridgesRight.Count == 1 && island.BridgesRight.First().IsHint,
            BridgeTypeEnum.HorizontalDoubleRight => island.BridgesRight.Count == 2 &&
                                                    island.BridgesRight.Any(x => x.IsHint),
            BridgeTypeEnum.VerticalUp => island.BridgesUp.Count == 1 && island.BridgesUp.First().IsHint,
            BridgeTypeEnum.VerticalDoubleUp => island.BridgesUp.Count == 2 && island.BridgesUp.Any(x => x.IsHint),
            BridgeTypeEnum.VerticalDown => island.BridgesDown.Count == 1 && island.BridgesDown.First().IsHint,
            BridgeTypeEnum.VerticalDoubleDown => island.BridgesDown.Count == 2 && island.BridgesDown.Any(x => x.IsHint),
            _ => false
        };

        if (!runFadeInAnimation) return;

        line.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#3abf5b")!;
        line.Effect = new BlurEffect { Radius = 10 };
        line.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
    }

    /// <summary>
    ///     Called when the <see cref="AllConnections" /> property changes.
    /// </summary>
    /// <param name="d">The <see cref="DependencyObject" />.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" />.</param>
    private static void OnAllConnectionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BridgeVisibilityBehavior behavior) return;
        if (e.OldValue is ObservableCollection<IHashiPoint> oldCollection)
            oldCollection.CollectionChanged -= behavior.OnCollectionChanged!;

        if (e.NewValue is ObservableCollection<IHashiPoint> newCollection)
            newCollection.CollectionChanged += behavior.OnCollectionChanged!;

        behavior.UpdateVisibility();
    }

    /// <summary>
    ///     Called when the collection of connections changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" />.</param>
    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateVisibility();
    }

    /// <summary>
    ///     Called when the <see cref="BridgeType" /> property changes.
    /// </summary>
    /// <param name="d">The <see cref="DependencyObject" />.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" />.</param>
    private static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BridgeVisibilityBehavior behavior)
            behavior.UpdateVisibility();
        else
            throw new InvalidOperationException("DependencyObject is not a BridgeVisibilityBehavior.");
    }
}