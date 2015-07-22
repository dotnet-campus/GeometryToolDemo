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
    public class GraphAdd
    {
        myBorder border;
        public void AddLine(Point vPoint,GraphAppearance vGraphAppearance,Canvas vRootCanvas,ref Path vPath,Path HitPath,ref bool vIsStartPoint)
        {

            if (vIsStartPoint)
            {
                Path p = new Path() 
                { 
                    Stroke = vGraphAppearance.Stroke,
                    StrokeThickness = vGraphAppearance.StrokeThickness
                };
                LineGeometry vLastLine = new LineGeometry();
                vPath.Data = vLastLine;

                EllipseGeometry e = HitPath.Data as EllipseGeometry;
                Binding binding = new Binding("Center") { Source = e };
                BindingOperations.SetBinding(vLastLine,LineGeometry.StartPointProperty,binding);
                


                vIsStartPoint = false;
                border = new myBorder();
                border.Child = p;
                vRootCanvas.Children.Add(border);
            }
            else
            {
                vLastLine.X2 = vPoint.X;
                vLastLine.Y2 = vPoint.Y;
                vIsStartPoint = true;
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(vRootCanvas);
                if (border != null)
                {
                    Adorner myAdorner=new SimpleCircleAdorner(vLastLine, Math.Min(vLastLine.X2, vLastLine.X1), Math.Min(vLastLine.Y2, vLastLine.Y1), Math.Abs(vLastLine.X2 - vLastLine.X1), Math.Abs(vLastLine.Y2 - vLastLine.Y1), border);
                    
                    //layer.Add(new SimpleCircleAdorner(vLastLine, Math.Min(vLastLine.X2, vLastLine.X1), Math.Min(vLastLine.Y2, vLastLine.Y1), Math.Abs(vLastLine.X2 - vLastLine.X1), Math.Abs(vLastLine.Y2 - vLastLine.Y1), border));
                    // layer.Add(new GeometryTool.Adorners.RectAdorner(vLastLine,Math.Min(vLastLine.X2 , vLastLine.X1),Math.Min(vLastLine.Y2 ,vLastLine.Y1) ,Math.Abs(vLastLine.X2 - vLastLine.X1), Math.Abs(vLastLine.Y2 - vLastLine.Y1)));
                    layer.Add(myAdorner);
                   
                    myAdorner.MouseMove += new MouseEventHandler(Element_MouseMove);
                    myAdorner.MouseLeftButtonDown += new MouseButtonEventHandler(Element_MouseLeftButtonDown);
                    myAdorner.MouseLeftButtonUp += new MouseButtonEventHandler(Element_MouseLeftButtonUp);
                }
            }
        }

        public void AddPoint(Point vPoint, GraphAppearance vGraphAppearance, Canvas vRootCanvas) 
        {
            Path p   = new Path();
            p.Stroke = Brushes.Black;
            p.StrokeThickness =2;
            EllipseGeometry ellipse = new EllipseGeometry();
            ellipse.RadiusX = vGraphAppearance.StrokeThickness+3;
            ellipse.RadiusY = vGraphAppearance.StrokeThickness +3;
            ellipse.Center  = vPoint;
            p.Data = ellipse;
            border = new myBorder();
            border.Child = p;
            vRootCanvas.Children.Add(border);
        }

        bool isDragDropInEffect = false;
        Point pos = new Point();
        Point oldPoint = new Point();


        void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragDropInEffect)
            {
                AdornerLayer layer = sender as AdornerLayer;
                
                FrameworkElement currEle = sender as FrameworkElement;
                System.Windows.Point p = e.GetPosition(Application.Current.MainWindow);
                
                SimpleCircleAdorner myAdorner = currEle as SimpleCircleAdorner;
                myAdorner.myBorder.Margin = new Thickness(p.X - oldPoint.X, p.Y - oldPoint.Y, 0, 0);
                
            }
        }


        void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            FrameworkElement fEle = sender as FrameworkElement;
            isDragDropInEffect = true;
            oldPoint = e.GetPosition(Application.Current.MainWindow);
            fEle.CaptureMouse();
            fEle.Cursor = Cursors.Hand;
        }

        void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragDropInEffect)
            {
                FrameworkElement ele = sender as FrameworkElement;
                isDragDropInEffect = false;
                ele.ReleaseMouseCapture();
            }
        } 

    

    }
}
