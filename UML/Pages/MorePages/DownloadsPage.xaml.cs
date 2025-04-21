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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Net.Http;
using System.Windows.Threading;

namespace UML.Pages
{
    public partial class DownloadsPage : Page
    {
        private DispatcherTimer _refreshTimer;
        private string _latestProgress = "Waiting...";

        public DownloadsPage()
        {
            InitializeComponent();
            Class.DiscordRPCManager.SetPresence("Online", "On the Downloads page.");
            //UpdateProgress();
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromMilliseconds(100);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            DownloadProgressText.Text = _latestProgress;
            DownloadProgressText.UpdateLayout();
        }

        public void UpdateProgress(string progress)
        {
            _latestProgress = progress;
            DownloadProgressText.Text = progress;
            DownloadProgressText.UpdateLayout();
        }

        private void DownloadsShut_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CloseDownloads();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
            //await Task.Delay(1300);
        }

        private void DownloadsPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focusable = true;
            this.Focus();
        }

        //private void ExtractRarFile(string rarPath, string extractPath)
        //{
        //    using (var archive = ArchiveFactory.Open(rarPath))
        //    {
        //        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
        //        {
        //            string outputPath = Path.Combine(extractPath, entry.Key);

        //            string directoryPath = Path.GetDirectoryName(outputPath);
        //            if (!Directory.Exists(directoryPath))
        //            {
        //                Directory.CreateDirectory(directoryPath);
        //            }

        //            if (File.Exists(outputPath))
        //            {
        //                File.Delete(outputPath);
        //            }

        //            using (var fileStream = File.Create(outputPath))
        //            {
        //                entry.WriteTo(fileStream);
        //            }
        //        }
        //    }
        //} 
    }
}
