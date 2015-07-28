using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GeometryTool
{
    /// <summary>
    /// 画板的缩放的Converter
    /// </summary>
    public class CanvasScaleTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value > 0)
            {
                return System.Convert.ToDouble(String.Format("{0:F}", (double)value)); //等于Slider的Value的十分之一
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
