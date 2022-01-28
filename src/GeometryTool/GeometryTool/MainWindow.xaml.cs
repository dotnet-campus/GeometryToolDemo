using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using GeometryPath = System.Windows.Shapes;

namespace GeometryTool;

/// <summary>
///     MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    ///     依赖属性，用于控制是否启用粘贴功能 
    /// </summary>
    public static readonly DependencyProperty CanPasteProperty =
        DependencyProperty.Register("CanPaste", typeof(bool), typeof(MainWindow),
            new FrameworkPropertyMetadata(false, null));

    /// <summary>
    /// 表示装着当前图形的Canvas
    /// </summary>
    public static Canvas MyRootCanvas { set; get; }

    /// <summary>
    /// 表示当前和之前选择中的图形
    /// </summary>
    public static BorderWithAdorner SelectedBorder { set; get; }

    /// <summary>
    /// 用于存储剪切或者复制的图形
    /// </summary>
    public static BorderWithAdorner CopyBorderWithAdorner { set; get; }

    /// <summary>
    /// 表示当前图形被复制了多少遍
    /// </summary>
    public static int PasteCount { set; get; }

    /// <summary>
    /// 表示当前鼠标的模式
    /// </summary>
    public static string ActionMode { set; get; } = "";

    /// <summary>
    /// 表示画板大小
    /// </summary>
    public static int GridSize { set; get; }

    /// <summary>
    /// 表示最底层大小
    /// </summary>
    public static int LowestLevel { set; get; } = -1;

    /// <summary>
    /// 表示最顶层大小
    /// </summary>
    public static int HightestLevel { set; get; } = 1;

    /// <summary>
    /// 表示画板放大的倍数
    /// </summary>
    public static int Multiple { set; get; } = 1;

    /// <summary>
    /// 表示图形的图片背景
    /// </summary>
    private BitmapImage _backgroundImage;

    /// <summary>
    /// 表示绘制三次方贝塞尔曲线时候，曲线所在的Path
    /// </summary>
    private GeometryPath.Path _bezierPath;

    /// <summary>
    /// 表示图形是否可以拖动
    /// </summary>
    private bool _canMove;

    /// <summary>
    /// 表示绘制圆的时候，圆所在的Path
    /// </summary>
    private GeometryPath.Path _circlePath;

    /// <summary>
    /// 表示绘制椭圆的时候，椭圆所在的Path
    /// </summary>
    private GeometryPath.Path _ellipseGeometryPath;

    private List<GeometryPath.Path> _ellipseList;

    /// <summary>
    /// 表示绘制图形的时候，点所在Path
    /// </summary>
    private GeometryPath.Path _ellipsePath;

    /// <summary>
    /// 表示打开的文件名
    /// </summary>
    public string FileName { set; get; }

    /// <summary>
    /// 表示绘制动作的类
    /// </summary>
    private readonly GraphAdd _graphAdd;

    /// <summary>
    /// 表示图形的外观
    /// </summary>
    private readonly GraphAppearance _graphAppearance;

    /// <summary>
    /// 绘制网格时所使用的Brush
    /// </summary>
    private DrawingBrush _gridBrush;

    /// <summary>
    /// 表示图形是否是闭合的
    /// </summary>
    private bool _isClose;

    /// <summary>
    /// 表示当前图形已经保存了
    /// </summary>
    private bool _isSave;

    /// <summary>
    /// 绘制直线的时候，表示是否为第一个点
    /// </summary>
    private int _isStartPoint;

    /// <summary>
    /// 表示绘制直线的时候，直线的Path
    /// </summary>
    private GeometryPath.Path _linePath;

    /// <summary>
    /// 表示绘制直线的时候，直线所在的PathFigure
    /// </summary>
    private PathFigure _pathFigure;

    /// <summary>
    /// 表示绘制二次方贝塞尔曲线时候，曲线所在的Path
    /// </summary>
    private GeometryPath.Path _qBezierPath;

    /// <summary>
    /// 表示绘制正方形的时候，正方形所在的Path
    /// </summary>
    private GeometryPath.Path _rectanglePath;

    /// <summary>
    /// 表示绘制三角形的时候，三角形所在的Path
    /// </summary>
    private GeometryPath.Path _trianglePath;

    /// <summary>
    ///     构造函数，用于初始化对象
    /// </summary>
    public MainWindow()
    {
        _graphAppearance = new GraphAppearance();
        _graphAdd = new GraphAdd();
        _ellipseList = new List<GeometryPath.Path>();
        _ellipsePath = new GeometryPath.Path();
        _pathFigure = new PathFigure();
        _isStartPoint = 0;
        _linePath = new GeometryPath.Path();
        _graphAppearance.StrokeThickness = 0.1;
        InitializeComponent();

        ActionMode = "Select";
        WindowState = WindowState.Maximized; //设置窗口最大化
        docCanvas_Loaded();
        StrokeCurrentColor.Background = _graphAppearance.Stroke;
        FillCurrentColor.Background = _graphAppearance.Fill;
        MyRootCanvas = RootCanvas;
        GridSizeComboBox.SelectedIndex = 3;
        CanvasChange.Value = 20;
        _graphAppearance.Fill = Brushes.Transparent;
        FileName = "";
        RootCanvasBackGround.SelectedIndex = 0;
        PanProperty.DataContext = _graphAppearance;
        MenuOptions.DataContext = this;
        AddCommand();
    }

    public bool CanPaste
    {
        get => (bool)GetValue(CanPasteProperty);
        set => SetValue(CanPasteProperty, value);
    }

    #region 文件操作

    /// <summary>
    ///     将图形保存为XML文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var canSave = true;
        if (string.IsNullOrEmpty(FileName))
        {
            var save = new SaveFileDialog();
            save.Filter = "XML Files |*.xml";
            if ((bool)save.ShowDialog())
            {
                FileName = save.FileName;
            }
            else
            {
                canSave = false;
            }
        }

        if (canSave)
        {
            var sw = new StreamWriter(FileName);
            var xmlGeomertyStringConverter = new GeomertyStringConverter(RootCanvas, _graphAppearance);
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            sb.AppendLine("<Canvase>");
            sb.AppendLine(" <Geometry>");
            sb.Append("     <Figures>");
            foreach (UIElement item in RootCanvas.Children)
            {
                var borderWithAdorner = item as BorderWithAdorner; //点是有BorderWithDrag包含着的，图形是Path
                if (borderWithAdorner != null)
                {
                    sb.Append(xmlGeomertyStringConverter.StringFromGeometry(
                        borderWithAdorner.Child as GeometryPath.Path)); //构造Mini-Language
                }
            }

            sb.AppendLine("</Figures>");
            sb.AppendLine(" </Geometry>");
            sb.AppendLine("</Canvase>");
            sw.Write(sb.ToString());
            sw.Close();
        }
    }

    /// <summary>
    ///     把图形存储为PNG文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var oldMutiple = Convert.ToInt32(CanvasChange.Value);
        CanvasChange.Value = 1;
        var save = new SaveFileDialog();
        save.Filter = "png Files |*.png";
        RootCanvas.Background = null; //清空背景
        if ((bool)save.ShowDialog()) //选择要保存的文件名
        {
            var pngFileName = save.FileName;

            foreach (var item in RootCanvas.Children) //隐藏点
            {
                var borderWithDrag = item as BorderWithDrag;
                if (borderWithDrag != null)
                {
                    borderWithDrag.Visibility = Visibility.Hidden;
                }
            }

            var fs = new FileStream(pngFileName, FileMode.Create);

            var bmp = new RenderTargetBitmap((int)RootCanvas.ActualWidth, (int)RootCanvas.ActualHeight, 96, 96,
                PixelFormats.Pbgra32);
            bmp.Render(RootCanvas);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();
            fs.Dispose();

            foreach (var item in RootCanvas.Children)
            {
                var borderWithDrag = item as BorderWithDrag;
                if (borderWithDrag != null)
                {
                    borderWithDrag.Visibility = Visibility.Visible;
                }
            }

            _isSave = true;
        }

        CanvasChange.Value = oldMutiple; //显示点
    }

    /// <summary>
    ///     将图形保存为XML文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveXML_Click(object sender, RoutedEventArgs e)
    {
        var save = new SaveFileDialog();
        save.Filter = "XML Files |*.xml";
        if ((bool)save.ShowDialog()) //选择要保存的文件名
        {
            FileName = save.FileName;

            var sw = new StreamWriter(FileName);
            var xmlGeometryStringConverter = new GeomertyStringConverter(RootCanvas, _graphAppearance);
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            sb.AppendLine("<Canvase>");
            sb.AppendLine(" <Geometry>");
            sb.Append("     <Figures>");
            foreach (UIElement item in RootCanvas.Children)
            {
                var borderWithAdorner = item as BorderWithAdorner; //点是有BorderWithDrag包含着的，图形是Path
                if (borderWithAdorner != null)
                {
                    sb.Append(xmlGeometryStringConverter.StringFromGeometry(
                        borderWithAdorner.Child as GeometryPath.Path)); //构造Mini-Language
                }
            }

            sb.AppendLine("</Figures>");
            sb.AppendLine(" </Geometry>");
            sb.AppendLine("</Canvase>");
            sw.Write(sb.ToString());
            sw.Close();
            _isSave = true;
        }
    }

    /// <summary>
    ///     使用PNG图片作为背景
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenPNG_Click(object sender, RoutedEventArgs e)
    {
        var opf = new OpenFileDialog();
        opf.DefaultExt = ".png";
        opf.Filter = "(*.jpg,*.gif,*.bmp;*.png;)|*.jpg;*.gif;*.bmp;*.png"; //只选择.xml文件
        opf.ShowDialog();
        if (!string.IsNullOrEmpty(opf.SafeFileName))
        {
            var pngFileName = opf.FileName;
            var uri = new Uri(pngFileName, UriKind.Absolute);
            _backgroundImage = new BitmapImage(uri);
            if (_backgroundImage == null)
            {
                RootCanvas.Background = Brushes.White;
            }
            else
            {
                RootCanvas.Background = new ImageBrush(_backgroundImage);
            }

            RootCanvasBackGround.SelectedIndex = 1;
            _isSave = false;
        }
    }

    /// <summary>
    ///     把图形保存成为DrawingImage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveDrawingImage_Click(object sender, RoutedEventArgs e)
    {
        var save = new SaveFileDialog();
        save.Filter = "XML Files |*.xml";
        if ((bool)save.ShowDialog()) //选择要保存的文件名
        {
            FileName = save.FileName;

            var sw = new StreamWriter(FileName);
            var gsc = new GeomertyStringConverter(RootCanvas, _graphAppearance);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            stringBuilder.AppendLine("<DrawingImage >");
            stringBuilder.AppendLine("  <DrawingImage.Drawing>");
            stringBuilder.AppendLine("      <DrawingGroup>");
            foreach (UIElement item in RootCanvas.Children)
            {
                var borderWithAdorner = item as BorderWithAdorner; //点是有BorderWithDrag包含着的，图形是Path
                if (borderWithAdorner != null)
                {
                    stringBuilder.AppendLine(
                        gsc.StringFromPathGeometry(borderWithAdorner.Child as GeometryPath.Path)); //构造Mini-Language
                }
            }

            stringBuilder.AppendLine("      </DrawingGroup>");
            stringBuilder.AppendLine("  </DrawingImage.Drawing>");
            stringBuilder.AppendLine("</DrawingImage>");
            sw.Write(stringBuilder.ToString());
            sw.Close();
            _isSave = true;
        }
    }

    /// <summary>
    ///     打开图形，并绘制成为Path
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDrawingImage_Click(object sender, RoutedEventArgs e)
    {
        var messageBoxResult = MessageBox.Show(" 是否要保存当前文件", "", MessageBoxButton.YesNoCancel);
        if (messageBoxResult == MessageBoxResult.Yes)
        {
            SaveXML_Click(null, null);
            _isSave = true;
        }
        else if (messageBoxResult == MessageBoxResult.No)
        {
            var openFileDlg = new OpenFileDialog();
            openFileDlg.DefaultExt = ".xml";
            openFileDlg.Filter = "xml file|*.xml"; //只选择.xml文件
            if (openFileDlg.ShowDialog() == true) //打开对话框
            {
                if (!string.IsNullOrEmpty(openFileDlg.FileName)) //如果文件名不为空
                {
                    var xmlHelper = new XMLHelper();
                    var geomertyStringConverter = new GeomertyStringConverter(RootCanvas, _graphAppearance);
                    var streamReader = new StreamReader(openFileDlg.FileName);
                    var xmlString = streamReader.ReadToEnd();
                    var borderWithAdornerList = geomertyStringConverter.PathGeometryFromString(xmlString);
                    foreach (var borderWithAdorner in borderWithAdornerList)
                    {
                        AddGeometryIntoCanvas(borderWithAdorner, 0, 0);
                    }
                }
            }
        }
    }

    /// <summary>
    ///     打开XML文件读取XML中的图形
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Open_Click(object sender, RoutedEventArgs e)
    {
        if (_isSave == false)
        {
            var messageBoxResult = MessageBox.Show(" 是否要保存当前文件", "", MessageBoxButton.YesNoCancel);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveXML_Click(null, null);
                RootCanvas.Children.Clear();
                _isSave = true;
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                var openFileDlg = new OpenFileDialog();
                openFileDlg.DefaultExt = ".xml";
                openFileDlg.Filter = "xml file|*.xml"; //只选择.xml文件
                if (openFileDlg.ShowDialog() == true) //打开对话框
                {
                    if (!string.IsNullOrEmpty(openFileDlg.FileName)) //如果文件名不为空
                    {
                        var xmlHelper = new XMLHelper();
                        var geomertyStringConverter = new GeomertyStringConverter(RootCanvas, _graphAppearance);
                        var match = xmlHelper.ReadXml(openFileDlg.FileName); //读取XML文件
                        var matchList = Regex.Matches(match.Groups[0].ToString(), @"M[\.\,\s\+\-\dLACQZ]+");
                        foreach (Match item in matchList)
                        {
                            var borderWithAdorner =
                                geomertyStringConverter.GeomotryFromString(item.Groups[0].ToString()); //转化成为图形
                            RootCanvas.Children.Add(borderWithAdorner); //把图形添加到Canvas中
                            foreach (var ellipse in borderWithAdorner.EllipseList) //把点添加到Canvas中
                            {
                                var borderWithDrag = ellipse.Parent as BorderWithDrag;
                                RootCanvas.Children.Add(borderWithDrag);
                                var borderLock = new BorderLock(borderWithDrag);
                                borderLock.Lock(((borderWithDrag.Child as GeometryPath.Path).Data as EllipseGeometry)
                                    .Center);
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Canvas点击操作

    /// <summary>
    ///     当前鼠标的模式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Select_Click(object sender, RoutedEventArgs e)
    {
        //设置选定的模式
        var radioButton = sender as RadioButton;
        if (radioButton != null)
        {
            ActionMode = radioButton.ToolTip.ToString();
        }

        if (_isStartPoint != 0 && _pathFigure.Segments.Count > 0) //移除额外的线
        {
            _pathFigure.Segments.RemoveAt(_pathFigure.Segments.Count - 1);
        }

        if (ActionMode != "Point") //移除划线功能
        {
            RootCanvas.RemoveHandler(MouseMoveEvent, new MouseEventHandler(DrawLine));
            _ellipseList = new List<GeometryPath.Path>();
            if (_isStartPoint != 0)
            {
                _isStartPoint = 0;
            }
        }

        if (ActionMode != "Select")
        {
            NowSelectedLabel.Content = "画笔属性";
            PanProperty.DataContext = _graphAppearance;
            SliderStyle.Value = _graphAppearance.StrokeThickness * 10;
            StrokeDash1.Value = _graphAppearance.StrokeDashArray[0];
            StrokeDash2.Value = _graphAppearance.StrokeDashArray[1];
        }

        if (SelectedBorder != null) //隐藏之前点击的图形的选择框
        {
            SelectedBorder.GAdorner.Visibility = Visibility.Hidden;
        }

        SelectedBorder = null;
        e.Handled = true;
    }

    /// <summary>
    ///     执行鼠标的操作，例如选择，添加点，连线等
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement)); //获取鼠标当前位置

        if (ActionMode == "Point") //判断是不是画线
        {
            _isClose = false;
            if (_isStartPoint == 0)
            {
                _pathFigure = new PathFigure();
                _graphAdd.AddPointWithNoBorder(p, _graphAppearance, RootCanvas, out _ellipsePath); //进行画点
                _graphAdd.AddLine(_graphAppearance, RootCanvas, ref _linePath, _ellipsePath, ref _isStartPoint,
                    ref _pathFigure, _isClose); //进行划线
                RootCanvas.AddHandler(MouseMoveEvent, new MouseEventHandler(DrawLine));
            }
            else
            {
                _isStartPoint++;
                _graphAdd.AddPointWithNoBorder(p, _graphAppearance, RootCanvas, out _ellipsePath); //进行画点
                _graphAdd.AddLine(_graphAppearance, RootCanvas, ref _linePath, _ellipsePath, ref _isStartPoint,
                    ref _pathFigure, _isClose); //进行划线
            }

            _ellipseList.Add(_ellipsePath);
            var border = new BorderWithDrag(_linePath, _isStartPoint, _ellipseList);
            border.Child = _ellipsePath;
            RootCanvas.Children.Add(border);
            _isSave = false;
            e.Handled = true;
        }

        if (SelectedBorder != null) //设置右侧画板属性
        {
            PanProperty.DataContext = SelectedBorder.Child;
            NowSelectedLabel.Content = "图形属性";
            SliderStyle.Value = (SelectedBorder.Child as GeometryPath.Path).StrokeThickness * 10;
            StrokeDash1.Value = (SelectedBorder.Child as GeometryPath.Path).StrokeDashArray[0];
            StrokeDash2.Value = (SelectedBorder.Child as GeometryPath.Path).StrokeDashArray[1];
            Select.IsChecked = true;
            var borderWithAdorner = SelectedBorder;
            var path = borderWithAdorner.Child as GeometryPath.Path;
            var pg = path.Data as PathGeometry;
            var arcSegment = pg.Figures[0].Segments[0] as ArcSegment;

            if (arcSegment != null && pg.Figures[0].Segments.Count == 1) //设置曲线属性
            {
                var isClockwise = arcSegment.SweepDirection == SweepDirection.Clockwise ? true : false;
                var isLargeArc = arcSegment.IsLargeArc;
                var RotationAngle = arcSegment.RotationAngle;
                var size = arcSegment.Size;
            }
        }
    }

    /// <summary>
    ///     鼠标移动的时候，画一条线段
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DrawLine(object sender, MouseEventArgs e)
    {
        if (ActionMode == "Point")
        {
            //isStartPoint = 1;
            var p = Mouse.GetPosition(e.Source as FrameworkElement); //获取鼠标当前位置
            _graphAdd.AddHorvorLine(_pathFigure, p); //进行划线
        }
    }

    /// <summary>
    ///     修改图形的位置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (_canMove)
        {
            var p = new AutoPoints().GetAutoAdsorbPoint(e.GetPosition(RootCanvas)); //获取当前鼠标的位置
            if (ActionMode == "AddTriangle") //修改三角形的位置
            {
                if (_trianglePath != null)
                {
                    var triangle = _trianglePath.Data as PathGeometry;
                    var line2 = triangle.Figures[0].Segments[1] as LineSegment;
                    var oldPoint = line2.Point;
                    line2.Point = new Point { X = p.X, Y = p.Y };
                    var line1 = triangle.Figures[0].Segments[0] as LineSegment;
                    oldPoint = triangle.Figures[0].StartPoint;
                    line1.Point = new Point { X = oldPoint.X + (oldPoint.X - p.X), Y = p.Y };
                    e.Handled = true;
                }
            }
            else if (ActionMode == "AddRectangular") //修改矩形的位置
            {
                var triangle = _rectanglePath.Data as PathGeometry;
                var oldPaint = triangle.Figures[0].StartPoint;

                var line1 = triangle.Figures[0].Segments[0] as LineSegment;
                line1.Point = new Point { X = oldPaint.X, Y = p.Y };
                var line2 = triangle.Figures[0].Segments[1] as LineSegment;
                line2.Point = new Point { X = p.X, Y = p.Y };
                var line3 = triangle.Figures[0].Segments[2] as LineSegment;
                line3.Point = new Point { X = p.X, Y = oldPaint.Y };
                e.Handled = true;
            }
            else if (ActionMode == "AddCircle") //修改圆的位置
            {
                var circel = _circlePath.Data as PathGeometry;
                var line1 = circel.Figures[0].Segments[1] as ArcSegment;
                line1.Point = new Point { X = p.X, Y = p.Y };
                e.Handled = true;
            }
            else if (ActionMode == "AddEllipse") //修改椭圆的位置
            {
                var circel = _ellipseGeometryPath.Data as PathGeometry;
                var line1 = circel.Figures[0].Segments[0] as ArcSegment;
                var line2 = circel.Figures[0].Segments[1] as ArcSegment;
                var oldPoint1 = line1.Point;
                var oldPoint2 = line2.Point;
                line1.Point = new Point { X = p.X, Y = oldPoint1.Y };
                if (oldPoint2.X - oldPoint1.X != 0 && p.Y - oldPoint1.Y != 0) //保证被除数被为0
                {
                    line1.Size = new Size
                        { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0, Height = Math.Abs(p.Y - oldPoint1.Y) };
                    line2.Size = new Size
                        { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0, Height = Math.Abs(p.Y - oldPoint1.Y) };
                }
                else if (oldPoint2.X - oldPoint1.X != 0)
                {
                    line1.Size = new Size { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0 };
                    line2.Size = new Size { Width = Math.Abs(oldPoint2.X - oldPoint1.X) / 2.0 };
                }
                else if (p.Y - oldPoint1.Y != 0)
                {
                    line1.Size = new Size { Height = Math.Abs(p.Y - oldPoint1.Y) };
                    line2.Size = new Size { Height = Math.Abs(p.Y - oldPoint1.Y) };
                }

                e.Handled = true;
            }
            else if (ActionMode == "QBezier") //修改二次方贝塞尔曲线的位置
            {
                var qBezier = _qBezierPath.Data as PathGeometry;
                var qbSegment = qBezier.Figures[0].Segments[0] as QuadraticBezierSegment;
                var oldPoint = qbSegment.Point1;
                var oldPoint2 = qbSegment.Point2;
                qbSegment.Point1 = new Point { X = (p.X + oldPoint.X) / 2.0, Y = p.Y };
                qbSegment.Point2 = new Point { X = p.X, Y = oldPoint2.Y };
            }
            else if (ActionMode == "Bezier") //修改三次方贝塞尔曲线的位置
            {
                var bezier = _bezierPath.Data as PathGeometry;
                var bSegment = bezier.Figures[0].Segments[0] as BezierSegment;
                var oldPoint = bSegment.Point1;
                var oldPoint2 = bSegment.Point2;
                var oldPoint3 = bSegment.Point3;
                bSegment.Point1 = new Point { X = oldPoint.X, Y = p.Y };
                bSegment.Point2 = new Point { X = p.X, Y = p.Y };
                bSegment.Point3 = new Point { X = p.X, Y = oldPoint3.Y };
            }
        }
    }

    /// <summary>
    ///     鼠标左击时，拖动图形移动
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var geomertyStringConverter = new GeomertyStringConverter(RootCanvas, _graphAppearance);

        if (SelectedBorder != null && SelectedBorder.GAdorner != null) //隐藏之前点击的图形的选择框
        {
            SelectedBorder.GAdorner.Visibility = Visibility.Hidden;
        }

        if (ActionMode == "Select")
        {
            var p = Mouse.GetPosition(e.Source as FrameworkElement);
            VisualTreeHelper.HitTest(RootCanvas, null, //进行命中测试
                MyHitTestResult,
                new PointHitTestParameters(p));
        }
        else if (ActionMode == "AddTriangle") //绘制三角形
        {
            var p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
            var miniLanguage = "M " + p.X + "," + p.Y + " L " + p.X + "," + p.Y + " L " + p.X + "," + p.Y + " Z";

            SelectedBorder = geomertyStringConverter.GeomotryFromString(miniLanguage);
            _trianglePath = SelectedBorder.Child as GeometryPath.Path;
            AddGeometryIntoCanvas(SelectedBorder, 0, 0);
            PanProperty.DataContext = _trianglePath;
            _canMove = true;
        }
        else if (ActionMode == "AddRectangular") //绘制矩形
        {
            var p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
            var miniLanguage = "M " + p.X + "," + p.Y + " L " + p.X + "," + p.Y + " L " + p.X + "," + p.Y + " L " +
                               p.X + "," + p.Y + " Z";

            SelectedBorder = geomertyStringConverter.GeomotryFromString(miniLanguage);
            _rectanglePath = SelectedBorder.Child as GeometryPath.Path;
            AddGeometryIntoCanvas(SelectedBorder, 0, 0);
            PanProperty.DataContext = _rectanglePath;
            _canMove = true;
        }
        else if (ActionMode == "AddCircle") //绘制圆
        {
            var p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
            SelectedBorder = _graphAdd.AddGeometryOfCricle(p, _graphAppearance, RootCanvas, out _circlePath);
            PanProperty.DataContext = _circlePath;
            _canMove = true;
        }
        else if (ActionMode == "AddEllipse") //绘制椭圆
        {
            var p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
            SelectedBorder = _graphAdd.AddGeometryOfCricle(p, _graphAppearance, RootCanvas, out _ellipseGeometryPath);
            PanProperty.DataContext = _ellipseGeometryPath;
            _canMove = true;
        }
        else if (ActionMode == "QBezier") //绘制二次方贝塞尔曲线
        {
            var p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
            var miniLanguage = "M " + p.X + "," + p.Y + " Q " + p.X + "," + p.Y
                               + " " + p.X + "," + p.Y;
            SelectedBorder = geomertyStringConverter.GeomotryFromString(miniLanguage);
            _qBezierPath = SelectedBorder.Child as GeometryPath.Path;
            AddGeometryIntoCanvas(SelectedBorder, 0, 0);
            PanProperty.DataContext = _qBezierPath;
            _canMove = true;
        }
        else if (ActionMode == "Bezier") //绘制三次方贝塞尔曲线
        {
            var p = new AutoPoints().GetAutoAdsorbPoint(Mouse.GetPosition(e.Source as FrameworkElement));
            var miniLanguage = "M " + p.X + "," + p.Y + " C " + p.X + "," + p.Y
                               + " " + p.X + "," + p.Y + " " + p.X + "," + p.Y;
            SelectedBorder = geomertyStringConverter.GeomotryFromString(miniLanguage);
            _bezierPath = SelectedBorder.Child as GeometryPath.Path;
            AddGeometryIntoCanvas(SelectedBorder, 0, 0);
            PanProperty.DataContext = _bezierPath;
            _canMove = true;
        }

        _isSave = false;
    }

    /// <summary>
    ///     图形更改为不能拖动
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_canMove) //图形更改为不能拖动
        {
            BorderWithAdorner borderWithAdorner = null;
            if (ActionMode == "AddCircle") //绘制圆
            {
                borderWithAdorner = _circlePath.Parent as BorderWithAdorner;
            }
            else if (ActionMode == "AddEllipse") //绘制椭圆
            {
                borderWithAdorner = _ellipseGeometryPath.Parent as BorderWithAdorner;
            }

            if (borderWithAdorner != null)
            {
                var borderWithDrag = borderWithAdorner.EllipseList[0].Parent as BorderWithDrag;
                if (borderWithDrag != null)
                {
                    var borderLock = new BorderLock(borderWithDrag);
                    borderLock.Lock(((borderWithDrag.Child as GeometryPath.Path).Data as EllipseGeometry).Center);
                }
            }

            SelectedBorder.ShowAdorner();
            ActionMode = "Select";
            _canMove = false;
        }

        if (SelectedBorder != null && SelectedBorder.Child != null)
        {
            PanProperty.DataContext = SelectedBorder.Child;
            NowSelectedLabel.Content = "图形属性";
            SliderStyle.Value = (SelectedBorder.Child as GeometryPath.Path).StrokeThickness * 10;
            StrokeDash1.Value = (SelectedBorder.Child as GeometryPath.Path).StrokeDashArray[0];
            StrokeDash2.Value = (SelectedBorder.Child as GeometryPath.Path).StrokeDashArray[1];
            Select.IsChecked = true;
        }
        else
        {
            PanProperty.DataContext = _graphAppearance;
            NowSelectedLabel.Content = "画笔属性";
            SliderStyle.Value = _graphAppearance.StrokeThickness * 10;
            StrokeDash1.Value = _graphAppearance.StrokeDashArray[0];
            StrokeDash2.Value = _graphAppearance.StrokeDashArray[1];
        }
    }

    /// <summary>
    ///     右击鼠标，变成选择模式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (ActionMode != "Select")
        {
            Select.IsChecked = true; //更正鼠标模式为选择模式
            _ellipseList = new List<GeometryPath.Path>();
            ActionMode = "Select";
            if (_isStartPoint != 0 && _pathFigure.Segments.Count > 0) //移除额外的线
            {
                _pathFigure.Segments.RemoveAt(_pathFigure.Segments.Count - 1);
            }

            if (ActionMode != "Point") //移除划线功能
            {
                RootCanvas.RemoveHandler(MouseMoveEvent, new MouseEventHandler(DrawLine));
                if (_isStartPoint != 0)
                {
                    _isStartPoint = 0;
                }
            }

            e.Handled = true;
        }
    }

    #endregion

    #region 右侧属性面板操作

    /// <summary>
    ///     设置Stroke的颜色
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StrokeColors_Click(object sender, RoutedEventArgs e)
    {
        var button = e.OriginalSource as Button;
        if (button != null)
        {
            StrokeCurrentColor.Background = button.Background;
        }
    }

    /// <summary>
    ///     设置Fill的颜色
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FillColors_Click(object sender, RoutedEventArgs e)
    {
        var button = e.OriginalSource as Button;
        if (button != null)
        {
            FillCurrentColor.Background = button.Background;
        }
    }

    /// <summary>
    ///     调整画布，进行缩放
    /// </summary>
    private void CanvasChange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        try
        {
            if (CanvasBorder is null) return;

            Multiple = (int)CanvasChange.Value;
            double height = GridSize * (int)CanvasChange.Value + 100; //计算画布放大后的坐标
            double width = GridSize * (int)CanvasChange.Value + 100;
            CanvasBorder.Height =
                height >= CanvasBorder.ActualHeight ? height : RootGrid.ActualHeight - 115; //修改Border的大小，使得其能显示放大后的画布
            CanvasBorder.Width = width >= CanvasBorder.ActualWidth ? width : RootGrid.ActualWidth - 215;
            if (RootCanvasBackGround.SelectedIndex == 0)
            {
                docCanvas_Loaded();
            }
            else if (RootCanvasBackGround.SelectedIndex == 1)
            {
                if (_backgroundImage == null)
                {
                    RootCanvas.Background = Brushes.White;
                }
                else
                {
                    RootCanvas.Background = new ImageBrush(_backgroundImage);
                }
            }
            else
            {
                RootCanvas.Background = Brushes.White;
            }
        }
        catch
        {
            // 忽略
        }
    }


    /// <summary>
    ///     拖动Slider改变StrokeThickness
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SliderStyle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        LineStyle.StrokeThickness = e.NewValue;
    }

    /// <summary>
    ///     选择画布的大小
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var comboBoxItem = (ComboBoxItem)(sender as ComboBox).SelectedItem;
            if (comboBoxItem != null)
            {
                RootCanvas.Height = Convert.ToInt32(comboBoxItem.Tag);
                RootCanvas.Width = Convert.ToInt32(comboBoxItem.Tag);
                GridSize = Convert.ToInt32(comboBoxItem.Tag);
                double height = GridSize * (int)CanvasChange.Value + 200; //计算画布放大后的坐标
                double width = GridSize * (int)CanvasChange.Value + 200;
                CanvasBorder.Height =
                    height >= CanvasBorder.ActualHeight
                        ? height
                        : RootGrid.ActualHeight - 115; //修改Border的大小，使得其能显示放大后的画布
                CanvasBorder.Width = width >= CanvasBorder.ActualWidth ? width : RootGrid.ActualWidth - 215;
                docCanvas_Loaded();
            }

            _isSave = true;
        }
        catch
        {
            // 忽略
        }
    }

    /// <summary>
    ///     改变画笔的每个实线的长度
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StrokeDash1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var oldStrokeDashArray = new DoubleCollection();
        oldStrokeDashArray.Add(e.NewValue);
        try
        {
            oldStrokeDashArray.Add(LineStyle.StrokeDashArray[1]);
            if (SelectedBorder != null)
            {
                _isSave = false;
            }
        }
        catch
        {
            oldStrokeDashArray.Add(0);
        }

        LineStyle.StrokeDashArray = oldStrokeDashArray;
    }

    /// <summary>
    ///     该变画笔的每个虚线的长度
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StrokeDash2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var oldStrokeDashArray = new DoubleCollection();
        try
        {
            oldStrokeDashArray.Add(LineStyle.StrokeDashArray[0]);
            if (SelectedBorder != null)
            {
                _isSave = false;
            }
        }
        catch
        {
            oldStrokeDashArray.Add(1);
        }

        oldStrokeDashArray.Add(e.NewValue);
        LineStyle.StrokeDashArray = oldStrokeDashArray;
    }

    /// <summary>
    ///     实现图形的复制功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PasteItem_Click(object sender, RoutedEventArgs e)
    {
        var borderWithAdorner = new BorderWithAdorner();
        var newBorderWithAdorner = borderWithAdorner.CopyBorder(CopyBorderWithAdorner); //获取要粘贴的图形的副本
        AddGeometryIntoCanvas(newBorderWithAdorner, 2 * (PasteCount + 1), 2 * (PasteCount + 1));
        PasteCount++;
        _isSave = false;
    }

    /// <summary>
    ///     把图形放进rootCanvas
    /// </summary>
    /// <param name="vBorderWithAdorner"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void AddGeometryIntoCanvas(BorderWithAdorner vBorderWithAdorner, int x, int y)
    {
        RootCanvas.Children.Add(vBorderWithAdorner);
        foreach (var item in vBorderWithAdorner.EllipseList) //修改图形的点的位置，并把他放进Canvas
        {
            var p = (item.Data as EllipseGeometry).Center;
            (item.Data as EllipseGeometry).Center = new Point { X = p.X + x, Y = p.Y + y };
            RootCanvas.Children.Add(item.Parent as BorderWithDrag);
        }
    }


    /// <summary>
    ///     选择画板
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootCanvasBackGround_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RootCanvasBackGround.SelectedIndex == 0)
        {
            docCanvas_Loaded();
        }
        else if (RootCanvasBackGround.SelectedIndex == 1)
        {
            if (_backgroundImage == null)
            {
                RootCanvas.Background = Brushes.White;
            }
            else
            {
                RootCanvas.Background = new ImageBrush(_backgroundImage);
            }
        }
        else
        {
            RootCanvas.Background = Brushes.White;
        }
    }

    #endregion

    #region 图形操作

    /// <summary>
    ///     创建一个新的画板
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CreateNewCanvas_Click(object sender, RoutedEventArgs e)
    {
        if (_isSave == false)
        {
            var messageBoxResult = MessageBox.Show(" 是否要保存当前文件", "", MessageBoxButton.YesNoCancel);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveXML_Click(null, null);
                RootCanvas.Children.Clear();
                _isSave = true;
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                RootCanvas.Children.Clear();
                _isSave = true;
            }
        }
    }

    /// <summary>
    ///     实现垂直翻转
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void VerticalOverturn_Click(object sender, MouseButtonEventArgs e)
    {
        if (SelectedBorder != null)
        {
            var borderWithAdorner = SelectedBorder;
            borderWithAdorner.GetFourPoint(borderWithAdorner); //计算这个图形四个角落的位置
            var averageY = (borderWithAdorner.MinY + borderWithAdorner.MaxY) / 2.0;
            foreach (var item in borderWithAdorner.EllipseList) //修改图形的点的位置
            {
                var borderWithDrag = item.Parent as BorderWithDrag;
                if (borderWithDrag.HasOtherPoint)
                {
                    continue;
                }

                var p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point { X = p.X, Y = p.Y - (p.Y - averageY) * 2 };
            }

            _isSave = false;
        }

        e.Handled = true;
    }

    /// <summary>
    ///     实现水平翻转
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HorizontalOverturn_Click(object sender, MouseButtonEventArgs e)
    {
        if (SelectedBorder != null)
        {
            var selectedBorder = SelectedBorder;
            selectedBorder.GetFourPoint(selectedBorder); //计算这个图形四个角落的位置
            var averageX = (selectedBorder.MinX + selectedBorder.MaxX) / 2.0;
            foreach (var item in selectedBorder.EllipseList) //修改图形的点的位置
            {
                var borderWithDrag = item.Parent as BorderWithDrag;
                if (borderWithDrag.HasOtherPoint)
                {
                    continue;
                }

                var p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point { X = p.X - (p.X - averageX) * 2, Y = p.Y };
            }

            _isSave = false;
        }

        e.Handled = true;
    }

    /// <summary>
    ///     实现右镜像功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RightMirror_Click(object sender, MouseButtonEventArgs e)
    {
        if (SelectedBorder != null)
        {
            var selectedBorder = SelectedBorder;
            selectedBorder.GetFourPoint(selectedBorder); //计算这个图形四个角落的位置
            var newBorderWithAdorner = selectedBorder.CopyBorder(selectedBorder); //获取右镜像的图形的副本
            RootCanvas.Children.Add(newBorderWithAdorner);
            foreach (var item in newBorderWithAdorner.EllipseList) //修改图形的点的位置，并把他放进Canvas
            {
                var p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center =
                    new Point { X = p.X - (p.X - selectedBorder.MaxX) * 2, Y = p.Y };
                RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }

            _isSave = false;
        }

        e.Handled = true;
    }

    /// <summary>
    ///     实现左镜像
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LetfMirror_Click(object sender, MouseButtonEventArgs e)
    {
        if (SelectedBorder != null)
        {
            var borderWithAdorner = SelectedBorder;
            borderWithAdorner.GetFourPoint(borderWithAdorner); //计算这个图形四个角落的位置
            var newBorderWithAdorner = borderWithAdorner.CopyBorder(borderWithAdorner); //获取左镜像的图形的副本
            RootCanvas.Children.Add(newBorderWithAdorner);
            foreach (var item in newBorderWithAdorner.EllipseList) //修改图形的点的位置，并把他放进Canvas
            {
                var p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point
                    { X = p.X - (p.X - borderWithAdorner.MinX) * 2, Y = p.Y };
                RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }

            _isSave = false;
        }

        e.Handled = true;
    }

    /// <summary>
    ///     实现下镜像功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BottomMirror_Click(object sender, MouseButtonEventArgs e)
    {
        if (SelectedBorder != null)
        {
            var borderWithAdorner = SelectedBorder;
            borderWithAdorner.GetFourPoint(borderWithAdorner); //计算这个图形四个角落的位置
            var newBorderWithAdorner = borderWithAdorner.CopyBorder(borderWithAdorner); //获取下镜像的图形的副本
            RootCanvas.Children.Add(newBorderWithAdorner);
            foreach (var item in newBorderWithAdorner.EllipseList) //修改图形的点的位置，并把他放进Canvas
            {
                var p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point
                    { X = p.X, Y = p.Y - (p.Y - borderWithAdorner.MaxY) * 2 };
                RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }

            _isSave = false;
        }

        e.Handled = true;
    }

    /// <summary>
    ///     实现上镜像功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TopMirror_Click(object sender, MouseButtonEventArgs e)
    {
        if (SelectedBorder != null)
        {
            var borderWithAdorner = SelectedBorder;
            borderWithAdorner.GetFourPoint(borderWithAdorner); //计算这个图形四个角落的位置
            var newBorderWithAdorner = borderWithAdorner.CopyBorder(borderWithAdorner); //获取上镜像的图形的副本
            RootCanvas.Children.Add(newBorderWithAdorner);
            foreach (var item in newBorderWithAdorner.EllipseList) //修改图形的点的位置，并把他放进Canvas
            {
                var p = (item.Data as EllipseGeometry).Center;
                (item.Data as EllipseGeometry).Center = new Point
                    { X = p.X, Y = p.Y - (p.Y - borderWithAdorner.MinY) * 2 };
                RootCanvas.Children.Add(item.Parent as BorderWithDrag);
            }

            _isSave = false;
        }

        e.Handled = true;
    }

    #endregion

    #region 被调用的方法

    /// <summary>
    ///     用于绘制网格网格
    /// </summary>
    private void docCanvas_Loaded()
    {
        _gridBrush = new DrawingBrush(new GeometryDrawing(
            new SolidColorBrush(Colors.Transparent),
            new Pen(new SolidColorBrush(Colors.LightGray), 0.1), //网格粗细为1
            new RectangleGeometry(new Rect(0, 0, 1, 1)))); //绘制网格的右边和下边
        _gridBrush.Stretch = Stretch.None;
        _gridBrush.TileMode = TileMode.Tile;
        _gridBrush.Viewport = new Rect(0.0, 0.0, 1, 1);
        _gridBrush.ViewportUnits = BrushMappingMode.Absolute;
        RootCanvas.Background = _gridBrush;
    }

    /// <summary>
    ///     显示被选中的图形的选择框
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public HitTestResultBehavior MyHitTestResult(HitTestResult result)
    {
        var path = result.VisualHit as GeometryPath.Path;
        if (path != null)
        {
            var borderWithAdorner = path.Parent as BorderWithAdorner;
            if (borderWithAdorner != null)
            {
                borderWithAdorner.GAdorner.Visibility = Visibility.Visible;
                SelectedBorder = borderWithAdorner;
                return HitTestResultBehavior.Stop;
            }

            return HitTestResultBehavior.Stop;
        }

        SelectedBorder = null;

        return HitTestResultBehavior.Continue;
    }

    /// <summary>
    ///     绑定路由事件
    /// </summary>
    private void AddCommand()
    {
        CommandBindings.Add(new CommandBinding
        (
            RoutedCommands.PasteCommand,
            (sender, e) => { PasteItem_Click(sender, e); },
            (sender, e) =>
            {
                if (CanPaste)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        ));
        CommandBindings.Add(new CommandBinding
        (
            RoutedCommands.SaveCommand,
            (sender, e) => { Save_Click(sender, e); },
            (sender, e) => { e.CanExecute = true; }
        ));
        CommandBindings.Add(new CommandBinding
        (
            RoutedCommands.NewCommand,
            (sender, e) => { CreateNewCanvas_Click(sender, e); },
            (sender, e) => { e.CanExecute = true; }
        ));
        CommandBindings.Add(new CommandBinding
        (
            RoutedCommands.OpenCommand,
            (sender, e) => { Open_Click(sender, e); },
            (sender, e) => { e.CanExecute = true; }
        ));

        CommandBindings.Add(new CommandBinding
        (
            RoutedCommands.CopyCommand,
            (sender, e) =>
            {
                if (SelectedBorder != null && SelectedBorder.GAdorner != null)
                {
                    SelectedBorder.GAdorner.chrome.CopyItem_Click(sender, e);
                }
            },
            (sender, e) =>
            {
                if (SelectedBorder != null && SelectedBorder.GAdorner != null)
                {
                    e.CanExecute = true;
                }
            }
        ));
        CommandBindings.Add(new CommandBinding
        (
            RoutedCommands.CutCommand,
            (sender, e) =>
            {
                if (SelectedBorder != null && SelectedBorder.GAdorner != null)
                {
                    SelectedBorder.GAdorner.chrome.CutItem_Click(sender, e);
                }
            },
            (sender, e) =>
            {
                if (SelectedBorder != null && SelectedBorder.GAdorner != null)
                {
                    e.CanExecute = true;
                }
            }
        ));
        CommandBindings.Add(new CommandBinding
        (
            RoutedCommands.DeleteCommand,
            (sender, e) =>
            {
                if (SelectedBorder != null && SelectedBorder.GAdorner != null)
                {
                    SelectedBorder.GAdorner.chrome.DeleteItem_Click(sender, e);
                }
            },
            (sender, e) =>
            {
                if (SelectedBorder != null && SelectedBorder.GAdorner != null)
                {
                    e.CanExecute = true;
                }
            }
        ));
    }

    #endregion
}