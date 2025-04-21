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

namespace UML.Pages.Experiments
{
    /// <summary>
    /// Interaction logic for NoticePage.xaml
    /// </summary>
    public partial class NoticePage : Page
    {
        public event RoutedEventHandler OkButtonClicked;

        public NoticePage(string header, string description, string errorcode, string buttonamount, string buttonText1, string buttonText2)
        {
            InitializeComponent();
            OnStartup(header, description, errorcode, buttonamount, buttonText1, buttonText2);
        }

        private void OnStartup(string header, string description, string errorcode, string buttonamount, string buttonText1, string buttonText2)
        {
            DisplayError(header, description, errorcode, buttonamount, buttonText1, buttonText2);
        }

        private void DisplayError(string header, string description, string errorcode, string buttonamount, string buttonText1, string buttonText2)
        {
            HeaderText.Text = header;
            DescText.Text = description;

            if (errorcode is null)
            {
                ErrorCode.Opacity = 0;
            }
            else
            {
                ErrorCode.Opacity = 1;
                ErrorButton.Content = errorcode;
            }

            if (buttonamount is "2")
            {
                secondBtn.Content = buttonText2;
                secondBtn.Opacity = 1;
            }

            OkBtn.Content = buttonText1;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OkButtonClicked?.Invoke(this, e);
        }

        private void SecondButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ErrorCode_Click(object sender, RoutedEventArgs e)
        {
            // add logic later as it will just be opening a link that needs to be passed to it.
        }
    }
}
