using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GeometryTool
{
    public class ImageLockSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return new BitmapImage(new Uri(@"E:\暑期考核\GeometryTool\GeometryTool\Image\lock.png", UriKind.Absolute));
            else
                return new BitmapImage(new Uri(@"E:\暑期考核\GeometryTool\GeometryTool\Image\unlock.png", UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
