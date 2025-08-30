using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Universal_THCRAP_Launcher.Core;

namespace Universal_THCRAP_Launcher.MVVM.View.Dialogs
{
    /// <summary>
    /// Interaction logic for NameGameDialog.xaml
    /// </summary>
    public partial class NameGameDialog : Window
    {
        public string InputText => DialogResult == true ? test.Text : null;

        public string GameName { get; private set; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }

        public NameGameDialog()
        {
            InitializeComponent();

            CancelCommand = new RelayCommand(() => Cancel_Click(null, null));
            SaveCommand = new RelayCommand(() => Save_Click(null, null));

            Loaded += NameGameDialog_Loaded;
        }

        private void NameGameDialog_Loaded(object sender, RoutedEventArgs e)
        {
            test.Focus();
            NameTextBox_TextChanged(null, null);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string input = test.Text;

            if (ContainsIllegalJsonChars(input))
                return;

            GameName = input;
            DialogResult = true;
            Close();
        }

        private void Window_LeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = test.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                WarningText.Text = "Empty names reset the custom name.";
                WarningText.Foreground = new SolidColorBrush(Color.FromRgb(66, 165, 245));
                WarningText.Visibility = Visibility.Visible;
            }
            else if (ContainsIllegalJsonChars(text))
            {
                WarningText.Text = "Name contains illegal characters.";
                WarningText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
                WarningText.Visibility = Visibility.Visible;
            }
            else
            {
                WarningText.Visibility = Visibility.Collapsed;
            }
        }

        private bool ContainsIllegalJsonChars(string input)
        {
            return Regex.IsMatch(input, @"[\\\""\u0000-\u001F]");
        }
    }
}
