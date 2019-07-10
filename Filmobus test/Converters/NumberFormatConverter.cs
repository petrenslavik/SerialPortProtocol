using System;
using System.Globalization;
using System.Windows.Data;

namespace Filmobus_test.Converters
{
    public class NumberFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isHex = (bool?) values[1] == true;
            if (values[0] is int)
            {
                int val = (int?) values[0] ?? 0;
                if (isHex)
                {
                    return val.ToString("X");
                }

                return val.ToString();
            }

            if (values[0] is int[])
            {
                var arr = (int[]) values[0];
                var str = string.Empty;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (isHex)
                    {
                        str += arr[i].ToString("X");
                    }
                    else
                    {
                        str += arr[i].ToString();
                    }

                    if (i - 1 != arr.Length)
                    {
                        str += " | ";
                    }
                }

                return str;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
