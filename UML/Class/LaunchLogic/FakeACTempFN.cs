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
    // I don't see any reason to replace this, yes it's Eon but I actually don't give a fuck. The rest is the same, I don't understand what "string t = r" actually is.
    // And if I write that into mine it's just gonna be the same thing.

    public class FakeACTempFN
    {
        public static Process? _FNLauncherProcess;
        public static Process? _FNAntiCheatProcess;

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
                        _FNAntiCheatProcess = Process.Start(FortniteProcess);
                        if (_FNAntiCheatProcess.Id == 0)
                        {
                            MessageBox.Show("Fortnite has failed to start, try again or contact support if the issue still persists.");
                            Logger.Log("AntiCheat failed to start... Aborting game launch. WR-1006");
                        }
                        _FNAntiCheatProcess.FreezeFN();
                    }
                    else
                    {
                        _FNLauncherProcess = Process.Start(FortniteProcess);
                        if (_FNLauncherProcess.Id == 0)
                        {
                            MessageBox.Show("Fortnite has failed to start, try again or contact support if the issue still persists.");
                            Logger.Log("AntiCheat failed to start... Aborting game launch. WR-1007");
                        }
                        _FNLauncherProcess.FreezeFN();
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
