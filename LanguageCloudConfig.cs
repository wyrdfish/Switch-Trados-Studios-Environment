using System;
using System.IO;

namespace Switch_Trados_Studios_Environment
{
    internal class LanguageCloudConfig
    {

        private static string _machineUserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        InstalledBuilds installedBuilds = new InstalledBuilds();

        public void DeleteLanguageCloudConfig(int studioBuildType)
        {
            string buildType;
            if (installedBuilds.nrOfInstalledBuilds > studioBuildType)
            {
                buildType = installedBuilds.studioBuildTypeDictionary[studioBuildType];
            }
            else
            {
                buildType = "Studio17";
            }
            string languageCloudMachineTranslationLocation = buildType.Contains("17") ? "Trados\\Trados Studio" : "SDL\\SDL Trados Studio";

            try
            {
                string loginFileLocation = $"{_machineUserPath}\\AppData\\Roaming\\{languageCloudMachineTranslationLocation}\\{buildType}\\{Constants.LanguageCloudMachineTranslation}";
                FileInfo fileInfo = new FileInfo(loginFileLocation);
                fileInfo.Delete();
            }
            finally { };
        }
    }
}
