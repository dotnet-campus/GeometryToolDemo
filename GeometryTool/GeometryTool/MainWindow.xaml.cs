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

namespace GeometryTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string ActionMode="";     //表示当前鼠标的模式
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
        public MainWindow()
        {
            InitializeComponent();
            graphAdd    = new GraphAdd();
            ellipsePath = new System.Windows.Shapes.Path();
            graphAppearance = new GraphAppearance();
            pathFigure      = new PathFigure();
            isStartPoint = 0;
            linePath     = new System.Windows.Shapes.Path();
            this.RootCanvas.Tag = "Select";
        }


        private void Select_Click(object sender, RoutedEventArgs e)
        {
            //设置选定的模式
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                this.RootCanvas.Tag = radioButton.ToolTip;
                ActionMode = radioButton.ToolTip.ToString();
            }
            if (this.RootCanvas.Tag.ToString() == "Select")
            {
                this.RootCanvas.RemoveHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                if (isStartPoint != 0)
                {
                    isStartPoint = 0;
                }         
            } 
            e.Handled = true;
        }

        private void AddTriangle_Click(object sender, RoutedEventArgs e)
        {
            this.RootCanvas.Tag = "AddTriangle";
            e.Handled = true;
        }

        /// <summary>
        /// 执行鼠标的操作，例如选择，添加点，连线等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);   //获取鼠标当前位置

            if (this.RootCanvas.Tag.ToString() == "Point") //判断是不是画线
            {
                isClose = false;
                if (isStartPoint == 0)
                {
                    pathFigure = new PathFigure();
                    graphAdd.AddPoint(p, graphAppearance, this.RootCanvas, out ellipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref linePath, ellipsePath, ref isStartPoint, ref  pathFigure, isClose); //进行划线
                    this.RootCanvas.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                }
                else
                {
                    isStartPoint = 2;
                    graphAdd.AddPoint(p, graphAppearance, this.RootCanvas, out ellipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref  linePath, ellipsePath, ref isStartPoint, ref  pathFigure, isClose); //进行划线
                }
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
                isStartPoint = 1;
                System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);   //获取鼠标当前位置
                graphAdd.AddHorvorLine(pathFigure, p); //进行划线
            }
        }

        /// <summary>
        /// 选择线条颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrokColor_Click(object sender, RoutedEventArgs e)
        {
            StylusSettings dlg = new StylusSettings();
            dlg.Owner = this;
            dlg.currColor = graphAppearance.Stroke;
            if ((bool)dlg.ShowDialog().GetValueOrDefault())
            {
                graphAppearance.Stroke = dlg.currColor;
            }
        }

        /// <summary>
        /// 选择填充的颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillColor_Click(object sender, RoutedEventArgs e)
        {
            StylusSettings dlg = new StylusSettings();
            dlg.Owner = this;
            dlg.currColor = graphAppearance.Stroke;
            if ((bool)dlg.ShowDialog().GetValueOrDefault())
            {
                graphAppearance.Stroke = dlg.currColor;
            }
        }

        /// <summary>
        /// 选择线条的粗细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">1</param>
        private void ThickNess_Click(object sender, RoutedEventArgs e)
        {
            StrokeThickness dlg = new StrokeThickness();
            dlg.Owner = this;
            dlg.ThickNess = graphAppearance.StrokeThickness;
            if ((bool)dlg.ShowDialog().GetValueOrDefault())
            {
                graphAppearance.StrokeThickness = (int)dlg.ThickNess;
            }
        }

        private void DashArray_Click(object sender, RoutedEventArgs e)
        {
            StrokeDashArray dlg = new StrokeDashArray();
            dlg.Owner = this;
            dlg.strokeDashArray = graphAppearance.StrokeDashArray;
            if ((bool)dlg.ShowDialog().GetValueOrDefault())
            {
                graphAppearance.StrokeDashArray = dlg.strokeDashArray;
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream(@"E:\项目\GeometryTool\GeometryTool\bin\Debug\save.XML",FileMode.OpenOrCreate);
            //foreach (UIElement item in this.RootCanvas.Children)
            //{
            //    if()
            //}
        }

        /// <summary>
        /// 读取xml文件，生成图形
        /// </summary>
        /// <param name="vPath"></param>
        /// <param name="vRootCanvas"></param>
        public void ReadXml(string vPath, Canvas vRootCanvas)
        {
            StreamReader streamReader = new StreamReader(vPath);
            GeomortyConvertFromXML GCxml = new GeomortyConvertFromXML();
            MatchCollection MatchList = GCxml.GeomotryFromXML(streamReader.ReadToEnd().ToString(),@"<Geometry>\s*<Figures>([^<]*)</Figures>");
            foreach (Match match in MatchList)
            {
                GCxml.GeomotryFromString(match.Groups[1].ToString(), vRootCanvas, graphAppearance);
            }

        }

        private void RootCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canMove)
            {
                System.Windows.Point p = e.GetPosition(Application.Current.MainWindow);
                if (this.RootCanvas.Tag.ToString() == "AddTriangle")
                {
                    if (trianglePath != null)
                    {
                        PathGeometry triangle = trianglePath.Data as PathGeometry;
                        LineSegment line2 = triangle.Figures[0].Segments[1] as LineSegment;
                        Point oldPoint = line2.Point;
                        line2.Point = new Point() { X = p.X, Y = p.Y };
                        LineSegment line1 = triangle.Figures[0].Segments[0] as LineSegment;
                        oldPoint=triangle.Figures[0].StartPoint;
                        line1.Point = new Point() { X = oldPoint.X +(oldPoint.X-p.X), Y = p.Y };
                        e.Handled = true;
                    }
                }
                else if (this.RootCanvas.Tag.ToString() == "AddRectangular") 
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
            }
        }

        private void RootCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.RootCanvas.Tag.ToString() == "AddTriangle")
            {
                System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);

                graphAdd.AddGeometry(p, graphAppearance, this.RootCanvas, out trianglePath, 3);
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "AddRectangular")
            {
                System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);
                graphAdd.AddGeometry(p, graphAppearance, this.RootCanvas, out rectanglePath, 4);
                canMove = true;
            }
            else if (this.RootCanvas.Tag.ToString() == "AddCircle")
            {
                System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);
                graphAdd.AddGeometryOfCricle(p, graphAppearance, this.RootCanvas, out circlePath);
            }
        }

        private void RootCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (canMove)
            {
                canMove = false;
            }
        }

        private void AddRectangular_Click(object sender, RoutedEventArgs e)
        {
            this.RootCanvas.Tag = "AddRectangular";
            e.Handled = true;
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            this.RootCanvas.Tag = "AddCircle";
            e.Handled = true;
        }

    }
   
}
