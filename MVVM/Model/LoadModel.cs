using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Would make more sense to put this inside the ConfigModel, but I would rather keep it as clean as possible
        public bool ConfigCheck()
        {
            string roamingPath = AppDomain.CurrentDomain.BaseDirectory + "\\thcrap\\config";

            if (!directoryExists(roamingPath + "\\thcrap\\config\\"))
                return false;

            return true;
        }

        // Helper functions

        private bool directoryExists(string filePath)
        {
            if (System.IO.Directory.Exists(filePath))
                return true;
            else
                return false;
        }

        private bool fileExists(string filePath)
        {
            if (System.IO.File.Exists(filePath))
                return true;
            else
                return false;
        }
    }
}
