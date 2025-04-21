using Discord.Rest;
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

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for ConnectionFailure.xaml
    /// </summary>
    public partial class ConnectionFailure : Page
    {
        public event RoutedEventHandler? OkButtonClicked;
        public event RoutedEventHandler? CloseButtonClicked;

        public ConnectionFailure()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OkButtonClicked?.Invoke(this, e);
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CloseShutdown_Click(object sender, RoutedEventArgs e)
        {
            CloseButtonClicked?.Invoke(this, e);
        }
    }
}
