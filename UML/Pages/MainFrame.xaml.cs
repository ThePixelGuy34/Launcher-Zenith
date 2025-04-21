using System;
using System.Diagnostics;
using System.Windows;
using DiscordRPC.Logging;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using NetDiscordRpc;
using System.Windows.Controls;
using UML.Class;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UML.Pages
{
    public partial class MainFrame : System.Windows.Controls.Page
    {
        public MainFrame()
        {
            InitializeComponent();
            Frame4PageSwitch.Navigate(new LauncherMain());
            GetRid();
        }

        private void GetRid()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ByeSettings();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
            //await Task.Delay(1300);
        }

        private void FNPage_Click(object sender, RoutedEventArgs e)
        {
            //ButtonActivity.SetIsSelected(CommunityButton, false);
            //ButtonActivity.SetIsSelected(UT, false);
            ButtonActivity.SetIsSelected(FortniteButton, true);
            Frame4PageSwitch.Navigate(new FortnitePage());
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://discord.gg/zj399J5smC";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void OldHome_Click(object sender, RoutedEventArgs e)
        {
            //Frame4PageSwitch.Navigate(new MainWindow());
        }

        private void UTPage_Click(object sender, RoutedEventArgs e)
        {
            //ButtonActivity.SetIsSelected(CommunityButton, false);
            ButtonActivity.SetIsSelected(FortniteButton, false);
            //ButtonActivity.SetIsSelected(UT, true);
            Frame4PageSwitch.Navigate(new Pages.RLPage());
        }

        private void HomeLogo_Click(object sender, RoutedEventArgs e)
        {
            //ButtonActivity.SetIsSelected(CommunityButton, false);
            ButtonActivity.SetIsSelected(FortniteButton, false);
            //ButtonActivity.SetIsSelected(UT, false);
            Frame4PageSwitch.Navigate(new LauncherMain());
        }

        //private void Community_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonActivity.SetIsSelected(CommunityButton, true);
        //    ButtonActivity.SetIsSelected(FortniteButton, false);
        //    ButtonActivity.SetIsSelected(UT, false);
        //    Frame4PageSwitch.Navigate(new CommunityTab());
        //}
    }
}
