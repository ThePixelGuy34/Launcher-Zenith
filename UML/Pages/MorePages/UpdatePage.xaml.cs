using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for UpdatePage.xaml
    /// </summary>
    public partial class UpdatePage : Page
    {
        private static DispatcherTimer? timer;
        private static int countdown = 30;
        string shutdownInterval = string.Empty;

        public UpdatePage()
        {
            InitializeComponent();
            BeginUpdate();
            OnStartup();
        }

        private void OnStartup()
        {
            BeginTimer();
        }

        private void BeginTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            countdown--;
            shutdownInterval = countdown.ToString();
            UpdateText.Text = $"The update will begin automatically in: {shutdownInterval} seconds.";
            if (countdown <= 0)
            {
                timer?.Stop();
                CloseAndStartUpdate();
            }
        }

        private void CloseAndStartUpdate()
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenUpdater();
            }
        }

        private void BeginUpdate()
        {
            UpdateText.Text = "";
        }

        private void UpdateInstantly_Click(object sender, RoutedEventArgs e)
        {
            CloseAndStartUpdate();
        }
    }
}
