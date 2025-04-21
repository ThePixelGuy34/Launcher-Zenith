using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UML.Class;
using Newtonsoft.Json;
using UML.Services;
using System.Windows.Media;
using DiscordRPC.Helper;
using UML.Class.DontBreakPls;

namespace UML.Pages
{
    public partial class LauncherMain : Page
    {
        private DiscordRPCManager discordRPC;

        private string json = string.Empty;

        public LauncherMain()
        {
            InitializeComponent();
            discordRPC = new DiscordRPCManager();
            DiscordRPCManager.SetPresence("Online", "On the Home Menu!");
            Loaded += LauncherMain_Loaded;
        }

        private async void LauncherMain_Loaded(object sender, RoutedEventArgs e)
        {
            string id = "mainpage";
            string id1 = "newsImageBig";
            string id2 = "newsImageSmall1";
            string id3 = "newsImageSmall2";
            string id4 = "newsImageSmall3";

            var mainPage = await UML.Class.HttpFunctions.LoadMainBackground(id, id1, id2, id3, id4);

            ScrollViewer_ScrollChanged(UAintNothingButAScrollBoyBoyBoyBoyBoy, null);
            DisplayBackdropFrame();

            await LoadMainBackground(mainPage);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                double maxScroll = scrollViewer.ScrollableHeight;
                Console.WriteLine($"Scroll Offset: {scrollViewer.VerticalOffset}, Max Scrollable Height: {maxScroll}");
                TopShadow.Opacity = scrollViewer.VerticalOffset > 1 ? 0.4 : 0;
                BottomShadow.Opacity = scrollViewer.VerticalOffset >= maxScroll - 1 ? 0 : 0.4;
            }
        }

        private void RunIt_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FortnitePage());
        }

        private void DisplayBackdropFrame()
        {
            BackdropFrame.Navigate((object)new LauncherMainBackgroundVide());
        }
        private void FNFromMain(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FortnitePage());
        }

        private async Task LoadMainBackground(GameConfig mainPage)
        {
            try
            {
                
                var appContent = JsonConvert.DeserializeObject<Class.DontBreakPls.AppConfig>(json);

                if (mainPage != null)
                {
                    var images = mainPage.images;
                    var bgImage = images.FirstOrDefault(img => img.id == "newsImageBig");
                    var firstImage = images.FirstOrDefault(img => img.id == "newsImageSmall1");
                    var secondImage = images.FirstOrDefault(img => img.id == "newsImageSmall2");
                    var thirdImage = images.FirstOrDefault(img => img.id == "newsImageSmall3");
                    if (bgImage != null) LoadImage(bgImage.image, newsMainBig);
                    if (firstImage != null) LoadImage(firstImage.image, newsMainSmall1);
                    if (secondImage != null) LoadImage(secondImage.image, newsMainSmall2);
                    if (thirdImage != null) LoadImage(thirdImage.image, newsMainSmall3);

                    if (bgImage != null && !string.IsNullOrEmpty(bgImage.header))
                        BigPictureHeader.Text = bgImage.header;
                    if (bgImage != null && !string.IsNullOrEmpty(bgImage.description))
                        BigPictureDesc.Text = bgImage.description;
                    if (bgImage != null && !string.IsNullOrEmpty(bgImage.buttontxt))
                        NewsButton.Content = bgImage.buttontxt;
                    if (firstImage != null && !string.IsNullOrEmpty(firstImage.description))
                        SmallPictureDesc1.Text = firstImage.description;
                    if (firstImage != null && !string.IsNullOrEmpty(firstImage.header))
                        SmallPictureHeader1.Text = firstImage.header;
                    if (secondImage != null && !string.IsNullOrEmpty(secondImage.header))
                        SmallPictureHeader2.Text = secondImage.header;
                    if (secondImage != null && !string.IsNullOrEmpty(secondImage.description))
                        SmallPictureDesc2.Text = secondImage.description;
                    if (thirdImage != null && !string.IsNullOrEmpty(thirdImage.header))
                        SmallPictureHeader3.Text = thirdImage.header;
                    if (thirdImage != null && !string.IsNullOrEmpty(thirdImage.description))
                        SmallPictureDesc3.Text = thirdImage.description;

                }
                else
                {
                    Logger.Log($"Failed to load contentpages | LM-0002");
                }
            }
            catch (TaskCanceledException)
            {
                Logger.Log("Connection timeout when loading contentpages. - LM-0003");
            }
            catch (JsonException)
            {
                Logger.Log("Contentpages JSON is invalid, contact isaac or zippywippy. - LM-0005");
            }
            catch (Exception)
            {
                Logger.Log("An error occurred when loading contentpages, is the backend down? - LM-0006");
                await Task.Delay(1);
            }
        }

        private void LoadImage(string imageUrl, Image imageControl)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imageUrl, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                imageControl.Dispatcher.Invoke(() =>
                {
                    NewsElements.Opacity = 1;
                    imageControl.Source = bitmapImage;
                });
            }
            catch (Exception ex)
            {
                Logger.Log("Error loading image: " + ex.Message);
                ShowFallbackImage();
            }
        }

        private void ShowFallbackImage()
        {
            try
            {
                NewsElements.Opacity = 0;
                //this.imageControl.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/assetfailfallback.png"));
                this.imageControl.Visibility = Visibility.Visible;
                //this.videoPlayer.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                //this.imageControl.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,,/src/Assets/assetfailfallback.png"));
            }
        }
    }
}
