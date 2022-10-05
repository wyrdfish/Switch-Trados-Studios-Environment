using System;
using System.IO;

namespace Switch_Trados_Studios_Environment
{
    internal class LanguageCloudConfig
    {

        private static string _machineUserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        InstalledBuilds installedBuilds = new InstalledBuilds();
        DirectoryInfo appdataDirectory = new DirectoryInfo($@"{_machineUserPath}\AppData\Roaming\");

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


            if (Directory.Exists($@"{appdataDirectory}{languageCloudMachineTranslationLocation}"))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo($@"{appdataDirectory}{languageCloudMachineTranslationLocation}\{Constants.LanguageCloudMachineTranslation}");
                    fileInfo.Delete();
                }
                catch (Exception) { };
            }
        }
    }
}
