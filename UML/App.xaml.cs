using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using System.IO;
using System.Windows;
using UML.Services;
using System;

namespace UML
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MutexName = "ZenithApp";
        private const string PipeName = "ZenithPipe";
        private Mutex _appMutex;
        private bool _isFirstInstance;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _appMutex = new Mutex(true, MutexName, out _isFirstInstance);
            if (!_isFirstInstance)
            {
                if (e.Args.Length > 0)
                {
                    await SendUriToExistingInstance(e.Args[0]);
                }
                Shutdown();
                return;
            }

            StartPipeServer();
            Logger.Log("PipeServer started.");

            if (e.Args.Length > 0)
            {
                Logger.Log("Processing callback!");
                ProcessInitialUri(e.Args[0]);
            }
            else
            {
                Logger.Log("Trying to login...");
                await Task.Delay(500);
                NavigateToLogin();
            }
        }

        private void LoadCauseWeDontKnowWhatTheFucksGoingOn()
        {
            if (MainWindow is MainWindow mainWindow)
            {
                BringWindowToFront();
            }
        }

        private void ProcessInitialUri(string uri)
        {
            Dispatcher.Invoke(() =>
            {
                if (MainWindow is MainWindow mainWindow)
                {
                    mainWindow.ProcessUriScheme(uri);
                    //mainWindow.NavMain();
                    BringWindowToFront();
                }
            });
        }

        private async Task SendUriToExistingInstance(string uri)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    await client.ConnectAsync(1000);
                    using (var writer = new StreamWriter(client))
                    {
                        await writer.WriteLineAsync(uri);
                    }
                }
            }
            catch { /* Handle errors */ }
        }

        private void StartPipeServer()
        {
            Task.Run(async () =>
            {
                while (_isFirstInstance)
                {
                    using (var server = new NamedPipeServerStream(PipeName, PipeDirection.In))
                    {
                        try
                        {
                            Logger.Log("Waiting for connection...");

                            await server.WaitForConnectionAsync();

                            Logger.Log("Connection established.");

                            using (var reader = new StreamReader(server))
                            {
                                var uri = await reader.ReadLineAsync();
                                Logger.Log($"Received Info {uri}");

                                if (!string.IsNullOrEmpty(uri))
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        if (MainWindow is MainWindow mainWindow)
                                        {
                                            Logger.Log("Processing callback in MainWindow.");
                                            mainWindow.ProcessUriScheme(uri);
                                            //mainWindow.NavigateMain();
                                            BringWindowToFront();
                                        }
                                    });
                                }
                                else
                                {
                                    Logger.Log("Received no callback data.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"An error occured trying to connect via PipeServer. {ex.Message}");
                        }
                    }
                }
            });
        }

        private void BringWindowToFront()
        {
            if (MainWindow != null)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                    MainWindow.WindowState = WindowState.Normal;

                MainWindow.Activate();
                MainWindow.Topmost = true;
                MainWindow.Topmost = false;
                MainWindow.Focus();
            }
        }

        private async void NavigateToLogin()
        {
            if (MainWindow is MainWindow mainWindow)
            {
                await Task.Delay(500);
                mainWindow.CheckUpdates();
            }
        }

        private void SetDefaultLanguage()
        {
            string lang = UpdateINI.ReadValue("Settings", "Language");

            if (string.IsNullOrEmpty(lang))
            {
                lang = "en-US";
                UpdateINI.WriteToConfig("Settings", "Language", lang);
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            LoadResourceDictionary(lang);
        }

        private void LoadResourceDictionary(string lang)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            ResourceDictionary resDict = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Lang/{lang}.xaml", UriKind.Absolute)
            };
            Application.Current.Resources.MergedDictionaries.Add(resDict);
        }

        private void ChangeLanguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            Application.Current.Resources.MergedDictionaries.Clear();

            try
            {
                string langFilePath = $"/Lang/{lang}.xaml";
                ResourceDictionary langDict = new ResourceDictionary
                {
                    Source = new Uri(langFilePath, UriKind.RelativeOrAbsolute)
                };

                Application.Current.Resources.MergedDictionaries.Add(langDict);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading language file: {ex.Message}");
            }
        }

    }

}
