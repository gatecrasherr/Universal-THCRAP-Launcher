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
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : UserControl
    {
        public LoadingScreen()
        {
            InitializeComponent();
        }

        public void UpdateStatus(string status)
        {
            StatusTextBlock.Text = status;
        }

        public void FadeOut(Action onCompleted = null)
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(1.2)
            };

            fadeOutAnimation.Completed += (s, e) =>
            {
                Visibility = Visibility.Collapsed;
                onCompleted?.Invoke();
            };

            BeginAnimation(OpacityProperty, fadeOutAnimation);
        }
    }
}
