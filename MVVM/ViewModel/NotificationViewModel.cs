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
        private NotificationType _type;
        private int _duration = 5;

        public enum NotificationType
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

        public NotificationType Type
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

        public NotificationViewModel(string title, string message, NotificationType type = NotificationType.Info, int duration = 5)
        {
            Title = title;
            Message = message;
            Type = type;
            Duration = duration;
        }

        // This is just SVG data. In the future, feel free to replace them by opening any .SVG in Notepad. These ones have been taken off Google Fonts.
        private void UpdateAppearanceByType()
        {
            switch (Type)
            {
                case NotificationType.Info:
                    IconData = Geometry.Parse("M440-280h80v-240h-80v240Zm40-320q17 0 28.5-11.5T520-640q0-17-11.5-28.5T480-680q-17 0-28.5 11.5T440-640q0 17 11.5 28.5T480-600Zm0 520q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(66, 165, 245));
                    break;

                case NotificationType.Success:
                    IconData = Geometry.Parse("m424-296 282-282-56-56-226 226-114-114-56 56 170 170Zm56 216q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    break;

                case NotificationType.Warning:
                    IconData = Geometry.Parse("m40-120 440-760 440 760H40Zm440-120q17 0 28.5-11.5T520-280q0-17-11.5-28.5T480-320q-17 0-28.5 11.5T440-280q0 17 11.5 28.5T480-240Zm-40-120h80v-200h-80v200Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    break;

                case NotificationType.Error:
                    IconData = Geometry.Parse("M480-280q17 0 28.5-11.5T520-320q0-17-11.5-28.5T480-360q-17 0-28.5 11.5T440-320q0 17 11.5 28.5T480-280Zm-40-160h80v-240h-80v240Zm40 360q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Z");
                    IconColor = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                    break;
            }
        }
    }
}
