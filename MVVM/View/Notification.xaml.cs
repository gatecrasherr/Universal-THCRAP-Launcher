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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Universal_THCRAP_Launcher.MVVM.View
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        private Storyboard _showStoryboard;
        private Storyboard _hideStoryboard;

        public event EventHandler Closed;

        public Notification()
        {
            InitializeComponent();

            _showStoryboard = (Storyboard)FindResource("ShowNotificationStoryboard");
            _hideStoryboard = (Storyboard)FindResource("HideNotificationStoryboard");

            _hideStoryboard.Completed += (s, e) =>
            {
                Closed?.Invoke(this, EventArgs.Empty);
            };
        }

        public void Show()
        {
            _showStoryboard.Begin(this);
        }

        public void Hide()
        {
            _hideStoryboard.Begin(this);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
