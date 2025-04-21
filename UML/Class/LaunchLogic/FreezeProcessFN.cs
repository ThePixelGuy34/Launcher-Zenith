using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UML.Class.LaunchLogic
{
    public static class FreezeProcessFN
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr hThread);

        public static void FreezeFN(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var Thread = OpenThread(2, false, (uint)thread.Id);
                if (Thread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(Thread);
            }
        }
    }
}
