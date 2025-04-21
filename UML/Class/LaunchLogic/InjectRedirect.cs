using System;
using System.Runtime.InteropServices;
using System.Text;
using UML.Services;

namespace UML.Class.LaunchLogic
{
    internal class InjectRedirect
    {
        public bool InjectDLL(int processId, string dllPath)
        {
            try
            {
                IntPtr hProcess = OpenProcess(ProcessAccessFlags.All, false, processId);
                if (hProcess == IntPtr.Zero)
                {
                    Logger.Log("OpenProcess failed.");
                    return false;
                }

                byte[] dllPathBytes = Encoding.ASCII.GetBytes(dllPath + "\0");
                IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)dllPathBytes.Length,
                    AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
                if (allocMemAddress == IntPtr.Zero)
                {
                    Logger.Log("VirtualAllocEx failed.");
                    CloseHandle(hProcess);
                    return false;
                }

                if (!WriteProcessMemory(hProcess, allocMemAddress, dllPathBytes, (uint)dllPathBytes.Length, out _))
                {
                    Logger.Log("WriteProcessMemory failed.");
                    CloseHandle(hProcess);
                    return false;
                }

                IntPtr hKernel32 = GetModuleHandle("kernel32.dll");
                IntPtr hLoadLibraryA = GetProcAddress(hKernel32, "LoadLibraryA");

                IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, hLoadLibraryA, allocMemAddress, 0, IntPtr.Zero);
                if (hThread == IntPtr.Zero)
                {
                    Logger.Log("CreateRemoteThread failed.");
                    CloseHandle(hProcess);
                    return false;
                }

                WaitForSingleObject(hThread, 5000);
                CloseHandle(hThread);
                CloseHandle(hProcess);

                Logger.Log("DLL Injected Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"DLL Injection error: {ex.Message}");
                return false;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF
        }

        [Flags]
        public enum AllocationType : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000
        }

        [Flags]
        public enum MemoryProtection : uint
        {
            ReadWrite = 0x04,
            ExecuteReadWrite = 0x40
        }
    }
}
