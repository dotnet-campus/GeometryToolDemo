using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GeometryTool
{
    /// <summary>
    /// 用于Int和Bool之间的转换，其中0代表false，1代表true
    /// </summary>
    public class IntBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)//判断是否为空
            {
                if ((int)value ==0)      
                    return false;
                else
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)//判断是否为空
            {
                if ((int)value == 0)
                    return false;
                else
                    return true;
            }
            return false;
        }
    }
}
