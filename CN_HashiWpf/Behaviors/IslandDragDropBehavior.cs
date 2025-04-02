using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace CNHashiWpf.Behaviors
{
    /// <summary>
    /// Represents the behavior for the island drag and drop.
    /// </summary>
    public class IslandDragDropBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty DragEnterCommandProperty =
            DependencyProperty.Register(nameof(DragEnterCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

        public static readonly DependencyProperty DragOverCommandProperty =
            DependencyProperty.Register(nameof(DragOverCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.Register(nameof(DropCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand), typeof(IslandDragDropBehavior));

        /// <summary>
        /// Gets or sets the drag enter command.
        /// </summary>
        public ICommand DragEnterCommand
        {
            get => (ICommand)GetValue(DragEnterCommandProperty);
            set => SetValue(DragEnterCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the drag over command.
        /// </summary>
        public ICommand DragOverCommand
        {
            get => (ICommand)GetValue(DragOverCommandProperty);
            set => SetValue(DragOverCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the drop command.
        /// </summary>
        public ICommand DropCommand
        {
            get => (ICommand)GetValue(DropCommandProperty);
            set => SetValue(DropCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the mouse move command.
        /// </summary>
        public ICommand MouseMoveCommand
        {
            get => (ICommand)GetValue(MouseMoveCommandProperty);
            set => SetValue(MouseMoveCommandProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragEnter += OnDragEnter;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.DragEnter -= OnDragEnter;
            AssociatedObject.DragOver -= OnDragOver;
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            DragEnterCommand.Execute(e);
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            DragOverCommand.Execute(e);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            DropCommand.Execute(e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveCommand.Execute(e);
        }
    }
}
