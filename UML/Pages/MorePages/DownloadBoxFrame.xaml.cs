using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace UML.Pages.MorePages
{
    public partial class DownloadBoxFrame : Page, INotifyPropertyChanged
    {
        private WebClient _webClient;
        private DateTime _startTime;
        private double _downloadPercentage;

        public double DownloadPercentage
        {
            get => _downloadPercentage;
            set
            {
                _downloadPercentage = value;
                OnPropertyChanged();

                // Update the percentage text
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PercentageTextBlock.Text = $"{value:0}%";
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadBoxFrame()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void StartDownload(string fileUrl, string savePath)
        {
            PercentageTextBlock.Text = "0%";
            DownloadPercentage = 0;

            _webClient = new WebClient();

            _webClient.DownloadProgressChanged += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DownloadPercentage = e.ProgressPercentage;
                });
            };

            _webClient.DownloadFileCompleted += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show("Download failed: " + e.Error.Message);
                    }
                    else if (e.Cancelled)
                    {
                        PercentageTextBlock.Text = "Cancelled";
                    }
                    else
                    {
                        DownloadPercentage = 100;
                        PercentageTextBlock.Text = "100%";
                        MessageBox.Show("Download complete!");
                    }
                });
            };

            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            _startTime = DateTime.Now;
            _webClient.DownloadFileAsync(new Uri(fileUrl), savePath);
        }

        public void CancelDownload()
        {
            if (_webClient != null && _webClient.IsBusy)
            {
                _webClient.CancelAsync();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}