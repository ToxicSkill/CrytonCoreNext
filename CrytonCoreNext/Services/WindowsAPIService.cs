using Microsoft.Win32;
using System;

namespace CrytonCoreNext.Services
{
    public static class WindowsAPIService
    {
        public static int GetWindowsBuild()
        {
            var registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var currentBuild = registryKey.GetValue("CurrentBuild").ToString();
            if (Int32.TryParse(currentBuild, out int result))
            {
                return result;
            }
            else
            {
                return -1;
            }
        }
    }
}