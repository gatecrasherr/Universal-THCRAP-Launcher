using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Universal_THCRAP_Launcher.MVVM.View;
using Universal_THCRAP_Launcher.MVVM.ViewModel;
using Universal_THCRAP_Launcher.MVVM.ViewModel.Service;

namespace Universal_THCRAP_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isConfigOpen = false;
        private bool _isInstanceOpen = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaxHeight = SystemParameters.WorkArea.Height + 8;
            MaxWidth = SystemParameters.WorkArea.Width + 12;
            NotificationService.Instance.Initialize(NotificationContainer);
            DialogService.Instance.Initialize(this);

            

            if (DataContext is MainViewModel vm)
            {
                vm.InitializeAsync();
            }
        }

        private void StackMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element)
            {
                if (element.DataContext is GameItem)
                    return;
            }

            if (DataContext is MainViewModel viewModel)
                viewModel.UnselectGame();
        }

        private void PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void InstancesButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (_isInstanceOpen)
                {
                    button.ContextMenu.IsOpen = false;
                    _isInstanceOpen = false;
                    return;
                }

                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
                _isInstanceOpen = true;

                button.ContextMenu.Closed += (s, args) =>
                {
                    _isInstanceOpen = false;
                };
            }
        }

        private void InstancesButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        private async void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (_isConfigOpen)
                {
                    ConfigContextMenu.IsOpen = false;
                    _isConfigOpen = false;
                    return;
                }

                ConfigContextMenu.Items.Clear();

                await AddDynamicMenuItemsAsync();

                ConfigContextMenu.PlacementTarget = button;
                ConfigContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                ConfigContextMenu.IsOpen = true;
                _isConfigOpen = true;

                ConfigContextMenu.Closed += (s, args) =>
                {
                    _isConfigOpen = false;
                };
            }
        }

        private void RenameInstance(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                string newName = DialogService.Instance.NameGameDialog();
                viewModel.RenameInstance(newName);
            }
        }

        private void ChangeCategory(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                DialogService.Instance.ChangeCategoryDialog();
            }
        }

        private void DeleteInstance(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                bool toDelete = DialogService.Instance.DeleteGameDialog();
                viewModel.DeleteInstance();
            }
        }

        private void Config_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        private async Task AddDynamicMenuItemsAsync()
        {
            if (DataContext is MainViewModel viewModel)
            {
                ConfigContextMenu.Items.Clear();

                var menuItems = await viewModel.ConfigMenuItemsAsync();

                foreach (var menuItem in menuItems)
                    ConfigContextMenu.Items.Add(menuItem);
            }
        }

        public void UpdateWelcomeOverlayVisibility()
        {
            bool hasExpanders = MainStackPanel.Children.OfType<Expander>().Any();

            WelcomeOverlay.Visibility = hasExpanders ? Visibility.Collapsed : Visibility.Visible;
        }

        // Navbar item click handler (temporary, most will be in a ViewModel)

        private void RefreshInstance_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.ShowNotification("Refresh Instance", "Not implemented yet.", NotificationViewModel.NotificationType.Info, 10);
        }

        private void AutoInstance_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.ShowNotification("Auto Instance", "Not implemented yet.", NotificationViewModel.NotificationType.Info, 10);
        }

        private void ManualInstance_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.ShowNotification("Manual Instance", "Not implemented yet.", NotificationViewModel.NotificationType.Info, 10);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            settingsWindow.ShowDialog();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.ShowNotification("Help", "Not implemented yet.", NotificationViewModel.NotificationType.Info, 10);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.ShowNotification("About", "Not implemented yet.", NotificationViewModel.NotificationType.Info, 10);
        }

        // Window behavior

        private void Window_LeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                BorderThickness = new Thickness(0);
            }
            else
            {
                WindowState = WindowState.Maximized;
                BorderThickness = new Thickness(4,4,4,0);
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
