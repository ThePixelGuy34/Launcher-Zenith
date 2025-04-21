using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UML.Class;
using UML.Services;

namespace UML.Pages
{
    public partial class CommunityTab : Page
    {
        private const string VolumeSection = "Settings";
        private const string VolumeKey = "Volume";

        public CommunityTab()
        {
            InitializeComponent();
            Class.DiscordRPCManager.SetPresence("Online", "Checking out the Career page!");
            UserBtn.Content = StringSharing.Username;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (BackgroundMusic != null)
            {
                LoadVolumeSetting();
                BackgroundMusic.Play();
            }
        }

        private void UserStats_Click(object sender, RoutedEventArgs e)
        {
            ButtonActivity.SetIsSelected(CommunityBtn, false);
            ButtonActivity.SetIsSelected(PollsBtn, false);
            ButtonActivity.SetIsSelected(UserBtn, true);
            PageFrame.Navigate(new FortnitePage());
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://discord.gg/zj399J5smC";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void PollsPage_Click(object sender, RoutedEventArgs e)
        {
            ButtonActivity.SetIsSelected(CommunityBtn, false);
            ButtonActivity.SetIsSelected(UserBtn, false);
            ButtonActivity.SetIsSelected(PollsBtn, true);
            PageFrame.Navigate(new Pages.RLPage());
        }

        private void Community_Click(object sender, RoutedEventArgs e)
        {
            ButtonActivity.SetIsSelected(CommunityBtn, true);
            ButtonActivity.SetIsSelected(UserBtn, false);
            ButtonActivity.SetIsSelected(PollsBtn, false);
            PageFrame.Navigate(new CommunityTab());
        }

        private void LoadVolumeSetting()
        {
            if (VolumeSlider == null)
            {
                return;
            }

            string volumeValue = UpdateINI.ReadValue(VolumeSection, VolumeKey);
            double volume = 0.5;
            if (!string.IsNullOrEmpty(volumeValue) && double.TryParse(volumeValue, out double parsedVolume))
            {
                volume = Math.Max(0, Math.Min(1, parsedVolume));
            }

            VolumeSlider.Value = volume;

            if (BackgroundMusic != null)
            {
                BackgroundMusic.Volume = volume;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BackgroundMusic == null)
            {
                MessageBox.Show("BackgroundMusic control not found. Please check your XAML file.");
                return;
            }

            double newVolume = e.NewValue;
            BackgroundMusic.Volume = newVolume;
            UpdateINI.WriteToConfig(VolumeSection, VolumeKey, newVolume.ToString("F2"));
        }

        private void BackgroundMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (BackgroundMusic == null)
            {
                return;
            }

            BackgroundMusic.Position = TimeSpan.Zero;
            BackgroundMusic.Play();
        }

        private void Error2_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://discord.com/channels/1271809431432335461/1294392363417862238";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}