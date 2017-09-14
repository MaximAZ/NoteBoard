using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace NoteBoard
{
    public class NumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is double)
            {
                return string.Format("{0:n2}", value);
            }
            else
                return "1.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                double val;
                if (double.TryParse((string)value, out val))
                {
                    if (val > 0)
                        return val;
                }
            }
            return 1.00;
        }
    }
}
