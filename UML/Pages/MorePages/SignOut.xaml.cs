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

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for SignOut.xaml
    /// </summary>
    public partial class SignOut : Page
    {
        public SignOut()
        {
            InitializeComponent();
            FadeIn();
        }

        private async void SignOutPlayer()
        {
            SignOutText.Text = $"We're signing you out of your account...";
            await Task.Delay(2000);

            if(StringSharing.LoginMethod is "email")
            {
                SignOutText.Text = $"Goodbye, {StringSharing.displayName}!";
            }
            else
            {
                SignOutText.Text = $"Goodbye, {StringSharing.Username}!";
            }
        }

        private async void FadeIn()
        {
            SignOutPlayer();

            DoubleAnimation SignOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.6))
            };

            SignOutGrid.BeginAnimation(UIElement.OpacityProperty, SignOutAnimation);

            await Task.Delay(3000);
            HeadBackToLoginScreen();
        }

        private void HeadBackToLoginScreen()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenLoginPage();
                mainWindow.CloseLoadingBar();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }
    }
}
