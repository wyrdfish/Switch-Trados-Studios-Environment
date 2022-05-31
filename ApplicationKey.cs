using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Switch_Trados_Studios_Environment
{
    public class ApplicationKey
    {
        public Dictionary<string, string> ApplicationKeys = new Dictionary<string, string>()
        {
            {"Studio17" ,"n7JUV2wjQeKOVB0muXqL0Q%3D%3D"},
            {"Studio16" ,"n6JUV2wjQeKOVB0muXqL0Q%3D%3D"}
        };

        public void ChangeApplicationKey(string location, XmlNode configuration)
        {
            if (location.Contains("Studio17"))
            {
                configuration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value = ApplicationKeys["Studio17"];
            }
            else
            {
                configuration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value = ApplicationKeys["Studio16"];
            }
        }
    }
}
