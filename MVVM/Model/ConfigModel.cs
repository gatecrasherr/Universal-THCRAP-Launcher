using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class ConfigModel
    {
        public void CreateConfig()
        {
            // Change to the thcrap folder
            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configPath = System.IO.Path.Combine(roamingPath, "UTL", "config");
            string logsPath = System.IO.Path.Combine(roamingPath, "UTL", "logs");

            try
            {
                System.IO.Directory.CreateDirectory(configPath);
                System.IO.Directory.CreateDirectory(logsPath);
            }
            catch
            {
                Console.WriteLine("Failed to create config directory.");
            }
        }
    }
}
