using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using UML.Services;

namespace UML.Class.LaunchLogic
{
    public class StartGameRL
    {
        private static Process _RLProcess;

        public static void Launch(string TAPath, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Logger.Log("Invalid launch arguments, this shouldn't be possible, contact owner. Error Code: WR-1005");
                MessageBox.Show("Invalid launch arguments, this shouldn't be possible, contact owner. Error Code: WR-1005");
                return;
            }

            if (string.IsNullOrEmpty(TAPath))
            {
                throw new ArgumentException("Rocket League executable path is not set.");
            }

            UpdateAccountName(username);
            string exePath = Path.Combine(TAPath, "Binaries\\Win32", "RocketLeague.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"-AUTH-LOGIN=unused -AUTH_PASSWORD={password} -AUTH_TYPE=exchangecode -epicapp=Sugar -epicenv=Prod -EpicPortal -epicusername={username} -epicuserid=yapping -epiclocale=en -epicsandboxid=yappingv2",
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = false
            };

            
            WebClient RedirectDownload = new WebClient();
            string downloadPath = Path.Combine(TAPath, "Binaries\\Win32", "steam_api.dll");
            RedirectDownload.DownloadFile("https://zenith-api.zippywippy.online/zenith/api/v1/launcher/tagame", downloadPath);

            try
            {
                _RLProcess = Process.Start(startInfo);
                Logger.Log("Rocket League is launching...");

                if (_RLProcess != null)
                {
                    _RLProcess.WaitForInputIdle();
                    _RLProcess.EnableRaisingEvents = true;
                    _RLProcess.Exited += OnRLExit;
                }

                //await Task.Delay(6000);
                //var patchInjector = new InjectRedirect();
                //patchInjector.InjectDLL(_RLProcess.Id, downloadPath);
            }
            catch (Exception ex)
            {
                Logger.Log("Error launching Rocket League: " + ex.Message);
            }
        }

        private static void UpdateAccountName(string username)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string goldbergPath = Path.Combine(appDataPath, "Goldberg SteamEmu Saves/settings");
                string accountFilePath = Path.Combine(goldbergPath, "account_name.txt");

                if (File.Exists(accountFilePath))
                {
                    File.WriteAllText(accountFilePath, username);
                    Logger.Log("Updated account_name.txt with username: " + username);
                }
                else
                {
                    Logger.Log("account_name.txt not found at: " + accountFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error updating account_name.txt: " + ex.Message);
            }
        }

        public static void OnRLExit(object sender, EventArgs e)
        {
            if (_RLProcess != null && _RLProcess.HasExited)
            {
                Logger.Log("Rocket League process exited.");
                _RLProcess = null;
            }

            FakeACTempRL._RLLauncherProcess?.Kill();
            FakeACTempRL._RLAntiCheatProcess?.Kill();
        }
    }
}
