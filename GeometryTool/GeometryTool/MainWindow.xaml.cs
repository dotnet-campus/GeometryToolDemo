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
        public static string ActionMode="";
        public int isStartPoint;
        Polygon polygon;
        System.Windows.Shapes.Path LinePath;
        System.Windows.Shapes.Path EllipsePath;
        GraphAdd graphAdd;
        GraphAppearance graphAppearance;
        PathFigure pathFigure;
        System.Windows.Shapes.Path trianglePath;
        bool canMove = false;
        public MainWindow()
        {
            InitializeComponent();
            polygon = new Polygon();
            graphAdd = new GraphAdd();
            EllipsePath = new System.Windows.Shapes.Path();
            graphAppearance = new GraphAppearance();
            pathFigure = new PathFigure();
            isStartPoint = 0;
            LinePath = null;
            this.RootCanvas.Tag = "Select";
            ReadXml(@"C:\Users\WeiCong\Documents\图形\123.XML", this.RootCanvas);
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
                if (pathFigure.Segments.Count >= 1)
                    pathFigure.Segments.RemoveAt(pathFigure.Segments.Count - 1);
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
                if (isStartPoint == 0)
                {
                    pathFigure = new PathFigure();
                    graphAdd.AddPoint(p, graphAppearance, this.RootCanvas, out EllipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref LinePath, EllipsePath, ref isStartPoint, ref  pathFigure); //进行划线
                    this.RootCanvas.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                }
                else
                {
                    isStartPoint = 2;
                    graphAdd.AddPoint(p, graphAppearance, this.RootCanvas, out EllipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref  LinePath, EllipsePath, ref isStartPoint, ref  pathFigure); //进行划线
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
        /// <param name="e"></param>
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
                if (this.RootCanvas.Tag.ToString() == "AddTriangle")
                {
                    System.Windows.Point p = e.GetPosition(Application.Current.MainWindow);

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
            }
        }

        private void RootCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.RootCanvas.Tag.ToString() == "AddTriangle")
            {
                System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);

                graphAdd.AddTriangle(p, graphAppearance, this.RootCanvas, out trianglePath);
                canMove = true;
            }
        }

        private void RootCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (canMove)
            {
                canMove = false;
            }
        }
    }
   
}
