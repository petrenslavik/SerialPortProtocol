using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Filmobus_test.Converters
{
    public class IntArrayToStringConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is int[]))
                return null;
            int[] val = values[0] == null ? new int[16] : (int[]) values[0];
            bool isHex = (bool?)values[1] == true;
            var str = "";
            if (isHex)
            {
                str = val.Aggregate(str, (current, value) => current + $"{value:X} |");
            }
            else
            {
                str = val.Aggregate(str, (current, value) => current + $"{value} |");
            }

            return str;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
