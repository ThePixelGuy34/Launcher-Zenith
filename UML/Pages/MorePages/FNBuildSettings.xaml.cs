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
    /// Interaction logic for FNBuildSettings.xaml
    /// </summary>
    public partial class FNBuildSettings : Page
    {
        public FNBuildSettings()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
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

        private void ClearBuild_Click(object sender, RoutedEventArgs e)
        {
            // clear fortnite path from registry.
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.DeleteFortnitePathFromRegistry();
            }
        }
    }
}
