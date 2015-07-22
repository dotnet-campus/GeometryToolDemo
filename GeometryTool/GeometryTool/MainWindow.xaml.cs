using System;
using System.Collections.Generic;
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
        Path LinePath;
        Path EllipsePath;
        GraphAdd graphAdd;
        GraphAppearance graphAppearance;
        public MainWindow()
        {
            InitializeComponent();
            polygon = new Polygon();
            graphAdd = new GraphAdd();
            EllipsePath = new Path();
            graphAppearance = new GraphAppearance();
            isStartPoint = 0;
            LinePath = null;
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
            if (ActionMode != "Point")
            {
                this.RootCanvas.RemoveHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                if (isStartPoint != 0) 
                {
                    
                    isStartPoint = 0;
                }
                this.RootCanvas.Children.RemoveAt(this.RootCanvas.Children.Count-1);
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
            System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);   //获取鼠标当前位置
   
            if(this.RootCanvas.Tag.ToString()== "Point") //判断是不是画线
            {
                if (isStartPoint == 0)
                {
                    graphAdd.AddPoint(p, graphAppearance, this.RootCanvas, out EllipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref LinePath, EllipsePath, ref isStartPoint, p); //进行划线
                    this.RootCanvas.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(DrawLine));
                }
                else
                {
                    isStartPoint = 2;
                    graphAdd.AddPoint(p, graphAppearance, this.RootCanvas, out EllipsePath);            //进行画点
                    graphAdd.AddLine(graphAppearance, RootCanvas, ref LinePath, EllipsePath, ref isStartPoint, p); //进行划线
                }
                e.Handled = true;
                       
            } 
            
            
        }

        private void DrawLine(object sender, MouseEventArgs e)
        {
            isStartPoint = 1;
            System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);   //获取鼠标当前位置
            graphAdd.AddHorvorLine(LinePath, p); //进行划线
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
    }
   
}
