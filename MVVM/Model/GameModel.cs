﻿using Newtonsoft.Json.Linq;
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

namespace Universal_THCRAP_Launcher.MVVM.Model
{
    class GameModel
    {
        private static string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private string _thcrapFolder = _basePath + "\\thcrap";
        private string _thcrapConfigs = _basePath + "\\thcrap\\config";
        private string _thcrapLoader = _basePath + "\\thcrap\\bin\\thcrap_loader.exe";

        private List<string> _excludedConfigs = new List<string>
        {
            "games.js",
            "config.js"
        };

        public async Task CheckSetupAsync()
        {
            GameModel game = new GameModel();

            /*if (!Directory.Exists(_basePath + "\\thcrap"))
            {
                await Task.Run(() => {
                    Directory.CreateDirectory(_basePath + "\\thcrap");
                    Directory.CreateDirectory(_basePath + "\\thcrap\\config");
                    //new DownloadModel().DownloadLatestReleaseZipAsync("thpatch", "thcrap", _basePath + "\\thcrap").Wait();
                });
            }*/

            game._thcrapFolder = _basePath + "\\thcrap";
            game._thcrapConfigs = _basePath + "\\thcrap\\config";
            game._thcrapLoader = _basePath + "\\thcrap\\bin\\thcrap_loader.exe";

        }

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
                        var allProcesses = Process.GetProcesses();

                        if (allProcesses.Any(p => p.ProcessName.Equals(exeName, StringComparison.OrdinalIgnoreCase)))
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

                if (!File.Exists(resolvedPath))
                    throw new FileNotFoundException("File not found", resolvedPath);

                return resolvedPath;
            }
        }

        // Should get the stringDef location automatically, remove later
        public async Task<string> IDtoFullNameAsync(string gameID, string stringDefsPath)
        {
            JObject jsonObject = await JsonReaderAsync(_basePath + stringDefsPath);

            try
            {
                if(jsonObject.ContainsKey(gameID))
                    return jsonObject[gameID].ToString();

                return "Unknown Game Name";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public ImageSource GameImage(string filePath, string gameID)
        {
            ImageSource gameIcon;

            string updatedPath = filePath.Replace("vpatch.exe", $"{gameID}.exe");

            if (gameID == "th06")
                updatedPath = updatedPath.Replace($"{gameID}.exe", "/東方紅魔郷.exe");

            Icon icon = Icon.ExtractAssociatedIcon(updatedPath);

            gameIcon = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return gameIcon;
        }

        //Helper Function
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
