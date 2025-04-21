using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using UML.Class.LaunchLogic;
using UML.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Net.Http;
using System.Windows.Media.Imaging;
using UML.Class;
using Newtonsoft.Json;

namespace UML.Pages.MorePages
{
    /// <summary>
    /// Interaction logic for ImportAndDownloadRL.xaml
    /// </summary>
    public partial class ImportAndDownloadRL : Page
    {
        public ImportAndDownloadRL()
        {
            InitializeComponent();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.IsFolderPicker = true;
                folderDialog.Title = "Select a Rocket League Build";

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedPath = folderDialog.FileName;

                    if (File.Exists(Path.Combine(selectedPath, "Binaries\\Win32\\RocketLeague.exe")))
                    {
                        SaveTAGamePathToRegistry(selectedPath);
                        Logger.Log($"Game path set to: {selectedPath}");
                        RLPage RlP = new RLPage();
                        RlP.UpdateRLLaunchButton();
                        ThenCloseIt();
                    }
                    else
                    {
                        Logger.Log("Ensure that the folder contains Binaries, Engine and TAGame.");
                    }
                }
            }
        }

        private void ThenCloseIt()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ClosePageIRL();
            }
            else
            {
                throw new InvalidOperationException("Navigation error.");
            }
        }

        private void SaveTAGamePathToRegistry(string selectedPath)
        {
            try
            {
                string registryKey = @"Zenith";
                string rlGamePathKey = "TAGamePath";

                using (var key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(registryKey))
                {
                    if (key != null)
                    {
                        key.SetValue(rlGamePathKey, selectedPath);
                        Logger.Log($"GamePath saved to registry, {selectedPath}");
                    }
                    else
                    {
                        Logger.Log("Error: Unable to create or open registry key.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error saving GamePath to registry, {ex.Message}");
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            // no downloading rn.
            MessageBox.Show("Downloading is currently unavailable.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            //if (Application.Current.MainWindow is MainWindow mainWindow)
            //{
            //    mainWindow.OpenDownloadRL();
            //}
            //else
            //{
            //    throw new InvalidOperationException("Navigation error.");
            //}
        }

        private void CloseImport_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
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
