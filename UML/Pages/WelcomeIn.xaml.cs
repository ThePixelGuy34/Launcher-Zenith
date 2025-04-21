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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace UML.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeIn.xaml
    /// </summary>
    public partial class WelcomeIn : Page
    {
        public WelcomeIn(string Username, string loginMethod)
        {
            InitializeComponent();
            string LoginMethod = loginMethod;
            WelcomeLoaded(LoginMethod);
        }

        private async void WelcomeLoaded(string LoginMethod)
        {
            if (LoginMethod is "email")
            {
                await Task.Delay(700);
                StatusText.Text = "We're signing you into your account...";
                await Task.Delay(2000);
                try
                {
                    UserText.Text = $"Welcome Back, {StringSharing.displayName}!";
                }
                catch (Exception)
                {
                    UserText.Text = $"Welcome Back, ZenithUser!";
                }
                CheckSettings();
                CloseLoading();
                await Task.Delay(1000);
                NavigateToMainNow();
            }
            else
            {
                await Task.Delay(700);
                StatusText.Text = "We're signing you into your account...";
                await Task.Delay(2000);
                try
                {
                    UserText.Text = $"Welcome Back, {StringSharing.Username}!";
                }
                catch (Exception)
                {
                    UserText.Text = $"Welcome Back, ZenithUser!";
                }
                CheckSettings();
                CloseLoading();
                await Task.Delay(1000);
                NavigateToMainNow();
            }
        }

        private void NavigateToMainNow()
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

        private void CloseLoading()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CloseLoadingBar();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }

        private void CheckSettings()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.IsSettingsOpen();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
            //await Task.Delay(1300);
        }
    }
}
