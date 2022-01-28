using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeometryTool
{
    /// <summary>
    /// 点的融合和拆分
    /// </summary>
    public class BorderLock
    {
        public BorderWithDrag Border;   //用于记录要进行命中测试的图形
        int Count = 0;                  //用于计算命中的BorderWithDrag的数量
        public BorderLock(BorderWithDrag border)
        {
            Border = border;
        }

        public BorderLock(){}
        /// <summary>
        /// 点的合并
        /// </summary>
        /// <param name="point"></param>
        public void Lock(Point point)
        {
            Point _pt = point;
            
            Border.PointList.Clear();
            VisualTreeHelper.HitTest(Border.Parent as Canvas, null,   //进行命中测试
                new HitTestResultCallback(MyHitTestResult),
                new PointHitTestParameters(_pt));


            if (Count > 1)      //如果该点有多于两个BorderWithDrag，说明有点融合
            {
                if (Border.BrotherBorder == null)
                {
                    Border.BrotherBorder = Border.PointList[1];
                }
                if (Border.BrotherBorder.lockAdornor == null)  //只选择一个图层来显示Adorner
                {
                    Border.BrotherBorder.lockAdornor = new LockAdorner(Border.BrotherBorder);
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(Border.BrotherBorder.Parent as Canvas);
                    if (layer != null)
                    {
                        layer.Add(Border.BrotherBorder.lockAdornor);
                    }
                }

                Border.BrotherBorder.HasOtherPoint = true;     //显示锁
                Border.BrotherBorder.lockAdornor.chrome.Source = new BitmapImage(new Uri("Image/lock.png", UriKind.Relative));

                if (Border != Border.BrotherBorder)
                {
                    Border.BrotherBorder.PointList = Border.PointList;
                }
                foreach (BorderWithDrag border in Border.BrotherBorder.PointList)
                {
                    if (border != Border.BrotherBorder)
                    {
                        BindingOperations.ClearBinding(((border.Child as Path).Data as EllipseGeometry), EllipseGeometry.CenterProperty);
                        Binding binding = new Binding("Center") { Source = ((Border.BrotherBorder.Child as Path).Data as EllipseGeometry) };
                        binding.Mode = BindingMode.TwoWay;
                        BindingOperations.SetBinding(((border.Child as Path).Data as EllipseGeometry), EllipseGeometry.CenterProperty, binding);
                    }
                }
            }
            else
            {
                Border.HasOtherPoint = false;
            }
        }

        /// <summary>
        /// 命中测试
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {

            Path path = result.VisualHit as Path;
            if (path != null)
            {
                BorderWithDrag border = path.Parent as BorderWithDrag;
                if (border != null)         //如果命中的图形是BorderWithDrag，就Count++
                {
                    Border.PointList.Add(border);
                    Count++;
                    if (border.lockAdornor != null) //选择一个已经带有Adorner的Border来显示锁
                    {
                        Border.BrotherBorder = border;
                    }
                }
            }

            return HitTestResultBehavior.Continue;
        }

        /// <summary>
        /// 解开点的融合
        /// </summary>
        /// <param name="border"></param>
        public void unLock(BorderWithDrag border)
        {
            BorderWithDrag brotherBorder = border.BrotherBorder;
            Point p = ((border.Child as Path).Data as EllipseGeometry).Center;
            foreach (BorderWithDrag item in border.PointList)   //解开所用的Binding
            {
                BindingOperations.ClearBinding(((item.Child as Path).Data as EllipseGeometry), EllipseGeometry.CenterProperty);
                item.BrotherBorder = null;
                ((item.Child as Path).Data as EllipseGeometry).Center = p;  //重定位到原来的位置
            }
            border.HasOtherPoint = false;
        }
    }
}
