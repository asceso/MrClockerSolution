using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MrAllarmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Timer timer = new();
            timer.Interval = 5000;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Hide();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MediaPlayer player1 = new();
            player1.Open(new Uri("./alarm.mp3", UriKind.Relative));
            player1.Play();
        }

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
    }
}
