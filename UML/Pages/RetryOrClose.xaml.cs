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
using System.Windows.Shapes;
using UML.Pages.Experiments;

namespace UML.Pages
{
    /// <summary>
    /// Interaction logic for RetryOrClose.xaml
    /// </summary>
    public partial class RetryOrClose : Page
    {
        public RetryOrClose(string openReason)
        {
            InitializeComponent();
            CheckForWhy(openReason);
        }

        private void CheckForWhy(string openReason)
        {
            if (openReason == "HttpFailure")
            {
                var httpFailPage = new MorePages.ConnectionFailure();
                httpFailPage.OkButtonClicked += ConnectionFailure_CloseShutdown_Click;
                Frame4ConnectionProblem.Navigate(httpFailPage);
            }
            else
            {
                string errorheader = "Connection failure.";
                string errordesc = "No response was received from Zenith Services.";
                string errorCode = "CF-0001";
                string buttonText1 = "OK";
                string buttonText2 = string.Empty;
                string buttonamount = "1";
                var cannotConnectPage = new NoticePage(errorheader, errordesc, errorCode, buttonamount, buttonText1, buttonText2);
                cannotConnectPage.OkButtonClicked += CannotConnectPage_OkButtonClicked;
                Frame4ConnectionProblem.Navigate(cannotConnectPage);
            }
        }

        private void CannotConnectPage_OkButtonClicked(object sender, RoutedEventArgs e)
        {
            Frame4ConnectionProblem.Visibility = Visibility.Collapsed;
        }

        private void ConnectionFailure_CloseShutdown_Click(object sender, RoutedEventArgs e)
        {
            Frame4ConnectionProblem.Visibility = Visibility.Collapsed;
        }

        public void HideCannotConnectPage()
        {
            Frame4ConnectionProblem.Visibility = Visibility.Collapsed;
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CheckUpdates();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
