using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UML.Class;
using UML.Services;

namespace UML.Pages
{
    public partial class LauncherMainBackgroundVide : Page
    {
        public static Class.DontBreakPls.AppConfig AppContent { get; private set; }

        public LauncherMainBackgroundVide()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.LauncherMain_Loaded);
        }

        private async void LauncherMain_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAppContent();
        }

        private void PlayVideo(string videoUrl)
        {
            try
            {
                this.videoPlayer.Source = new Uri(videoUrl);
                this.videoPlayer.Visibility = Visibility.Visible;
                this.videoPlayer.Play();
                this.videoPlayer.Volume = 0;
                this.imageControl.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                this.videoPlayer.Visibility = Visibility.Collapsed;
                this.ShowFallbackImage();
            }
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.videoPlayer.Position = TimeSpan.Zero;
            this.videoPlayer.Play();
        }

        private void ShowFallbackImage()
        {
            try
            {
                //this.imageControl.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/assetfailfallback.png"));
                this.imageControl.Visibility = Visibility.Visible;
                this.videoPlayer.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                //this.imageControl.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/assetfailfallback.png"));
            }
        }

        private async Task LoadAppContent()
        {
            string apiUrl = "https://zenith-api.zippywippy.online/zenith/api/v1/launcher/pages";
            using (HttpClient client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            }))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var appContent = JsonConvert.DeserializeObject<UML.Class.DontBreakPls.AppConfig>(json);
                        string? videoUrl = appContent?.Main?.video;
                        if (!string.IsNullOrEmpty(videoUrl))
                        {
                            bool videoLoaded = false;
                            this.videoPlayer.MediaOpened += (s, e) => { videoLoaded = true; };
                            await Dispatcher.InvokeAsync(() => PlayVideo(videoUrl));
                            var timeoutTask = Task.Delay(5000);
                            while (!videoLoaded && !timeoutTask.IsCompleted)
                            {
                                await Task.Delay(500);
                            }
                            if (!videoLoaded)
                            {
                                await Dispatcher.InvokeAsync(ShowFallbackImage);
                            }
                        }
                        else
                        {
                            await Dispatcher.InvokeAsync(ShowFallbackImage);
                            Logger.Log($"Malformed JSON, contact isaac or zippywippy on Discord. - LM-0007");
                        }
                    }
                    else
                    {
                        Logger.Log($"Failed to load background video: {(int)response.StatusCode} - LM-0008");
                        await Dispatcher.InvokeAsync(ShowFallbackImage);
                    }
                }
                catch (TaskCanceledException)
                {
                    Logger.Log("Connection timeout when loading video. - LM-0009");
                    await Dispatcher.InvokeAsync(ShowFallbackImage);
                }
                catch (HttpRequestException)
                {
                    Logger.Log("Unable to connect to Zenith services when loading contentpages. - LM-0010");
                    await Dispatcher.InvokeAsync(ShowFallbackImage);
                }
                catch (JsonException)
                {
                    Logger.Log("Malformed JSON, contact isaac or zippywippy on Discord. - LM-0011");
                    await Dispatcher.InvokeAsync(ShowFallbackImage);
                }
                catch (Exception)
                {
                    Logger.Log("An error occurred when loading contentpages. - LM-0012");
                    await Dispatcher.InvokeAsync(ShowFallbackImage);
                }
            }
        }


        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.videoPlayer.Visibility = Visibility.Visible;
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.videoPlayer.Volume > 0.0)
            {
                this.videoPlayer.Volume = 0.0;
                this.muteButton.Content = (object)"\uD83D\uDD07";
            }
            else
            {
                this.videoPlayer.Volume = 1.0;
                this.muteButton.Content = (object)"\uD83D\uDD0A";
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.videoPlayer.Volume = this.volumeSlider.Value;
        }

        private void MuteButton_MouseEnter(object sender, MouseEventArgs e)
        {
            volumeSlider.Visibility = Visibility.Visible;
        }

        private void MuteButton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.volumeSlider.Visibility = Visibility.Collapsed;
        }
    }
}
