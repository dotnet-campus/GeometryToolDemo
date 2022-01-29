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

        /// <summary>
        /// 表示同一图层中
        /// </summary>
        public BorderWithDrag BrotherBorder;

        private readonly List<BorderWithDrag> _borderWithDragList = new();
        /// <summary>
        /// 表示是否可以拖动该点
        /// </summary>
        private bool _canLock;
        /// <summary>
        /// 用于表示所在的图形中所拥有的点集
        /// </summary>
        private readonly List<Path> _ellipseList;
        /// <summary>
        /// 表示点所在的图形
        /// </summary>
        private readonly Path _geometryPath;
        /// <summary>
        /// 表示是否可以拖动
        /// </summary>
        private bool _isDragDropInEffect;
        /// <summary>
        /// 表示控件的装饰器
        /// </summary>
        public LockAdorner LockAdornor { set; get; }
        /// <summary>
        /// 表示这是图形中第几个点
        /// </summary>
        private int _number;
        public List<BorderWithDrag> PointList = new();
        /// <summary>
        /// 表示当前点的位置
        /// </summary>
        private Point _currentPoint;

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
        /// <param name="number"></param>
        /// <param name="ellipseList"></param>
        public BorderWithDrag(Path path, int number, List<Path> ellipseList)
        {
            MouseLeftButtonDown += Element_MouseLeftButtonDown;
            MouseMove += Element_MouseMove;
            MouseLeftButtonUp += Element_MouseLeftButtonUp;
            _geometryPath = path;
            this._number = number;
            this._ellipseList = ellipseList;

            ContextMenu = new ContextMenu();
            var deleteItem = new MenuItem();
            deleteItem.Header = "删除";
            deleteItem.Click += DeletedItem_Click;
            ContextMenu.Items.Add(deleteItem);
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
            if (_isDragDropInEffect && MainWindow.ActionMode == "Select")
            {
                var oldPoint = e.GetPosition(MainWindow.MyRootCanvas); //获取目前鼠标的相对位置
                var point = new AutoPoints().GetAutoAdsorbPoint(oldPoint); //计算最近的网格的位置
                _currentPoint = point;
                var currentFrameworkElement = sender as FrameworkElement;
                var borderWithDrag = currentFrameworkElement as BorderWithDrag;
                var path = borderWithDrag.Child as Path;
                var ellipse = path.Data as EllipseGeometry;

                ellipse.Center = new Point { X = point.X, Y = point.Y };
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
                var currentFrameworkElement = sender as FrameworkElement;

                //判断是否命中的是点
                if (((currentFrameworkElement as BorderWithDrag).Child as Path).Data as EllipseGeometry != null)
                {
                    currentFrameworkElement.CaptureMouse();
                    currentFrameworkElement.Cursor = Cursors.Hand;
                    _isDragDropInEffect = true; //设置可以拖动
                    _canLock = true;
                }

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
            var currentFrameworkElement = sender as FrameworkElement;
            if (_isDragDropInEffect && MainWindow.ActionMode == "Select" && _canLock)
            {
                _currentPoint = e.GetPosition((UIElement)sender); //获取当前鼠标的位置
                _currentPoint = new AutoPoints().GetAutoAdsorbPoint(_currentPoint); //修正该位置
                var borderLock = new BorderLock(this); //进行点融合的判读
                borderLock.Lock(_currentPoint);
            }

            _isDragDropInEffect = false;
            currentFrameworkElement.ReleaseMouseCapture();
            _canLock = false;
        }

        /// <summary>
        ///     添加Border的删除功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeletedItem_Click(object sender, RoutedEventArgs e)
        {
            _borderWithDragList.Clear();
            VisualTreeHelper.HitTest(Parent as Canvas, null, //进行命中测试
                MyHitTestResult,
                new PointHitTestParameters(_currentPoint));
            var deleteAll = false;

            foreach (var item in _borderWithDragList)
            {
                if (item.HasOtherPoint)
                {
                    deleteAll = true;
                    break;
                }
            }

            if (deleteAll)
            {
                foreach (var item in _borderWithDragList)
                {
                    if (item._number != 0)
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
                    _borderWithDragList.Add(border);
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
            var borderLock = new BorderLock(this);
            borderLock.Lock(_currentPoint);
            HasOtherPoint = true;
        }

        /// <summary>
        ///     拆分点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UnLockItem_Click(object sender, RoutedEventArgs e)
        {
            var borderLock = new BorderLock();
            borderLock.UnLock(this);
            HasOtherPoint = false;
        }

        /// <summary>
        ///     删除不必要的点
        /// </summary>
        /// <param name="item"></param>
        public void DeletePoint(BorderWithDrag item)
        {
            if (item._number != 0)
            {
                var path = item._geometryPath.Data as PathGeometry; //获取该Border所在PathGeometry
                var pathFigure = path.Figures[0];
                if (item._number != 1) //如果要删除的不是起点
                {
                    pathFigure.Segments.RemoveAt(item._number - 2); //直接移除Segment
                }
                else //如果要删除的是起点
                {
                    var pathFigure2 = new PathFigure();
                    for (var i = 0; i < pathFigure.Segments.Count; ++i)
                    {
                        if (i == 0) //复制出另外一个PathFigure，令其起点为原来的PathFigure中的LineSegment的Point
                        {
                            var binding = new Binding("Center")
                                { Source = item._ellipseList[1].Data as EllipseGeometry };
                            BindingOperations.SetBinding(pathFigure2, PathFigure.StartPointProperty, binding);
                        }
                        else
                        {
                            pathFigure2.Segments.Add(pathFigure.Segments[i]);
                        }
                    }

                    path.Figures.Add(pathFigure2);
                    path.Figures.RemoveAt(0);
                }

                MainWindow.MyRootCanvas.Children.Remove(item); //在窗体上移除该点

                item._ellipseList.RemoveAt(item._number - 1);
                for (var j = 0; j < item._ellipseList.Count; ++j) //重新定位点集的位置
                {
                    var border = item._ellipseList[j].Parent as BorderWithDrag;
                    border._number = j + 1;
                }
            }
        }
    }
}

namespace BitsOfStuff
{
}