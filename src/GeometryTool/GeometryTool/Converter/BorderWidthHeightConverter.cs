using System;
using System.Globalization;
using System.Windows.Data;

namespace GeometryTool;

/// <summary>
///     用于使画板自动适应屏幕的大小
/// </summary>
public class BorderWidthHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((double)value > 0)
        {
            return (double)value - System.Convert.ToDouble(parameter);
        }

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}