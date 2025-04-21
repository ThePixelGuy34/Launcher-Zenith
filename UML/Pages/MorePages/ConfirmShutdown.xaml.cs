using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using UML.Class.LaunchLogic;

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for ConfirmShutdowen.xaml
    /// </summary>
    public partial class ConfirmShutdown : Page
    {
        private bool _isFNrunning = false;
        private bool _isRLrunning = false;

        public ConfirmShutdown()
        {
            InitializeComponent();
            DisplayActiveClients();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CloseShutdown();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CloseAllProcesses();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }

        private void CloseShutdown_Click(object sender, MouseEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CloseShutdown();
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }

        private void DisplayActiveClients()
        {
            //if (StartGameFN._FortniteEACProcess == null)
            //{
            //    _isFNrunning = false;
            //}
            //else
            //{
            //    _isFNrunning = true;
            //}

            //if (StartGameRL._RLProcess == null)
            //{
            //    _isRLrunning = false;
            //}
            //else
            //{
            //    _isRLrunning = true;
            //}

            DisplayClientsInText();
        }

        private void DisplayClientsInText()
        {
            List<string> runningGames = new List<string>();

            if (_isFNrunning)
                runningGames.Add("Fortnite");

            if (_isRLrunning)
                runningGames.Add("Rocket League");

            string output = runningGames.Count > 0 ? string.Join(", ", runningGames) : "No games running.";
            RunningGames.Text = $"Games that will be affected: {output}";
        }
    }
}
