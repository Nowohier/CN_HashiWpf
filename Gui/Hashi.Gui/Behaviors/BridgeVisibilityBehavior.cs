using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Messages;
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
public class BridgeVisibilityBehavior : Behavior<Line>, IRecipient<IRuleMessageClearedMessage>
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

    /// <summary>
    ///     Identifies the <see cref="AllConnections" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty BrushResolverProperty =
        DependencyProperty.Register(nameof(BrushResolver), typeof(IHashiBrushResolver),
            typeof(BridgeVisibilityBehavior));

    private readonly DoubleAnimation fadeInAnimation = new()
    {
        From = 0.0,
        To = 1.0,
        Duration = TimeSpan.FromSeconds(0.5),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
    };

    private ColorAnimation? fadeOutAnimation;

    /// <summary>
    ///     Gets or sets the collection of all connections for the island.
    /// </summary>
    public ObservableCollection<IHashiPoint> AllConnections
    {
        get => (ObservableCollection<IHashiPoint>)GetValue(AllConnectionsProperty);
        set => SetValue(AllConnectionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the helper used for color-related operations.
    /// </summary>
    public IHashiBrushResolver BrushResolver
    {
        get => (IHashiBrushResolver)GetValue(BrushResolverProperty);
        set => SetValue(BrushResolverProperty, value);
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
    ///     Receives the <see cref="RuleMessageClearedMessage" />.
    /// </summary>
    /// <param name="messageClearedMessage">The message.</param>
    public void Receive(IRuleMessageClearedMessage messageClearedMessage)
    {
        if (AssociatedObject is not { Stroke: SolidColorBrush solidColorBrush } line)
        {
            return;
        }

        if (AssociatedObject.DataContext is not IIslandViewModel island)
        {
            throw new InvalidOperationException(
                $"{nameof(BridgeVisibilityBehavior)}: DataContext must be of type IIslandViewModel.");
        }

        if (line.Effect == null)
        {
            return;
        }

        foreach (var connection in island.AllConnections) connection.PointType = HashiPointTypeEnum.Normal;

        solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, fadeOutAnimation);
        line.Effect = null;
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        fadeOutAnimation = new()
        {
            From = ((SolidColorBrush)BrushResolver.ResolveBrush(HashiColor.IntenseGreenBrush).Brush).Color,
            To = Colors.Black,
            Duration = TimeSpan.FromSeconds(0.5),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };

        WeakReferenceMessenger.Default.Register(this);
        AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        UpdateVisibility();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        WeakReferenceMessenger.Default.Unregister<IRuleMessageClearedMessage>(this);
        AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
        AllConnections.CollectionChanged -= OnCollectionChanged!;
    }

    /// <summary>
    ///     Updates the visibility of the bridge based on the number of connections.
    /// </summary>
    private void UpdateVisibility()
    {
        if (AssociatedObject is not { } line || AssociatedObject.DataContext is not IIslandViewModel island)
        {
            return;
        }

        var descriptor = GetDescriptor(BridgeType);
        if (descriptor == null)
        {
            line.Visibility = Visibility.Hidden;
            return;
        }

        var primary = descriptor.GetPrimary(island);
        var secondary = descriptor.GetSecondary?.Invoke(island);
        line.Visibility = DetermineVisibility(descriptor.ExpectedCount, primary, secondary);
    }

    private Visibility DetermineVisibility(int expected, List<IHashiPoint> values1, List<IHashiPoint>? values2 = null)
    {
        // Take test connections into account when determining visibility
        // Test connections are not visible

        // Handle the case with two value lists
        if (values2 != null)
        {
            if ((values1.Count == 1 && values2.Count == 1) || AnyPointIsTest(values1) || AnyPointIsTest(values2))
            {
                if (AllPointsAreTest(values1) || AllPointsAreTest(values2))
                {
                    return Visibility.Hidden;
                }
            }

            return values1.Count == expected && values2.Count == expected ? Visibility.Visible : Visibility.Hidden;
        }

        // Handle the case with a single value list
        if (values1.Count == 1 || AnyPointIsTest(values1))
        {
            if (AllPointsAreTest(values1))
            {
                return Visibility.Hidden;
            }
        }

        return values1.Count == expected ? Visibility.Visible : Visibility.Hidden;


        bool AnyPointIsTest(List<IHashiPoint> values)
        {
            return values.Any(x => x.PointType == HashiPointTypeEnum.Test);
        }

        bool AllPointsAreTest(List<IHashiPoint> values)
        {
            return values.All(x => x.PointType == HashiPointTypeEnum.Test);
        }
    }

    private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (AssociatedObject is not { Visibility: Visibility.Visible } line)
        {
            return;
        }

        if (AssociatedObject.DataContext is not IIslandViewModel island)
        {
            throw new InvalidOperationException(
                $"{nameof(BridgeVisibilityBehavior)}: DataContext must be of type IIslandViewModel.");
        }

        var descriptor = GetDescriptor(BridgeType);
        var runFadeInAnimation = descriptor != null &&
                                 HasHintConnections(descriptor, island);

        if (!runFadeInAnimation)
        {
            return;
        }

        line.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom(((SolidColorBrush)BrushResolver.ResolveBrush(HashiColor.IntenseGreenBrush).Brush).Color.ToString())!;
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
        if (d is not BridgeVisibilityBehavior behavior)
        {
            return;
        }

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
        {
            behavior.UpdateVisibility();
        }
        else
        {
            throw new InvalidOperationException("DependencyObject is not a BridgeVisibilityBehavior.");
        }
    }

    private static BridgeTypeDescriptor? GetDescriptor(BridgeTypeEnum bridgeType)
    {
        return bridgeType switch
        {
            BridgeTypeEnum.Horizontal => new(1, i => i.BridgesLeft, i => i.BridgesRight),
            BridgeTypeEnum.HorizontalDouble => new(2, i => i.BridgesLeft, i => i.BridgesRight),
            BridgeTypeEnum.Vertical => new(1, i => i.BridgesUp, i => i.BridgesDown),
            BridgeTypeEnum.VerticalDouble => new(2, i => i.BridgesUp, i => i.BridgesDown),
            BridgeTypeEnum.HorizontalLeft => new(1, i => i.BridgesLeft, null),
            BridgeTypeEnum.HorizontalDoubleLeft => new(2, i => i.BridgesLeft, null),
            BridgeTypeEnum.HorizontalRight => new(1, i => i.BridgesRight, null),
            BridgeTypeEnum.HorizontalDoubleRight => new(2, i => i.BridgesRight, null),
            BridgeTypeEnum.VerticalUp => new(1, i => i.BridgesUp, null),
            BridgeTypeEnum.VerticalDoubleUp => new(2, i => i.BridgesUp, null),
            BridgeTypeEnum.VerticalDown => new(1, i => i.BridgesDown, null),
            BridgeTypeEnum.VerticalDoubleDown => new(2, i => i.BridgesDown, null),
            _ => null
        };
    }

    private static bool HasHintConnections(BridgeTypeDescriptor descriptor, IIslandViewModel island)
    {
        var primary = descriptor.GetPrimary(island);
        var secondary = descriptor.GetSecondary?.Invoke(island);

        if (secondary != null)
        {
            return primary.Count == descriptor.ExpectedCount && secondary.Count == descriptor.ExpectedCount &&
                   (primary.Any(x => x.PointType == HashiPointTypeEnum.Hint) ||
                    secondary.Any(x => x.PointType == HashiPointTypeEnum.Hint));
        }

        return primary.Count == descriptor.ExpectedCount &&
               primary.Any(x => x.PointType == HashiPointTypeEnum.Hint);
    }

    private sealed record BridgeTypeDescriptor(
        int ExpectedCount,
        Func<IIslandViewModel, List<IHashiPoint>> GetPrimary,
        Func<IIslandViewModel, List<IHashiPoint>>? GetSecondary);
}