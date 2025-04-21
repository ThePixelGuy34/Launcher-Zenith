using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UML.Services;

namespace UML.Class.LaunchLogic
{
    public class FakeACTempRL
    {
        public static Process? _RLLauncherProcess;
        public static Process? _RLAntiCheatProcess;

        public static void Start(string fortnitePath, string FileName, string args = "", string t = "r")
        {
            try
            {
                if (File.Exists(Path.Combine(fortnitePath, "FortniteGame\\Binaries\\Win64\\", FileName)))
                {
                    ProcessStartInfo FortniteProcess = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(fortnitePath, "FortniteGame\\Binaries\\Win64\\", FileName),
                        Arguments = args,
                        CreateNoWindow = true,
                    };

                    if (t == "r")
                    {
                        _RLAntiCheatProcess = Process.Start(FortniteProcess);
                        if (_RLAntiCheatProcess.Id == 0)
                        {
                            MessageBox.Show("Rocket League has failed to start, try again or contact support if the issue still persists.");
                            Logger.Log("AntiCheat failed to start... Aborting game launch. WR-1006");
                        }
                        _RLAntiCheatProcess.FreezeRL();
                    }
                    else
                    {
                        _RLLauncherProcess = Process.Start(FortniteProcess);
                        if (_RLLauncherProcess.Id == 0)
                        {
                            MessageBox.Show("Rocket League has failed to start, try again or contact support if the issue still persists.");
                            Logger.Log("AntiCheat failed to start... Aborting game launch. WR-1007");
                        }
                        _RLLauncherProcess.FreezeRL();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occured, please try and restart your game.");
            }
        }
    }
}
