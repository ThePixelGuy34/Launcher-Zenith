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

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for DownloadsPageRL.xaml
    /// </summary>
    public partial class DownloadsPageRL : Page
    {
        public DownloadsPageRL()
        {
            InitializeComponent();
        }

        private void StartDownload_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderDialog.SelectedPath;
                    PassDownload(selectedFolder);
                }
            }
        }

        private void PassDownload(string selectedFolder)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
            {
                //mainWindow.StartDownload(selectedFolder);
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }

        private void CloseDownloads_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ClosePageIRL();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }
    }
}