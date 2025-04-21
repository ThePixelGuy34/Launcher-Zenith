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
using UML.Pages.MorePages;

namespace UML.Pages
{
    /// <summary>
    /// Interaction logic for DownloadsPageV2.xaml
    /// </summary>
    public partial class DownloadsPageV2 : Page
    {

        public DownloadsPageV2()
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
                    DownloadFortnite(selectedFolder);
                }
            }
        }

        private void DownloadFortnite(string selectedFolder)
        {
            if (FortnitePage.Current != null)
            {
                FortnitePage.Current.StartDownload(selectedFolder);
            }
        }

        private void CloseDownloads_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ClosePage();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }
    }
}
