using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Universal_THCRAP_Launcher.MVVM.View.Dialogs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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

        public string NameGameDialog()
        {
            var dialog = new NameGameDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
                return dialog.InputText;

            return null;
        }

        public bool DeleteGameDialog()
        {
            var dialog = new DeleteGameDialog();
            if (_mainWindow != null && _mainWindow.IsVisible)
            {
                dialog.Owner = _mainWindow;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            bool? result = dialog.ShowDialog();
            return result == true;
        }

        public void ChangeCategoryDialog()
        {
            var dialog = new ChangeCategoryDialog();
            //dialog.InitializeCategories(categories);
            if (_mainWindow != null && _mainWindow.IsVisible)
            {
                dialog.Owner = _mainWindow;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }
    }
}
