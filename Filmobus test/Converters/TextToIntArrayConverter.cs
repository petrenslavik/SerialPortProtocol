using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Filmobus_test.Converters
{
    public class TextToIntArrayConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;
            if (targetType == typeof(string))
            {
                var arr = value as int[];
                var sb = new StringBuilder(32);
                for (int i = 0; i < arr.Length; i++)
                {
                    sb.Append(arr[i]);
                }

                return sb.ToString();
            }

            if (targetType == typeof(int[]))
            {
                var str = value as string;
                var arr = new int[str.Length];
                for (int index = 0; index < str.Length; index++)
                {
                    arr[index] = str[index]-'0';
                }

                return arr;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;
            if (targetType == typeof(string))
            {
                var arr = value as int[];
                var sb = new StringBuilder(32);
                for (int i = 0; i < arr.Length; i++)
                {
                    sb.Append(arr[i]);
                }

                return sb.ToString();
            }

            if (targetType == typeof(int[]))
            {
                var str = value as string;
                var arr = new int[str.Length];
                for (int index = 0; index < str.Length; index++)
                {
                    arr[index] = str[index] - '0';
                }

                return arr;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
