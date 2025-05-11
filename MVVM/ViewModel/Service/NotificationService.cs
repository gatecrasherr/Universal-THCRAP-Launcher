using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Universal_THCRAP_Launcher.MVVM.View;

namespace Universal_THCRAP_Launcher.MVVM.ViewModel.Service
{
    class NotificationService
    {
        private static NotificationService _instance;
        private Grid _notificationGrid;
        private readonly List<Notification> _activeNotifications = new List<Notification>();
        private readonly int _maxNotifications = 5;
        private readonly double _spacing = 4;

        public static NotificationService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NotificationService();
                return _instance;
            }
        }

        private NotificationService() { }

        public void Initialize(Grid containerGrid)
        {
            _notificationGrid = containerGrid;
        }

        public Notification ShowNotification(string title, string message, NotificationViewModel.NotificationType type = NotificationViewModel.NotificationType.Info, int duration = 5)
        {
            if (_notificationGrid == null)
                throw new InvalidOperationException("NotificationService is not initialized. Call Initialize() first.");

            var viewModel = new NotificationViewModel(title, message, type, duration);
            var notification = new Notification
            {
                DataContext = viewModel
            };

            _notificationGrid.Children.Add(notification);

            PositionNotification(notification);

            notification.Show();

            _activeNotifications.Add(notification);

            if(duration > 0)
            {
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(duration)
                };

                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    notification.Hide();
                };
                timer.Start();
            }

            notification.Closed += (s, e) =>
            {
                _activeNotifications.Remove(notification);
                _notificationGrid.Children.Remove(notification);
                RepositionNotification();
            };

            if(_activeNotifications.Count > _maxNotifications)
            {
                _activeNotifications.First().Hide();
            }

            return notification;
        }

        public void Destroy(Notification notification)
        {
            if(notification == null || !_activeNotifications.Contains(notification))
                return;

            notification.Hide();

            notification.Closed += (s, e) =>
            {
                _activeNotifications.Remove(notification);
                _notificationGrid.Children.Remove(notification);
                RepositionNotification();
            };
        }

        private void PositionNotification(Notification notification)
        {
            notification.HorizontalAlignment = HorizontalAlignment.Right;
            notification.VerticalAlignment = VerticalAlignment.Bottom;

            double bottomMargin = _spacing;

            foreach(var activeNotification in _activeNotifications)
            {
                if (activeNotification.ActualHeight > 0)
                    bottomMargin += activeNotification.ActualHeight + _spacing;
            }

            notification.Margin = new Thickness(0, 0, _spacing, bottomMargin);
        }

        private void RepositionNotification()
        {
            double bottomMargin = _spacing;

            foreach(var notification in _activeNotifications)
            {
                notification.Margin = new Thickness(0, 0, _spacing, bottomMargin);
                bottomMargin += notification.ActualHeight + _spacing;
            }
        }
    }
}
