using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Universal_THCRAP_Launcher.MVVM.ViewModel.Service
{
    public class DialogService : Window
    {
        private static DialogService _instance;
        private Window _mainWindow;

        public static DialogService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DialogService();
                return _instance;
            }
        }

        private DialogService() { }

        public void Initialize(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public InstallationChoiceWindow.DialogueResult ShowInstallationChoiceDialog()
        {
            var dialog = new InstallationChoiceWindow();

            if (_mainWindow != null && _mainWindow.IsVisible)
            {
                dialog.Owner = _mainWindow;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}
