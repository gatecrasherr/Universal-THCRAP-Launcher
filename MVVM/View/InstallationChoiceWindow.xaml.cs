using System;
using System.Windows;
using System.Windows.Input;

namespace Universal_THCRAP_Launcher
{
    public partial class InstallationChoiceWindow : Window
    {
        public enum DialogueResult
        {
            None,
            DownloadLatest,
            PickExisting
        }

        public DialogueResult Result { get; private set; } = DialogueResult.None;

        public InstallationChoiceWindow()
        {
            InitializeComponent();
            this.Loaded += InstallationDialogue_Loaded;
        }

        private void InstallationDialogue_Loaded(object sender, RoutedEventArgs e)
        {
            this.Activate();
            this.Focus();
        }

        private void DownloadLatest_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogueResult.DownloadLatest;
            this.Close();
        }

        private void PickExisting_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogueResult.PickExisting;
            this.Close();
        }

        private void Window_LeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
