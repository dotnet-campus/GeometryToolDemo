using System;
using System.Globalization;
using System.Windows.Data;

namespace GeometryTool;

/// <summary>
///     线条粗细的转换
/// </summary>
internal class SliderConverterToDouble : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null) //判断是否为空
        {
            var sliderValue = (double)value;
            if (sliderValue <= 1)
            {
                return sliderValue * 10;
            }

            return sliderValue;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null) //判断是否为空
        {
            var sliderValue = (double)value;
            if (sliderValue < 1)
            {
                return sliderValue;
            }

            return System.Convert.ToDouble(sliderValue / 10);
        }

        return null;
    }
}