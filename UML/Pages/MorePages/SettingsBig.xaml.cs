using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using UML.Class;
using UML.Services;

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for SettingsBig.xaml
    /// </summary>
    public partial class SettingsBig : Page
    {
        public SettingsBig()
        {
            InitializeComponent();
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

        private void SettingsBigShut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.CloseSettingsBig();
                }
                else
                {
                    throw new InvalidOperationException("Navigation error.");
                }
            }
            catch (Exception ex)
            {
                Services.Logger.Log($"Navigation error. {ex.Message}");
            }
        }
    }
}
