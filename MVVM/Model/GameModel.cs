using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Universal_THCRAP_Launcher.MVVM.View;

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class GameModel
    {
        private static string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private string _thcrapFolder = _basePath + "\\thcrap";
        private string _thcrapConfigs = _basePath + "\\thcrap\\config";
        private string _thcrapLoader = _basePath + "\\thcrap\\bin\\thcrap_loader.exe";
        private string _UTLConfig = _basePath + "\\thcrap\\config\\UTL\\config\\config.json";

        private List<string> _excludedConfigs = new List<string>
        {
            "games.js",
            "config.js"
        };

        public void StartGame(string game, string config)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = $"{_thcrapLoader}",
                Arguments = $"{config + ".js"} {game}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            Debug.WriteLine(processInfo.Arguments);

            Process.Start(processInfo);
        }

        public async Task<List<string>> ConfigListAsync()
        {
            return await Task.Run(() => Directory.GetFiles(_thcrapConfigs).Select(Path.GetFileName).Where(file => !_excludedConfigs.Contains(file)).Select(file => Path.GetFileNameWithoutExtension(file)).ToList());
        }

        public async Task<bool> ScanForExeAsync(string exeName, TimeSpan timeout)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var scanTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (Process.GetProcesses().Any(p => p.ProcessName.Equals(exeName, StringComparison.OrdinalIgnoreCase)))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    await Task.Delay(100, token);
                }

                return false;
            }, token);

            var completedTask = await Task.WhenAny(scanTask, Task.Delay(timeout, token));

            if (completedTask == scanTask)
            {
                cts.Cancel();
                return await scanTask;
            }
            else
            {
                cts.Cancel();
                return false;
            }
        }

        public string ResolvePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                string combinedPath = Path.Combine(_thcrapFolder, path);
                string resolvedPath = Path.GetFullPath(combinedPath);

                if (!Directory.Exists(resolvedPath))
                    throw new DirectoryNotFoundException("Directory not found");

                return resolvedPath;
            }
        }

        public async Task<string> IDtoFullNameAsync(string gameID, string stringDefsPath)
        {
            JObject jsonObject = await JsonReaderAsync(_basePath + stringDefsPath);

            try
            {
                if(jsonObject.ContainsKey(gameID))
                    return jsonObject[gameID].ToString();

                return "Unknown Game Name";
            }
            catch
            {
                return "Unknown Game Name";
            }
        }

        public async Task<int> GameCount(string filePath, string custom)
        {
            JObject jsonObject = await JsonReaderAsync(_basePath + filePath);

            return jsonObject.Count - jsonObject.Properties().Count(prop => prop.Name.Contains(custom));
        }

        public async Task<List<string>> GameID(string filePath, string custom)
        {
            JObject jsonObject = await JsonReaderAsync(_basePath + filePath);

            return jsonObject.Properties().Where(prop => !prop.Name.Contains(custom)).Select(prop => prop.Name).ToList();
        }

        public async Task<List<string>> GamePathAsync(string filePath, string custom)
        {
            JObject jsonObject = await JsonReaderAsync(_basePath + filePath);

            return jsonObject.Properties().Where(prop => !prop.Value.ToString().Contains(custom)).Select(prop => prop.Value.ToString()).ToList();
        }

        // CreateBitmapSourceFromHIcon is a very expensive call,
        // should be cached to reduce the number of calls to it
        public ImageSource GameImage(string filePath, string gameID)
        {
            string possiblePngPath = Path.Combine(_basePath, "thcrap", "config", "UTL", "cache", $"{gameID}.png");

            if (File.Exists(possiblePngPath))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(possiblePngPath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                if (image.CanFreeze)
                    image.Freeze();

                return image;
            }

            string updatedPath = filePath.Replace("vpatch.exe", $"{gameID}.exe");

            if (gameID == "th06")
                updatedPath = updatedPath.Replace($"{gameID}.exe", "/東方紅魔郷.exe");

            Icon icon = Icon.ExtractAssociatedIcon(updatedPath);

            if (icon == null)
                return null;

            ImageSource gameIcon = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            if (gameIcon.CanFreeze)
                gameIcon.Freeze();

            icon.Dispose();

            CacheImage(gameIcon, gameID);

            return gameIcon;
        }

        public void GameToConfig(GameItem game, string config, string category)
        {
            var existingJson = File.ReadAllText(_UTLConfig);
            JObject rootObject = JObject.Parse(existingJson);

            var gameObject = new JObject
            {
                ["gameTitle"] = game.DisplayTitle,
                ["customName"] = null,
                ["gamePath"] = game.GamePath,
                ["config"] = config,
                ["category"] = category
            };

            rootObject[game.GameId] = gameObject;

            File.WriteAllText(_UTLConfig, rootObject.ToString(Formatting.Indented));
        }

        // Helper Function

        private static void CacheImage(ImageSource img, string gameID)
        {
            string savePath = Path.Combine(_basePath, "thcrap", "config", "UTL", "cache", $"{gameID}.png");

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img as BitmapSource));

            using (var fileStream = new FileStream(savePath, FileMode.Create))
                encoder.Save(fileStream);
        }

        private async Task<JObject> JsonReaderAsync(string filePath)
        {
            try
            {
                using (var reader = new StreamReader (filePath))
                {
                    string json = await reader.ReadToEndAsync();
                    return JObject.Parse(json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
