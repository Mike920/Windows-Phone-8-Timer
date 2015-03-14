using System;
using System.Windows.Data;

namespace Timer
{
    public class TimerBoolToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool) value)
                return "/Images/pause.png";
            else
                return "/Images/play.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
