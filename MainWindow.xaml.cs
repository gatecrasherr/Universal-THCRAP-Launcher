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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaxHeight = SystemParameters.WorkArea.Height + 8;
            MaxWidth = SystemParameters.WorkArea.Width + 12;
            NotificationService.Instance.Initialize(NotificationContainer);
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

        private void InstancesButton_Click(object sender, RoutedEventArgs e)
        {
            //Button button = sender as Button;

            //if(button?.ContextMenu != null)
            if (sender is Button button)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void InstancesButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ConfigContextMenu.Items.Clear();

                AddDynamicMenuItems();

                ConfigContextMenu.PlacementTarget = button;
                ConfigContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                ConfigContextMenu.IsOpen = true;
            }
        }

        private void Config_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        private void AddDynamicMenuItems()
        {
            if (DataContext is MainViewModel viewModel)
            {
                ConfigContextMenu.Items.Clear();

                var menuItems = viewModel.ConfigMenuItems();

                foreach (var menuItem in menuItems)
                    ConfigContextMenu.Items.Add(menuItem);
            }
        }

        private void AutoInstance_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.ShowNotification("Auto Instance", "Not implemented yet.", NotificationViewModel.ToastType.Info, 10);
        }

        private void settingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settingsWindow.ShowDialog();
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
