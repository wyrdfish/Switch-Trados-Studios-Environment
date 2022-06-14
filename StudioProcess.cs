using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Trados_Studios_Environment
{
    internal class StudioProcess
    {
        public void CloseStudio(Process[] studioProcesses)
        {
            foreach (Process proc in studioProcesses)
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }
        }
        public Process[] GetStudioProcesses()
        {
            var runningProcesses = Process.GetProcessesByName("SDLTradosStudio");
            return runningProcesses.Where(p => !p.HasExited).ToArray();
        }
    }
}
