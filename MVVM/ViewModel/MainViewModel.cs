using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Universal_THCRAP_Launcher.Core;
using Universal_THCRAP_Launcher.MVVM.Model;
using Universal_THCRAP_Launcher.MVVM.View;
using Universal_THCRAP_Launcher.MVVM.ViewModel.Service;
using Newtonsoft.Json.Linq;
using thcrap_cs_lib;

namespace Universal_THCRAP_Launcher.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private GameModel gameModel = new GameModel();
        private ConfigModel configModel = new ConfigModel();
        private LoadingScreenViewModel _loadingScreenViewModel;
        private DialogService _dialogService = DialogService.Instance;
        private bool _contentLoaded = false;
        private readonly Dictionary<string, WrapPanel> categoryPanels = new Dictionary<string, WrapPanel>();

        private string _selectedGameTitle;
        private ImageSource _selectedGameIcon;
        private bool _isGameSelected;
        private string _selectedConfig;
        private string _selectedPath;
        private string _selectedID;

        private GameItem _selectedGameItem;

        public ICommand LaunchGameCommand { get; }
        public ICommand OpenDirectory { get; }

        public string SelectedGameTitle
        {
            get => _selectedGameTitle;
            set => SetProperty(ref _selectedGameTitle, value);
        }

        public ImageSource SelectedGameIcon
        {
            get => _selectedGameIcon;
            set => SetProperty(ref _selectedGameIcon, value);
        }

        public bool IsGameSelected
        {
            get => _isGameSelected;
            set => SetProperty(ref _isGameSelected, value);
        }

        public string SelectedConfig
        {
            get => _selectedConfig;
            set => SetProperty(ref _selectedConfig, value);
        }

        public string SelectedPath
        {
            get => _selectedPath;
            set => SetProperty(ref _selectedPath, value);
        }

        public string SelectedID
        {
            get => _selectedID;
            set => SetProperty(ref _selectedID, value);
        }

        public LoadingScreenViewModel LoadingViewModel
        {
            get => _loadingScreenViewModel;
            set => SetProperty(ref _loadingScreenViewModel, value);
        }

        public bool ContentLoaded
        {
            get => _contentLoaded;
            set => SetProperty(ref _contentLoaded, value);
        }

        public MainViewModel()
        {
            LoadingViewModel = new LoadingScreenViewModel();
            LaunchGameCommand = new RelayCommand(async () => await LaunchGameAsync(), CanLaunchGame);
            OpenDirectory = new RelayCommand(OpenLocation);
            //Application.Current.Dispatcher.BeginInvoke(new Action(async () => await InitializeAsync()));
        }

        private async Task LaunchGameAsync()
        {

            if (!CanLaunchGame())
                return;

            var attemptLaunchNotification = NotificationService.Instance.ShowNotification("Launching...", $"Attempting to launch Instance...", NotificationViewModel.NotificationType.Info, 500);

            gameModel.StartGame(SelectedID, SelectedConfig);

            // Thinking about this again, this isn't the best way to do it. What if the loader takes too long to update?
            bool found = await gameModel.ScanForExeAsync(SelectedID, TimeSpan.FromSeconds(20));

            // The Delay is only so that the Notification has the time to disappear. Will move it.
            if (found)
            {
                NotificationService.Instance.Destroy(attemptLaunchNotification);
                await Task.Delay(280);
                NotificationService.Instance.ShowNotification("Success!", $"Successfully launched Instance.", NotificationViewModel.NotificationType.Success, 6);
            }
            else
            {
                NotificationService.Instance.Destroy(attemptLaunchNotification);
                await Task.Delay(280);
                NotificationService.Instance.ShowNotification("Error!", $"Failed to launch Instance!", NotificationViewModel.NotificationType.Error, 6);
            }
        }

        private void OpenLocation()
        {
            // For now, I see no use to have a way to delete the FileName.exe if a path ends with it, other than in here.
            string selectedGamePath = GetGamePath(SelectedPath);

            if (Directory.Exists(selectedGamePath))
                Process.Start("explorer.exe", selectedGamePath);
            else
                Console.WriteLine("Directory not found.");
        }

        public void UnselectGame()
        {
            SelectedGameTitle = null;
            SelectedGameIcon = null;
            IsGameSelected = false;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            foreach (var child in mainWindow.MainStackPanel.Children)
            {
                if (child is Expander expander && expander.Content is WrapPanel wrapPanel)
                {
                    foreach (var item in wrapPanel.Children)
                    {
                        if (item is GameItem gameItem)
                        {
                            gameItem.DisplayIconOpacity = 0.0f;
                            gameItem.DisplayBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"));
                        }
                    }
                }
            }
        }

        public async Task InitializeAsync()
        {
            await LoadingViewModel.InitializeAsync();

            LoadingViewModel.FadeOutAsync();

            if (!LoadingViewModel.foundConfig)
                await LoadFromGamesJSAsync();
            else
                await LoadFromConfigAsync();

            ContentLoaded = true;
        }

        private async Task LoadFromConfigAsync()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thcrap", "config", "UTL", "config", "config.json");
                string strDef = "\\thcrap\\repos\\thpatch\\lang_en\\stringdefs.js"; // change this depending on the language perhaps?

                string json = File.ReadAllText(configPath);
                JObject rootObject = JObject.Parse(json);

                foreach (var property in rootObject.Properties())
                {
                    string gameID = property.Name;
                    JObject gameData = property.Value as JObject;

                    string customName = gameData["customName"]?.ToString();
                    string title = !string.IsNullOrEmpty(customName)
                        ? customName
                        : await gameModel.IDtoFullNameAsync(gameID, strDef);
                    string path = gameData["path"]?.ToString() ?? string.Empty;
                    string category = gameData["category"]?.ToString() ?? "Uncategorized";
                    string config = gameData["config"]?.ToString() ?? "ERR";

                    SelectedConfig = config;

                    if (!categoryPanels.ContainsKey(category))
                        AddCategory(category);

                    var gameItem = new GameItem
                    {
                        DisplayTitle = title,
                        GameId = gameID,
                        GamePath = gameModel.ResolvePath(path),
                        DisplayIcon = await Task.Run(() => gameModel.GameImage(path, gameID))
                    };

                    gameItem.GameSelected += GameItem_GameSelected;
                    gameItem.MouseDoubleClick += GameItem_MouseDoubleClick;

                    categoryPanels[category].Children.Add(gameItem);
                }
            }
        }

        private async Task LoadFromGamesJSAsync()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                AddCategory("Uncategorized");

                string filePath = "\\thcrap\\config\\games.js";
                string strDef = "\\thcrap\\repos\\thpatch\\lang_en\\stringdefs.js";
                string custom = "custom";

                int num = await Task.Run(() => gameModel.GameCount(filePath, custom));
                List<string> paths = await Task.Run(() => gameModel.GamePathAsync(filePath, custom));
                List<string> id = await Task.Run(() => gameModel.GameID(filePath, custom));
                List<string> configs = await Task.Run(() => gameModel.ConfigListAsync());

                SelectedConfig = configs[0];

                for (int i = 0; i < num; i++)
                {
                    string currentGamePath = gameModel.ResolvePath(paths[i]);

                    var gameItem = new GameItem
                    {
                        DisplayTitle = await gameModel.IDtoFullNameAsync(id[i], strDef),
                        GameId = id[i],
                        GamePath = currentGamePath,
                        DisplayIcon = await Task.Run(() => gameModel.GameImage(currentGamePath, id[i]))
                    };

                    gameItem.GameSelected += GameItem_GameSelected;
                    gameItem.MouseDoubleClick += GameItem_MouseDoubleClick;

                    gameModel.GameToConfig(gameItem, SelectedConfig, "Uncategorized");

                    categoryPanels["Uncategorized"].Children.Add(gameItem);
                }
            }
        }

        private void GameItem_GameSelected(object sender, GameSelectedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                foreach (var child in mainWindow.MainStackPanel.Children.OfType<Expander>())
                {
                    if (child.Content is WrapPanel wrapPanel)
                    {
                        foreach (var item in wrapPanel.Children.OfType<GameItem>())
                        {
                            if (item.GameId == e.GameId)
                                SelectGameItem(item, e);
                            else
                                ResetGameItem(item);
                        }
                    }
                }
            }
        }

        private async void GameItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await LaunchGameAsync();
        }

        public async Task<IEnumerable<MenuItem>> ConfigMenuItemsAsync()
        {
            var menuItems = new List<MenuItem>();
            List<string> configs = await gameModel.ConfigListAsync();

            int num = configs.Count;

            var newConfigItem = new MenuItem
            {
                Header = "Add a New Config...",
                Command = new RelayCommand(() => MessageBox.Show("New Config"))
            };

            for (int i = 0; i < num; i++)
            {
                int capturedIndex = i;

                var menuItem = new MenuItem
                {
                    Header = configs[capturedIndex],
                    Command = new RelayCommand(() =>
                    {
                        SelectedConfig = configs[capturedIndex];
                        if (!string.IsNullOrEmpty(SelectedID))
                            LastConfigToJSON(SelectedID, SelectedConfig);
                    })
                };

                menuItems.Add(menuItem);
            }

            menuItems.Add(newConfigItem);

            return menuItems;
        }

        // AddGame here...

        public void AddCategory(string categoryName)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                if (categoryPanels.ContainsKey(categoryName))
                    return;

                Expander newExpander = new Expander
                {
                    Header = categoryName,
                    IsExpanded = true,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2E32")),
                    Foreground = Brushes.White
                };

                var newWrapPanel = new WrapPanel
                {
                    Name = $"{categoryName}",
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                newExpander.Style = mainWindow.FindResource("CategoryStyle") as Style;

                newExpander.Content = newWrapPanel;

                mainWindow.MainStackPanel.Children.Add(newExpander);

                categoryPanels[categoryName] = newWrapPanel;

                mainWindow.UpdateWelcomeOverlayVisibility();
            }
        }

        public void RenameInstance(string name)
        {
            if (name == null)
                return;

            if (_selectedGameItem == null)
                return;

            if (string.IsNullOrWhiteSpace(name))
            {
                string oldName = gameModel.RestoreName(SelectedID);
                SelectedGameTitle = oldName;
                _selectedGameItem.DisplayTitle = oldName;
                return;
            }

            SelectedGameTitle = name;
            _selectedGameItem.DisplayTitle = name;

            gameModel.ChangeName(SelectedID, name);
        }

        public void DeleteInstance()
        {
            if (_selectedGameItem == null)
                return;

            string gameId = _selectedGameItem.GameId;

            gameModel.RemoveGame(gameId);
            categoryPanels[GetSelectedGameItemCategory()].Children.Remove(_selectedGameItem);
            UnselectGame();
        }

        // Helper functions

        public string GetSelectedGameItemCategory()
        {
            if (_selectedGameItem == null)
                return null;

            foreach (var kvp in categoryPanels)
            {
                if (kvp.Value.Children.Contains(_selectedGameItem))
                    return kvp.Key;
            }
            return null;
        }

        private void LastConfigToJSON(string gameID, string configName)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thcrap", "config", "UTL", "config", "config.json");

            var configJson = File.ReadAllText(configPath);
            var rootObject = JObject.Parse(configJson);

            if (rootObject[gameID] is JObject gameObj)
            {
                gameObj["config"] = configName;
                File.WriteAllText(configPath, rootObject.ToString());
            }
        }

        private string GetGamePath(string path)
        {
            if (path.EndsWith(".exe"))
            {
                int lastSlashIndex = path.LastIndexOf("/");

                if (lastSlashIndex == -1)
                    lastSlashIndex = path.LastIndexOf("\\");

                path = path.Substring(0, lastSlashIndex);
            }

            path = path.Replace('/', '\\');

            return path;
        }

        private bool CanLaunchGame()
        {
            return IsGameSelected && !string.IsNullOrEmpty(SelectedConfig) && !string.IsNullOrEmpty(SelectedPath) && !string.IsNullOrEmpty(SelectedID);
        }

        private void SelectGameItem(GameItem item, GameSelectedEventArgs e)
        {
            _selectedGameItem = item;

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thcrap", "config", "UTL", "config", "config.json");

            item.DisplayIconOpacity = 0.3f;
            item.DisplayBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F485E"));

            SelectedGameTitle = e.GameTitle;
            SelectedGameIcon = e.GameIcon;
            SelectedPath = item.GamePath;
            SelectedID = item.GameId;
            IsGameSelected = true;

            var configJson = File.ReadAllText(configPath);
            var rootObject = JObject.Parse(configJson);

            if (rootObject[item.GameId] is JObject gameObj)
                SelectedConfig = gameObj["config"]?.ToString() ?? "ERR";
        }

        private void ResetGameItem(GameItem item)
        {
            item.DisplayIconOpacity = 0.0f;
            item.DisplayBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"));
        }
    }
}
