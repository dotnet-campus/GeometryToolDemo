using System;
using System.Windows.Data;

namespace GeometryTool
{
    /// <summary>
    /// 线条粗细的转换
    /// </summary>
    class SliderConverterToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)      //判断是否为空
            {
                double sliderValue = (double)value;
                if (sliderValue <= 1)
                    return sliderValue*10;
                return sliderValue;
            }
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)      //判断是否为空
            {
                double sliderValue = (double)value;
                if (sliderValue < 1)
                    return sliderValue;
                return System.Convert.ToDouble(sliderValue/10);
            }
            return null;
        }
    }
}
