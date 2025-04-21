using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using System.Windows.Media.Animation;
using Newtonsoft.Json.Linq;
using System.Text;
using UML.Services;

namespace UML.Pages
{
    public partial class LoginPage : Page
    {
        private string Token { get; set; }
        private string RefreshToken { get; set; }
        private string AccountId { get; set; }
        private string DisplayName { get; set; }

        public LoginPage()
        {
            InitializeComponent();
            SignInBtn.IsEnabled = false;
            EmailBox.TextChanged += ValidateInputs;
            PasswordBox.TextChanged += ValidateInputs;
            OnStartup();
        }

        private async void OnStartup()
        {
            Class.DiscordRPCManager.SetPresence("Online", "Signing into the launcher!");
            EnsureSettingsClosed();
            FadeInEverything();
            await AuthenticateUser();
        }

        private void ValidateInputs(object sender, EventArgs e)
        {
            SignInBtn.IsEnabled = IsValidEmail(EmailBox.Text) && PasswordBox.Text.Length >= 6;
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }

        private async void SignInEmail_Click(object sender, RoutedEventArgs e)
        {
            //TempErrorText.Opacity = 0;
            if (string.IsNullOrWhiteSpace(EmailBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                DisplayError("Invalid or missing credentials.");
                Logger.Log("Sign in cancelled.");
                return;
            }
            else
            {
                SignInBtn.IsEnabled = false;
                SignInBtn.Visibility = Visibility.Collapsed;
                await AuthenticateUser();
            }
        }

        private async Task AuthenticateUser()
        {
            if (Token is null)
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Fortnite/++Fortnite+Release-10.41 Windows/10.0.19045.1.256.64bit");

                string requestBody = $"grant_type=client_credentials&token_type=eg1";
                var content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("https://zenith-api.zippywippy.online/account/api/oauth/token", content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        DisplayError($"Login failed: {response.StatusCode}");
                        Logger.Log("Server: Failed to connect to servers.");
                        return;
                    }

                    var authResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    if (authResponse == null || string.IsNullOrEmpty(authResponse.access_token))
                    {
                        DisplayError("Invalid response from server. Try again later.");
                        Logger.Log("Malformed response from /account/api/oauth/token");
                        return;
                    }

                    Token = authResponse.access_token;
                    RefreshToken = authResponse?.refresh_token ?? "N/A";
                }
                catch (Exception ex)
                {
                    DisplayError($"Exception during login: {ex.Message}");
                }
            }
            else
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Fortnite/++Fortnite+Release-10.41 Windows/10.0.19045.1.256.64bit");

                string requestBody = $"grant_type=password&username={Uri.EscapeDataString(EmailBox.Text)}&password={Uri.EscapeDataString(PasswordBox.Text)}&includePerms=true&token_type=eg1";
                var content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("https://zenith-api.zippywippy.online/account/api/oauth/token", content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        DisplayError($"Login failed: {response.StatusCode}");
                        Logger.Log("Server: Failed to connect to servers.");
                        return;
                    }

                    var authResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    if (authResponse == null || string.IsNullOrEmpty(authResponse.access_token))
                    {
                        DisplayError("Invalid response from server. Try again later.");
                        Logger.Log("Malformed response from /account/api/oauth/token");
                        return;
                    }

                    StringSharing.authToken = PasswordBox.Text;
                    StringSharing.access_token = authResponse.access_token;
                    StringSharing.refresh_token = authResponse?.refresh_token ?? "N/A";
                    StringSharing.account_id = authResponse?.account_id ?? "N/A";
                    StringSharing.displayName = authResponse?.displayName ?? "N/A";

                    StringSharing.LoginMethod = "email";
                    NavigateToMain();
                }
                catch (Exception ex)
                {
                    DisplayError($"Exception during login: {ex.Message}");
                }
            }
        }

        private void NavigateToMain()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavMain(StringSharing.Username, StringSharing.LoginMethod);
            }
        }

        private void DisplayError(string message)
        {
            //TempErrorText.Text = message;
            //TempErrorText.Opacity = 1;
            SignInBtn.IsEnabled = true;
            SignInBtn.Visibility = Visibility.Visible;
        }

        private void FadeInEverything()
        {
            FadeInBackgroundToo();

            DoubleAnimation LoginAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.7))
            };

            LoginGrid.BeginAnimation(UIElement.OpacityProperty, LoginAnimation);
        }

        private async void FadeInBackgroundToo()
        {
            await Task.Delay(500);

            DoubleAnimation BackgroundAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.7))
            };

            //LoginBG.BeginAnimation(UIElement.OpacityProperty, BackgroundAnimation);

            DoubleAnimation BackgroundOpacityAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 0.7,
                Duration = new Duration(TimeSpan.FromSeconds(0.7))
            };

            //LoginBGDarkness.BeginAnimation(UIElement.OpacityProperty, BackgroundOpacityAnimation);
        }

        private void EnsureSettingsClosed()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    if (mainWindow.isSetAnimating) return;

                    if (mainWindow.isBigSettingsPageOpen)
                    {
                        mainWindow.SlideInBigSettings();
                        mainWindow.isBigSettingsPageOpen = false;
                    }
                    else
                    {
                        // don't have to do anything here!
                    }
                }
                else
                {
                    throw new InvalidOperationException("Navigation error.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error. {ex.Message}");
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            webView.Visibility = Visibility.Visible;
            await webView.EnsureCoreWebView2Async();

            if (webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false; // Optional: Disable right-click menu
                webView.CoreWebView2.Settings.AreDevToolsEnabled = true; // Optional: Disable dev tools
            }

            string url = $"https://discord.com/oauth2/authorize?client_id=1339466329010343966&response_type=code&redirect_uri=https%3A%2F%2Fzenith-api.zippywippy.online%2Fzenith%2Fapi%2Fv1%2Foauth%2Fdiscord&scope=identify+email";
            webView.Source = new Uri(url);
        }

        private class LoginResponse
        {
            public string? access_token { get; set; }
            public int expires_in { get; set; }
            public string? expires_at { get; set; }
            public string? token_type { get; set; }
            public string? refresh_token { get; set; }
            public int refresh_expires { get; set; }
            public string? refresh_expires_at { get; set; }
            public string? account_id { get; set; }
            public string? client_id { get; set; }
            public bool internal_client { get; set; }
            public string? client_service { get; set; }
            public string? displayName { get; set; }
            public string? app { get; set; }
            public string? in_app_id { get; set; }
            public string? device_id { get; set; }
        }
    }
}
