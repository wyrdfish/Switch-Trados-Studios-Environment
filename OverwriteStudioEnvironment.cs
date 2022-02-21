using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using System.Linq;

namespace Switch_Trados_Studios_Environment
{
    class OverwriteStudioEnvironment
    {
        Dictionary<int, string> studioBuildTypeDictionary = new Dictionary<int, string>();
        Dictionary<int, string> environmentDictionary = new Dictionary<int, string>();
        private int nrOfStudioBuilds;

        private static string _machineUser = Environment.UserName;
        private const string LanguageCloudMachineTranslation = "LanguageCloudMachineTranslation.bin";
        private const string TradosStudioConfigFile = "SDLTradosStudio.exe.config";
        private const string BestMatchServiceSettingsNode = "/configuration/BestMatchServiceSettings";
        private const string BestMatchServiceUrlsConfigNode = "/configuration/BestMatchServiceUrlsConfig";
        private const string SdlInstallRegistryPath = @"SOFTWARE\WOW6432Node\SDL";
        private const string TradosInstallRegistryPath = @"SOFTWARE\WOW6432Node\Trados";

        public List<string> GetListOfInstalledTradosStudioBuilds()
        {
            RegistryKey SdlInstallRegistry = Registry.LocalMachine.OpenSubKey(SdlInstallRegistryPath);
            RegistryKey TradosInstallRegistry = Registry.LocalMachine.OpenSubKey(TradosInstallRegistryPath);
            List<string> listOfInstalledBuilds = new List<string>();
            if (SdlInstallRegistry != null)
            {
                var installedStudioBuilds = SdlInstallRegistry.GetSubKeyNames()
               .Where<string>(val => (val.Contains("Studio16") || val.Contains("Studio17")) && !val.Contains("License")).ToList();
                listOfInstalledBuilds.AddRange(installedStudioBuilds);
            }
            if (TradosInstallRegistry != null)
            {
                var installedStudioBuilds = TradosInstallRegistry.GetSubKeyNames()
               .Where<string>(val => (val.Contains("Studio16") || val.Contains("Studio17")) && !val.Contains("License")).ToList();
                listOfInstalledBuilds.AddRange(installedStudioBuilds);
            }
            return listOfInstalledBuilds;
        }

        public string ChooseCustomLocation()
        {
            var dialog = new OpenFileDialog();
            try
            {
                //dialog.IsFolderPicker = true;
                //dialog.EnsurePathExists = true;
                dialog.ShowDialog();
                return dialog.FileName;
            }
            catch 
            {
                return string.Empty;
            }
        }

        public void SwitchStudiosLcEnvironment(int selectedStudioTypIndexe, int environment)
        {
            XmlDocument bestMatchServicesSettings = new XmlDocument();
            bestMatchServicesSettings.Load(GetPathToTheSpecificEnvironmentFile(environment));
            var newBestMatchServiceSettings = bestMatchServicesSettings.SelectSingleNode(BestMatchServiceSettingsNode);
            var newBestMatchServiceUrlConfig = bestMatchServicesSettings.SelectSingleNode(BestMatchServiceUrlsConfigNode);

            XmlDocument tradosStudioConfigFile = new XmlDocument();
            string lcEnvironmentFilePath = nrOfStudioBuilds <= selectedStudioTypIndexe ? 
                studioBuildTypeDictionary[selectedStudioTypIndexe] : 
                Path.Combine(GetInstallPathToTheSpecificStudioBuild(selectedStudioTypIndexe), TradosStudioConfigFile);
            
            tradosStudioConfigFile.Load(lcEnvironmentFilePath);
            var oldBestMatchServiceSettings = tradosStudioConfigFile.SelectSingleNode(BestMatchServiceSettingsNode);
            var oldBestMatchServiceUrlConfig = tradosStudioConfigFile.SelectSingleNode(BestMatchServiceUrlsConfigNode);

            var configuration = oldBestMatchServiceSettings.ParentNode;

            configuration.InsertBefore(tradosStudioConfigFile.ImportNode(newBestMatchServiceSettings, true), oldBestMatchServiceSettings);
            configuration.RemoveChild(oldBestMatchServiceSettings);

            configuration.InsertBefore(tradosStudioConfigFile.ImportNode(newBestMatchServiceUrlConfig, true), oldBestMatchServiceUrlConfig);
            configuration.RemoveChild(oldBestMatchServiceUrlConfig);

            tradosStudioConfigFile.Save(lcEnvironmentFilePath);
        }

        public string GetEnvironmentFolder()
        {
            string environmentFilesLocation = Path.Combine(Environment.CurrentDirectory, @"Environment Files\");
            return environmentFilesLocation;
        }

        public string GetPathToTheSpecificEnvironmentFile(int environment)
        {
            string environmentFolderPath = GetEnvironmentFolder();
            return Path.Combine(environmentFolderPath, environmentDictionary[environment]);
        }

        public string GetInstallPathToTheSpecificStudioBuild(int studioBuildType)
        {
            string StudioInstallRegistryPath = studioBuildTypeDictionary[studioBuildType].Contains("17") ? TradosInstallRegistryPath : SdlInstallRegistryPath;
            string buildRegistryKeyPath = Path.Combine(StudioInstallRegistryPath, studioBuildTypeDictionary[studioBuildType]);
            RegistryKey SdlInstallRegistry = Registry.LocalMachine.OpenSubKey(buildRegistryKeyPath);
            var installLocation = SdlInstallRegistry.GetValue("InstallLocation");
            return (string)installLocation;
        }

        public void PopulateStudioDictionaries()
        {
            var installedStudioBuilds = GetListOfInstalledTradosStudioBuilds();
            int Key = 0;
            foreach(string build in installedStudioBuilds)
            {
                studioBuildTypeDictionary.Add(Key, build);
                Key++;
            }
            nrOfStudioBuilds = studioBuildTypeDictionary.Count;
        }

        public bool AddCustomBuildPathToDictionary(string customPath)
        {
            bool valueExists = studioBuildTypeDictionary.Values.Contains<string>(customPath);
            if (!valueExists)
            {
                int Key = studioBuildTypeDictionary.Count;
                studioBuildTypeDictionary.Add(Key, customPath);
                return true;
            }
            return false;
        }

        public void PopulateEnvironmentDictionaries()
        {
            string environmentFolderPath = GetEnvironmentFolder();
            var environmentConfigFiles = Directory.GetFiles(environmentFolderPath);

            for (int position = 0; position < environmentConfigFiles.Count(); position++)
            {
                FileInfo fileInfo = new FileInfo(environmentConfigFiles[position]);
                environmentDictionary.Add(position, fileInfo.Name);
            }
        }

        public void CloseStudio(Process[] studioProcesses)
        {
            foreach (Process proc in studioProcesses)
            {
                proc.Kill();
            }
        }

        public Process[] GetStudioProcesses()
        {
            return Process.GetProcessesByName("SDLTradosStudio");
        }

        public void DeleteLanguageCloudLoggedInCofig(int studioBuildType)
        {
            string buildType = nrOfStudioBuilds <= studioBuildType ? "Studio17" : studioBuildTypeDictionary[studioBuildType];
            string loginFileLocation = $@"C:\Users\{_machineUser}\AppData\Roaming\Trados\Trados Studio\{buildType}\{LanguageCloudMachineTranslation}";
            try
            {
                FileInfo fileInfo = new FileInfo(loginFileLocation);
                fileInfo.Delete();
            }
            finally { };
        }
    }
}
