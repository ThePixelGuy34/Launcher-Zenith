using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UML.Services;

namespace UML.Class
{
    internal class InjectDLL
    {
        public bool Inject(int processId, string dllPath)
        {
            try
            {
                IntPtr hProcess = OpenProcess(ProcessAccessFlags.All, false, processId);
                if (hProcess == IntPtr.Zero) return false;

                IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
                if (allocMemAddress == IntPtr.Zero) return false;

                byte[] dllPathBytes = System.Text.Encoding.ASCII.GetBytes(dllPath);
                WriteProcessMemory(hProcess, allocMemAddress, dllPathBytes, (uint)dllPathBytes.Length, out _);

                IntPtr hKernel32 = GetModuleHandle("kernel32.dll");
                IntPtr hLoadLibraryA = GetProcAddress(hKernel32, "LoadLibraryA");

                IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, hLoadLibraryA, allocMemAddress, 0, IntPtr.Zero);
                if (hThread == IntPtr.Zero) return false;

                CloseHandle(hThread);
                CloseHandle(hProcess);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"DLL injection error: {ex.Message}");
                return false;
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

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
            ExecuteReadWrite = 0x40
        }
    }
}
