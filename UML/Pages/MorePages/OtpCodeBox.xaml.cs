using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UML.Pages.MorePages
{
    public partial class OtpCodeBox : UserControl
    {
        public event EventHandler<string> OtpCompleted;
        private static readonly Regex DigitRegex = new Regex("[0-9]");

        public OtpCodeBox()
        {
            InitializeComponent();
            Digit1.Focus();
        }

        private void Digit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox) || string.IsNullOrEmpty(textBox.Text))
                return;

            if (textBox == Digit1 && textBox.Text.Length == 1)
                Digit2.Focus();
            else if (textBox == Digit2 && textBox.Text.Length == 1)
                Digit3.Focus();
            else if (textBox == Digit3 && textBox.Text.Length == 1)
                Digit4.Focus();
            else if (textBox == Digit4 && textBox.Text.Length == 1)
                Digit5.Focus();
            else if (textBox == Digit5 && textBox.Text.Length == 1)
                Digit6.Focus();

            CheckForCompletion();
        }

        private void Digit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !DigitRegex.IsMatch(e.Text);
            if (e.Text.Length > 1 && DigitRegex.IsMatch(e.Text))
            {
                TryFillFromPaste(e.Text);
                e.Handled = true;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Back)
            {
                if (Keyboard.FocusedElement is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
                {
                    if (textBox == Digit6)
                        Digit5.Focus();
                    else if (textBox == Digit5)
                        Digit4.Focus();
                    else if (textBox == Digit4)
                        Digit3.Focus();
                    else if (textBox == Digit3)
                        Digit2.Focus();
                    else if (textBox == Digit2)
                        Digit1.Focus();
                }
            }
        }

        private void TryFillFromPaste(string text)
        {
            var digits = text.Where(char.IsDigit).Take(6).ToArray();

            if (digits.Length >= 6)
            {
                Digit1.Text = digits[0].ToString();
                Digit2.Text = digits[1].ToString();
                Digit3.Text = digits[2].ToString();
                Digit4.Text = digits[3].ToString();
                Digit5.Text = digits[4].ToString();
                Digit6.Text = digits[5].ToString();
                Digit6.Focus();
                Digit6.SelectionStart = 1;
            }
        }

        private void CheckForCompletion()
        {
            if (!string.IsNullOrEmpty(Digit1.Text) &&
                !string.IsNullOrEmpty(Digit2.Text) &&
                !string.IsNullOrEmpty(Digit3.Text) &&
                !string.IsNullOrEmpty(Digit4.Text) &&
                !string.IsNullOrEmpty(Digit5.Text) &&
                !string.IsNullOrEmpty(Digit6.Text))
            {
                string otp = Digit1.Text + Digit2.Text + Digit3.Text + Digit4.Text + Digit5.Text + Digit6.Text;
                OtpCompleted?.Invoke(this, otp);
            }
        }

        public string GetOtpValue()
        {
            return Digit1.Text + Digit2.Text + Digit3.Text + Digit4.Text + Digit5.Text + Digit6.Text;
        }

        public void Clear()
        {
            Digit1.Text = string.Empty;
            Digit2.Text = string.Empty;
            Digit3.Text = string.Empty;
            Digit4.Text = string.Empty;
            Digit5.Text = string.Empty;
            Digit6.Text = string.Empty;
            Digit1.Focus();
        }
    }
}