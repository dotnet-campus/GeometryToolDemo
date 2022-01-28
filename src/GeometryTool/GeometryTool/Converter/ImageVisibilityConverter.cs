using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeometryTool;

/// <summary>
///     用于控制所的显示或者隐藏
/// </summary>
internal class ImageVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
        {
            return Visibility.Visible;
        }

        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}