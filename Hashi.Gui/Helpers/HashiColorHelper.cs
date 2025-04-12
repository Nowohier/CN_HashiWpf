using System.Windows;
using System.Windows.Media;

namespace Hashi.Gui.Helpers;

public static class HashiColorHelper
{
    public static SolidColorBrush MenuBackgroundBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(MenuBackgroundBrush)]!;

    public static SolidColorBrush BackgroundBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(BackgroundBrush)]!;

    public static SolidColorBrush BasicBrush = (SolidColorBrush)Application.Current.Resources[nameof(BasicBrush)]!;

    public static SolidColorBrush BasicIslandBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(BasicIslandBrush)]!;

    public static SolidColorBrush GreenIslandBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(GreenIslandBrush)]!;

    public static SolidColorBrush IntenseGreenBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(IntenseGreenBrush)]!;

    public static SolidColorBrush MaxBridgesReachedBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(MaxBridgesReachedBrush)]!;

    public static SolidColorBrush PotentialConnectionBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(PotentialConnectionBrush)]!;

    public static SolidColorBrush GridLineBrush =
        (SolidColorBrush)Application.Current.Resources[nameof(GridLineBrush)]!;
}