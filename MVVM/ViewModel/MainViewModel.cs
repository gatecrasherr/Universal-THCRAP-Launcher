using System;
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

namespace Universal_THCRAP_Launcher.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        GameModel gameModel = new GameModel();
        private LoadingScreenViewModel _loadingScreenViewModel;
        private bool _contentLoaded = false;

        private string _selectedGameTitle;
        private ImageSource _selectedGameIcon;
        private bool _isGameSelected;
        private string _selectedConfig;
        private string _selectedPath;
        private string _selectedID;

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
            LaunchGameCommand = new RelayCommand(async () => await LaunchGameAsync(), CanLaunchGame);
            OpenDirectory = new RelayCommand(OpenLocation);
            LoadingViewModel = new LoadingScreenViewModel();
            Application.Current.Dispatcher.BeginInvoke(new Action(async () => await InitializeAsync()));
        }

        private async Task LaunchGameAsync()
        {
            if(!CanLaunchGame())
                return;

            var attemptLaunchNotification = NotificationService.Instance.ShowNotification("Launching...", $"Attempting to launch Instance...", NotificationViewModel.NotificationType.Info, 500);

            gameModel.StartGame(SelectedID, SelectedConfig);

            // Thinking about this again, this isn't the best way to do it. What if the loader takes too long to update?
            bool found = await gameModel.ScanForExeAsync(SelectedID, TimeSpan.FromSeconds(5));

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

            if (System.IO.Directory.Exists(selectedGamePath))
                Process.Start("explorer.exe", selectedGamePath);
            else
                Console.WriteLine("Directory not found.");
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
            LoadAppContent();

            await LoadingViewModel.InitializeAsync();

            ContentLoaded = true;
        }

        private void LoadAppContent()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                string filePath = "\\thcrap\\config\\games.js";
                string strDef = "\\thcrap\\repos\\thpatch\\lang_en\\stringdefs.js";
                string custom = "custom";

                int num = gameModel.GameCount(filePath, custom);
                List<string> paths = gameModel.GamePath(filePath, custom);
                List<string> id = gameModel.GameID(filePath, custom);
                List<string> configs = gameModel.ConfigList();

                SelectedConfig = gameModel.ConfigList()[0];

                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < num; i++)
                    {
                        string iconPath = gameModel.ResolvePath(paths[i]);

                        Debug.WriteLine(configs[i]);

                        var gameItem = new GameItem
                        {
                            DisplayTitle = gameModel.IDtoFullName(id[i], strDef),
                            GameId = id[i],
                            GamePath = gameModel.ResolvePath(paths[i]),
                            DisplayIcon = gameModel.GameImage(iconPath, id[i])
                        };

                        gameItem.GameSelected += GameItem_GameSelected;

                        gameItem.MouseDoubleClick += GameItem_MouseDoubleClick;

                        mainWindow.UncategorizedWrapPanel.Children.Add(gameItem);
                    }
                });
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
            if (sender is GameItem gameItem)
                await LaunchGameAsync();
        }

        public IEnumerable<MenuItem> ConfigMenuItems()
        {
            var menuItems = new List<MenuItem>();
            List<string> configs = gameModel.ConfigList();

            int num = configs.Count;
            Debug.WriteLine($"Number of configs: {num}");

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
                    })
                };

                menuItems.Add(menuItem);
            }

            menuItems.Add(newConfigItem);

            return menuItems;
        }

        // AddGame and Category here...

        // Helper functions

        private void SelectGameItem(GameItem item, GameSelectedEventArgs e)
        {
            item.DisplayIconOpacity = 0.3f;
            item.DisplayBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F485E"));

            SelectedGameTitle = e.GameTitle;
            SelectedGameIcon = e.GameIcon;
            SelectedPath = item.GamePath;
            SelectedID = item.GameId;
            IsGameSelected = true;
        }

        private void ResetGameItem(GameItem item)
        {
            item.DisplayIconOpacity = 0.0f;
            item.DisplayBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"));
        }
    }
}
