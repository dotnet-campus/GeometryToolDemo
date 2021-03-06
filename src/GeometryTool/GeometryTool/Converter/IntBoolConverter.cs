using System;
using System.Globalization;
using System.Windows.Data;

namespace GeometryTool;

/// <summary>
///     用于Int和Bool之间的转换，其中0代表false，1代表true
/// </summary>
public class IntBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value != null) //判断是否为空
            {
                if ((int)value == 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
        catch
        {
            if (value != null) //判断是否为空
            {
                if ((bool)value)
                {
                    return 1;
                }

                return 0;
            }

            return 0;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value != null) //判断是否为空
            {
                if ((int)value == 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
        catch
        {
            if (value != null) //判断是否为空
            {
                if ((bool)value)
                {
                    return 1;
                }

                return 0;
            }

            return 0;
        }
    }
}