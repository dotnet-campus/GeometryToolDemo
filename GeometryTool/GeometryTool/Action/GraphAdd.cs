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
/// <summary>
/// 用于绘制图形，如点，直线，三角形，矩形，圆，椭圆
/// </summary>
    public class GraphAdd
    {
        BorderWithDrag border;//包装Path的border

        /// <summary>
        /// 新建一个PathGeometry
        /// </summary>
        /// <param name="vGraphAppearance"></param>
        /// <param name="vRootCanvas"></param>
        /// <param name="vPath"></param>
        /// <param name="vEllipsePath"></param>
        /// <param name="vIsClose"></param>
        public void NewPathGeomotry(GraphAppearance vGraphAppearance, Canvas vRootCanvas, out Path vPath, Path vEllipsePath, bool vIsClose)
        {
            //设置直线的外观
            vPath = new Path()
            {
                Stroke = vGraphAppearance.Stroke,
                StrokeThickness = vGraphAppearance.StrokeThickness,
                Fill = vGraphAppearance.Fill
               
            };

            PathGeometry pathGeometry = new PathGeometry();
            vPath.Data = pathGeometry;
            pathGeometry.FillRule = vGraphAppearance.FillRule;
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure pathFigure = new PathFigure() { IsClosed = vIsClose };
            pathGeometry.Figures.Add(pathFigure);
            PathSegmentCollection segmentCollection = new PathSegmentCollection();
            pathFigure.Segments = segmentCollection;

            //设置起点绑定该Point
            EllipseGeometry e = vEllipsePath.Data as EllipseGeometry;
            Binding binding = new Binding("Center") { Source = e };
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, binding);
            vRootCanvas.Children.Add(vPath);
        }

        /// <summary>
        /// 添加一个LineSegment
        /// </summary>
        /// <param name="vPathFigure"></param>
        /// <param name="vEllipsePath"></param>
        public void AddLineSegment(PathFigure vPathFigure, Path vEllipsePath)
        {
            EllipseGeometry e = (EllipseGeometry)vEllipsePath.Data ;        //获取要绑定的点
            LineSegment newLineSegment=new LineSegment();                   
            Binding binding=new Binding("Center"){ Source = e };            //绑定点
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(newLineSegment, LineSegment.PointProperty, binding);
            vPathFigure.Segments.Add(newLineSegment);
        }

        /// <summary>
        /// 添加一条ArgSegment
        /// </summary>
        /// <param name="vPathFigure"></param>
        /// <param name="vEllipsePath"></param>
        public void AddArcSegment(PathFigure vPathFigure, Path vEllipsePath,Size vSize, double vRotationAngle ,SweepDirection vSweepDirection,bool vIsLargeArc)
        {
            EllipseGeometry e = (EllipseGeometry)vEllipsePath.Data;        //获取要绑定的点
            ArcSegment newArcSegment = new ArcSegment();
            newArcSegment.Size = vSize;
            newArcSegment.SweepDirection = vSweepDirection;
            newArcSegment.IsLargeArc = vIsLargeArc;
            newArcSegment.RotationAngle = vRotationAngle;
            Binding binding = new Binding("Center") { Source = e };            //绑定点
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(newArcSegment, ArcSegment.PointProperty, binding);
            vPathFigure.Segments.Add(newArcSegment);
        }

        /// <summary>
        /// 三次方贝塞尔曲线
        /// </summary>
        /// <param name="vPathFigure"></param>
        /// <param name="vEllipsePath1"></param>
        /// <param name="vEllipsePath2"></param>
        /// <param name="vEllipsePath3"></param>
        public void AddBezierSegment(PathFigure vPathFigure, Path vEllipsePath1, Path vEllipsePath2, Path vEllipsePath3)
        {
            EllipseGeometry e1 = (EllipseGeometry)vEllipsePath1.Data;        //获取要绑定的点1
            EllipseGeometry e2 = (EllipseGeometry)vEllipsePath2.Data;        //获取要绑定的点2
            EllipseGeometry e3 = (EllipseGeometry)vEllipsePath3.Data;        //获取要绑定的点3
            BezierSegment newLineSegment = new BezierSegment();

            Binding binding1 = new Binding("Center") { Source = e1 };            //绑定点1
            binding1.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(newLineSegment, BezierSegment.Point1Property, binding1);

            Binding binding2 = new Binding("Center") { Source = e2 };            //绑定点2
            binding2.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(newLineSegment, BezierSegment.Point2Property, binding2);

            Binding binding3 = new Binding("Center") { Source = e3 };            //绑定点3
            binding3.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(newLineSegment, BezierSegment.Point3Property, binding3);
           
            vPathFigure.Segments.Add(newLineSegment);
        }

        /// <summary>
        /// 两次方贝塞尔曲线
        /// </summary>
        /// <param name="vPathFigure"></param>
        /// <param name="vEllipsePath1"></param>
        /// <param name="vEllipsePath2"></param>
        /// <param name="vEllipsePath3"></param>
        public void AddQuadraticSegment(PathFigure vPathFigure, Path vEllipsePath1, Path vEllipsePath2)
        {
            EllipseGeometry e1 = (EllipseGeometry)vEllipsePath1.Data;        //获取要绑定的点1
            EllipseGeometry e2 = (EllipseGeometry)vEllipsePath2.Data;        //获取要绑定的点2
            QuadraticBezierSegment newLineSegment = new QuadraticBezierSegment();

            Binding binding1 = new Binding("Center") { Source = e1 };            //绑定点1
            binding1.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(newLineSegment, QuadraticBezierSegment.Point1Property, binding1);

            Binding binding2 = new Binding("Center") { Source = e2 };            //绑定点2
            binding2.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(newLineSegment, QuadraticBezierSegment.Point2Property, binding2);

            vPathFigure.Segments.Add(newLineSegment);
        }


        /// <summary>
        /// 绘制一个随鼠标移动的线段
        /// </summary>
        /// <param name="vPathFigure"></param>
        /// <param name="vPoint"></param>
        public void AddHorvorLine(PathFigure vPathFigure,Point vPoint)
        {
            LineSegment vlastLine = vPathFigure.Segments[vPathFigure.Segments.Count - 1] as LineSegment;
            vlastLine.Point = vPoint;
        }

       
        
        /// <summary>
        /// 画一条直线
        /// </summary>
        /// <param name="vGraphAppearance">直线的外观</param>
        /// <param name="vRootCanvas">直线的容器</param>
        /// <param name="vPath">为了修改终点而是用的变量</param>
        /// <param name="vHitPath">鼠标点击的图形</param>
        /// <param name="vIsStartPoint">表示是起点还是终点</param>
        public void AddLine(GraphAppearance vGraphAppearance,Canvas vRootCanvas,ref Path vPath,Path vHitPath,ref int vIsStartPoint,ref PathFigure vPathFigure,bool isClose)
        {
            if (vIsStartPoint==0)  //表示起点
            {
                //设置直线的外观
                vPath = new Path() 
                { 
                    Stroke = vGraphAppearance.Stroke,
                    StrokeThickness = vGraphAppearance.StrokeThickness,
                    Fill=vGraphAppearance.Fill
               };

                //定义直线
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures = new PathFigureCollection();
                vPathFigure = new PathFigure() { IsClosed=isClose};
                pathGeometry.Figures.Add(vPathFigure);
                PathSegmentCollection segmentCollection = new PathSegmentCollection();
                vPathFigure.Segments = segmentCollection;
                LineSegment lineSegment = new LineSegment();
                
                vPathFigure.Segments.Add(lineSegment);      
                vPath.Data = pathGeometry;

                
                //将直线的起点和某个点绑定在一起
                EllipseGeometry e = vHitPath.Data as EllipseGeometry;
                Binding binding = new Binding("Center") { Source = e };
                binding.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(vPathFigure, PathFigure.StartPointProperty, binding);
                lineSegment.Point = e.Center;
                vRootCanvas.Children.Add(vPath);
                vIsStartPoint = 1;
            }
            else     //表示终点
            {
                //将直线的终点和某个点绑定在一起
                LineSegment lineSegment =vPathFigure.Segments[vPathFigure.Segments.Count-1] as LineSegment;
                EllipseGeometry e = vHitPath.Data as EllipseGeometry;
                LineGeometry vlastLine = vPath.Data as LineGeometry;
                Binding binding1 = new Binding("Center") { Source = e };
                BindingOperations.SetBinding(lineSegment, LineSegment.PointProperty, binding1);

                
                LineSegment newlineSegment = new LineSegment();
                newlineSegment.Point = new Point() { X=e.Center.X,Y=e.Center.Y};
                vPathFigure.Segments.Add(newlineSegment);
                

                if (border != null)
                {
                    // Adorner myAdorner=new SimpleCircleAdorner(vlastLine, Math.Min(vlastLine.X2, vlastLine.X1), Math.Min(vlastLine.Y2, vlastLine.Y1), Math.Abs(vlastLine.X2 - vlastLine.X1), Math.Abs(vlastLine.Y2 - vlastLine.Y1), border);

                    //layer.Add(new SimpleCircleAdorner(vlastLine, Math.Min(vlastLine.X2, vlastLine.X1), Math.Min(vlastLine.Y2, vlastLine.Y1), Math.Abs(vlastLine.X2 - vlastLine.X1), Math.Abs(vlastLine.Y2 - vlastLine.Y1), border));
                     //layer.Add(new GeometryTool.RectAdorner(vlastLine,Math.Min(vlastLine.X2 , vlastLine.X1),Math.Min(vlastLine.Y2 ,vlastLine.Y1) ,Math.Abs(vlastLine.X2 - vlastLine.X1), Math.Abs(vlastLine.Y2 - vlastLine.Y1)));
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
            vPath.Stroke = vGraphAppearance.Stroke;
            vPath.Fill = vGraphAppearance.Stroke;
            vPath.StrokeThickness = 2;
            EllipseGeometry ellipse = new EllipseGeometry();
            ellipse.RadiusX = vGraphAppearance.StrokeThickness+2;
            ellipse.RadiusY = vGraphAppearance.StrokeThickness +2;
            ellipse.Center  = vPoint;
            vPath.Data = ellipse;

            //把圆放进border里面，因为Border中有装饰器，同时可以使圆可以被拖动
            border = new BorderWithDrag();
            border.Child = vPath;
            vRootCanvas.Children.Add(border);
           
        }

        /// <summary>
        /// 在面板中绘制一个多边形
        /// </summary>
        /// <param name="vPoint">需要绘制的点</param>
        /// <param name="vGraphAppearance">图形的外形</param>
        /// <param name="vRootCanvas">所在的面板</param>
        /// <param name="vPath">返回的Path</param>
        /// <param name="count">代表是几边型</param>
        public void AddGeometry(Point vPoint,GraphAppearance vGraphAppearance, Canvas vRootCanvas,out Path vPath,int count,bool isClose)
        {

            //设置直线的外观
            vPath = new Path()
            {
                Stroke = vGraphAppearance.Stroke,
                StrokeThickness = vGraphAppearance.StrokeThickness,
                Fill = vGraphAppearance.Fill
            };

            //定义直线
            
            PathGeometry pathGeometry = new PathGeometry();
            vPath.Data =pathGeometry;
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure pathFigure = new PathFigure() { IsClosed = isClose };
            pathGeometry.Figures.Add(pathFigure);
            PathSegmentCollection segmentCollection = new PathSegmentCollection();
            pathFigure.Segments = segmentCollection;
            
            
            //定义第一个点,并绑定起点
            Path ellipsePath;
            AddPoint(vPoint, vGraphAppearance, vRootCanvas, out  ellipsePath);
            EllipseGeometry ellipseGeometry = ellipsePath.Data as EllipseGeometry;
            Binding FirstPointBD = new Binding("Center") { Source = ellipseGeometry };
            FirstPointBD.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, FirstPointBD);
            
            for (int i = 0; i < count-1; ++i)
            {
                AddPoint(vPoint, vGraphAppearance, vRootCanvas, out  ellipsePath);  //添加点
                ellipseGeometry = ellipsePath.Data as EllipseGeometry;
                LineSegment firstLineSe = new LineSegment();
                Binding SecondPointBD = new Binding("Center") { Source = ellipseGeometry };
                SecondPointBD.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(firstLineSe, LineSegment.PointProperty, SecondPointBD); //绑定Point
                pathFigure.Segments.Add(firstLineSe);
             }
            vRootCanvas.Children.Add(vPath);
        }

        /// <summary>
        /// 绘制一个圆或者椭圆
        /// </summary>
        /// <param name="vPoint"></param>
        /// <param name="vGraphAppearance"></param>
        /// <param name="vRootCanvas"></param>
        /// <param name="vPath"></param>
        /// <param name="count"></param>
        public void AddGeometryOfCricle(Point vPoint, GraphAppearance vGraphAppearance, Canvas vRootCanvas, out Path vPath)
        {

            //设置直线的外观
            vPath = new Path()
            {
                Stroke = vGraphAppearance.Stroke,
                StrokeThickness = vGraphAppearance.StrokeThickness,
                Fill = vGraphAppearance.Fill
            };

            //定义第一个PathFigure
            PathGeometry pathGeometry = new PathGeometry();
            vPath.Data = pathGeometry;
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure pathFigure = new PathFigure();
            //pathFigure.IsClosed = true;
            pathGeometry.Figures.Add(pathFigure);
            PathSegmentCollection segmentCollection = new PathSegmentCollection();
            pathFigure.Segments = segmentCollection;


            //绘制第一个点,并绑定起点
            Path ellipsePath;
            AddPoint(vPoint, vGraphAppearance, vRootCanvas, out  ellipsePath);
            EllipseGeometry ellipseGeometry = ellipsePath.Data as EllipseGeometry;
            EllipseGeometry startGeometry = ellipseGeometry;
            Binding FirstPointBD = new Binding("Center") { Source = ellipseGeometry };
            FirstPointBD.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, FirstPointBD);

            //绘制第二个点和第一条曲线，并绑定终点
            AddPoint(vPoint, vGraphAppearance, vRootCanvas, out  ellipsePath);  //添加点
            ellipseGeometry = ellipsePath.Data as EllipseGeometry;
            ArcSegment firstLineSe = new ArcSegment();
            Binding SecondPointBD = new Binding("Center") { Source = ellipseGeometry };
            SecondPointBD.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(firstLineSe, ArcSegment.PointProperty, SecondPointBD); //绑定Point
            pathFigure.Segments.Add(firstLineSe);

            //绘制第二条曲线，并绑定终点
            ArcSegment secondLineSe = new ArcSegment();
            Binding forthPointBD = new Binding("Center") { Source = startGeometry };
            forthPointBD.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(secondLineSe, ArcSegment.PointProperty, forthPointBD); //绑定Point
            pathFigure.Segments.Add(secondLineSe);

            secondLineSe.Size = new Size() { Height = 0.1, Width = 0.1 };
            firstLineSe.Size = new Size() { Height = 0.1, Width = 0.1 };
           
            vRootCanvas.Children.Add(vPath);
        }

        /// <summary>
        /// 绘制曲线
        /// </summary>
        /// <param name="vPoint"></param>
        /// <param name="vGraphAppearance"></param>
        /// <param name="vRootCanvas"></param>
        /// <param name="vPath"></param>
        public void AddCurve(Point vPoint, GraphAppearance vGraphAppearance, Canvas vRootCanvas, out Path vPath)
        {
            //设置直线的外观
            vPath = new Path()
            {
                Stroke = vGraphAppearance.Stroke,
                StrokeThickness = vGraphAppearance.StrokeThickness,
                Fill = vGraphAppearance.Fill
            };

            //定义第一个PathFigure
            PathGeometry pathGeometry = new PathGeometry();
            vPath.Data = pathGeometry;
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);
            PathSegmentCollection segmentCollection = new PathSegmentCollection();
            pathFigure.Segments = segmentCollection;

            //绘制第一个点,并绑定起点
            Path ellipsePath;
            AddPoint(vPoint, vGraphAppearance, vRootCanvas, out  ellipsePath);
            EllipseGeometry ellipseGeometry = ellipsePath.Data as EllipseGeometry;
            EllipseGeometry startGeometry = ellipseGeometry;
            Binding FirstPointBD = new Binding("Center") { Source = ellipseGeometry };
            FirstPointBD.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, FirstPointBD);

            //绘制第二个点和第一条曲线，并绑定终点
            AddPoint(vPoint, vGraphAppearance, vRootCanvas, out  ellipsePath);  //添加点
            ellipseGeometry = ellipsePath.Data as EllipseGeometry;
            ArcSegment firstLineSe = new ArcSegment() { Size = new Size() { Width=50,Height=25} };
            Binding SecondPointBD = new Binding("Center") { Source = ellipseGeometry };
            SecondPointBD.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(firstLineSe, ArcSegment.PointProperty, SecondPointBD); //绑定Point
            pathFigure.Segments.Add(firstLineSe);

            vRootCanvas.Children.Add(vPath);
        }
        
        public void AddGeomotryOfXml()
        {

        }
    }
}
