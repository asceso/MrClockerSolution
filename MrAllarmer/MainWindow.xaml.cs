using Notifications.Wpf;
using System;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MrAllarmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotificationManager Notification = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Режим работы 25 - 5

        private const string DisabledString = "Режим ожидания";
        private const string InChealString = "Отдых";
        private const string InWorkString = "Работа";
        private const string ToChealSoundPath = @"./sounds/cheal_work_25/to_cheal.wav";
        private const string ToWorkSoundPath = @"./sounds/cheal_work_25/to_work.wav";
        private readonly NotificationContent ChealStartContent = new()
        {
            Message = "До начала работы 5 минут",
            Title = "Начало отдыха",
            Type = NotificationType.Information
        };
        private readonly NotificationContent ChealEndContent = new()
        {
            Message = "Время отдыха закончилось, время работы - 25 минут",
            Title = "Конец отдыха",
            Type = NotificationType.Warning
        };
        private readonly NotificationContent ChealWorkModeDisabledContent = new()
        {
            Message = "Режим 25-5 выключен",
            Title = "Выключение режима",
            Type = NotificationType.Success
        };
        private WorkChealModeState currentChealWorkModeState = WorkChealModeState.Disabled;

        private readonly Timer msWorkChealTimer = new(5);
        private DateTime StartListeningStamp;

        private void ChealWorkToggleClick(object sender, RoutedEventArgs e)
        {
            switch (ChealWorkModeToggle.IsChecked)
            {
                case true:
                    {
                        ShowFullNotifyWithStatus(ChealStartContent, ToChealSoundPath, InChealString);
                        currentChealWorkModeState = WorkChealModeState.InCheal;
                        StartListeningStamp = DateTime.Now;
                        msWorkChealTimer.Elapsed += MsWorkChealTimerElapsed;
                        msWorkChealTimer.Start();
                    }
                    break;
                case false:
                    {
                        ShowFullNotifyWithStatus(ChealWorkModeDisabledContent, ToChealSoundPath, DisabledString);
                        currentChealWorkModeState = WorkChealModeState.Disabled;
                    }
                    break;
                default:
                    break;
            }
        }

        private void MsWorkChealTimerElapsed(object sender, ElapsedEventArgs e)
        {
            double stampMinutes = (DateTime.Now - StartListeningStamp).TotalMinutes;
            if (currentChealWorkModeState == WorkChealModeState.Disabled)
            {
                msWorkChealTimer.Elapsed -= MsWorkChealTimerElapsed;
                msWorkChealTimer.Stop();
                return;
            }
            if (stampMinutes >= 5 && currentChealWorkModeState == WorkChealModeState.InCheal)
            {
                ShowFullNotifyWithStatus(ChealEndContent, ToWorkSoundPath, InWorkString);
                StartListeningStamp = DateTime.Now;
                currentChealWorkModeState = WorkChealModeState.InWork;
                return;
            }
            if (stampMinutes >= 25 && currentChealWorkModeState == WorkChealModeState.InWork)
            {
                ShowFullNotifyWithStatus(ChealStartContent, ToChealSoundPath, InChealString);
                StartListeningStamp = DateTime.Now;
                currentChealWorkModeState = WorkChealModeState.InCheal;
                return;
            }
        }
        private void ShowFullNotifyWithStatus(NotificationContent content, string pathToSound, string tooltipText)
        {
            Notification.Show(content);
            SoundPlayer player = new();
            player.SoundLocation = pathToSound;
            player.LoadAsync();
            player.Play();
            Application.Current.Dispatcher.Invoke(() => NotifyIcon.ToolTipText = tooltipText);
        }

        #endregion
        #region interface methods

        private void CloseAppClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void MinimizeAppClick(object sender, RoutedEventArgs e)
        {
            NotifyIcon.Visibility = Visibility.Visible;
            Hide();
        }
        private void ShowAppClick(object sender, RoutedEventArgs e)
        {
            Show();
            NotifyIcon.Visibility = Visibility.Collapsed;
            if (sender is Button btn)
            {
                if (btn.Parent is Grid grd)
                {
                    if (grd.Parent is Popup pp)
                    {
                        pp.IsOpen = false;
                    }
                }
            }
        }
        private void HeaderMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #endregion
    }

    public enum WorkChealModeState
    {
        Disabled, InCheal, InWork
    }
}
