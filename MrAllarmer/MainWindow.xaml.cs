using Notifications.Wpf;
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
        private readonly Timer chealModeTimer = new(300000);
        private readonly Timer workModeTimer = new(1500000);

        private void ChealModeTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (currentChealWorkModeState != WorkChealModeState.Disabled)
            {
                ShowFullNotifyWithStatus(ChealEndContent, @"./sounds/wav/cheal_work_mode.wav", InWorkString);
                chealModeTimer.Stop();
                chealModeTimer.Elapsed -= ChealModeTimerElapsed;
                workModeTimer.Elapsed += WorkModeTimerElapsed;
                workModeTimer.Start();
            }
        }
        private void WorkModeTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (currentChealWorkModeState != WorkChealModeState.Disabled)
            {
                ShowFullNotifyWithStatus(ChealStartContent, @"./sounds/wav/cheal_work_mode.wav", InChealString);
                workModeTimer.Stop();
                workModeTimer.Elapsed -= WorkModeTimerElapsed;
                chealModeTimer.Elapsed += ChealModeTimerElapsed;
                chealModeTimer.Start();
            }
        }
        private void ChealWorkToggleClick(object sender, RoutedEventArgs e)
        {
            switch (ChealWorkModeToggle.IsChecked)
            {
                case true:
                    {
                        ShowFullNotifyWithStatus(ChealStartContent, @"./sounds/wav/cheal_work_mode.wav", InChealString);
                        currentChealWorkModeState = WorkChealModeState.InWork;
                        chealModeTimer.Elapsed += ChealModeTimerElapsed;
                        chealModeTimer.Start();
                    }
                    break;
                case false:
                    {
                        ShowFullNotifyWithStatus(ChealWorkModeDisabledContent, @"./sounds/wav/cheal_work_mode.wav", DisabledString);
                        currentChealWorkModeState = WorkChealModeState.Disabled;
                        chealModeTimer.Elapsed -= ChealModeTimerElapsed;
                        workModeTimer.Elapsed -= WorkModeTimerElapsed;
                    }
                    break;
                default:
                    break;
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

        #endregion
    }

    public enum WorkChealModeState
    {
        Disabled, InCheal, InWork
    }
}
