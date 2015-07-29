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
        public int Number;
        public BorderWithDrag BrotherBorder;
        bool isDragDropInEffect = false;    //表示是否可以拖动
        public List<BorderWithDrag> PointList = new List<BorderWithDrag>();
        public Point Pt;
        bool CanLock=false;

        /// <summary>
        /// 依赖属性，用于表示该位置是否含有多个点，来控制Adorner的显示
        /// </summary>
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
            LockItem.Click += new RoutedEventHandler(LockItem_Click);
            this.ContextMenu.Items.Add(LockItem);

            MenuItem unLockItem = new MenuItem();
            unLockItem.Header = "取消组合";
            unLockItem.Click += new RoutedEventHandler(unLockItem_Click);
            this.ContextMenu.Items.Add(unLockItem);
        }

        public BorderWithDrag(Path path, int Number, List<Path> EllipseList)
        {
            this.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            this.MouseMove += Element_MouseMove;
            this.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            this.GeometryPath = path;
            this.Number = Number;
            this.EllipseList = EllipseList;

            this.ContextMenu = new ContextMenu();
            MenuItem LockItem = new MenuItem();
            Binding _isEnabledLockBinding = new Binding("HasOtherPoint") { Source = this,Converter=new IsEnbleBindingConverter() };
            LockItem.SetBinding(MenuItem.IsEnabledProperty, _isEnabledLockBinding);
            LockItem.Header = "组合";
            LockItem.Click += new RoutedEventHandler(LockItem_Click);
            this.ContextMenu.Items.Add(LockItem);

            MenuItem unLockItem = new MenuItem();
            Binding _isEnabledunLockBinding=new Binding("HasOtherPoint"){Source=this};
            unLockItem.SetBinding(MenuItem.IsEnabledProperty, _isEnabledunLockBinding);
            unLockItem.Header = "取消组合";
            unLockItem.Click += new RoutedEventHandler(unLockItem_Click);
            this.ContextMenu.Items.Add(unLockItem);

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
                Point _oldPoint = e.GetPosition(MainWindow.myRootCanvas); //获取目前鼠标的相对位置
                System.Windows.Point _pt = (new AutoPoints()).GetAutoAdsorbPoint(_oldPoint);    //计算最近的网格的位置

                FrameworkElement _currEle = sender as FrameworkElement;
                BorderWithDrag _borderWithDrag = _currEle as BorderWithDrag;
                ((_borderWithDrag.Child as Path).Data as EllipseGeometry).Center = new Point() { X = _pt.X, Y = _pt.Y };
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
                FrameworkElement _currEle = sender as FrameworkElement;
                
                //判断是否命中的是点
                if (((_currEle as BorderWithDrag).Child as Path).Data as EllipseGeometry != null)
                {
                    _currEle.CaptureMouse();
                    _currEle.Cursor = Cursors.Hand;
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
            FrameworkElement _currEle = sender as FrameworkElement;
            if (isDragDropInEffect&&MainWindow.ActionMode == "Select"&&CanLock)
            {
                Pt = e.GetPosition((UIElement)sender);        //获取当前鼠标的位置
                Pt = (new AutoPoints()).GetAutoAdsorbPoint(Pt);     //修正该位置
                BorderLock _borderLock = new BorderLock(this);      //进行点融合的判读
                _borderLock.Lock(Pt);
            }
            isDragDropInEffect = false;
            _currEle.ReleaseMouseCapture();
            CanLock = false;
           // e.Handled=true;
            
        }

       

        /// <summary>
        /// 添加Border的删除功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       public void DeletedItem_Click(object sender, RoutedEventArgs e)
       {
           PathGeometry _path = GeometryPath.Data as PathGeometry;   //获取该Border所在PathGeometry
           PathFigure _pf = _path.Figures[0];
           if (Number != 1)                             //如果要删除的不是起点
           {
               _pf.Segments.RemoveAt(Number - 2);        //直接移除Segment
           }
           else                                         //如果要删除的是起点
           {
               PathFigure _pf2 = new PathFigure();
               for (int i=0;i<_pf.Segments.Count;++i)
               {
                   if (i == 0)                          //复制出另外一个PathFigure，令其起点为原来的PathFigure中的LineSegment的Point
                   {
                       Binding binding = new Binding("Center") { Source=(EllipseList[1].Data as EllipseGeometry)};
                       BindingOperations.SetBinding(_pf2,PathFigure.StartPointProperty,binding);
                   }
                   else
                   {
                       _pf2.Segments.Add(_pf.Segments[i]);
                   }
               }
               _path.Figures.Add(_pf2);
               _path.Figures.RemoveAt(0);
           }

           MainWindow.myRootCanvas.Children.Remove(this); //在窗体上移除该点
           EllipseList.RemoveAt(Number-1);                //在该图形中的点集中移除该点

           for (int j = 0; j < EllipseList.Count; ++j)      //重新定位点集的位置
           {
               BorderWithDrag border = EllipseList[j].Parent as BorderWithDrag;
               border.Number = j + 1;
           }
       }
       
        /// <summary>
        /// 融合点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LockItem_Click(object sender, RoutedEventArgs e) 
        {
            BorderLock _borderLock = new BorderLock(this);
            _borderLock.Lock(Pt);
            this.HasOtherPoint = true;
        }

        /// <summary>
        /// 拆分点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void unLockItem_Click(object sender, RoutedEventArgs e)
        {
            BorderLock _borderLock = new BorderLock();
            _borderLock.unLock(this);
            this.HasOtherPoint = false;
        }  
    }
}

namespace BitsOfStuff { }
