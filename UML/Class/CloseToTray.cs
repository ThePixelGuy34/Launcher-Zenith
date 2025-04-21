using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using UML.Pages.MorePages;
using System.IO;
namespace UML.Class
{
    public class TrayManager
    {
        private readonly Window _mainWindow;
        private readonly NotifyIcon _trayIcon;
        int _timesMinimized = 0;

        public TrayManager(Window mainWindow, string tooltip = "Zenith Launcher", string iconPath = null)
        {
            iconPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Zenith", "app.ico");
            _mainWindow = mainWindow;
            _trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(iconPath),
                Visible = true,
                Text = tooltip,
                ContextMenuStrip = BuildContextMenu()
            };
            _trayIcon.DoubleClick += (s, e) => ShowWindow();
            _mainWindow.Closing += OnWindowClosing;
        }

        private void ShowWindow()
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }
        private ContextMenuStrip BuildContextMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Fortnite", null, (s, e) => ShowWindow());
            menu.Items.Add("Sign Out", null, (s, e) => SignOut());
            menu.Items.Add("Exit", null, (s, e) => Application.Current.Shutdown());
            return menu;
        }
        private void SignOut()
        {
            _mainWindow.Show();
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.SignOut_Zenith();
            }
        }
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (_mainWindow.WindowState == WindowState.Minimized)
            {
                if (_timesMinimized < 0)
                {
                    _mainWindow.Hide();
                    _timesMinimized++;
                    _trayIcon.ShowBalloonTip(1000, "Still Running", "Closed to tray.", ToolTipIcon.None);
                }
                else
                {
                    _mainWindow.Hide();
                    _timesMinimized++;
                }
            }
        }
    }
}