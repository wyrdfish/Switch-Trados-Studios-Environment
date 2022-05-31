using Microsoft.Win32;
using System.IO;


namespace Switch_Trados_Studios_Environment
{
    internal class InstallPath
    {
        public string installLocation;

        public InstallPath(string buildName)
        {
            if(buildName.Contains("Program Files (x86)"))
            {
                installLocation = buildName;
            }
            else
            {
                string StudioInstallRegistryPath = buildName.Contains("17") ? Constants.TradosInstallRegistryPath : Constants.SdlInstallRegistryPath;
                string buildRegistryKeyPath = Path.Combine(StudioInstallRegistryPath, buildName);
                RegistryKey SdlInstallRegistry = Registry.LocalMachine.OpenSubKey(buildRegistryKeyPath);
                installLocation = (string)SdlInstallRegistry.GetValue("InstallLocation");
            }
        }
    }
}
