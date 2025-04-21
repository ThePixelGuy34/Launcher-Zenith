using Discord.Rest;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using UML.Services;
using UML.Pages;
using System.IO;
using System.Net;
using UML.Class;
using System.Security.Cryptography;
using UML.Class.LaunchLogic;
using System.Windows.Forms;
using System.Windows.Threading;

namespace UML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Exclude = true)]
    public partial class MainWindow : Window
    {
        public static int HttpFailure = 0;
        public bool isSettingsPageOpen = false;
        public bool isBigSettingsPageOpen = false;
        public bool isDownloadsPageOpen = false;
        public bool isAnimating = false;
        public bool isSetAnimating = false;
        private bool isAccountFrameOpen = false;
        private string updaterBasePath = string.Empty;
        private string updaterPath = string.Empty;
        private TrayManager _trayManager;

        public string? DiscordId { get; private set; }
        public string? Username { get; private set; }
        public string? AuthKey { get; private set; }
        public string? Hash { get; private set; }
        public string? LoginReason { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            //CheckUpdates();
            RegisterCustomUriScheme();
            _trayManager = new TrayManager(this, "Zenith Launcher", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Zenith", "app.ico"));
            this.PreviewMouseDown += DownloadsPage_PreviewMouseDown;
            this.PreviewMouseDown += SettingsPage_PreviewMouseDown;
            this.PreviewMouseDown += SettingsPageBig_PreviewMouseDown;
            this.PreviewMouseDown += UserSettings_PreviewMouseDown;
            this.StateChanged += UML_StateChanged;
            OnLoad();
        }

        private void UML_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState is WindowState.Maximized)
            {
                MainGrid.Margin = new Thickness(8);
            }
            else if (this.WindowState is WindowState.Normal)
            {
                MainGrid.Margin = new Thickness(0);
            }
        }

        private void OnLoad()
        {
            SettingsFrame.IsHitTestVisible = false;
            DownloadsFrame.IsHitTestVisible = false;
            SettingsBigFrame.IsHitTestVisible = false;
        }

        ///////////////////////////////////////// MISC LOGIC.

        private void UserText_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double textWidth = e.NewSize.Width;

            if (textWidth > 7)
            {
                //UserOnlineIcon.Margin = new Thickness(textWidth + -20, 0, 0, 0);
            }
            else
            {
                //UserOnlineIcon.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        ///////////////////////////////////////// CALLBACK/AUTH LOGIC.


        public void ProcessUriScheme(string uri)
        {
            try
            {
                CloseLoginPage();
                Services.Logger.Log($"Processing Callback, {uri}");

                string cleanedUri = uri.Replace(" ", "").Replace("zenith:", "").Trim('/');
                Services.Logger.Log($"Received callback and separated fields, {cleanedUri}");

                var parts = cleanedUri.Split('/');
                Services.Logger.Log($"Parts: {string.Join(", ", parts)}");

                if (parts.Length != 5)
                {
                    Services.Logger.Log("Callback doesn't contain the 5 required fields.");
                    return;
                }

                DiscordId = parts[0].Trim();
                Username = parts[1].Trim();
                AuthKey = parts[2].Trim();
                Hash = parts[3].Trim();
                LoginReason = parts[4].Trim();

                // assign to shared storage
                StringSharing.DiscordId = DiscordId;
                StringSharing.Username = Username;
                StringSharing.AuthKey = AuthKey;
                StringSharing.Hash = Hash;
                StringSharing.LoginReason = LoginReason;

                Services.Logger.Log($"Received Callback in MainWindow.");
                GoToMain();
            }
            catch (Exception ex)
            {
                Services.Logger.Log($"Error processing callback. {ex.Message}");
            }
        }

        public static void RegisterCustomUriScheme()
        {
            try
            {
                string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Zenith", "app", "Zenith.exe");
                string command = $"\"{appPath}\" \"%1\""; 

                Services.Logger.Log($"App Path: {appPath}");

                using (RegistryKey oasisKey = Registry.CurrentUser.CreateSubKey(@"Software\Zenith"))
                {
                    if (oasisKey != null)
                    {
                        oasisKey.SetValue("", "URL:Zenith Protocol");
                        oasisKey.SetValue("URL Protocol", "");

                        using (RegistryKey commandKey = oasisKey.CreateSubKey(@"shell\open\command"))
                        {
                            if (commandKey != null)
                            {
                                commandKey.SetValue("", command);
                                Services.Logger.Log("Command for callback set.");
                            }
                            else
                            {
                                Services.Logger.Log("Unable to create callback.");
                            }
                        }

                        // debug
                         Services.Logger.Log("Callback registered successfully.");
                    }
                    else
                    {
                        Services.Logger.Log("Unable to create Zenith callback.");
                    }
                }
            }
            catch (Exception ex)
            {
                Services.Logger.Log($"Error: {ex.Message}");
            }
        }

        /////////////////////////////////////////////////////////// TROLLING LOGIC

        private void GoToMain()
        {
            var loginMethod = "discord";
            NavMain(StringSharing.Username, loginMethod);
        }

        ////////////////////////////////////////////////////////////// NAVIGATION/ANIMATIONS BELOW.

        public void OpenUpdateUI()
        {
            PageNavigation.Content = null;
            UpdateFrame.Navigate(new Pages.MorePages.UpdatePage());
        }

        public void InitLogout()
        {
            ShutdownFrame.Navigate(new Pages.MorePages.LogoutError());
            ShutdownFrame.IsHitTestVisible = true;
            ShutdownFrame.Opacity = 1;
        }

        private void IncrementHttpFailed()
        {
            HttpFailure++;
        }

        public async void OpenUpdater()
        {
            try
            {
                updaterBasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                updaterPath = Path.Combine(updaterBasePath, "Zenith", "Updater", "ZenithLauncherUpdater.exe");

                Process.Start(updaterPath);

                await Task.Delay(500);
                System.Windows.Application.Current.Shutdown();
            }
            catch
            {
                Logger.Log($"Cannot open the updater, the launcher may be corrupted? | UL-0001 | {updaterPath}");
                OpenBuggedLauncherUI();
            }
        }

        private void OpenBuggedLauncherUI()
        {
            ShutdownFrame.Navigate(new Pages.MorePages.LauncherMissingFiles());
            ShutdownFrame.IsHitTestVisible = true;
        }

        public void OpenDownloadOrImport()
        {
            ImportFrame.Visibility = Visibility.Visible;
            ImportFrame.IsHitTestVisible = true;
            ImportFrame.Navigate(new Pages.ImportAndDownload());
        }

        public void OpenDownloadOrImportRL()
        {
            ImportFrame.Visibility = Visibility.Visible;
            ImportFrame.IsHitTestVisible = true;
            ImportFrame.Navigate(new Pages.MorePages.ImportAndDownloadRL());
        }

        public void ClosePage()
        {
            ImportFrame.IsHitTestVisible = false;
            ImportFrame.Visibility = Visibility.Collapsed;
        }

        public void OpenFNSettings()
        {
            ImportFrame.IsHitTestVisible = true;
            ImportFrame.Navigate(new Pages.MorePages.FNBuildSettings());
            ImportFrame.Visibility = Visibility.Visible;
        }
        public void ClosePageIRL()
        {
            ImportFrame.IsHitTestVisible = false;
            ImportFrame.Visibility = Visibility.Collapsed;
        }

        public void OpenDownload()
        {
            ImportFrame.Navigate(new Pages.DownloadsPageV2());
        }

        public void OpenDownloadRL()
        {
            ImportFrame.Navigate(new Pages.MorePages.DownloadsPageRL());
        }

        private void LoadUser()
        {
            if (StringSharing.LoginMethod is "email")
            {
                UserText.Text = StringSharing.displayName;
            }
            else
            {
                UserText.Text = StringSharing.Username;
            }
        }

        public void CloseLoginPage()
        {
            OpenLoadingBar();
            PageNavigation.Content = null;
        }

        public void SignOut_Zenith()
        {
            SettingsPanel.Opacity = 0;
            MainFrameFrameLol.Content = null;
            PageNavigation.Content = null;
            ToiletImg.Opacity = 1;
            ToiletImg.Visibility = Visibility.Visible;
            BackgroundOpacity.Opacity = 0.7;
            BackgroundOpacity.Visibility = Visibility.Visible;
            UserText.Opacity = 0;
            MainFrameFrameLol.Opacity = 1;
            PageNavigation.Opacity = 1;
            PageNavigation.Visibility = Visibility.Visible;
            ZenithLogoLogin.Opacity = 1;
            SettingsBigBtn.Visibility = Visibility.Visible;
            SettingsBigBtn.IsHitTestVisible = true;
            SettingsBigBtn.Opacity = 1;
            SettingsBtn.Visibility = Visibility.Collapsed;
            SettingsBtn.Opacity = 0;
            TasksBtn.Visibility = Visibility.Collapsed;
            TasksBtn.Opacity = 0;
            PageNavigation.Navigate(new Pages.MorePages.SignOut());
            UserSettingsButton.Visibility = Visibility.Collapsed;
            UserSettingsButton.IsHitTestVisible = false;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (isAnimating) return;

            if (isDownloadsPageOpen)
            {
                SlideInDownloads();
                isDownloadsPageOpen = false;
            }

            if (isSettingsPageOpen)
            {
                SlideInSettings();
                isSettingsPageOpen = false;
            }
            else
            {
                SlideOutSettings();
                isSettingsPageOpen = true;
            }
        }

        private void SettingsBig_Click(object sender, RoutedEventArgs e)
        {
            if (isSetAnimating) return;

            if (isBigSettingsPageOpen)
            {
                SlideInBigSettings();
                isBigSettingsPageOpen = false;
            }
            else
            {
                SlideOutBigSettings();
            }
        }

        public void CloseSettingsBig()
        {
            SlideInBigSettings();
        }

        public void CloseDownloads()
        {
            SlideInDownloads();
        }

        public void CloseSettings()
        {
            SlideInSettings();
        }

        private void Downloads_Click(object sender, RoutedEventArgs e)
        {
            if (isAnimating) return;

            if (isSettingsPageOpen)
            {
                SlideInSettings();
                isSettingsPageOpen = false;
            }

            if (isDownloadsPageOpen)
            {
                SlideInDownloads();
                isDownloadsPageOpen = false;
            }
            else
            {
                SlideOutDownloads();
                isDownloadsPageOpen = true;
            }
        }

        private void SlideInDownloads()
        {
            isAnimating = true;
            DownloadsFrame.IsHitTestVisible = false;

            DoubleAnimation fadeInDescTxt = new DoubleAnimation
            {
                From = 0.6,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.15))
            };
            SettingsThing.BeginAnimation(UIElement.OpacityProperty, fadeInDescTxt);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 300,
                Duration = TimeSpan.FromSeconds(0.15)
            };

            animation.Completed += (s, a) =>
            {
                DownloadsFrame.Visibility = Visibility.Collapsed;
                isAnimating = false;
                isDownloadsPageOpen = false;
            };

            DownloadsFrameTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void SlideOutDownloads()
        {
            isAnimating = true;
            isDownloadsPageOpen = true;
            DownloadsFrame.Navigate(new Pages.DownloadsPage());
            DownloadsFrame.Visibility = Visibility.Visible;
            DownloadsFrame.IsHitTestVisible = true;

            DoubleAnimation fadeInDescTxt = new DoubleAnimation
            {
                From = 0,
                To = 0.6,
                Duration = new Duration(TimeSpan.FromSeconds(0.15))
            };
            SettingsThing.BeginAnimation(UIElement.OpacityProperty, fadeInDescTxt);

            var animation = new DoubleAnimation
            {
                From = 300,
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 150)
            };

            animation.Completed += (s, a) =>
            {
                isAnimating = false;
            };

            DownloadsFrameTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }



        private void SlideOutSettings()
        {
            isAnimating = true;
            SettingsFrame.Navigate(new Pages.SettingsPage());
            SettingsFrame.Visibility = Visibility.Visible;
            SettingsFrame.IsHitTestVisible = true;
            isSettingsPageOpen = true;

            DoubleAnimation fadeInDescTxt = new DoubleAnimation
            {
                From = 0,
                To = 0.6,
                Duration = new Duration(TimeSpan.FromSeconds(0.15))
            };
            SettingsThing.BeginAnimation(UIElement.OpacityProperty, fadeInDescTxt);

            var animation = new DoubleAnimation
            {
                From = 300,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.15)
            };

            animation.Completed += (s, a) =>
            {
                isAnimating = false;
            };

            SettingsFrameTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void SlideInSettings()
        {
            isAnimating = true;
            SettingsFrame.IsHitTestVisible = false;

            DoubleAnimation fadeInDescTxt = new DoubleAnimation
            {
                From = 0.6,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.15))
            };
            SettingsThing.BeginAnimation(UIElement.OpacityProperty, fadeInDescTxt);
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 300,
                Duration = TimeSpan.FromSeconds(0.15)
            };

            animation.Completed += (s, a) =>
            {
                SettingsFrame.Visibility = Visibility.Collapsed;
                isAnimating = false;
                isSettingsPageOpen = false;
            };

            SettingsFrameTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void SlideOutBigSettings()
        {
            isSetAnimating = true;
            SettingsBigFrame.Navigate(new Pages.MorePages.SettingsBig());
            SettingsBigFrame.Visibility = Visibility.Visible;
            SettingsBigFrame.IsHitTestVisible = true;
            isBigSettingsPageOpen = true;

            DoubleAnimation fadeInDescTxt = new DoubleAnimation
            {
                From = 0,
                To = 0.6,
                Duration = new Duration(TimeSpan.FromSeconds(0.15))
            };
            SettingsBigThing.BeginAnimation(UIElement.OpacityProperty, fadeInDescTxt);

            var animation = new DoubleAnimation
            {
                From = 300,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.15)
            };

            animation.Completed += (s, a) =>
            {
                isSetAnimating = false;
            };

            SettingsFrameBigTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        public void SlideInBigSettings()
        {
            isSetAnimating = true;
            SettingsBigFrame.IsHitTestVisible = false;
            isBigSettingsPageOpen = false;

            DoubleAnimation fadeInDescTxt = new DoubleAnimation
            {
                From = 0.6,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.15))
            };
            SettingsBigThing.BeginAnimation(UIElement.OpacityProperty, fadeInDescTxt);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 300,
                Duration = TimeSpan.FromSeconds(0.15)
            };

            animation.Completed += (s, a) =>
            {
                SettingsBigFrame.Visibility = Visibility.Collapsed;
                isSetAnimating = false;
                isBigSettingsPageOpen = false;
            };

            SettingsFrameBigTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void SettingsPageBig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isBigSettingsPageOpen && !SettingsBigFrame.IsMouseOver)
            {
                SlideInBigSettings();
            }
        }

        public void OpenMainPage()
        {
            PageNavigation.Visibility = Visibility.Collapsed;
            ToiletImg.Visibility = Visibility.Collapsed;
            MainFrameFrameLol.Navigate(new Pages.MainFrame());
            SettingsPanel.Opacity = 1;
            LoadUser();
            BackgroundOpacity.Opacity = 0;
            ZenithLogoLogin.Opacity = 0;
            SettingsBtn.Opacity = 1;
            TasksBtn.Opacity = 1;
            SettingsBtn.Visibility = Visibility.Visible;
            TasksBtn.Visibility = Visibility.Visible;
            UserText.Visibility = Visibility.Visible;
            UserSettingsButton.Visibility = Visibility.Visible;
            UserSettingsButton.IsHitTestVisible = true;
        }

        public async void IsSettingsOpen()
        {
            if (isBigSettingsPageOpen)
            {
                SlideInBigSettings();
                isBigSettingsPageOpen = false;
                await Task.Delay(1300);
            }
            else
            {
                // don't have to do anything here!
            }
        }

        public void ByeSettings()
        {
            SettingsBigFrame.Visibility = Visibility.Collapsed;
            SettingsBigBtn.Visibility = Visibility.Collapsed;
            SettingsBigBtn.IsHitTestVisible = false;
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            if (isAccountFrameOpen)
            {
                AccountUtilsFrame.Content = null;
                isAccountFrameOpen = false;
            }
            else
            {
                AccountUtilsFrame.Navigate(new Pages.MorePages.AccountSettings());
                isAccountFrameOpen = true;
            }
        }

        private void UserSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isAccountFrameOpen && !AccountUtilsFrame.IsMouseOver)
            {
                AccountUtilsFrame.Content = null;
                isAccountFrameOpen = false;
            }
        }

        private void DownloadsPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isDownloadsPageOpen && !DownloadsFrame.IsMouseOver)
            {
                DownloadsFrame.IsHitTestVisible = false;
                SlideInDownloads();
                isDownloadsPageOpen = false;
            }
        }

        private void SettingsPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSettingsPageOpen && !SettingsFrame.IsMouseOver)
            {
                SettingsFrame.IsHitTestVisible = false;
                SlideInSettings();
                isSettingsPageOpen = false;
            }
        }
        public void NavMain(string Username, string loginMethod)
        {
            //PageNavigation.Navigate(new Pages.MorePages.AuthenticationCode());

            if (StringSharing.LoginReason == "true")
            {
                PageNavigation.Navigate(new Pages.OnBoarding.MessageBoxShow());
            }
            else
            {
                PageNavigation.Navigate(new Pages.WelcomeIn(StringSharing.Username, StringSharing.LoginMethod));
            }
        }

        public void OpenLoadingBar()
        {
            LoadingBarFrame.Navigate(new Pages.MorePages.LoadingBar());
            LoadingBarFrame.RenderTransform = new TranslateTransform(0, 8);
            ((TranslateTransform)LoadingBarFrame.RenderTransform).BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(0, -8, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } });
        }

        public void CloseLoadingBar()
        {
            if (LoadingBarFrame.RenderTransform is TranslateTransform translateTransform)
            {
                translateTransform.BeginAnimation(
                    TranslateTransform.YProperty,
                    new DoubleAnimation(-8, 8, TimeSpan.FromMilliseconds(150))
                    {
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    }
                );
            }
            else
            {
                LoadingBarFrame.RenderTransform = new TranslateTransform(-8, -8);
                ((TranslateTransform)LoadingBarFrame.RenderTransform).BeginAnimation(
                    TranslateTransform.YProperty,
                    new DoubleAnimation(-8, 8, TimeSpan.FromMilliseconds(150))
                    {
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    }
                );
            }
        }

        public void OpenMessageBox()
        {
            PageNavigation.Navigate(new Pages.OnBoarding.MessageBoxShow());
        }

        public void OpenWelcomeIn()
        {
            PageNavigation.Navigate(new Pages.WelcomeIn(StringSharing.Username, StringSharing.LoginMethod));
        }

        public void OpenAllat()
        {
            PageNavigation.Navigate(new Pages.OnBoardingAnimations());
        }

        public void AnimLoad()
        {
            PageNavigation.Navigate(new Pages.OnBoardingLoadingScreen());
        }

        public void CheckUpdates()
        {
            PageNavigation.Navigate(new Security.CheckUpdates());
        }

        private void DragMoveDock(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Point mousePos = PointToScreen(e.GetPosition(this));
                double ratio = e.GetPosition(this).X / this.ActualWidth;
                double curHeight = this.ActualHeight;
                MainGrid.Margin = new Thickness(0);
                this.WindowState = WindowState.Normal;
                double restoredWidth = this.Width;
                this.Left = mousePos.X - (restoredWidth * ratio);
                this.Top = mousePos.Y - (e.GetPosition(this).Y);
                Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                {
                    this.DragMove();
                }));
            }
            else
            {
                this.DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ////bool isFNRunning = StartGameFN._FortniteEACProcess != null;
            //bool isRLRunning = StartGameRL._RLProcess != null;

            //if (!isRLRunning) //!isFNRunning && removed temporarily.
            //{
            //    Application.Current.Shutdown();
            //}
            //else
            //{
            //    ShutdownNotice();
            //}

            // commented while i fix issues.

            //this.Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            CloseBtn.Margin = new Thickness(0);
            MaximizeBtn.Margin = new Thickness(0, 0, 48, 0);
            RestoreBtn.Margin = new Thickness(0, 0, 48, 0);
            MinimizeBtn.Margin = new Thickness(0, 0, 76, 0);
            MaximizeBtn.Opacity = 0;
            MaximizeBtn.IsHitTestVisible = false;
            RestoreBtn.Opacity = 1;
            RestoreBtn.IsHitTestVisible = true;
            this.WindowState = WindowState.Maximized;
            MainGrid.Margin = new Thickness(8);
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            CloseBtn.Margin = new Thickness(0,7,7,0);
            MaximizeBtn.Margin = new Thickness(0,7,56,0);
            RestoreBtn.Margin = new Thickness(0,7,56,0);
            MinimizeBtn.Margin = new Thickness(0,7,85,0);
            MaximizeBtn.Opacity = 1;
            MaximizeBtn.IsHitTestVisible = true;
            RestoreBtn.Opacity = 0;
            RestoreBtn.IsHitTestVisible = false;
            this.WindowState = WindowState.Normal;
            MainGrid.Margin = new Thickness(0);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ShutdownNotice()
        {
            ShutdownFrame.Opacity = 1;
            ShutdownFrame.IsHitTestVisible = true;
            ShutdownFrame.Navigate(new Pages.MorePages.ConfirmShutdown());
            ShutdownFrame.IsHitTestVisible = true;
        }

        public void CloseAllProcesses()
        {
            Logger.Log("Fortnite process exited before shutdown.");
            FakeACTempFN._FNLauncherProcess?.Kill();
            FakeACTempFN._FNAntiCheatProcess?.Kill();

            Logger.Log("Rocket League process exited before shutdown.");
            //StartGameRL._RLProcess?.Kill();
            KillExistingProcesses();

            System.Windows.Application.Current.Shutdown();
        }

        private static void KillExistingProcesses()
        {
            string[] processNames = {
                "EpicGamesLauncher",
                "FortniteLauncher",
                "FortniteClient-Win64-Shipping_BE",
                "FortniteClient-Win64-Shipping_EAC",
                "FortniteClient-Win64-Shipping",
                "EasyAntiCheat_EOS",
                "EpicWebHelper",
                "EAC_Oasis"
            };

            foreach (string processName in processNames)
            {
                try
                {
                    Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName));
                    foreach (Process process in processes)
                    {
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to kill existing process, {processName}: {ex.Message}");
                }
            }
        }

        public void OpenRetryOrClose()
        {
            string openReason = "FailOnLogin";
            PageNavigation.Navigate(new Pages.RetryOrClose(openReason));
        }

        public async void OpenHttpFail()
        {
            HttpFailure++;
            PageNavigation.Content = null;
            await Task.Delay(500);
            string openReason = "HttpFailure";
            PageNavigation.Navigate(new Pages.RetryOrClose(openReason));
        }

        public void OpenLoginPage()
        {
            PageNavigation.Navigate(new Pages.LoginPage());
        }

        public void CloseShutdown()
        {
            ShutdownFrame.Opacity = 0;
            ShutdownFrame.IsHitTestVisible = false;
        }

        public string DeleteFortnitePathFromRegistry()
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
                            key.DeleteValue(gamePathKey, false);
                            Logger.Log("User deleted game path.");
                        }
                    }
                }

                if (retrievedPath == null)
                {
                    Logger.Log("Failed to delete game path.");
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