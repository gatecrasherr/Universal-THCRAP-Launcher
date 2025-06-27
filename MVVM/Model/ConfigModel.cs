using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class ConfigModel
    {
        // Move logs to a LogModel.cs later
        private string thConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\config";
        private string thLogsPath = AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\logs";
        private string configPath;
        private string imageCachePath;
        private string logsPath;

        public void CreateConfig()
        {
            configPath = Path.Combine(thConfigPath, "UTL", "config");
            imageCachePath = Path.Combine(thConfigPath, "UTL", "cache");
            logsPath = Path.Combine(thLogsPath, "UTL", "logs");

            try
            {
                Directory.CreateDirectory(configPath);
                File.WriteAllText(Path.Combine(configPath, "config.json"), "{}");
                Directory.CreateDirectory(imageCachePath);
                Directory.CreateDirectory(logsPath);
            }
            catch
            {
                Console.WriteLine("Failed to create config directory.");
            }
        }

        // Move the games.js to config function here?
    }
}
