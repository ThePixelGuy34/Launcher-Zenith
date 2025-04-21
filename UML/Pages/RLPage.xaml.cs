using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
using System.Net;
using System.Timers;
using UML.Class.LaunchLogic;

namespace UML.Pages
{
    /// <summary>
    /// Interaction logic for UTPage.xaml
    /// </summary>
    public partial class RLPage : Page
    {
        private DiscordRPCManager discordRPC;
        private System.Timers.Timer _configCheckTimer;
        private System.Timers.Timer _processCheckTimer;
        private string dllDownloadUrl = "https://zenith-api.zippywippy.online/launcher/api/redirect";
        private string UUdllDownloadUrl = "https://zenith-api.zippywippy.online/launcher/api/consoleunlocker";
        private bool isTAGameRunning = false;
        private string rlPath = string.Empty;

        public RLPage()
        {
            InitializeComponent();
            Loaded += RLPage_Loaded;
        }

        private void RLPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateRLLaunchButton();
            DiscordRPCManager.SetPresence("Online", "Looking at the Rocket League page!");
            StartProcessCheckTimer();
            Load();
            AreUrServersOn();
        }

        private async void Load()
        {
            await LoadRLBackground();
        }

        private async void AreUrServersOn()
        {
            string url = "https://zenith-api.zippywippy.online/zenith/status/backend/launcher";
            await CheckEndpointStatusAsync(url);
        }

