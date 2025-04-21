using System;
using System.Globalization;
using System.Windows.Data;

namespace UML.Class
{
    public class DownloadBar : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double progressValue))
                return 0;

            double totalWidth = 255;
            if (parameter != null && double.TryParse(parameter.ToString(), out double paramWidth))
            {
                totalWidth = paramWidth;
            }

            return (progressValue / 100) * totalWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}