using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using UML.Class.LaunchLogic;
using UML.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Net.Http;
using System.Windows.Media.Imaging;
using UML.Class;
using Newtonsoft.Json;
using UML.Security;
using UML.Pages.MorePages;
using Newtonsoft.Json.Linq;
using UML.Class.DontBreakPls;

namespace UML.Pages
{
    public partial class FortnitePage : Page
    {
        private System.Timers.Timer _configCheckTimer;
        private System.Timers.Timer _processCheckTimer;
        private string dllDownloadUrl = "https://zenith-api.zippywippy.online/launcher/api/redirect";
        private string UUdllDownloadUrl = "https://zenith-api.zippywippy.online/launcher/api/consoleunlocker";
        private DiscordRPCManager discordRPC;
        private bool isFNGameRunning = false;
        private string fortnitePath = string.Empty;
        public string username = string.Empty;
        private string password = string.Empty;
        public static FortnitePage Current { get; private set; }
        

        public FortnitePage()
        {
            InitializeComponent();
            Loaded += FortnitePage_Loaded;
            Current = this;
            AreUrServersOn();
            //GetFortnitePathFromRegistry();
            UpdateFNLaunchButton();
            DiscordRPCManager.SetPresence("Online", "Looking at the Fortnite page!");
            StartProcessCheckTimer();
            Load();
            CheckFortnitePath();
            FNLaunchButton.IsEnabled = false;
        }

        public void StartDownload(string installPath)
        {
            var downloadFrame = new DownloadBoxFrame();
            DownloadBoxFrame.Navigate(downloadFrame);
            string fileUrl = "https://zenith-api.zippywippy.online/zenith/api/v1/build/5.41";
            string fileName = "5.41.rar";
            string fullPath = Path.Combine(installPath, fileName);
            downloadFrame.StartDownload(fileUrl, fullPath);
        }

        private void CheckFortnitePath()
        {
            string gamePath = GetFortnitePathFromRegistry();
            if (!string.IsNullOrEmpty(gamePath) && IsValidFortnitePath(gamePath))
            {
                // valid path found, do nothing here
            }
            else
            {
                DeleteFortnitePathFromRegistry();
            }
        }

