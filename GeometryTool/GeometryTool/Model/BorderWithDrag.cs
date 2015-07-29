using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GeometryTool
{
    /// <summary>
    /// 主要让border添加了拖动响应，用于点
    /// </summary>
    public class BorderWithDrag : Border
    {
        public LockAdorner lockAdornor;     //表示控件的装饰器
        public Path path;
        public BorderWithDrag BrotherBorder;
        public BorderWithDrag FirstBrotherBorder;
        bool isDragDropInEffect = false;    //表示是否可以拖动
        public Binding binding;
        public List<BorderWithDrag> PointList = new List<BorderWithDrag>();
        System.Windows.Point p;
        int count;
        /// <summary>
        /// 无参数的构造函数，主要是为了给Border响应鼠标的事件
        /// </summary>
        public BorderWithDrag()
        {
            this.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            this.MouseMove += Element_MouseMove;
            this.MouseLeftButtonUp += Element_MouseLeftButtonUp;
          
        }

        /// <summary>
        ///拖动鼠标，移动控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseMove(object sender, MouseEventArgs e)
        {
            //判断是否在选择模式下的操作以及是否可以拖动
            if (isDragDropInEffect && MainWindow.ActionMode == "Select")
            {
                Point oldPoint = e.GetPosition(MainWindow.myRootCanvas); //获取目前鼠标的相对位置
                p = (new AutoPoints()).GetAutoAdsorbPoint(oldPoint);    //计算最近的网格的位置

                FrameworkElement currEle = sender as FrameworkElement;
                BorderWithDrag myBorder = currEle as BorderWithDrag;
                ((myBorder.Child as Path).Data as EllipseGeometry).Center = new Point() { X = p.X , Y = p.Y  };
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标左击控件，使控件可以被拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //判断是否在选择模式下的操作
            if (MainWindow.ActionMode == "Select")
            {
                FrameworkElement currEle = sender as FrameworkElement;
                
                //判断是否命中的是点
                if (((currEle as BorderWithDrag).Child as Path).Data as EllipseGeometry != null)
                {
                    currEle.CaptureMouse();
                    currEle.Cursor = Cursors.Hand;
                    isDragDropInEffect = true;      //设置可以拖动
                };
                
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标释放，使控件不能被拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       public void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //判断是否在选择模式下的操作以及是否可以拖动
            FrameworkElement currEle = sender as FrameworkElement;
            if (isDragDropInEffect&&MainWindow.ActionMode == "Select")
            {
                Point pt = e.GetPosition((UIElement)sender);
                pt = (new AutoPoints()).GetAutoAdsorbPoint(pt);
                count = 0;
                PointList.Clear();
                VisualTreeHelper.HitTest(this.Parent as Canvas, null,   //进行命中测试
                    new HitTestResultCallback(MyHitTestResult),
                    new PointHitTestParameters(pt));

               
                if (count > 1)      //如果该点有多于两个BorderWithDrag，说明有点融合
                {
                    if (BrotherBorder == null)
                    {
                        BrotherBorder = this;
                    }
                    if (BrotherBorder.lockAdornor == null)  //只选择一个图层来显示Adorner
                    {
                        BrotherBorder.lockAdornor = new LockAdorner(this);
                        AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.Parent as Canvas);
                        if (layer != null)
                        {
                            layer.Add(lockAdornor);
                        }
                    }

                    BrotherBorder.HasOtherPoint = true;     //显示锁
                    BrotherBorder.lockAdornor.chrome.Source = new BitmapImage(new Uri("Image/lock.png", UriKind.Relative));

                    BrotherBorder.PointList = PointList;    //把所有的Border都绑定到显示Adorner的Border的Point上
                    foreach (BorderWithDrag border in PointList)
                    {
                        if (border != BrotherBorder)
                        {
                            binding = new Binding("Center") { Source = ((BrotherBorder.Child as Path).Data as EllipseGeometry) };
                            binding.Mode = BindingMode.TwoWay;
                            BindingOperations.SetBinding(((border.Child as Path).Data as EllipseGeometry), EllipseGeometry.CenterProperty, binding);
                        }
                    }
                }
                else
                {
                    this.HasOtherPoint = false;
                }

                isDragDropInEffect = false;
                currEle.ReleaseMouseCapture();
                e.Handled=true;
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
               if (border != null)
               {
                   PointList.Add(border);
                   count++;
                   if (border.lockAdornor != null)
                   {
                       BrotherBorder = border;
                   }
               }
           }
               
           return HitTestResultBehavior.Continue;
       }

       public static readonly DependencyProperty HasOtherPointProperty =
           DependencyProperty.Register("HasOtherPoint", typeof(bool), typeof(BorderWithDrag),
           new FrameworkPropertyMetadata(false, null));

       public bool HasOtherPoint
       {
           get
           {
               return (bool)this.GetValue(HasOtherPointProperty);
           }
           set
           {
               this.SetValue(HasOtherPointProperty, value);
           }
       }

       public static readonly DependencyProperty IsLockProperty =
              DependencyProperty.Register("IsLock", typeof(bool), typeof(BorderWithDrag),
              new FrameworkPropertyMetadata(false, null));

       public bool IsLock
       {
           get
           {
               return (bool)this.GetValue(IsLockProperty);
           }
           set
           {
               this.SetValue(IsLockProperty, value);
           }
       }
    }
}

namespace BitsOfStuff { }
