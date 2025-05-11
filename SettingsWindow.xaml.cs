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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Universal_THCRAP_Launcher
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Brush _selectedColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, (Duration)TimeSpan.FromSeconds(0.15));
            this.BeginAnimation(UIElement.OpacityProperty, animation);

            if (this.FindName("ButtonContainer") is StackPanel buttonContainer)
            {
                var defaultButton = buttonContainer.Children.OfType<Button>().FirstOrDefault();

                if (defaultButton != null)
                {
                    defaultButton.Background = _selectedColor;
                    defaultButton.Foreground = Brushes.Black;
                    defaultButton.Tag = true;
                }
            }
        }

        private void ConfigTabButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                if (clickedButton.Parent is Panel parentPanel)
                {
                    foreach (var child in parentPanel.Children)
                    {
                        if (child is Button button)
                        {
                            button.Background = Brushes.Transparent;
                            button.Foreground = Brushes.White;
                            button.Tag = false;
                        }
                    }
                }

                clickedButton.Background = _selectedColor;
                clickedButton.Foreground = Brushes.Black;
                clickedButton.Tag = true;
            }
        }


        private void Window_LeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
