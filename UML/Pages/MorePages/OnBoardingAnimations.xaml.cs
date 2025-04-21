using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UML.Services;
using System.Windows.Shapes;

namespace UML.Pages
{
    public partial class OnBoardingAnimations : Page
    {
        public OnBoardingAnimations()
        {
            InitializeComponent();
            Loaded += OnBoardingAnimations_Loaded;
        }

        private async void OnBoardingAnimations_Loaded(object sender, RoutedEventArgs e)
        {
            await StartAnimations();
        }

        private void CloseConnection_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async Task StartAnimations()
        {
            Logger.Log("StartAnimations called.");

            WelcomeText.Opacity = 0;
            WelcomeText.Visibility = Visibility.Visible;

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(1000)
            };
            WelcomeText.BeginAnimation(TextBlock.OpacityProperty, fadeInAnimation);

            await Task.Delay(3000);

            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(1500)
            };
            WelcomeText.BeginAnimation(TextBlock.OpacityProperty, fadeOutAnimation);

            await Task.Delay(1500);

            AdditionalTextBlock.Opacity = 0;
            AdditionalTextBlock.Visibility = Visibility.Visible;
            var additionalTextFadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(1000)
            };
            AdditionalTextBlock.BeginAnimation(TextBlock.OpacityProperty, additionalTextFadeInAnimation);

            AdditionalTextBlock.RenderTransform = new TranslateTransform(0, 0);
            var upwardAnimation = new DoubleAnimation
            {
                From = 0,
                To = -50,
                Duration = TimeSpan.FromMilliseconds(1000)
            };
            AdditionalTextBlock.RenderTransform.BeginAnimation(TranslateTransform.YProperty, upwardAnimation);

            await Task.Delay(1000);

            BeginButton.Opacity = 0;
            BeginButton.Visibility = Visibility.Visible;
            var buttonFadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500)
            };
            BeginButton.BeginAnimation(Button.OpacityProperty, buttonFadeInAnimation);
        }

        private async void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            await ZoomAndFadeOut();
        }

        private async Task ZoomAndFadeOut()
        {
            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(1000)
            };

            FadeRectangle.BeginAnimation(Rectangle.OpacityProperty, fadeAnimation);

            await Task.Delay(1000);
            await OnAnimationCompleted();
        }

        private async Task OnAnimationCompleted()
        {
            await Task.Delay(3000);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.AnimLoad();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }
    }
}
