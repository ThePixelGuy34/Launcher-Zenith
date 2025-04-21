using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for LoadingBar.xaml
    /// </summary>
    public partial class LoadingBar : Page
    {
        public LoadingBar()
        {
            InitializeComponent();
            this.Loaded += LoadingAnimationPage_Loaded;
        }

        private void LoadingAnimationPage_Loaded(object sender, RoutedEventArgs e)
        {


            var animation = new DoubleAnimation
            {
                From = 0,
                To = 2000,
                Duration = TimeSpan.FromSeconds(60)
            };

            Storyboard storyboard = new Storyboard();
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTarget(animation, LoadingBarImage);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
    }
}
