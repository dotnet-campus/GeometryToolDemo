using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
namespace GeometryTool
{
    /*----------------------------------------------------------------

          // 文件功能描述：定义添加图形的动作

   ----------------------------------------------------------------*/
    public class GraphAdd
    {
        BorderWithAdornor border;//包装Path的border

        public void AddHorvorLine(Path vLinePath,Point vPoint)
        {
            LineGeometry vlastLine = vLinePath.Data as LineGeometry;
            vlastLine.EndPoint = vPoint;
        }

        /// <summary>
        /// 画一条直线
        /// </summary>
        /// <param name="vGraphAppearance">直线的外观</param>
        /// <param name="vRootCanvas">直线的容器</param>
        /// <param name="vPath">为了修改终点而是用的变量</param>
        /// <param name="vHitPath">鼠标点击的图形</param>
        /// <param name="vIsStartPoint">表示是起点还是终点</param>
        public void AddLine(GraphAppearance vGraphAppearance,Canvas vRootCanvas,ref Path vPath,Path vHitPath,ref int vIsStartPoint,Point vPoint)
        {
            if (vIsStartPoint==0)  //表示起点
            {
                //设置直线的外观
                vPath = new Path() 
                { 
                    Stroke = vGraphAppearance.Stroke,
                    StrokeThickness = vGraphAppearance.StrokeThickness
                };
                LineGeometry lastLine = new LineGeometry();
                vPath.Data = lastLine;
                
                //将直线的起点和某个点绑定在一起
                EllipseGeometry e = vHitPath.Data as EllipseGeometry;
                Binding binding = new Binding("Center") { Source = e };
                BindingOperations.SetBinding(lastLine,LineGeometry.StartPointProperty,binding);
                lastLine.EndPoint = e.Center;

                //把直线放进border里面，因为Border中有装饰器，用来控制装饰器
                border = new BorderWithAdornor();
                border.Child = vPath;
                vRootCanvas.Children.Add(border);
                vIsStartPoint = 1;
            }
            else     //表示终点
            {
                //将直线的终点和某个点绑定在一起
                EllipseGeometry e = vHitPath.Data as EllipseGeometry;
                LineGeometry vlastLine = vPath.Data as LineGeometry;
                Binding binding = new Binding("Center") { Source = e };
                BindingOperations.SetBinding(vlastLine, LineGeometry.EndPointProperty, binding);
                vIsStartPoint =0;
                AddLine(vGraphAppearance, vRootCanvas, ref vPath, vHitPath, ref vIsStartPoint, vPoint);

                if (border != null)
                {
                    // Adorner myAdorner=new SimpleCircleAdorner(vlastLine, Math.Min(vlastLine.X2, vlastLine.X1), Math.Min(vlastLine.Y2, vlastLine.Y1), Math.Abs(vlastLine.X2 - vlastLine.X1), Math.Abs(vlastLine.Y2 - vlastLine.Y1), border);

                    //layer.Add(new SimpleCircleAdorner(vlastLine, Math.Min(vlastLine.X2, vlastLine.X1), Math.Min(vlastLine.Y2, vlastLine.Y1), Math.Abs(vlastLine.X2 - vlastLine.X1), Math.Abs(vlastLine.Y2 - vlastLine.Y1), border));
                    // layer.Add(new GeometryTool.Adorners.RectAdorner(vlastLine,Math.Min(vlastLine.X2 , vlastLine.X1),Math.Min(vlastLine.Y2 ,vlastLine.Y1) ,Math.Abs(vlastLine.X2 - vlastLine.X1), Math.Abs(vlastLine.Y2 - vlastLine.Y1)));
                    //layer.Add(myAdorner);

                    //myAdorner.MouseMove += new MouseEventHandler(Element_MouseMove);
                    //myAdorner.MouseLeftButtonDown += new MouseButtonEventHandler(Element_MouseLeftButtonDown);
                    //myAdorner.MouseLeftButtonUp += new MouseButtonEventHandler(Element_MouseLeftButtonUp);
                }
            }
        }

        /// <summary>
        /// 在面板上面画一个圆
        /// </summary>
        /// <param name="vPoint"></param>
        /// <param name="vGraphAppearance"></param>
        /// <param name="vRootCanvas"></param>
        public void AddPoint(Point vPoint, GraphAppearance vGraphAppearance, Canvas vRootCanvas,out Path vPath) 
        {
            //1.设置圆的外观
            vPath = new Path();
            vPath.Stroke = Brushes.Black;
            vPath.Fill = Brushes.White;
            vPath.StrokeThickness = 2;
            EllipseGeometry ellipse = new EllipseGeometry();
            ellipse.RadiusX = vGraphAppearance.StrokeThickness+3;
            ellipse.RadiusY = vGraphAppearance.StrokeThickness +3;
            ellipse.Center  = vPoint;
            vPath.Data = ellipse;

            //把圆放进border里面，因为Border中有装饰器，同时可以使圆可以被拖动
            border = new BorderWithAdornor();
            border.Child = vPath;
            vRootCanvas.Children.Add(border);
        }

    }
}
