using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class LoadModel
    {
        public bool thcrapExists(string filePath)
        {
            if (System.IO.File.Exists(filePath))
                return true;
            else
                return false;
        }


    }
}
