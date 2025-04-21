using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using UML.Class;
using UML.Security;
using UML.Services;

namespace UML.Security
{
    public partial class CheckUpdates : Page
    {
        string url = "https://zenith-api.zippywippy.online/zenith/api/v1/launcher/status";

        public CheckUpdates()
        {
            InitializeComponent();
            EstablishConnection();
            Class.DiscordRPCManager.SetPresence("Online", "Checking for updates!");
            Logger.Log("Contacting services to ensure the launcher is on the correct version.");
        }

        private async void EstablishConnection()
        {
            await Task.Delay(1500);
            await CheckEndpointStatusAsync();
        }

        private async void OpenRetryOrCloseWindow()
        {
            Frame4ShittyPage.Visibility = Visibility.Visible;
            Frame4ShittyPage.Navigate(new Pages.LoadingScreen());

            await Task.Delay(2500);
            Frame4ShittyPage.Visibility = Visibility.Collapsed;
        }

        // doesn't actually check for updates.
        private async Task CheckEndpointStatusAsync()
        {
            var security = new CantCrackThis();
            using (HttpClient client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            }))
            {
                client.Timeout = TimeSpan.FromSeconds(10.0);
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"ZenithApp/{AppVersion.Current}");
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    string zenSig = security.GenerateAuthToken();
                    request.Headers.Add("ZenSig", zenSig);
                    var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                    var responseTokens = response.Headers.GetValues("ZenSig");
                    string responseToken = responseTokens.FirstOrDefault();
                    if (string.IsNullOrEmpty(responseToken) || !security.ValidateAuthToken(responseToken))
                    {
                        Logger.Log("Missing or invalid signature.");
                        StatusText.Text = "Failed to connect to Zenith services, please try again later.";
                        Loading.Visibility = Visibility.Collapsed;
                        await Task.Delay(1000);
                        OpenRetryOrClosePage();
                        return;
                    }

                    if (response.StatusCode == (System.Net.HttpStatusCode)200)
                    {
                        Logger.Log("Launcher is up to date - 200");
                        StatusText.Text = "You're up to date!";
                        await Task.Delay(500);
                        ShowLoginPage();
                    }
                    else if (response.StatusCode == (System.Net.HttpStatusCode)401)
                    {
                        Logger.Log("Missing or invalid signature. - 401");
                        StatusText.Text = "Failed.";
                        await Task.Delay(1000);
                        OpenRetryOrClosePage();
                    }
                    else if (response.StatusCode == (System.Net.HttpStatusCode)426)
                    {
                        Logger.Log("Launcher has been deprecated - Failed - 426");
                        StatusText.Text = "This version of the launcher has been deprecated, please update.";
                        Loading.Visibility = Visibility.Collapsed;
                        StartUpdate();
                    }
                    else if (response.StatusCode == (System.Net.HttpStatusCode)503)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        try
                        {
                            using (JsonDocument doc = JsonDocument.Parse(responseContent))
                            {
                                JsonElement root = doc.RootElement;

                                if (root.TryGetProperty("status", out JsonElement statusElement))
                                {
                                    string status = statusElement.GetString();

                                    if (status == "maintenance")
                                    {
                                        Logger.Log("Servers are undergoing maintenance, refused to continue - 502.");
                                        StatusText.Text = "Zenith Services are currently undergoing maintenance, please try again later.";
                                    }
                                    else if (status == "offline")
                                    {
                                        Logger.Log("Servers are deemed offline in server config - 502.");
                                        StatusText.Text = "Zenith Services are currently offline, please try again later.";
                                    }
                                    else
                                    {
                                        Logger.Log("Servers may be offline, failed to read value - 502.");
                                        StatusText.Text = "Zenith Services are temporarily unavailable, please try again later.";
                                    }
                                }
                                else
                                {
                                    Logger.Log("Backend sent 502 status code, but failed to read value - 502.");
                                    StatusText.Text = "An error has occured, please try again later.";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("Backend sent 502 status code, but the launcher hit a catch trying to read it - 502.");
                            StatusText.Text = "Zenith Services are temporarily unavailable, please try again later.";
                        }

                        Loading.Visibility = Visibility.Collapsed;
                        await Task.Delay(4000);
                        OpenRetryOrClosePage();
                    }
                    else
                    {
                        StatusText.Text = "Failed to connect to Zenith services, please try again later.";
                        Loading.Visibility = Visibility.Collapsed;
                        await Task.Delay(1000);
                        OpenRetryOrClosePage();
                    }
                }
                catch (TaskCanceledException)
                {
                    StatusText.Text = "Connection timeout. Please check your internet connection.";
                    await Task.Delay(1000);
                    OpenRetryOrClosePage();
                }
                catch (HttpRequestException)
                {
                    StatusText.Text = "Unable to connect to Zenith services. Please try again later.";
                    await Task.Delay(1000);
                    OpenRetryOrClosePage();
                }
                catch (Exception)
                {
                    StatusText.Text = "An error occurred. Please try again.";
                    OpenRetryOrClosePage();
                }
            }
        }

        private void StartUpdate()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenUpdateUI();
            }
        }

        private void ShowLoginPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.OpenLoginPage();
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

        private bool IsNewVersionAvailable(string currentVersion, string latestVersion)
        {
            return string.Compare(currentVersion, latestVersion) < 0;
        }

        private void OpenRetryOrClosePage()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenRetryOrClose();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
            //await Task.Delay(1300);
        }
    }
}