        private void FortnitePage_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer_ScrollChanged(MainScrollViewer, null);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                double maxScroll = scrollViewer.ScrollableHeight;
                TopShadow.Opacity = scrollViewer.VerticalOffset > 0 ? 0.4 : 0;
                BottomShadow.Opacity = scrollViewer.VerticalOffset < maxScroll ? 0.4 : 0;
            }
        }


        private async void Load()
        {
            await LoadFNBackground();
        }

        private async void AreUrServersOn()
        {
            string url = "https://zenith-api.zippywippy.online/lightswitch/api/service/Fortnite/status";
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

        private void FNGameLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                string gamePath = GetFortnitePathFromRegistry();

                if (!string.IsNullOrEmpty(gamePath))
                {
                    LaunchGame(gamePath);
                }
                else
                {
                    Logger.Log("Invalid or missing Fortnite path.");
                    mainWindow.OpenDownloadOrImport();
                }
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }

        private void UpdateSettings()
        {
            if (isFNGameRunning)
            {
                SettingsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#989898"));
                SettingsIcon.Visibility = Visibility.Visible;
            }
            else
            {
                SettingsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#78a375"));
                SettingsIcon.Visibility = Visibility.Visible;
            }
        }

        private bool IsValidFortnitePath(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Logger.Log($"Fortnite directory loaded. {path}");
                    return false;
                }

                string engineBinariesPath = Path.Combine(path, "Engine", "Binaries");
                string fortniteClientPath = Path.Combine(path, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe");

                if (!Directory.Exists(engineBinariesPath))
                {
                    Logger.Log($"Invalid Fortnite installation: Engine/Binaries directory not found.");
                    return false;
                }

                if (!File.Exists(fortniteClientPath))
                {
                    Logger.Log($"FortniteClient-Win64-Shipping.exe not found, Invalid installation.");
                    return false;
                }

                Logger.Log($"Valid Fortnite installation at: {path}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to locate Fortnite path: {ex.Message}");
                return false;
            }
        }


        private async Task LoadFNBackground()
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
                        var appContent = JsonConvert.DeserializeObject<AppConfig>(json);
                        var fortnitePage = appContent?.games?.FirstOrDefault(g => g.id == "fortnitepage");

                        if (fortnitePage != null)
                        {
                            var images = fortnitePage.images;
                            var bgImage = images.FirstOrDefault(img => img.id == "backgroundImageFNP");
                            var firstImage = images.FirstOrDefault(img => img.id == "newsImageFNP1");
                            var secondImage = images.FirstOrDefault(img => img.id == "newsImageFNP2");
                            var thirdImage = images.FirstOrDefault(img => img.id == "newsImageFNP3");

                            if (bgImage != null) LoadImage(bgImage.image, FortniteBG);
                            if (firstImage != null) LoadImage(firstImage.image, FortniteNews1);
                            if (secondImage != null) LoadImage(secondImage.image, FortniteNews2);
                            if (thirdImage != null) LoadImage(thirdImage.image, FortniteNews3);

                            //header1
                            if (firstImage != null && !string.IsNullOrEmpty(firstImage.header))
                                HeaderText1.Text = firstImage.header;
                            //desc1
                            if (firstImage != null && !string.IsNullOrEmpty(firstImage.description))
                                DescText1.Text = firstImage.description;
                            //header2
                            if (secondImage != null && !string.IsNullOrEmpty(secondImage.description))
                                DescText2.Text = secondImage.description;
                            //desc2
                            if (secondImage != null && !string.IsNullOrEmpty(secondImage.header))
                                HeaderText2.Text = secondImage.header;
                            //header3
                            if (thirdImage != null && !string.IsNullOrEmpty(thirdImage.header))
                                HeaderText3.Text = thirdImage.header;
                            //desc3
                            if (thirdImage != null && !string.IsNullOrEmpty(thirdImage.description))
                                DescText3.Text = thirdImage.description;
                        }
                        else
                        {
                            Logger.Log("Failed to load contentpages.");
                            ContentPagesFail();
                        }
                    }
                    else
                    {
                        Logger.Log($"Failed to load contentpages: {(int)response.StatusCode}");
                        ContentPagesFail();
                    }
                }
                catch (TaskCanceledException)
                {
                    Logger.Log("Failed to load contentpages.");
                    ContentPagesFail();
                }
                catch (HttpRequestException)
                {
                    Logger.Log("Failed to load contentpages.");
                    ContentPagesFail();
                }
                catch (JsonException)
                {
                    Logger.Log("Failed to load contentpages.");
                    ContentPagesFail();
                }
                catch (Exception)
                {
                    Logger.Log("Failed to load contentpages.");
                    ContentPagesFail();
                }
            }
        }

        private void ContentPagesFail()
        {
            News1.Opacity = 0;
            News2.Opacity = 0;
            News3.Opacity = 0;

            try
            {
                this.FortniteBG.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/Content/Web/background/bgFnFailure.png"));
                this.FortniteBG.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                // try again if u cant load locally then ur cooked i suppose
                this.FortniteBG.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/Content/Web/background/bgFnFailure.png"));
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
            }
        }

        private void FNGameSettings_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                string gamePath = GetFortnitePathFromRegistry();

                if (!string.IsNullOrEmpty(gamePath))
                {
                    mainWindow.OpenFNSettings();
                }
                else
                {
                    MessageBox.Show("Cannot edit build settings until you have imported the game.");
                }
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }

        private void CloseGame_Click(object sender, RoutedEventArgs e)
        {
            KillFortniteProcess();
        }

        private void OnConfigCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            string gamePath = GetFortnitePathFromRegistry();
            if (!string.IsNullOrEmpty(gamePath))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateFNLaunchButton();
                    StopConfigCheckTimer();
                });
            }
        }

        private async void LaunchGame(string fortnitePath)
        {
            try
            {
                if (string.IsNullOrEmpty(fortnitePath))
                {
                    Logger.Log("Fortnite path is empty or null.");
                    return;
                }

                if (!Directory.Exists(fortnitePath))
                {
                    Logger.Log($"Fortnite directory not found at: {fortnitePath}");
                    return;
                }

                if (StringSharing.LoginMethod is "email")
                {
                    username = StringSharing.displayName;
                    password = StringSharing.authToken;
                }
                else
                {
                    username = StringSharing.Username;
                    password = StringSharing.AuthKey;
                }

                string engineBinariesPath = Path.Combine(fortnitePath, "Engine", "Binaries");
                string fortniteClientPath = Path.Combine(fortnitePath, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe");
                string fortniteLauncherPath = Path.Combine(fortnitePath, "FortniteLauncher.exe");

                if (!Directory.Exists(engineBinariesPath))
                {
                    Logger.Log($"Invalid Fortnite installation: Engine/Binaries directory not found.");
                    return;
                }

                if (!File.Exists(fortniteClientPath))
                {
                    Logger.Log($"FortniteClient-Win64-Shipping.exe not found. This may not be a valid Fortnite installation.");
                    return;
                }

                string nvidiaPath = Path.Combine(fortnitePath, "Engine", "Binaries", "ThirdParty", "NVIDIA", "NVaftermath", "Win64");
                if (!Directory.Exists(nvidiaPath))
                {
                    Directory.CreateDirectory(nvidiaPath);
                }

                try
                {
                    WebClient RedirectDownload = new WebClient();
                    string downloadPath = Path.Combine(nvidiaPath, "GFSDK_Aftermath_Lib.x64.dll");

                    RedirectDownload.DownloadFile("https://zenith-api.zippywippy.online/zenith/api/v1/launcher/redirect", downloadPath);
                    await Task.Delay(2000);
                }
                catch
                {
                    Logger.Log("Failed to launch Fortnite. RI-0001");
                }

                await Task.Run(() =>
                {
                    StartGameFN.Launch(fortnitePath, "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -fromfl=eac -nobe -fltoken=3db3ba5dcbd2e16703f3978d", username, password);
                    FakeACTempFN.Start(fortnitePath, "FortniteClient-Win64-Shipping_BE.exe", "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -fromfl=eac -nobe -fltoken=3db3ba5dcbd2e16703f3978d", "r");
                    FakeACTempFN.Start(fortnitePath, "FortniteLauncher.exe", "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -fromfl=eac -nobe -fltoken=3db3ba5dcbd2e16703f3978d", "dsf");
                });
            }
            catch (Exception ex)
            {
                Logger.Log($"An exception occurred when trying to launch Fortnite: {ex.Message}");
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
                if (IsFortniteRunning())
                {
                    isFNGameRunning = true;
                    UpdateFNLaunchButton();
                }
                else
                {
                    isFNGameRunning = false;
                    UpdateFNLaunchButton();
                }
            });
        }
        private bool IsFortniteRunning()
        {
            return Process.GetProcessesByName("FortniteClient-Win64-Shipping").Length > 0;
        }

        public void UpdateFNLaunchButton()
        {
            string fnGamePath = GetFortnitePathFromRegistry();
            if (isFNGameRunning == true)
            {
                FNLaunchButton.Content = "Running";
                UpdateSettings();
                SettingsButton.Visibility = Visibility.Visible;
                FNLaunchButton.IsEnabled = false;
                SettingsButton.IsEnabled = false;
                FNLaunchButton.IsHitTestVisible = false;
                SettingsButton.IsHitTestVisible = false;
            }
            else
            {
                FNLaunchButton.Content = string.IsNullOrEmpty(fnGamePath) ? "Import" : "Launch";
                FNLaunchButton.Visibility = Visibility.Visible;
                SettingsButton.Visibility = Visibility.Visible;
                SettingsIcon.Visibility = Visibility.Visible;
                FNLaunchButton.IsEnabled = true;
                SettingsButton.IsEnabled = true;
                FNLaunchButton.IsHitTestVisible = true;
                SettingsButton.IsHitTestVisible = true;
            }
        }

        private void KillFortniteProcess()
        {
            var processes = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
            foreach (var process in processes)
            {
                process.Kill();
                Logger.Log($"Closed Fortnite process: {process.Id}");
            }

            StartGameFN.OnFortniteExit(null, null);
            UpdateFNLaunchButton();
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
            Process.Start(new ProcessStartInfo("https://www.youtube.com/@ZenithMultiplayer") { UseShellExecute = true });
        }

        private void SupportBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/gwxjYAPtxB") { UseShellExecute = true });
        }

        private string GetFortnitePathFromRegistry()
        {
            try
            {
                string registryKey = @"Software\Zenith";
                string gamePathKey = "AthenaGamePath";

                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryKey))
                {
                    if (key != null)
                    {
                        object gamePath = key.GetValue(gamePathKey);

                        if (gamePath != null)
                        {
                            fortnitePath = gamePath.ToString();
                            return fortnitePath;
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

        private string DeleteFortnitePathFromRegistry()
        {
            try
            {
                string registryKey = @"Zenith";
                string gamePathKey = "AthenaGamePath";
                string retrievedPath = null;

                using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(registryKey, true))
                {
                    if (key != null)
                    {
                        object gamePath = key.GetValue(gamePathKey);
                        if (gamePath != null)
                        {
                            retrievedPath = gamePath.ToString();
                            fortnitePath = retrievedPath;
                            key.DeleteValue(gamePathKey, false);
                            Logger.Log("Deleted path due to it being invalid.");
                        }
                    }
                }

                if (retrievedPath == null)
                {
                    Logger.Log("Game path isn't even set?");
                    return null;
                }

                return retrievedPath;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to delete registry: {ex.Message}");
                return null;
            }
        }
    }
}


