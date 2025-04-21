using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for AuthenticationCode.xaml
    /// </summary>
    public partial class AuthenticationCode : Page
    {
        private const string ApiEndpointUrl = "https://zenith-api.zippywippy.online/zenith/api/v1/oauth/login/verify";
        private readonly HttpClient _httpClient;

        public AuthenticationCode()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            VerifyButton.IsEnabled = false;

            otpInput.OtpCompleted += (sender, otpValue) =>
            {
                VerifyButton.IsEnabled = true;
            };
        }

        private async void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            VerifyButton.IsEnabled = false;
            VerifyButton.Content = "Please wait...";
            VerifyButton.Background = new System.Windows.Media.SolidColorBrush(
            System.Windows.Media.Color.FromRgb(27, 158, 79));

            try
            {
                string otpValue = otpInput.GetOtpValue();

                if (string.IsNullOrEmpty(otpValue) || otpValue.Length < 6)
                {
                    MessageBox.Show("Please enter all 6 digits", "Incomplete Code");
                    ResetButton();
                    return;
                }

                bool success = await VerifyOtpAsync(otpValue);

                if (success)
                {
                    NavigateToMain();
                }
                else
                {
                    ResetButton();
                    MessageBox.Show("Invalid code. Please try again.");
                    otpInput.Clear();
                }
            }
            catch
            {
                MessageBox.Show("Connection error. Please try again later.");
                ResetButton();
            }
            finally
            {
                ResetButton();
            }
        }

        private async Task<bool> VerifyOtpAsync(string otp)
        {
            try
            {
                string url = $"{ApiEndpointUrl}?code={otp}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API error: {ex.Message}");
                ResetButton();
                return false;
            }
        }

        private void NavigateToMain()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                if (StringSharing.LoginReason == "true")
                {
                    mainWindow.OpenMessageBox();
                }
                else
                {
                    mainWindow.OpenWelcomeIn();
                }
            }
            else
            {
                throw new InvalidOperationException("MainWindow is not available or of the expected type.");
            }
        }

        private void ResetButton()
        {
            VerifyButton.Content = "Continue";
            VerifyButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(75, 158, 79));
            VerifyButton.IsEnabled = true;
        }
    }
}
