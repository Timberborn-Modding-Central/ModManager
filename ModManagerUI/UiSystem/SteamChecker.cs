using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ModManagerUI.UiSystem
{
    public abstract class SteamChecker
    {
        public static bool IsRestartCompatible()
        {
            if (Application.platform != RuntimePlatform.WindowsPlayer) 
                return false;
            var currentProcess = Process.GetCurrentProcess();
            var parentProcess = GetParentProcess(currentProcess.Id);
            return parentProcess != null && parentProcess.ProcessName.ToLower() == "steam";
        }

        private static Process? GetParentProcess(int id)
        {
            var process = Process.GetProcessById(id);
            return GetParentProcess(process.Handle);
        }

        private static Process? GetParentProcess(IntPtr handle)
        {
            try
            {
                GetParentProcess(handle, out var processId);
                return Process.GetProcessById(Convert.ToInt32(processId));
            }
            catch
            {
                return null;
            }
        }
        
        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, out int returnLength);

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        private static void GetParentProcess(IntPtr handle, out uint processId)
        {
            var pbi = new PROCESS_BASIC_INFORMATION();
            var status = NtQueryInformationProcess(handle,  0, ref pbi, Marshal.SizeOf(pbi), out _);
            if (status !=  0)
                throw new Win32Exception(status);
            processId = (uint)pbi.InheritedFromUniqueProcessId;
        }
    }
}