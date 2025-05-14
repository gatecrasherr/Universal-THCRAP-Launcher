using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class DownloadModel
    {
        private static readonly HttpClient _htppClient = new HttpClient();

        static string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
        static string repoUrl = "https://github.com/thpatch/thcrap/releases/latest";
        private static readonly HttpClient _httpClient = new HttpClient();

    }
}
