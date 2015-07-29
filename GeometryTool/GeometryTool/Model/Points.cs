using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
namespace GeometryTool
{
    public class AutoPoints 
    {
        public Point GetAutoAdsorbPoint(Point oldPoint)
        {
            Point p = new Point();
            if (oldPoint.X * 10 % 10 >= 5)
            {
                int b = ((int)(oldPoint.X));           //计算离其最近的一个X坐标
                p.X = ((int)(oldPoint.X) + 1);
               
            }
            else
            {
                p.X = ((int)(oldPoint.X));
            }

            if (oldPoint.Y * 10 % 10 >= 5)              //计算离其最近的一个Y坐标
            {
                p.Y = ((int)(oldPoint.Y) + 1);
               
            }
            else
            {
                p.Y = ((int)(oldPoint.Y));
            }
            return p;
        }
    }
}
