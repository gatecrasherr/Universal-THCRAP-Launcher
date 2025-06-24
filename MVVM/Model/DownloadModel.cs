using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class DownloadModel
    {
        private readonly string releaseURL = $"https://api.github.com/repos/thpatch/thcrap/releases/latest";
        private readonly string installDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thcrap");

        public async Task DownloadLatest()
        {
            try
            {
                if (!Directory.Exists(installDirectory))
                    Directory.CreateDirectory(installDirectory);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "UTL");

                    string responseBody = await client.GetStringAsync(releaseURL);
                    JObject json = JObject.Parse(responseBody);
                    var assets = json["assets"];

                    string downloadURL = null;

                    foreach (var asset in assets)
                    {
                        string assetName = asset["name"].ToString();

                        if (assetName.Equals("thcrap.zip", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadURL = asset["browser_download_url"].ToString();
                            break;
                        }
                    }

                    if (downloadURL != null)
                    {
                        string zipFilePath = Path.Combine(installDirectory, "thcrap.zip");

                        var response = await client.GetAsync(downloadURL);
                        response.EnsureSuccessStatusCode();

                        using (var fs = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await response.Content.CopyToAsync(fs);
                        }

                        ZipFile.ExtractToDirectory(zipFilePath, installDirectory);
                        File.Delete(zipFilePath);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Damn");
            }
        }
    }
}
