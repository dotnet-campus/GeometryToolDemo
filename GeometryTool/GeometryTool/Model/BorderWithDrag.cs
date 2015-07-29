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
using System.Windows.Controls.Primitives;

namespace GeometryTool
{
    /// <summary>
    /// 主要让border添加了拖动响应，用于点
    /// </summary>
    public class BorderWithDrag : Border
    {
        public LockAdorner lockAdornor;     //表示控件的装饰器
        public List<Path> EllipseList;      //用于表示所在的图形中所拥有的点集
        public Path path;
        public Path GeometryPath;           //表示点所在的图形
        public int number;
        public BorderWithDrag BrotherBorder;
        bool isDragDropInEffect = false;    //表示是否可以拖动
        public List<BorderWithDrag> PointList = new List<BorderWithDrag>();
        System.Windows.Point p;
        bool CanLock=false;
        int count;
        /// <summary>
        /// 无参数的构造函数，主要是为了给Border响应鼠标的事件
        /// </summary>
        public BorderWithDrag()
        {
            this.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            this.MouseMove += Element_MouseMove;
            this.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            this.ContextMenu = new ContextMenu();
            MenuItem LockItem = new MenuItem();
            LockItem.Header = "组合";
            LockItem.Click += new RoutedEventHandler(DeletedItem_Click);
            this.ContextMenu.Items.Add(LockItem);
            MenuItem unLockItem = new MenuItem();
            unLockItem.Header = "取消组合";
            unLockItem.Click += new RoutedEventHandler(DeletedItem_Click);
            this.ContextMenu.Items.Add(unLockItem);
        }

        public BorderWithDrag(Path path, int number, List<Path> EllipseList)
        {
            GeometryPath = path;
            this.EllipseList = EllipseList;
            this.number = number;
            this.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            this.MouseMove += Element_MouseMove;
            this.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            this.ContextMenu = new ContextMenu();

            MenuItem DeleteItem = new MenuItem();
            DeleteItem.Header = "删除";
            DeleteItem.Click += new RoutedEventHandler(DeletedItem_Click);
            this.ContextMenu.Items.Add(DeleteItem);
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
                    CanLock = true;
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
            if (isDragDropInEffect&&MainWindow.ActionMode == "Select"&&CanLock)
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
                            Binding binding = new Binding("Center") { Source = ((BrotherBorder.Child as Path).Data as EllipseGeometry) };
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
                CanLock = false;
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

        /// <summary>
        /// 添加Border的删除功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       public void DeletedItem_Click(object sender, RoutedEventArgs e)
       {
           PathGeometry path = GeometryPath.Data as PathGeometry;   //获取该Border所在PathGeometry
           PathFigure pf = path.Figures[0];
           if (number != 1)                             //如果要删除的不是起点
           {
               pf.Segments.RemoveAt(number - 2);        //直接移除Segment
           }
           else                                         //如果要删除的是起点
           {
               PathFigure pf2 = new PathFigure();
               for (int i=0;i<pf.Segments.Count;++i)
               {
                   if (i == 0)                          //复制出另外一个PathFigure，令其起点为原来的PathFigure中的LineSegment的Point
                   {
                       Binding binding = new Binding("Center") { Source=(EllipseList[1].Data as EllipseGeometry)};
                       BindingOperations.SetBinding(pf2,PathFigure.StartPointProperty,binding);
                   }
                   else
                   {
                       pf2.Segments.Add(pf.Segments[i]);
                   }
               }
               path.Figures.Add(pf2);
               path.Figures.RemoveAt(0);
           }

           MainWindow.myRootCanvas.Children.Remove(this); //在窗体上移除该点
           EllipseList.RemoveAt(number-1);                //在该图形中的点集中移除该点

           for (int j = 0; j < EllipseList.Count; ++j)      //重新定位点集的位置
           {
               BorderWithDrag border = EllipseList[j].Parent as BorderWithDrag;
               border.number = j + 1;
           }
       }
    }
}

namespace BitsOfStuff { }
