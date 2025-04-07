using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hashi.Gui.EventArgs;
using Microsoft.Xaml.Behaviors;

namespace Hashi.Gui.Behaviors;

/// <summary>
///     Represents the behavior for the island drag and drop.
/// </summary>
public class IslandDragDropBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty DragEnterCommandProperty =
        DependencyProperty.Register(nameof(DragEnterCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

    public static readonly DependencyProperty DragOverCommandProperty =
        DependencyProperty.Register(nameof(DragOverCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

    public static readonly DependencyProperty DragLeaveCommandProperty =
        DependencyProperty.Register(nameof(DragLeaveCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

    public static readonly DependencyProperty DropCommandProperty =
        DependencyProperty.Register(nameof(DropCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

    public static readonly DependencyProperty MouseMoveCommandProperty =
        DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

    public static readonly DependencyProperty MouseLeftButtonDownCommandProperty =
        DependencyProperty.Register(nameof(MouseLeftButtonDownCommand), typeof(ICommand),
            typeof(IslandDragDropBehavior));

    public static readonly DependencyProperty MouseLeftButtonUpCommandProperty =
        DependencyProperty.Register(nameof(MouseLeftButtonUpCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

    public static readonly DependencyProperty ConnectedViewBoxProperty =
        DependencyProperty.Register(nameof(ConnectedViewBox), typeof(Viewbox), typeof(IslandDragDropBehavior));

    /// <summary>
    ///     Gets or sets the drag enter command.
    /// </summary>
    public Viewbox ConnectedViewBox
    {
        get => (Viewbox)GetValue(ConnectedViewBoxProperty);
        set => SetValue(ConnectedViewBoxProperty, value);
    }

    /// <summary>
    ///     Gets or sets the drag enter command.
    /// </summary>
    public ICommand DragEnterCommand
    {
        get => (ICommand)GetValue(DragEnterCommandProperty);
        set => SetValue(DragEnterCommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the drag over command.
    /// </summary>
    public ICommand DragOverCommand
    {
        get => (ICommand)GetValue(DragOverCommandProperty);
        set => SetValue(DragOverCommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the drag leave command.
    /// </summary>
    public ICommand DragLeaveCommand
    {
        get => (ICommand)GetValue(DragLeaveCommandProperty);
        set => SetValue(DragLeaveCommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the drop command.
    /// </summary>
    public ICommand DropCommand
    {
        get => (ICommand)GetValue(DropCommandProperty);
        set => SetValue(DropCommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the mouse move command.
    /// </summary>
    public ICommand MouseMoveCommand
    {
        get => (ICommand)GetValue(MouseMoveCommandProperty);
        set => SetValue(MouseMoveCommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the mouse left button down command.
    /// </summary>
    public ICommand MouseLeftButtonDownCommand
    {
        get => (ICommand)GetValue(MouseLeftButtonDownCommandProperty);
        set => SetValue(MouseLeftButtonDownCommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the mouse left button up command.
    /// </summary>
    public ICommand MouseLeftButtonUpCommand
    {
        get => (ICommand)GetValue(MouseLeftButtonUpCommandProperty);
        set => SetValue(MouseLeftButtonUpCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.AllowDrop = true;
        AssociatedObject.DragEnter += OnDragEnter;
        AssociatedObject.DragOver += OnDragOver;
        AssociatedObject.Drop += OnDrop;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        AssociatedObject.DragLeave += OnDragLeave;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.DragEnter -= OnDragEnter;
        AssociatedObject.DragOver -= OnDragOver;
        AssociatedObject.Drop -= OnDrop;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        AssociatedObject.DragLeave -= OnDragLeave;
    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
        DragEnterCommand.Execute(e);
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        DragOverCommand.Execute(e);
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        DragLeaveCommand.Execute(e);
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        DropCommand.Execute(e);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var dragStartPosition = e.GetPosition(ConnectedViewBox);
        MouseMoveCommand.Execute(new MouseEventArgsWithCorrectViewBoxPosition(e, dragStartPosition));
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        MouseLeftButtonDownCommand.Execute(e);
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        MouseLeftButtonUpCommand.Execute(e);
    }
}