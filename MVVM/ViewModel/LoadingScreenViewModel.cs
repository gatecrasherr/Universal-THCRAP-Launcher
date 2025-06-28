using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Universal_THCRAP_Launcher.Core;
using Universal_THCRAP_Launcher.MVVM.Model;
using Universal_THCRAP_Launcher.MVVM.ViewModel.Service;
using MessageBox = System.Windows.MessageBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace Universal_THCRAP_Launcher.MVVM.ViewModel
{
    class LoadingScreenViewModel : ObservableObject
    {
        private string _loadingText = "Loading...";
        private bool _isLoading = true;
        private double _opacity = 1.0;
        public bool foundConfig = true;

        private LoadModel _loadModel = new LoadModel();
        private DownloadModel _downloadModel = new DownloadModel();
        private ConfigModel _configModel = new ConfigModel();

        public string StatusText
        {
            get => _loadingText;
            set => SetProperty(ref _loadingText, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, value);
        }

        public string VersionNumber
        {
            get
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                if (version.Build != 0)
                    return $"THCRAP Launcher {version.Major}.{version.Minor}.{version.Build}";
                return $"THCRAP Launcher {version.Major}.{version.Minor}";
            }
        }

        public async Task InitializeAsync()
        {
            StatusText = "Finding the UTL config folder...";

            if (_loadModel.ConfigFolderCheck() == false)
            {
                StatusText = "Creating config folder...";

                _configModel.CreateConfig();
            }

            if (_loadModel.SanityCheck() == false)
            {
                DialogService _dialogService = DialogService.Instance;

                StatusText = "Missing THCRAP installation!";

                var result = _dialogService.ShowInstallationChoiceDialog();

                if (result == InstallationChoiceWindow.DialogueResult.DownloadLatest)
                {
                    StatusText = "Downloading THCRAP...";

                    await _downloadModel.DownloadLatest();
                } 
                else if (result == InstallationChoiceWindow.DialogueResult.PickExisting)
                {
                    StatusText = "Select the THCRAP directory...";

                    string selectedPath = selectDirectory();
                }
            }

            StatusText = "Loading content...";

            if(_loadModel.ConfigCheck() == false)
            {
                // We do this to convert games.js to config.json
                foundConfig = false;
            }

            StatusText = "Ready!";
        }

        public async Task FadeOutAsync()
        {
            for (double i = 1.0; i >= 0.0; i -= 0.05)
            {
                Opacity = i;
                await Task.Delay(30);
            }
            IsLoading = false;
        }

        private string selectDirectory()
        {
            using (var directoryPick = new CommonOpenFileDialog())
            {
                directoryPick.IsFolderPicker = true;
                directoryPick.Title = "Select THCRAP Directory";
                directoryPick.RestoreDirectory = true;

                if (directoryPick.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedPath = directoryPick.FileName;

                    return selectedPath;
                }
                else
                {
                    MessageBox.Show("Bro. Select something. #ChillaxBrah #PickSomethingBrah");
                    System.Windows.Application.Current.Shutdown();
                }

                return null;
            }
        }
    }
}
