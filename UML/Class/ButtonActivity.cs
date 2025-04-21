using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UML.Class
{
    public class ButtonActivity
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(ButtonActivity),
                new PropertyMetadata(false, OnIsSelectedChanged));

        public static bool GetIsSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedProperty, value);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                bool isSelected = (bool)e.NewValue;

                if (isSelected)
                {
                    button.Foreground = Brushes.White;
                }
                else
                {
                    button.ClearValue(Control.ForegroundProperty);
                }
            }
        }
    }
}
