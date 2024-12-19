using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeClock();
        }

        // Обработчик события "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        // Обновление времени
        private void InitializeClock()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { TimeTextBlock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); };
            timer.Start();
        }

        // Обработчик Navigated
        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            BackButton.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;

            if (e.Content is Page page)
            {
                this.Title = $"WPF App — {page.Title}";
            }
        }
    }
}
