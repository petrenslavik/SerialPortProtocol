using System;
using System.Globalization;
using System.Text;
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
                var sb = new StringBuilder(16);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (isHex)
                    {
                        sb.AppendFormat("{0:X}",arr[i]);
                    }
                    else
                    {
                        sb.Append(arr[i]);
                    }

                    if (i - 1 != arr.Length)
                    {
                        sb.Append(" | ");
                    }
                }

                return sb.ToString();
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
