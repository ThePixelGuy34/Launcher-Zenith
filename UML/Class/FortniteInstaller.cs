using System;
using System.IO;
using System.Net;
using System.Windows;
using UML.Class.ViewModels;

namespace UML.Class
{
    internal class FortniteInstaller
    {
        private WebClient? _webClient;
        private DownloadViewModel _viewModel = DownloadViewModel.Instance;

        public void StartDownload(string selectedPath)
        {
            _viewModel.IsDownloading = true;
            _viewModel.DownloadProgress = "0% downloaded";
            _viewModel.StatusMessage = "Starting download...";

            string fileUrl = "https://zenith-api.zippywippy.online/launcher/api/build/6";
            _webClient = new WebClient();

            _webClient.DownloadProgressChanged += (s, ev) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string progress = $"{ev.ProgressPercentage}% downloaded";
                    _viewModel.DownloadProgress = progress;

                    double bytesPerSecond = ev.BytesReceived / (DateTime.Now - _startTime).TotalSeconds;
                    string speed = FormatBytes(bytesPerSecond) + "/s";

                    _viewModel.StatusMessage = $"Downloading... {FormatBytes(ev.BytesReceived)} of {FormatBytes(ev.TotalBytesToReceive)} ({speed})";
                });
            };

            _webClient.DownloadFileCompleted += (s, ev) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _viewModel.IsDownloading = false;

                    if (ev.Error != null)
                    {
                        _viewModel.StatusMessage = "Download failed: " + ev.Error.Message;
                        _viewModel.DownloadProgress = "Download failed!";
                        MessageBox.Show("Download failed: " + ev.Error.Message);
                    }
                    else if (ev.Cancelled)
                    {
                        _viewModel.StatusMessage = "Download was cancelled";
                        _viewModel.DownloadProgress = "Download cancelled!";
                    }
                    else
                    {
                        _viewModel.StatusMessage = "Download completed successfully!";
                        _viewModel.DownloadProgress = "100% downloaded";
                        MessageBox.Show("Download complete!");
                    }
                });
            };

            string fileName = "6.21.rar";
            string fullPath = Path.Combine(selectedPath, fileName);

            Directory.CreateDirectory(selectedPath);

            _startTime = DateTime.Now;
            _webClient.DownloadFileAsync(new Uri(fileUrl), fullPath);
        }

        private DateTime _startTime;

        public void CancelDownload()
        {
            if (_webClient != null && _viewModel.IsDownloading)
            {
                _webClient.CancelAsync();
            }
        }

        private string FormatBytes(double bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }
            return String.Format("{0:0.##} {1}", bytes, sizes[order]);
        }
    }
}