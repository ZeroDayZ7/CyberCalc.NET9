using System.Windows;
using System.Windows.Input;

namespace WPF_CALC_NET_9.Helpers
{
    public static class DragBehavior
    {
        public static bool GetIsDraggable(Window window) => (bool)window.GetValue(IsDraggableProperty);
        public static void SetIsDraggable(Window window, bool value) => window.SetValue(IsDraggableProperty, value);

        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.RegisterAttached(
                "IsDraggable",
                typeof(bool),
                typeof(DragBehavior),
                new PropertyMetadata(false, OnIsDraggableChanged));

        private static void OnIsDraggableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                if ((bool)e.NewValue)
                {
                    window.MouseLeftButtonDown += (s, ev) =>
                    {
                        if (ev.ChangedButton == MouseButton.Left)
                        {
                            try { window.DragMove(); }
                            catch { }
                        }
                    };
                }
            }
            else if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.MouseLeftButtonDown += (s, ev) =>
                    {
                        if (ev.ChangedButton == MouseButton.Left)
                        {
                            Window.GetWindow(element)?.DragMove();
                        }
                    };
                }
            }
        }
    }
}
