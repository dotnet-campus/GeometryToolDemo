using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// 
    /// </summary>
    public class GeomortyConvertFromXML
    {
        public void GeomotryFromString(string vMiniLanguege, Canvas vRootCanvas, GraphAppearance vGraphAppearance)
        {
            GraphAdd grapAdd = new GraphAdd();
            Path path = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
                //Fill = vGraphAppearance.Fill
            };
            Regex regex = new Regex(@"([a-zA-Z])\s(\d*),(\d*)");
            MatchCollection matchList = regex.Matches(vMiniLanguege);

            //定义直线
            PathGeometry pathGeometry = new PathGeometry();
            path.Data = pathGeometry;
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure mPathFigure=new PathFigure();;
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point() { X = 0, Y = 0 };
            pathGeometry.Figures.Add(pathFigure);
           
            foreach (Match match in matchList)
            {
                //产生点
                Point EllipsePoint = new Point();
                EllipsePoint.X = Convert.ToDouble(match.Groups[2].ToString());
                EllipsePoint.Y = Convert.ToDouble(match.Groups[3].ToString());
                Path EllipsePath;
                grapAdd.AddPoint(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
                EllipseGeometry e = EllipsePath.Data as EllipseGeometry;

                switch (match.Groups[1].ToString())
                {
                    case "M": 
                        {
                            //创建PathFigure，并绑定StartPoint
                            pathGeometry.Figures.Clear();
                            mPathFigure = new PathFigure();
                            mPathFigure.StartPoint = EllipsePoint;
                            Binding binding = new Binding("Center") { Source = e };
                            BindingOperations.SetBinding(mPathFigure, PathFigure.StartPointProperty, binding);
                            pathGeometry.Figures.Add(mPathFigure);
                            PathSegmentCollection segmentCollection = new PathSegmentCollection();
                            mPathFigure.Segments = segmentCollection;
                            
                            break; 
                        }

                    case "L":
                        {
                            //创建LineSegment，并绑定Point
                            LineSegment lineSegment = new LineSegment();
                            lineSegment.Point = EllipsePoint;
                            Binding binding = new Binding("Center") { Source = e };
                            BindingOperations.SetBinding(lineSegment, LineSegment.PointProperty, binding);
                            mPathFigure.Segments.Add(lineSegment);
                            break;
                        }   
            
                }
                
            }
            vRootCanvas.Children.Add(path);
        }
    
        public MatchCollection GeomotryFromXML(string vXMLString,string vPattern)
        {
            Regex regex = new Regex(vPattern);
            MatchCollection matchList = regex.Matches(vXMLString);
            return matchList;
        }
    }
}
