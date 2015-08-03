using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

namespace GeometryTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Canvas myRootCanvas;      //表示装着当前图形的Canvas
        public static BorderWithAdorner SelectedBorder; //表示当前和之前选择中的图形
        public static BorderWithAdorner CopyBorderWA;   //用于存储剪切或者复制的图形
        public static int PasteCount;           //表示当前图形被复制了多少遍
        public static string ActionMode = "";   //表示当前鼠标的模式
        int GridSize = 0;                       //表示画板大小
        public int isStartPoint;                //绘制直线的时候，表示是否为第一个点
        System.Windows.Shapes.Path linePath;    //表示绘制直线的时候，直线的Path
        System.Windows.Shapes.Path ellipsePath; //表示绘制图形的时候，点所在Path
        GraphAdd graphAdd;                      //表示绘制动作的类
        GraphAppearance graphAppearance;        //表示图形的外观
        PathFigure pathFigure;                  //表示绘制直线的时候，直线所在的PathFigure
        System.Windows.Shapes.Path circlePath;  //表示绘制圆的时候，圆所在的Path
        bool isClose;                           //表示图形是否是闭合的
        System.Windows.Shapes.Path trianglePath;//表示绘制三角形的时候，三角形所在的Path
        System.Windows.Shapes.Path rectanglePath;//表示绘制正方形的时候，正方形所在的Path
        bool canMove = false;                   //表示图形是否可以拖动
        System.Windows.Shapes.Path ellipseGeometryPath; //表示绘制椭圆的时候，椭圆所在的Path
        System.Windows.Shapes.Path curvePath;   //表示绘制曲线的时候，曲线所在的Path
        System.Windows.Shapes.Path QBezierPath; //表示绘制二次方贝塞尔曲线时候，曲线所在的Path
        System.Windows.Shapes.Path BezierPath;  //表示绘制三次方贝塞尔曲线时候，曲线所在的Path
        List<System.Windows.Shapes.Path> EllipseList;


        public string FileName;

        private DrawingBrush _gridBrush;        //绘制网格时所使用的Brush
        /// <summary>
        /// 构造函数，用于初始化对象
        /// </summary>
        public MainWindow()
        {
            graphAppearance = new GraphAppearance();
            graphAppearance.StrokeThickness = 0.1;
            InitializeComponent();
            graphAdd = new GraphAdd();
            EllipseList = new List<System.Windows.Shapes.Path>();
            ellipsePath = new System.Windows.Shapes.Path();
            pathFigure = new PathFigure();
            isStartPoint = 0;
            linePath = new System.Windows.Shapes.Path();
            this.RootCanvas.Tag = "Select";
            MainWindow.ActionMode = "Select";
            this.WindowState = System.Windows.WindowState.Maximized;    //设置窗口最大化
            docCanvas_Loaded();
            StrokeCurrentColor.Background = graphAppearance.Stroke;
            FillCurrentColor.Background = graphAppearance.Fill;
            myRootCanvas = this.RootCanvas;
            CBGridSize.SelectedIndex = 3;
            CanvasChange.Value = 20;
            graphAppearance.Fill = Brushes.Transparent;
            FileName = "";
            PanProperty.DataContext = graphAppearance;
            MenuOptions.DataContext = this;
        }

        /// <summary>
        /// 当前鼠标的模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            //设置选定的模式
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                this.RootCanvas.Tag = radioButton.ToolTip;
                ActionMode = radioButton.ToolTip.ToString();
            }
            if (isStartPoint != 0 && pathFigure.Segments.Count > 0) //移除额外的线
            {
                pathFigure.Segments.RemoveAt(pathFigure.Segments.Count - 1);
            }
            if (this.RootCanvas.Tag.ToString() != "Point")          //移除划线功能
            {
                this.RootCanvas.RemoveHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                EllipseList = new List<System.Windows.Shapes.Path>();
                if (isStartPoint != 0)
                {
                    isStartPoint = 0;
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// 执行鼠标的操作，例如选择，添加点，连线等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p =(new AutoPoints()).GetAutoAdsorbPoint( Mouse.GetPosition(e.Source as FrameworkElement));   //获取鼠标当前位置

            if (this.RootCanvas.Tag.ToString() == "Point") //判断是不是画线
            {
                isClose = false;
                if (isStartPoint == 0)
                {
                    pathFigure = new PathFigure();
                    graphAdd.AddPointWithNoBorder(p, graphAppearance, this.RootCanvas, out ellipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref linePath, ellipsePath, ref isStartPoint, ref  pathFigure, isClose); //进行划线
                    this.RootCanvas.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                }
                else
                {
                    isStartPoint ++;
                    graphAdd.AddPointWithNoBorder(p, graphAppearance, this.RootCanvas, out ellipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref  linePath, ellipsePath, ref isStartPoint, ref  pathFigure, isClose); //进行划线
                }
                EllipseList.Add(ellipsePath);
                BorderWithDrag border = new BorderWithDrag(linePath, isStartPoint, EllipseList);
                    border.Child = ellipsePath;
                    this.RootCanvas.Children.Add(border);
                e.Handled = true;

            }
        }

        /// <summary>
        /// 鼠标移动的时候，画一条线段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawLine(object sender, MouseEventArgs e)
        {
            if (this.RootCanvas.Tag.ToString() == "Point")
            {
                //isStartPoint = 1;
                System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);   //获取鼠标当前位置
                graphAdd.AddHorvorLine(pathFigure, p); //进行划线
            }
        }

        /// <summary>
        /// 将图形保存为XML文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(FileName))
            {
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.ShowDialog();
                if (fbd.SelectedPath != string.Empty)
                    FileName = fbd.SelectedPath;
                FileName += "\\" + DateTime.Now.ToLongDateString()+".xml";
            }
            StreamWriter sw = new StreamWriter(FileName);
            GeomortyStringConverter GCXML = new GeomortyStringConverter(RootCanvas, graphAppearance);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            sb.AppendLine("<Canvase>");
            sb.AppendLine(" <Geometry>");
            sb.Append("     <Figures>");
            foreach (UIElement item in this.RootCanvas.Children)
            {
                BorderWithAdorner borderWA = item as BorderWithAdorner;  //点是有BorderWithDrag包含着的，图形是Path
                if (borderWA != null)
                {
                    sb.Append(GCXML.StringFromGeometry(borderWA.Child as System.Windows.Shapes.Path));            //构造Mini-Language
                   
                }
            }
            sb.AppendLine("</Figures>");
            sb.AppendLine(" </Geometry>");
            sb.AppendLine("</Canvase>");
            sw.Write(sb.ToString());
            sw.Close();
            MessageBox.Show(@"已保存到 "+FileName);
        }

        /// <summary>
        /// 修改图形的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canMove)
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint( e.GetPosition(RootCanvas));     //获取当前鼠标的位置
                if (this.RootCanvas.Tag.ToString() == "AddTriangle")                        //修改三角形的位置
                {
                    if (trianglePath != null)
                    {
                        PathGeometry triangle = trianglePath.Data as PathGeometry;
                        LineSegment line2 = triangle.Figures[0].Segments[1] as LineSegment;
                        Point oldPoint = line2.Point;
                        line2.Point = new Point() { X = p.X, Y = p.Y };
                        LineSegment line1 = triangle.Figures[0].Segments[0] as LineSegment;
                        oldPoint = triangle.Figures[0].StartPoint;
                        line1.Point = new Point() { X = oldPoint.X + (oldPoint.X - p.X), Y = p.Y };
                        e.Handled = true;
                    }
                }
                else if (this.RootCanvas.Tag.ToString() == "AddRectangular")                //修改矩形的位置
                {
                    PathGeometry triangle = rectanglePath.Data as PathGeometry;
                    Point oldPaint = triangle.Figures[0].StartPoint;

                    LineSegment line1 = triangle.Figures[0].Segments[0] as LineSegment;
                    line1.Point = new Point() { X = oldPaint.X, Y = p.Y };
                    LineSegment line2 = triangle.Figures[0].Segments[1] as LineSegment;
                    line2.Point = new Point() { X = p.X, Y = p.Y };
                    LineSegment line3 = triangle.Figures[0].Segments[2] as LineSegment;
                    line3.Point = new Point() { X = p.X, Y = oldPaint.Y };
                    e.Handled = true;
                }
                else if (this.RootCanvas.Tag.ToString() == "AddCircle")                     //修改圆的位置
                {
                    PathGeometry circel = circlePath.Data as PathGeometry;
                    ArcSegment line1 = circel.Figures[0].Segments[1] as ArcSegment;
                    line1.Point = new Point() { X = p.X, Y = p.Y };
                    e.Handled = true;
                }
                else if (this.RootCanvas.Tag.ToString() == "AddEllipse")                    //修改椭圆的位置
                {
                    PathGeometry circel = ellipseGeometryPath.Data as PathGeometry;
                    ArcSegment line1 = circel.Figures[0].Segments[0] as ArcSegment;
                    ArcSegment line2 = circel.Figures[0].Segments[1] as ArcSegment;
                    Point oldPoint1 = line1.Point;
                    Point oldPoint2 = line2.Point;
                    line1.Point = new Point() { X = p.X, Y = oldPoint1.Y };
                    if ((oldPoint2.X - oldPoint1.X) != 0 && (p.Y - oldPoint1.Y) != 0)         //保证被除数被为0
                    {
                        line1.Size = new Size() { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0, Height = Math.Abs(p.Y - oldPoint1.Y) };
                        line2.Size = new Size() { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0, Height = Math.Abs(p.Y - oldPoint1.Y) };
                    }
                    else if ((oldPoint2.X - oldPoint1.X) != 0)
                    {
                        line1.Size = new Size() { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0 };
                        line2.Size = new Size() { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0 };
                    }
                    else if ((p.Y - oldPoint1.Y) != 0)
                    {
                        line1.Size = new Size() { Height = Math.Abs(p.Y - oldPoint1.Y) };
                        line2.Size = new Size() { Height = Math.Abs(p.Y - oldPoint1.Y) };
                    }
                    e.Handled = true;
                }
                else if (this.RootCanvas.Tag.ToString() == "AddCurve")  //修改曲线的位置
                {
                    PathGeometry circel = curvePath.Data as PathGeometry;
                    ArcSegment line1 = circel.Figures[0].Segments[0] as ArcSegment;
                    line1.Point = new Point() { X = p.X, Y = p.Y };
                }
                else if (this.RootCanvas.Tag.ToString() == "QBezier")   //修改二次方贝塞尔曲线的位置
                {
                    PathGeometry QBezier = QBezierPath.Data as PathGeometry;
                    QuadraticBezierSegment qbSegment = QBezier.Figures[0].Segments[0] as QuadraticBezierSegment;
                    Point oldPoint = qbSegment.Point1;
                    Point oldPoint2 = qbSegment.Point2;
                    qbSegment.Point1 = new Point() { X = (p.X + oldPoint.X) / 2.0, Y = p.Y };
                    qbSegment.Point2 = new Point() { X = p.X, Y = oldPoint2.Y };
                }
                else if (this.RootCanvas.Tag.ToString() == "Bezier")    //修改三次方贝塞尔曲线的位置
                {
                    PathGeometry Bezier = BezierPath.Data as PathGeometry;
                    BezierSegment bSegment = Bezier.Figures[0].Segments[0] as BezierSegment;
                    Point oldPoint = bSegment.Point1;
                    Point oldPoint2 = bSegment.Point2;
                    Point oldPoint3 = bSegment.Point3;
                    bSegment.Point1 = new Point() { X = oldPoint.X, Y = p.Y };
                    bSegment.Point2 = new Point() { X = p.X, Y = p.Y };
                    bSegment.Point3 = new Point() { X = p.X, Y = oldPoint3.Y };
                }
            }
        }

        /// <summary>
        /// 鼠标左击时，拖动图形移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedBorder != null)       //隐藏之前点击的图形的选择框
                SelectedBorder.GAdorner.Visibility = Visibility.Hidden;
            if (this.RootCanvas.Tag.ToString() == "Select" )
            {
                System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);
                VisualTreeHelper.HitTest(RootCanvas, null,   //进行命中测试
                new HitTestResultCallback(MyHitTestResult),
                new PointHitTestParameters(p));
                if (SelectedBorder!=null&&SelectedBorder.Child != null)
                {
                    PanProperty.DataContext = SelectedBorder.Child;
                    LBNowSelected.Content = "图形属性";
                }
                else
                {
                    PanProperty.DataContext = graphAppearance;
                    LBNowSelected.Content = "画笔属性";
                }
            }
            else if (this.RootCanvas.Tag.ToString() == "AddTriangle")        //绘制三角形
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
                graphAdd.AddGeometry(p, graphAppearance, this.RootCanvas, out trianglePath, 3, true);
                PanProperty.DataContext = trianglePath;
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "AddRectangular")//绘制矩形
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
                graphAdd.AddGeometry(p, graphAppearance, this.RootCanvas, out rectanglePath, 4, true);
                PanProperty.DataContext = rectanglePath;
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "AddCircle")//绘制圆
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
                graphAdd.AddGeometryOfCricle(p, graphAppearance, this.RootCanvas, out circlePath);
                PanProperty.DataContext = circlePath;
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "AddEllipse")//绘制椭圆
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
                graphAdd.AddGeometryOfCricle(p, graphAppearance, this.RootCanvas, out ellipseGeometryPath);
                PanProperty.DataContext = ellipseGeometryPath;
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "AddCurve")//绘制曲线
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
                graphAdd.AddCurve(p, graphAppearance, this.RootCanvas, out curvePath);
                PanProperty.DataContext = curvePath;
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "QBezier")//绘制二次方贝塞尔曲线
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
                graphAdd.AddPointWithNoBorder(p, graphAppearance, RootCanvas, out ellipsePath);
                graphAdd.NewPathGeomotry(graphAppearance, RootCanvas, out QBezierPath, ellipsePath, false);
                BorderWithAdorner borderWA = new BorderWithAdorner();
                borderWA.Child = QBezierPath;
                borderWA.EllipseList.Add(ellipsePath);
                RootCanvas.Children.Add(borderWA);

                System.Windows.Shapes.Path ellipsePath2, ellipsePath3;
                graphAdd.AddPoint(p, graphAppearance, RootCanvas, out ellipsePath2);
                borderWA.EllipseList.Add(ellipsePath2);
                graphAdd.AddPoint(p, graphAppearance, RootCanvas, out ellipsePath3);
                borderWA.EllipseList.Add(ellipsePath3);

                PathGeometry path = QBezierPath.Data as PathGeometry;

                graphAdd.AddQuadraticSegment(path.Figures[0], ellipsePath3, ellipsePath2);
                BorderWithDrag border = new BorderWithDrag();
                border.Child = ellipsePath;
                this.RootCanvas.Children.Add(border);
                PanProperty.DataContext = QBezierPath;
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "Bezier")//绘制二次方贝塞尔曲线
            {
                System.Windows.Point p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
                graphAdd.AddPointWithNoBorder(p, graphAppearance, RootCanvas, out ellipsePath);
                graphAdd.NewPathGeomotry(graphAppearance, RootCanvas, out BezierPath, ellipsePath, false);
                BorderWithAdorner borderWA = new BorderWithAdorner();
                borderWA.Child = BezierPath;
                borderWA.EllipseList.Add(ellipsePath);
                RootCanvas.Children.Add(borderWA);

                System.Windows.Shapes.Path ellipsePath2, ellipsePath3, ellipsePath4;
                graphAdd.AddPoint(p, graphAppearance, RootCanvas, out ellipsePath2);
                borderWA.EllipseList.Add(ellipsePath2);
                graphAdd.AddPoint(p, graphAppearance, RootCanvas, out ellipsePath3);
                borderWA.EllipseList.Add(ellipsePath3);
                graphAdd.AddPoint(p, graphAppearance, RootCanvas, out ellipsePath4);
                borderWA.EllipseList.Add(ellipsePath4);
                PathGeometry path = BezierPath.Data as PathGeometry;

                graphAdd.AddBezierSegment(path.Figures[0], ellipsePath4, ellipsePath3, ellipsePath2);
                BorderWithDrag border = new BorderWithDrag();
                border.Child = ellipsePath;
                this.RootCanvas.Children.Add(border);
                PanProperty.DataContext = BezierPath;
                canMove = true;
            }
        }

        /// <summary>
        /// 图形更改为不能拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (canMove)        //图形更改为不能拖动
            {
                BorderWithAdorner borderWA = null;
                if (this.RootCanvas.Tag.ToString() == "AddCircle")//绘制圆
                {
                   borderWA = circlePath.Parent as BorderWithAdorner;
                    
                }
                else if (this.RootCanvas.Tag.ToString() == "AddEllipse")//绘制椭圆
                {
                   borderWA = ellipseGeometryPath.Parent as BorderWithAdorner;
                }
                if (borderWA != null)
                {
                    BorderWithDrag borderWD = borderWA.EllipseList[0].Parent as BorderWithDrag;
                    if (borderWD != null)
                    {
                        BorderLock borderLock = new BorderLock(borderWD);
                        borderLock.Lock(((borderWD.Child as System.Windows.Shapes.Path).Data as EllipseGeometry).Center);
                    }
                }

                canMove = false;
            }
        }

        /// <summary>
        /// 打开XML文件读取XML中的图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".xml";
            openFileDlg.Filter = "xml file|*.xml";      //只选择.xml文件
            if (openFileDlg.ShowDialog() == true)       //打开对话框
            {
                if (!string.IsNullOrEmpty(openFileDlg.FileName))    //如果文件名不为空
                {
                    XMLHelper xmlHelper = new XMLHelper();
                    GeomortyStringConverter GSC = new GeomortyStringConverter(RootCanvas, graphAppearance);
                    Match _match = xmlHelper.ReadXml(openFileDlg.FileName);    //读取XML文件
                    MatchCollection MatchList = Regex.Matches(_match.Groups[0].ToString(), @"M[\.\,\s\+\-\dLACQZ]+");
                    foreach (Match item in MatchList)
                    {
                        BorderWithAdorner borderWA = GSC.GeomotryFromString(item.Groups[0].ToString());                  //转化成为图形
                        RootCanvas.Children.Add(borderWA);              //把图形添加到Canvas中
                        foreach (var ellipse in borderWA.EllipseList)   //把点添加到Canvas中
                        {
                            BorderWithDrag borderWD = ellipse.Parent as BorderWithDrag;
                            RootCanvas.Children.Add(borderWD);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 设置Stroke的颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrokeColors_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null)
            {
                this.StrokeCurrentColor.Background = button.Background;
            }
        }

        /// <summary>
        /// 设置Fill的颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillColors_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null)
            {
                this.FillCurrentColor.Background = button.Background;
            }
        }

        /// <summary>
        /// 调整画布，进行缩放
        /// </summary>
        public static int multiple=1;
        private void CanvasChange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                multiple = (int)this.CanvasChange.Value ;
                double height = GridSize * (int)this.CanvasChange.Value+100; //计算画布放大后的坐标
                double width = GridSize * (int)this.CanvasChange.Value+100;
                this.CanvasBorder.Height = height >= CanvasBorder.ActualHeight ? height : RootGrid.ActualHeight - 115;  //修改Border的大小，使得其能显示放大后的画布
                this.CanvasBorder.Width = width >= CanvasBorder.ActualWidth ? width : RootGrid.ActualWidth - 215;
                docCanvas_Loaded();

                //Console.WriteLine("Height:{0} Width:{1} CB.Height{2} CB.Width{2} this.H：{3} this.W{4}", height, width, CanvasBorder.Height, CanvasBorder.Width, this.RootGrid.ActualHeight, this.RootGrid.ActualWidth);
            }
            catch { }
            
        }

     

        /// <summary>
        /// 用于绘制网格网格
        /// </summary>
        private void docCanvas_Loaded()
        {

            _gridBrush = new DrawingBrush(new GeometryDrawing(
                new SolidColorBrush(Colors.White),
                        new Pen(new SolidColorBrush(Colors.LightGray), 0.1),  //网格粗细为1
                            new RectangleGeometry(new Rect(0, 0, 1, 1))));   //绘制网格的右边和下边
            _gridBrush.Stretch = Stretch.None;
            _gridBrush.TileMode = TileMode.Tile;
            _gridBrush.Viewport = new Rect(0.0, 0.0, 1, 1);
            _gridBrush.ViewportUnits = BrushMappingMode.Absolute;
            RootCanvas.Background = _gridBrush;
 
        }

       

        /// <summary>
        /// 拖动Slider改变StrokeThickness
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderStyle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LineStyle.StrokeThickness = e.NewValue ;
        }

        /// <summary>
        /// 选择画布的大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem cbi = (ComboBoxItem)(sender as ComboBox).SelectedItem;
                if (cbi != null)
                {
                    RootCanvas.Height = Convert.ToInt32(cbi.Tag);
                    RootCanvas.Width = Convert.ToInt32(cbi.Tag) ;
                    GridSize = Convert.ToInt32(cbi.Tag);
                    double height = GridSize * (int)this.CanvasChange.Value + 200; //计算画布放大后的坐标
                    double width  = GridSize * (int)this.CanvasChange.Value + 200;
                    this.CanvasBorder.Height = height >= CanvasBorder.ActualHeight ? height : RootGrid.ActualHeight - 115;  //修改Border的大小，使得其能显示放大后的画布
                    this.CanvasBorder.Width = width >= CanvasBorder.ActualWidth ? width : RootGrid.ActualWidth - 215;
                    docCanvas_Loaded();
                }
            }
            catch { }
        }

        /// <summary>
        /// 右击鼠标，变成选择模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.ActionMode != "Select")
            {
                Select.IsChecked = true;            //更正鼠标模式为选择模式
                this.RootCanvas.Tag = "Select";
                EllipseList = new List<System.Windows.Shapes.Path>();
                ActionMode = "Select";
                if (isStartPoint != 0 && pathFigure.Segments.Count > 0) //移除额外的线
                {
                    pathFigure.Segments.RemoveAt(pathFigure.Segments.Count - 1);
                }
                if (this.RootCanvas.Tag.ToString() != "Point")          //移除划线功能
                {
                    this.RootCanvas.RemoveHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                    if (isStartPoint != 0)
                    {
                        isStartPoint = 0;
                    }
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// 显示被选中的图形的选择框
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            System.Windows.Shapes.Path path = result.VisualHit as System.Windows.Shapes.Path;
            if (path != null)
            {
                BorderWithAdorner borderWA = path.Parent as BorderWithAdorner;
                if (borderWA != null)
                {
                    borderWA.GAdorner.Visibility = Visibility.Visible;
                    SelectedBorder = borderWA;
                }
                return HitTestResultBehavior.Stop;
            }
            SelectedBorder = null;

            return HitTestResultBehavior.Continue;
        }

        private void StrokeDash1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            DoubleCollection oldStrokeDashArray =new DoubleCollection();
            oldStrokeDashArray.Add( e.NewValue);
            try
            {
                oldStrokeDashArray.Add(LineStyle.StrokeDashArray[1]);
            }
            catch { oldStrokeDashArray.Add(0); }
            LineStyle.StrokeDashArray = oldStrokeDashArray;
        }
        
        private void StrokeDash2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DoubleCollection oldStrokeDashArray = new DoubleCollection();
            try
            {
                oldStrokeDashArray.Add(LineStyle.StrokeDashArray[0]);
            }
            catch { oldStrokeDashArray.Add(1); }
            oldStrokeDashArray.Add(e.NewValue);
            LineStyle.StrokeDashArray = oldStrokeDashArray;
        }

        public static readonly DependencyProperty CanPasteProperty =
          DependencyProperty.Register("CanPaste", typeof(bool), typeof(MainWindow),
          new FrameworkPropertyMetadata(false, null));
        public bool CanPaste
        {
            get
            {
                return (bool)this.GetValue(CanPasteProperty);
            }
            set
            {
                this.SetValue(CanPasteProperty, value);
            }
        }

        private void PasteItem_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = new BorderWithAdorner();
            BorderWithAdorner newBorderWA =  borderWA.CopyBorder(MainWindow.CopyBorderWA);  //获取要粘贴的图形的副本
            AddGeometryIntoCanvas(newBorderWA, 2 * (MainWindow.PasteCount + 1), 2 * (MainWindow.PasteCount + 1));
            MainWindow.PasteCount++;
        }

        /// <summary>
        /// 实现上镜像功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopMirror_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = MainWindow.SelectedBorder;
            borderWA.GetFourPoint(borderWA);    //计算这个图形四个角落的位置
            BorderWithAdorner newBorderWA = borderWA.CopyBorder(borderWA);  //获取上镜像的图形的副本
            this.RootCanvas.Children.Add(newBorderWA);
            foreach (var item in newBorderWA.EllipseList)           //修改图形的点的位置，并把他放进Canvas
            {
                Point p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point() { X = p.X, Y = p.Y - (p.Y - borderWA.MinY) * 2 };
                this.RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }
        }

        /// <summary>
        /// 实现下镜像功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BottomMirror_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = MainWindow.SelectedBorder;
            borderWA.GetFourPoint(borderWA);    //计算这个图形四个角落的位置
            BorderWithAdorner newBorderWA = borderWA.CopyBorder(borderWA);  //获取下镜像的图形的副本
            this.RootCanvas.Children.Add(newBorderWA);
            foreach (var item in newBorderWA.EllipseList)           //修改图形的点的位置，并把他放进Canvas
            {
                Point p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point() { X = p.X, Y = p.Y - (p.Y - borderWA.MaxY) * 2 };
                this.RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }
        }

        /// <summary>
        /// 实现左镜像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LetfMirror_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = MainWindow.SelectedBorder;
            borderWA.GetFourPoint(borderWA);    //计算这个图形四个角落的位置
            BorderWithAdorner newBorderWA = borderWA.CopyBorder(borderWA);  //获取左镜像的图形的副本
            this.RootCanvas.Children.Add(newBorderWA);
            foreach (var item in newBorderWA.EllipseList)           //修改图形的点的位置，并把他放进Canvas
            {
                Point p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point() { X = p.X - (p.X - borderWA.MinX) * 2, Y = p.Y };
                this.RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }
        }

        /// <summary>
        /// 实现右镜像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightMirror_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = MainWindow.SelectedBorder;
            borderWA.GetFourPoint(borderWA);    //计算这个图形四个角落的位置
            BorderWithAdorner newBorderWA = borderWA.CopyBorder(borderWA);  //获取右镜像的图形的副本
            this.RootCanvas.Children.Add(newBorderWA);
            foreach (var item in newBorderWA.EllipseList)           //修改图形的点的位置，并把他放进Canvas
            {
                Point p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point() { X = p.X - (p.X - borderWA.MaxX) * 2, Y = p.Y };
                this.RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }
        }

        /// <summary>
        /// 把图形放进rootCanvas
        /// </summary>
        /// <param name="vBorderWA"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void AddGeometryIntoCanvas(BorderWithAdorner vBorderWA,int x,int y)
        {
            this.RootCanvas.Children.Add(vBorderWA);
            foreach (var item in vBorderWA.EllipseList)           //修改图形的点的位置，并把他放进Canvas
            {
                Point p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point() { X = p.X + x, Y = p.Y + y };
                this.RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }
        }

        /// <summary>
        /// 实现图形的垂直翻转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerticalOverturn_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = MainWindow.SelectedBorder;
            borderWA.GetFourPoint(borderWA);    //计算这个图形四个角落的位置
            double averageY = (borderWA.MinY + borderWA.MaxY) / 2.0;
            foreach (var item in borderWA.EllipseList)           //修改图形的点的位置，并把他放进Canvas
            {
                Point p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point() { X = p.X, Y = p.Y - (p.Y - averageY) * 2 };
            }
        }

        /// <summary>
        /// 实现图形的水平翻转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HorizontalOverturn_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = MainWindow.SelectedBorder;
            borderWA.GetFourPoint(borderWA);    //计算这个图形四个角落的位置
            double averageX = (borderWA.MinX + borderWA.MaxX) / 2.0;
            foreach (var item in borderWA.EllipseList)           //修改图形的点的位置，并把他放进Canvas
            {
                Point p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point() { X = p.X - (p.X - averageX) * 2, Y = p.Y };
            }
        }
    }

}
