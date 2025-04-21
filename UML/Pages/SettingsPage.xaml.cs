using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Globalization;
using System.Threading;
using UML.Services;
using UML.Class;
using System.Diagnostics;
using System.IO;

namespace UML.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadLanguageSetting();
            DiscordRPCManager.SetPresence("Online", "In the settings tab.");
            SetVersionID();
        }

        private void SupportBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/gwxjYAPtxB") { UseShellExecute = true });
        }

        private void ShowLogs_Click(object sender, RoutedEventArgs e)
        {
            string logsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Zenith", "Logs");
            Directory.CreateDirectory(logsPath);
            Process.Start("explorer.exe", logsPath);
        }

        private void SetVersionID()
        {
            VersionID.Text = AppVersion.FullVersion;
        }

        private void SettingsShut_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CloseSettings();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
            //await Task.Delay(1300);
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focusable = true;
            this.Focus();
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DropdownButton_Click(object sender, RoutedEventArgs e)
        {
            //DropdownMenu.IsOpen = !DropdownMenu.IsOpen;
        }

        private void English_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("en-US");
        }

        private void Spanish_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("es-ES");
        }

        private void SetLanguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            // Save the selected language to the config
            UpdateINI.WriteToConfig("Settings", "Language", lang);

            // Load the resource dictionary for the selected language
            LoadResourceDictionary(lang);
        }

        private void LoadResourceDictionary(string lang)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            ResourceDictionary resDict = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Lang/{lang}.xaml", UriKind.Absolute)
            };
            Application.Current.Resources.MergedDictionaries.Add(resDict);
        }

        private void LoadLanguageSetting()
        {
            // Attempt to read the language from the config
            string lang = UpdateINI.ReadValue("Settings", "Language");

            // If no language is found in the config, default to English
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en-US"; // Default to English if no language is set
                UpdateINI.WriteToConfig("Settings", "Language", lang); // Save default language to config
            }

            // Set the current culture and load the resource dictionary
            SetLanguage(lang);
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}
