using System;
using System.Windows.Data;

namespace GeometryTool
{
    class SliderConverterToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)      //判断是否为空
            {
                double sliderValue = (double)value;
                return System.Convert.ToDouble(String.Format("{0:F}", sliderValue));
            }
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
