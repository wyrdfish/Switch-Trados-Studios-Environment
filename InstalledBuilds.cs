using Microsoft.Win32;

using System.Collections.Generic;
using System.Linq;


namespace Switch_Trados_Studios_Environment
{
    public class InstalledBuilds
    {
        public List<string> listOfInstalledBuilds = new List<string>();
        public Dictionary<int, string> studioBuildTypeDictionary = new Dictionary<int, string>();
        public int nrOfInstalledBuilds;

        public InstalledBuilds()
        {
            listOfInstalledBuilds = PopulateListOfInstalledBuilds();
            studioBuildTypeDictionary = PopulateStudioDictionaries();
            nrOfInstalledBuilds = studioBuildTypeDictionary.Count();
        }


        private List<string> PopulateListOfInstalledBuilds()
        {
            RegistryKey SdlInstallRegistry = Registry.LocalMachine.OpenSubKey(Constants.SdlInstallRegistryPath);
            RegistryKey TradosInstallRegistry = Registry.LocalMachine.OpenSubKey(Constants.TradosInstallRegistryPath);
            List<string> installedBuilds = new List<string>();
            if (SdlInstallRegistry != null)
            {
                var installedStudioBuilds = SdlInstallRegistry.GetSubKeyNames()
               .Where<string>(val => (val.Contains("Studio16") || val.Contains("Studio17")) && !val.Contains("License")).ToList();
                installedBuilds.AddRange(installedStudioBuilds);
            }
            if (TradosInstallRegistry != null)
            {
                var installedStudioBuilds = TradosInstallRegistry.GetSubKeyNames()
               .Where<string>(val => (val.Contains("Studio16") || val.Contains("Studio17")) && !val.Contains("License")).ToList();
                installedBuilds.AddRange(installedStudioBuilds);
            }
            return installedBuilds;
        }

        private Dictionary<int, string> PopulateStudioDictionaries()
        {
            int Key = 0;
            Dictionary<int, string> buildTypeDictionary = new Dictionary<int, string>();
            foreach (string build in listOfInstalledBuilds)
            {
                buildTypeDictionary.Add(Key, build);
                Key++;
            }
            return buildTypeDictionary;
        }
    }
}
