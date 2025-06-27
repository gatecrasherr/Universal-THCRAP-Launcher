using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class LoadModel
    {
        public bool SanityCheck()
        {
            if (!directoryExists(AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\"))
                return false;

            return true;
        }

        public bool ConfigFolderCheck()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\config";

            if (!directoryExists(configPath))
                return false;

            return true;
        }

        public bool ConfigCheck()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\config\\UTL\\config\\config.json";

            if (fileExists(configPath))
                if (!emptyJSON(configPath))
                {
                    // doesnt write for some reason, CHECK LATER. NOW IT DOES??
                    Debug.WriteLine("TFW!!!");
                    return true;
                }

            return false;
        }

        // Helper functions

        private bool emptyJSON(string filePath)
        {
            var jObject = JObject.Parse(File.ReadAllText(filePath));
            return !jObject.HasValues;
        }

        private bool directoryExists(string filePath)
        {
            if (Directory.Exists(filePath))
                return true;

            return false;
        }

        private bool fileExists(string filePath)
        {
            if (File.Exists(filePath))
                return true;

            return false;
        }
    }
}
