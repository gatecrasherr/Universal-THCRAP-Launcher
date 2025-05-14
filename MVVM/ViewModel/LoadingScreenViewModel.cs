using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal_THCRAP_Launcher.Core;
using Universal_THCRAP_Launcher.MVVM.Model;

namespace Universal_THCRAP_Launcher.MVVM.ViewModel
{
    class LoadingScreenViewModel : ObservableObject
    {
        private string _loadingText = "Loading...";
        private bool _isLoading = true;
        private double _opacity = 1.0;

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

        public async Task InitializeAsync()
        {
            await Task.Delay(1);
            //await DownloadModel.Main();
            StatusText = "Downloading THCrap...";
            await Task.Delay(1);
            StatusText = "Loading game data...";
            await Task.Delay(1);
            StatusText = "Ready!";

            await FadeOutAsync();
        }

        private async Task FadeOutAsync()
        {
            for (double i = 1.0; i >= 0.0; i -= 0.05)
            {
                Opacity = i;
                await Task.Delay(30);
            }
            IsLoading = false;
        }
    }
}
