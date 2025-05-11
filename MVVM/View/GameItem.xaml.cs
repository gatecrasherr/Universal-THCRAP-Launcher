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

namespace Universal_THCRAP_Launcher.MVVM.View
{
    /// <summary>
    /// Interaction logic for GameItem.xaml
    /// </summary>
    public partial class GameItem : UserControl
    {
        public static readonly DependencyProperty GameTitleProperty =
            DependencyProperty.Register(nameof(DisplayTitle), typeof(string), typeof(GameItem), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty GameIconProperty =
            DependencyProperty.Register(nameof(DisplayIcon), typeof(ImageSource), typeof(GameItem), new PropertyMetadata(null));

        public static readonly DependencyProperty GameIconOpacity =
            DependencyProperty.Register(nameof(DisplayIconOpacity), typeof(double), typeof(GameItem), new PropertyMetadata(0.0));

        public static readonly DependencyProperty GameBackgroudColor =
            DependencyProperty.Register(nameof(DisplayBackgroundColor), typeof(Brush), typeof(GameItem), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"))));

        public static readonly DependencyProperty GameIdProperty =
            DependencyProperty.Register(nameof(GameId), typeof(string), typeof(GameItem), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty GamePathProperty =
            DependencyProperty.Register(nameof(GamePath), typeof(string), typeof(GameItem), new PropertyMetadata(string.Empty));

        //

        public delegate void GameSelectedEventHandler(object sender, GameSelectedEventArgs e);
        public event GameSelectedEventHandler GameSelected;

        //

        public string DisplayTitle
        {
            get => (string)GetValue(GameTitleProperty);
            set => SetValue(GameTitleProperty, value);
        }

        public ImageSource DisplayIcon
        {
            get => (ImageSource)GetValue(GameIconProperty);
            set => SetValue(GameIconProperty, value);
        }

        public double DisplayIconOpacity
        {
            get => (double)GetValue(GameIconOpacity);
            set => SetValue(GameIconOpacity, value);
        }

        public Brush DisplayBackgroundColor
        {
            get => (Brush)GetValue(GameBackgroudColor);
            set => SetValue(GameBackgroudColor, value);
        }

        public string GameId
        {
            get => (string)GetValue(GameIdProperty);
            set => SetValue(GameIdProperty, value);
        }

        public string GamePath
        {
            get => (string)GetValue(GamePathProperty);
            set => SetValue(GamePathProperty, value);
        }



        public GameItem()
        {
            InitializeComponent();
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            GameSelected?.Invoke(this, new GameSelectedEventArgs
            {
                GameTitle = this.DisplayTitle,
                GameIcon = this.DisplayIcon,
                GameId = this.GameId,
                GamePath = this.GamePath,
            });
        }
    }

    public class GameSelectedEventArgs : EventArgs
    {
        public string GameTitle { get; set; }
        public ImageSource GameIcon { get; set; }
        public string GameId { get; set; }
        public string GamePath { get; set; }
    }
}
