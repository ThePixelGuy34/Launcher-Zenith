using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using UML.Services;

namespace UML.Class.LaunchLogic
{
    public class StartGameFN
    {
        private static Process _FortniteProcess;

        public static void Launch(string fortnitePath, string ExeArgs, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Logger.Log("Invalid launch arguments, this shouldn't be possible, contact owner. Error Code: WR-1005");
                MessageBox.Show("Invalid launch arguments, this shouldn't be possible, contact owner. Error Code: WR-1005");
                return;
            }

            if (string.IsNullOrEmpty(fortnitePath))
            {
                throw new ArgumentException("Fortnite executable path is not set.");
            }

            string exePath = Path.Combine(fortnitePath, "FortniteGame\\Binaries\\Win64", "FortniteClient-Win64-Shipping.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"-AUTH_LOGIN={username} -AUTH_PASSWORD={password} -AUTH_TYPE=epic {ExeArgs}",
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = false
            };

            try
            {
                _FortniteProcess = Process.Start(startInfo);
                Logger.Log("Fortnite is launching...");

                if (_FortniteProcess != null)
                {
                    _FortniteProcess.WaitForInputIdle();
                    _FortniteProcess.EnableRaisingEvents = true;
                    _FortniteProcess.Exited += OnFortniteExit;

                    int processId = _FortniteProcess.Id;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error launching Fortnite: " + ex.Message);
            }
        }

        public static void OnFortniteExit(object sender, EventArgs e)
        {
            if (_FortniteProcess != null && _FortniteProcess.HasExited)
            {
                Logger.Log("Fortnite process exited.");
                _FortniteProcess = null;
            }

            FakeACTempFN._FNLauncherProcess?.Kill();
            FakeACTempFN._FNAntiCheatProcess?.Kill();
        }
    }
}
