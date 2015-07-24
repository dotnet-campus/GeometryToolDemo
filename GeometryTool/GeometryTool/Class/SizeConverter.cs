using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GeometryTool
{
    public class CircleSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Point startPoint = (Point)value ;
            Point endPoint = (Point)parameter;
            if (startPoint != null && endPoint!=null)
            {
                double x = Math.Abs(startPoint.X - endPoint.X);
                double y=Math.Abs(startPoint.Y-endPoint.Y);
                double width=Math.Sqrt(x*x+y*y);
                return new Size() { Width = width/2.0 ,Height=width/2.0};
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
