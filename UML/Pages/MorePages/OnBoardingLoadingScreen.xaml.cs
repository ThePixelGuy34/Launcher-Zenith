using System;
using System.Threading.Tasks; // Import for Task.Delay
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace UML.Pages
{
    public partial class OnBoardingLoadingScreen : Page
    {
        public OnBoardingLoadingScreen()
        {
            InitializeComponent();
            Class.DiscordRPCManager.SetPresence("Online", "Loading into the launcher!");
        }

        private void OnBoardingLoadingClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void OnBoardingLoadingScreen_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(1000)
            };

            var fadeOutAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500),
                BeginTime = TimeSpan.FromSeconds(3)
            };

            BlackOverlay.BeginAnimation(Rectangle.OpacityProperty, fadeInAnimation);
            BlackOverlay.BeginAnimation(Rectangle.OpacityProperty, fadeOutAnimation);
            await Task.Delay(5000);
            OpenMainFrame();
        }

        private void OpenMainFrame()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenMainPage();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }
    }
}
