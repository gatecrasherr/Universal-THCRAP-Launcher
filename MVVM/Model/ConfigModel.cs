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
            // Move logs to a LogModel.cs later
            string thConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\config";
            string thLogsPath = AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\logs";
            string configPath = System.IO.Path.Combine(thConfigPath, "UTL", "config");
            string imageCachePath = System.IO.Path.Combine(thConfigPath, "UTL", "cache");
            string logsPath = System.IO.Path.Combine(thLogsPath, "UTL", "logs");

            try
            {
                System.IO.Directory.CreateDirectory(configPath);
                System.IO.File.WriteAllText(System.IO.Path.Combine(configPath, "config.json"), "{}");
                System.IO.Directory.CreateDirectory(imageCachePath);
                System.IO.Directory.CreateDirectory(logsPath);
            }
            catch
            {
                Console.WriteLine("Failed to create config directory.");
            }
        }
    }
}
