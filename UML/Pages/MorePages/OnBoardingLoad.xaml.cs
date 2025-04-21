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
    /// Interaction logic for OnBoardingLoad.xaml
    /// </summary>
    public partial class OnBoardingLoad : Page
    {
        public OnBoardingLoad()
        {
            InitializeComponent();
            DoTheThingIg();
        }

        private async void DoTheThingIg()
        {
            await Task.Delay(2000);
            LoadFatass();
        }

        private void LoadFatass()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenAllat();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }
    }
}
