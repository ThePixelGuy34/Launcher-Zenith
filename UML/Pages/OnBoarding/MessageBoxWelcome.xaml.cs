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
using UML.Services;

namespace UML.Pages.OnBoarding
{
    /// <summary>
    /// Interaction logic for MessageBoxWelcome.xaml
    /// </summary>
    public partial class MessageBoxWelcome : Page
    {
        public MessageBoxWelcome()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenWelcomeIn();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }

        private void PlayClick()
        {
            try
            {
                MediaPlayer SoundPlayer = new MediaPlayer();
                SoundPlayer.Volume = 1;
                string soundPath = "pack://application:,,,/src/sounds/buttonClick.mp3";
                SoundPlayer.Open(new Uri(soundPath));
                SoundPlayer.Play();
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to play sound: {ex.Message}");
            }
        }
    }
}
