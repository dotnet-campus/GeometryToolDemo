using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
namespace GeometryTool
{
    /// <summary>
    /// parameter代表Slider2,value代表Slider1
    /// </summary>
    public class DashArrayConverter1: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null&&parameter!=null)      //判断是否为空
            {
                
                DoubleCollection dc = new DoubleCollection();
                dc.Add((double)value);
                dc.Add((double)parameter);
                return dc;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// parameter代表Slider1,value代表Slider2
    /// </summary>
    public class DashArrayConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && parameter != null)      //判断是否为空
            {
                DoubleCollection dc = new DoubleCollection();
                dc.Add((double)parameter);
                dc.Add((double)value);
                return dc;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
