using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Universal_THCRAP_Launcher.Core;

namespace Universal_THCRAP_Launcher.MVVM.ViewModel
{
    class NotificationViewModel : ObservableObject
    {
        private string _title;
        private string _message;
        private Geometry _iconData;
        private Brush _iconColor;
        private ToastType _type;
        private int _duration = 5;

        public enum ToastType
        {
            Info,
            Success,
            Warning,
            Error
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        public Geometry IconData
        {
            get => _iconData;
            set => SetProperty(ref _iconData, value);
        }

        public Brush IconColor
        {
            get => _iconColor;
            set => SetProperty(ref _iconColor, value);
        }

        public ToastType Type
        {
            get => _type;
            set
            {
                SetProperty(ref _type, value);
                UpdateAppearanceByType();
            }
        }

        public int Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public NotificationViewModel(string title, string message, ToastType type = ToastType.Info, int duration = 5)
        {
            Title = title;
            Message = message;
            Type = type;
            Duration = duration;
        }

        private void UpdateAppearanceByType()
        {
            switch (Type)
            {
                case ToastType.Info:
                    IconData = Geometry.Parse("M12,2C6.48,2 2,6.48 2,12C2,17.52 6.48,22 12,22C17.52,22 22,17.52 22,12C22,6.48 17.52,2 12,2ZM13,17H11V11H13V17ZM13,9H11V7H13V9Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(66, 165, 245));
                    break;

                case ToastType.Success:
                    IconData = Geometry.Parse("M12,2C6.48,2 2,6.48 2,12C2,17.52 6.48,22 12,22C17.52,22 22,17.52 22,12C22,6.48 17.52,2 12,2ZM10,17L5,12L6.41,10.59L10,14.17L17.59,6.58L19,8L10,17Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    break;

                case ToastType.Warning:
                    IconData = Geometry.Parse("M1,21H23L12,2L1,21ZM13,18H11V16H13V18ZM13,14H11V10H13V14Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    break;

                case ToastType.Error:
                    IconData = Geometry.Parse("M12,2C6.47,2 2,6.47 2,12C2,17.53 6.47,22 12,22C17.53,22 22,17.53 22,12C22,6.47 17.53,2 12,2ZM17,15.59L15.59,17L12,13.41L8.41,17L7,15.59L10.59,12L7,8.41L8.41,7L12,10.59L15.59,7L17,8.41L13.41,12L17,15.59Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                    break;
            }
        }
    }
}
