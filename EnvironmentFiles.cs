using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Trados_Studios_Environment
{
    internal class EnvironmentFiles
    {
        public Dictionary<int, string> environmentDictionary = new Dictionary<int, string>();
        public string environmentFolderPath;

        internal EnvironmentFiles()
        {
            
            environmentFolderPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), @"Environment Files\");
            PopulateEnvironmentDictionaries();
        }

        public string GetPathToTheSpecificEnvironmentFile(int environment)
        {
            return Path.Combine(environmentFolderPath, environmentDictionary[environment]);
        }

        public string AddCustomLocation()
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


        private void PopulateEnvironmentDictionaries()
        {
            var environmentConfigFiles = Directory.GetFiles(environmentFolderPath);

            for (int position = 0; position < environmentConfigFiles.Count(); position++)
            {
                FileInfo fileInfo = new FileInfo(environmentConfigFiles[position]);
                environmentDictionary.Add(position, fileInfo.Name);
            }
        }
    }
}
