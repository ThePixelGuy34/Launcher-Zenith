using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UML.Services;
using Discord.Rest;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Policy;
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
using UML.Pages;
using System.IO;
using System.Net;
using UML.Class;
using System.Security.Cryptography;
using UML.Class.LaunchLogic;

namespace UML.Class
{
    internal class HttpFunctions
    {
        public static async Task<UML.Class.DontBreakPls.GameConfig> LoadMainBackground(string contentId, string id1, string id2, string id3, string id4)
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
                        var mainPage = appContent?.games?.FirstOrDefault(g => g.id == contentId);   
                        if (mainPage != null)
                        {
                            Logger.Log($"{json} This is ur JSON");
                            return mainPage;
                        }
                        else
                        {
                            Logger.Log("Malformed JSON, contact isaac on Discord. - LM-0001");
                        }
                    }
                    else
                    {
                        HttpFailure();
                        Logger.Log($"Failed to load contentpages: {(int)response.StatusCode} - LM-0002");
                    }
                }
                catch (TaskCanceledException)
                {
                    HttpFailure();
                    Logger.Log("Connection timeout when loading contentpages. - LM-0003");
                }
                catch (HttpRequestException)
                {
                    HttpFailure();
                    Logger.Log("Unable to connect to Zenith services when loading contentpages. - LM-0004");
                }
                catch (JsonException)
                {
                    Logger.Log("Contentpages JSON is invalid, contact isaac or zippywippy. - LM-0005");
                }
                catch (Exception)
                {
                    HttpFailure();
                    Logger.Log("An error occurred when loading contentpages, is the backend down? - LM-0006");
                }
            }

            return null;
        }

        private static void HttpFailure()
        {
            if (Application.Current?.Dispatcher != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.OpenHttpFail();
                    }
                    else
                    {
                        Logger.Log("MainWindow is null; cannot call OpenHttpFail.");
                    }
                });
            }
        }

    }
}
