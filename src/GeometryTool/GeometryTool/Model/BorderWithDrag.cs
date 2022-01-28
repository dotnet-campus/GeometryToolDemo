using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryTool
{
    /// <summary>
    ///     主要让border添加了拖动响应，用于点
    /// </summary>
    public class BorderWithDrag : Border
    {
        /// <summary>
        ///     依赖属性，用于表示该位置是否含有多个点，来控制Adorner的显示
        /// </summary>
        public static readonly DependencyProperty HasOtherPointProperty =
            DependencyProperty.Register("HasOtherPoint", typeof(bool), typeof(BorderWithDrag),
                new FrameworkPropertyMetadata(false, null));

        public BorderWithDrag BrotherBorder; //表示同一图层中

        private readonly List<BorderWithDrag> BWDList = new();
        private bool CanLock; //表示是否可以拖动该店
        public List<Path> EllipseList; //用于表示所在的图形中所拥有的点集
        public Path GeometryPath; //表示点所在的图形
        private bool isDragDropInEffect; //表示是否可以拖动
        public LockAdorner lockAdornor; //表示控件的装饰器
        public int Number; //表示这是图形中第几个点
        public List<BorderWithDrag> PointList = new();
        public Point Pt; //表示当前点的位置

        /// <summary>
        ///     无参数的构造函数，主要是为了给Border响应鼠标的事件
        /// </summary>
        public BorderWithDrag()
        {
            MouseLeftButtonDown += Element_MouseLeftButtonDown;
            MouseMove += Element_MouseMove;
            MouseLeftButtonUp += Element_MouseLeftButtonUp;
        }

        /// <summary>
        ///     有参数的构造函数，主要添加了点的删除功能
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Number"></param>
        /// <param name="EllipseList"></param>
        public BorderWithDrag(Path path, int Number, List<Path> EllipseList)
        {
            MouseLeftButtonDown += Element_MouseLeftButtonDown;
            MouseMove += Element_MouseMove;
            MouseLeftButtonUp += Element_MouseLeftButtonUp;
            GeometryPath = path;
            this.Number = Number;
            this.EllipseList = EllipseList;

            ContextMenu = new ContextMenu();
            var DeleteItem = new MenuItem();
            DeleteItem.Header = "删除";
            DeleteItem.Click += DeletedItem_Click;
            ContextMenu.Items.Add(DeleteItem);
        }

        public bool HasOtherPoint
        {
            get => (bool)GetValue(HasOtherPointProperty);
            set => SetValue(HasOtherPointProperty, value);
        }

        /// <summary>
        ///     拖动鼠标，移动控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseMove(object sender, MouseEventArgs e)
        {
            //判断是否在选择模式下的操作以及是否可以拖动
            if (isDragDropInEffect && MainWindow.ActionMode == "Select")
            {
                var _oldPoint = e.GetPosition(MainWindow.myRootCanvas); //获取目前鼠标的相对位置
                var _pt = new AutoPoints().GetAutoAdsorbPoint(_oldPoint); //计算最近的网格的位置
                Pt = _pt;
                var _currEle = sender as FrameworkElement;
                var _borderWithDrag = _currEle as BorderWithDrag;
                var _path = _borderWithDrag.Child as Path;
                var ellipse = _path.Data as EllipseGeometry;

                ellipse.Center = new Point { X = _pt.X, Y = _pt.Y };
            }
        }

        /// <summary>
        ///     鼠标左击控件，使控件可以被拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //判断是否在选择模式下的操作
            if (MainWindow.ActionMode == "Select")
            {
                var _currEle = sender as FrameworkElement;

                //判断是否命中的是点
                if (((_currEle as BorderWithDrag).Child as Path).Data as EllipseGeometry != null)
                {
                    _currEle.CaptureMouse();
                    _currEle.Cursor = Cursors.Hand;
                    isDragDropInEffect = true; //设置可以拖动
                    CanLock = true;
                }

                ;

                e.Handled = true;
            }
        }

        /// <summary>
        ///     鼠标释放，使控件不能被拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //判断是否在选择模式下的操作以及是否可以拖动
            var _currEle = sender as FrameworkElement;
            if (isDragDropInEffect && MainWindow.ActionMode == "Select" && CanLock)
            {
                Pt = e.GetPosition((UIElement)sender); //获取当前鼠标的位置
                Pt = new AutoPoints().GetAutoAdsorbPoint(Pt); //修正该位置
                var _borderLock = new BorderLock(this); //进行点融合的判读
                _borderLock.Lock(Pt);
            }

            isDragDropInEffect = false;
            _currEle.ReleaseMouseCapture();
            CanLock = false;
        }

        /// <summary>
        ///     添加Border的删除功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeletedItem_Click(object sender, RoutedEventArgs e)
        {
            BWDList.Clear();
            VisualTreeHelper.HitTest(Parent as Canvas, null, //进行命中测试
                MyHitTestResult,
                new PointHitTestParameters(Pt));
            var _deleteAll = false;

            foreach (var item in BWDList)
            {
                if (item.HasOtherPoint)
                {
                    _deleteAll = true;
                    break;
                }
            }

            if (_deleteAll)
            {
                foreach (var item in BWDList)
                {
                    if (item.Number != 0)
                    {
                        DeletePoint(item);
                    }
                }
            }
            else
            {
                DeletePoint(this);
            }
        }

        /// <summary>
        ///     命中测试
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            var path = result.VisualHit as Path;
            if (path != null)
            {
                var border = path.Parent as BorderWithDrag;
                if (border != null) //如果命中的图形是BorderWithDrag，就Count++
                {
                    BWDList.Add(border);
                }
            }

            return HitTestResultBehavior.Continue;
        }

        /// <summary>
        ///     融合点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LockItem_Click(object sender, RoutedEventArgs e)
        {
            var _borderLock = new BorderLock(this);
            _borderLock.Lock(Pt);
            HasOtherPoint = true;
        }

        /// <summary>
        ///     拆分点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void unLockItem_Click(object sender, RoutedEventArgs e)
        {
            var _borderLock = new BorderLock();
            _borderLock.unLock(this);
            HasOtherPoint = false;
        }

        /// <summary>
        ///     删除不必要的点
        /// </summary>
        /// <param name="item"></param>
        public void DeletePoint(BorderWithDrag item)
        {
            if (item.Number != 0)
            {
                var _path = item.GeometryPath.Data as PathGeometry; //获取该Border所在PathGeometry
                var _pf = _path.Figures[0];
                if (item.Number != 1) //如果要删除的不是起点
                {
                    _pf.Segments.RemoveAt(item.Number - 2); //直接移除Segment
                }
                else //如果要删除的是起点
                {
                    var _pf2 = new PathFigure();
                    for (var i = 0; i < _pf.Segments.Count; ++i)
                    {
                        if (i == 0) //复制出另外一个PathFigure，令其起点为原来的PathFigure中的LineSegment的Point
                        {
                            var binding = new Binding("Center")
                                { Source = item.EllipseList[1].Data as EllipseGeometry };
                            BindingOperations.SetBinding(_pf2, PathFigure.StartPointProperty, binding);
                        }
                        else
                        {
                            _pf2.Segments.Add(_pf.Segments[i]);
                        }
                    }

                    _path.Figures.Add(_pf2);
                    _path.Figures.RemoveAt(0);
                }

                MainWindow.myRootCanvas.Children.Remove(item); //在窗体上移除该点

                item.EllipseList.RemoveAt(item.Number - 1);
                for (var j = 0; j < item.EllipseList.Count; ++j) //重新定位点集的位置
                {
                    var border = item.EllipseList[j].Parent as BorderWithDrag;
                    border.Number = j + 1;
                }
            }
        }
    }
}

namespace BitsOfStuff
{
}