        private async Task CheckEndpointStatusAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        StatusText.Text = "ONLINE";
                        StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ff00"));
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        StatusText.Text = "BUSY";
                        StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                    }
                    else
                    {
                        StatusText.Text = "OFFLINE";
                        StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ec1c24"));
                    }
                }
                catch (Exception)
                {
                    HideThisIg();
                }
            }
        }

        private void HideThisIg()
        {
            StatusText2.Visibility = Visibility.Collapsed;
            StatusText.Visibility = Visibility.Collapsed;
        }

        private void RLGameLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                string gamePath = GetRocketPathFromRegistry();

                if (!string.IsNullOrEmpty(gamePath))
                {
                    LaunchGame(gamePath);
                }
                else
                {
                    mainWindow.OpenDownloadOrImportRL();
                }
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }


        private async Task LoadRLBackground()
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
                        var appContent = JsonConvert.DeserializeObject<Class.DontBreakPls.AppConfig>(json);
                        var rlPage = appContent?.games?.FirstOrDefault(g => g.id == "rocketpage");
                        if (rlPage != null)
                        {
                            var images = rlPage.images;
                            var bgImage = images.FirstOrDefault(img => img.id == "backgroundImageRLP");
                            if (bgImage != null) LoadImage(bgImage.image, RLBG);

                            var logoImage = images.FirstOrDefault(img => img.id == "rocketleagueLogo");
                            if (logoImage != null) LoadImage(logoImage.image, RLLogo);
                        }
                        else
                        {
                            Logger.Log("Malformed JSON, contact zippywippy or isaac on Discord. - RP-0001");
                        }
                    }
                    else
                    {
                        Logger.Log($"Failed to load contentpages: {(int)response.StatusCode} - RP-0002");
                    }
                }
                catch (TaskCanceledException)
                {
                    Logger.Log("Connection timeout when loading contentpages. - RP-0003");
                }
                catch (HttpRequestException)
                {
                    Logger.Log("Unable to connect to Zenith services when loading contentpages. - RP-0004");
                }
                catch (JsonException)
                {
                    Logger.Log("Contentpages JSON is invalid, contact isaac or zippywippy. - RP-0005");
                }
                catch (Exception)
                {
                    Logger.Log("An error occurred when loading contentpages, is the backend down? - RP-0006");
                }
            }
        }

        private void LoadImage(string imageUrl, Image imageControl)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imageUrl, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                imageControl.Dispatcher.Invoke(() =>
                {
                    imageControl.Source = bitmapImage;
                });
            }
            catch (Exception ex)
            {
                Logger.Log("Error loading image: " + ex.Message);
            }
        }

        private void ShowFallbackImage()
        {
            try
            {
                //this.imageControl.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/assetfailfallback.png"));
                //this.imageControl.Visibility = Visibility.Visible;
                //this.videoPlayer.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                //this.imageControl.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/assetfailfallback.png"));
            }
        }

        private void StartConfigCheckTimer()
        {
            _configCheckTimer = new System.Timers.Timer(5000);
            _configCheckTimer.Elapsed += OnConfigCheckTimerElapsed;
            _configCheckTimer.AutoReset = true;
            _configCheckTimer.Enabled = true;
        }

        private void StopConfigCheckTimer()
        {
            if (_configCheckTimer != null)
            {
                _configCheckTimer.Stop();
                _configCheckTimer.Dispose();
                _configCheckTimer = null;
            }
        }

        private void RLGameSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Will be fixed soon! Gone for now.");
        }

        private void CloseGame_Click(object sender, RoutedEventArgs e)
        {
            KillTAProcess();
        }

        private void OnConfigCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            string gamePath = GetRocketPathFromRegistry();
            if (!string.IsNullOrEmpty(gamePath))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateRLLaunchButton();
                    StopConfigCheckTimer();
                });
            }
        }

        private async void LaunchGame(string TAPath)
        {
            string username = StringSharing.Username;
            string password = StringSharing.AuthKey;

            try
            {
                //WebClient RedirectDownload = new WebClient();
                //RedirectDownload.DownloadFile("https://zenith-api.zippywippy.online/zenith/api/v1/launcher/tagame/steam", System.IO.Path.Combine(TAPath, "Binaries\\Win32", "steam_api.dll"));
                //WebClient PatchDownload = new WebClient();
                //PatchDownload.DownloadFile("https://zenith-api.zippywippy.online/zenith/api/v1/launcher/tagame", System.IO.Path.Combine(TAPath, "Binaries\\Win64", "Patch.dll"));

                await Task.Delay(2000);

                await Task.Run(() =>
                {
                    StartGameRL.Launch(TAPath, username, password);
                });
            }
            catch (Exception ex)
            {
                Logger.Log($"An exception occured when trying to launch Rocket League. {ex.Message}");
            }
        }

        private void StartProcessCheckTimer()
        {
            _processCheckTimer = new System.Timers.Timer(1000);
            _processCheckTimer.Elapsed += OnProcessCheckTimerElapsed;
            _processCheckTimer.AutoReset = true;
            _processCheckTimer.Enabled = true;
        }

        private void OnProcessCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (IsTAGameRunning())
                {
                    isTAGameRunning = true;
                    UpdateRLLaunchButton();
                }
                else
                {
                    isTAGameRunning = false;
                    UpdateRLLaunchButton();
                }
            });
        }

        private bool IsTAGameRunning()
        {
            return Process.GetProcessesByName("RocketLeague").Length > 0;
        }

        public void UpdateRLLaunchButton()
        {
            string RLGamePath = GetRocketPathFromRegistry();
            if (isTAGameRunning)
            {
                //LaunchButton.Content = "Close";
                //LaunchButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bcbcbc"));
                //LaunchButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#474747"));
                LaunchButton.Visibility = Visibility.Collapsed;
                SettingsButton.Visibility = Visibility.Collapsed;
                SettingsIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                LaunchButton.Content = string.IsNullOrEmpty(RLGamePath) ? "Import" : "Launch";
                LaunchButton.Visibility = Visibility.Visible;
                SettingsButton.Visibility = Visibility.Visible;
                SettingsIcon.Visibility = Visibility.Visible;
                //LaunchButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95c990"));
                //LaunchButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff"));
            }
        }

        private void KillTAProcess()
        {
            var processes = Process.GetProcessesByName("RocketLeague");
            foreach (var process in processes)
            {
                process.Kill();
                Logger.Log($"Closed Rocket League process: {process.Id}");
            }

            StartGameRL.OnRLExit(null, null);
            UpdateRLLaunchButton();
        }

        private void DiscordUTPage_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/GGZ8D8NaPk") { UseShellExecute = true });
        }

        private void XUT_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://x.com/ZenithMP_") { UseShellExecute = true });
        }

        private void YTUT_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.youtube.com/@ZenithMP") { UseShellExecute = true });
        }

        private void SupportBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/gwxjYAPtxB") { UseShellExecute = true });
        }

        private void DiscordUT_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/Jq3GMkdusC") { UseShellExecute = true });
        }

        private void DevVidsUT_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.youtube.com/@ZenithMP") { UseShellExecute = true });
        }

        private void NewsUT_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.com/channels/715487758722007121/1337762536874971178") { UseShellExecute = true });
        }

        private string GetRocketPathFromRegistry()
        {
            try
            {
                string registryKey = @"Zenith";
                string gamePathKey = "TAGamePath";

                using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(registryKey))
                {
                    if (key != null)
                    {
                        object gamePath = key.GetValue(gamePathKey);

                        if (gamePath != null)
                        {
                            rlPath = gamePath.ToString();
                            return rlPath;
                        }
                    }
                }

                Logger.Log("Game path not found in registry.");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Log($"Error reading registry: {ex.Message}");
                return null;
            }
        }
    }

}
