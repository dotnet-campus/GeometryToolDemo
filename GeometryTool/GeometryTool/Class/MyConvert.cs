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
        Canvas vRootCanvas;
        GraphAppearance vGraphAppearance;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vRootCanvas"></param>
        /// <param name="vGraphAppearance"></param>
        public GeomortyConvertFromXML(Canvas vRootCanvas, GraphAppearance vGraphAppearance)
        {
            this.vRootCanvas=vRootCanvas;
            this.vGraphAppearance=vGraphAppearance;
        }

        /// <summary>
        /// 从字符串中读取图形，并绘制出来
        /// </summary>
        /// <param name="vMiniLanguege"></param>
        public void GeomotryFromString(string vMiniLanguege )
        {
            GraphAdd grapAdd = new GraphAdd();
            Path path = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
                //Fill = vGraphAppearance.Fill
            };
            Regex regex = new Regex(@"([a-zA-Z])\s*([^a-zA-Z]*)");
            MatchCollection matchList = regex.Matches(vMiniLanguege);

            //定义直线
            PathGeometry pathGeometry = new PathGeometry();
            path.Data = pathGeometry;

           

            pathGeometry.Figures = new PathFigureCollection();
            PathFigure mPathFigure=new PathFigure();;
            Point EllipsePoint=new Point();

            

            for (int i=0;i<matchList.Count;++i)
            {
                //产生点
                Match match = matchList[i];
                if (i == 0)  //处理起点的问题
                {
                    Path EllipsePath;
                    if (match.Groups[1].ToString() != "M")  //起点包含M
                    {
                        mPathFigure.StartPoint = new Point() { X = 0, Y = 0 };
                        grapAdd.AddPoint(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
                        pathGeometry.Figures.Add(mPathFigure);
                    }
                    else                                   //起点不包含M
                    {
                        //创建PathFigure，并绑定StartPoint
                        GetEllipsePoint(match, out EllipsePath, out EllipsePoint, @"([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint
                    }

                    EllipseGeometry e = EllipsePath.Data as EllipseGeometry;
                    pathGeometry.Figures.Clear();
                    mPathFigure = new PathFigure();
                    if (Regex.IsMatch(vMiniLanguege, @"\sZ"))
                    {
                        mPathFigure.IsClosed = true;
                    }
                    Binding binding = new Binding("Center") { Source = e };  //绑定起点
                    binding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(mPathFigure, PathFigure.StartPointProperty, binding);
                    pathGeometry.Figures.Add(mPathFigure);
                    PathSegmentCollection segmentCollection = new PathSegmentCollection();
                    mPathFigure.Segments = segmentCollection;
                }
                    
                switch (match.Groups[1].ToString())
                {
                    case "L":
                        {
                            //创建PathFigure，并绑定StartPoint
                            Path EllipsePath;
                            GetEllipsePoint(match, out EllipsePath, out EllipsePoint, @"([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint

                            //创建LineSegment，并绑定Point
                            grapAdd.AddLineSegment(mPathFigure, EllipsePath);
                            break;
                        }

                    case "H":
                        {
                            EllipsePoint.X = Convert.ToDouble((match.Groups[2].ToString()));
                            Path EllipsePath;
                            grapAdd.AddPoint(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
                            grapAdd.AddLineSegment(mPathFigure, EllipsePath);
                            break;
                        }

                    case "V":
                        {
                            EllipsePoint.Y = Convert.ToDouble((match.Groups[2].ToString()));
                            Path EllipsePath;
                            grapAdd.AddPoint(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
                            grapAdd.AddLineSegment(mPathFigure, EllipsePath);
                            break;
                        }

                    case "A":
                        {
                            //创建PathFigure，并绑定StartPoint
                            Path EllipsePath;
                            GetEllipsePoint(match, out EllipsePath, out EllipsePoint, @"\d ([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint

                            //创建LineSegment，并绑定Point
                            Size size = new Size() 
                            {
                                Width = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), @"A ([\+\-\d\.]*),([\+\-\d\.]*)").Groups[1].ToString()),
                                Height = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), @"A ([\+\-\d\.]*),([\+\-\d\.]*)").Groups[2].ToString())
                            };

                            SweepDirection vSweepDirection;
                            bool vIsLargeArc=false;
                            MatchCollection imatches = Regex.Matches(match.Groups[0].ToString(), @"([\+\-\d]+)");
                            if ((Convert.ToInt32(imatches[3].Groups[1].ToString()) == 1))  //设置SweepDirection
                            {
                                vSweepDirection=SweepDirection.Clockwise;
                            }
                            else
                                vSweepDirection=SweepDirection.Counterclockwise;
                            if ((Convert.ToInt32(imatches[4].Groups[1].ToString()) == 1)) //设置IsLargeArc
                            {
                                vIsLargeArc = true;
                            }
                            grapAdd.AddArcSegment(mPathFigure, EllipsePath,size, Convert.ToDouble(imatches[2].Groups[1].ToString()), vSweepDirection, vIsLargeArc);
                            break;
                        }

                    case "C": 
                        {
                            //创建PathFigure，并绑定StartPoint
                            Path EllipsePath;
                            GetEllipsePoint(match, out EllipsePath, out EllipsePoint, @"([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint

                            Path EllipsePath1;
                            GetEllipsePoint(match, out EllipsePath1, out EllipsePoint, @"([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint

                            Path EllipsePath2;
                            GetEllipsePoint(match, out EllipsePath2, out EllipsePoint, @"([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint
                            grapAdd.AddBezierSegment(mPathFigure, EllipsePath, EllipsePath1, EllipsePath2);
                            break;
                        }

                    case "Q":
                        {
                            //创建PathFigure，并绑定StartPoint
                            Path EllipsePath;
                            GetEllipsePoint(match, out EllipsePath, out EllipsePoint, @"([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint

                            Path EllipsePath1;
                            GetEllipsePoint(match, out EllipsePath1, out EllipsePoint, @"([\+\-\d\.]*),([\+\-\d\.]*)");   //构造EllipsePoint
                            grapAdd.AddQuadraticSegment(mPathFigure, EllipsePath, EllipsePath1);
                            break;
                        }
            
                }
                
            }
            vRootCanvas.Children.Add(path);
        }
    
        /// <summary>
        /// 把图形从XML中读取出来
        /// </summary>
        /// <param name="vXMLString"></param>
        /// <param name="vPattern"></param>
        /// <returns></returns>
        public MatchCollection GeomotryFromXML(string vXMLString,string vPattern)
        {
            Regex regex = new Regex(vPattern);
            MatchCollection matchList = regex.Matches(vXMLString);
            return matchList;
        }

        /// <summary>
        /// 产生一个EllipsePoint
        /// </summary>
        /// <param name="match"></param>
        void GetEllipsePoint(Match match, out Path EllipsePath, out Point EllipsePoint,string vPattern)
        {
            GraphAdd grapAdd = new GraphAdd();
            EllipsePoint = new Point();
            EllipsePoint.X = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), vPattern).Groups[1].ToString());
            EllipsePoint.Y = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), vPattern).Groups[2].ToString());
            EllipsePath = new Path();
            grapAdd.AddPoint(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
        }

        public string StringFromGeometry(Path vPath)
        {
            StringBuilder miniLanguage = new StringBuilder();
            PathGeometry pathGemetry = vPath.Data as PathGeometry;
            PathFigure pathFigure =pathGemetry.Figures[0];
            PathSegmentCollection segmentCol = pathFigure.Segments;
            miniLanguage.Append("M " + pathFigure.StartPoint.X.ToString() + "," + pathFigure.StartPoint.Y.ToString() + " ");

            foreach(PathSegment item in segmentCol)
            {
                if (item.GetType() == typeof(LineSegment))
                {
                    miniLanguage.Append("L "+(item as LineSegment).Point.X + "," + (item as LineSegment).Point.Y + " ");
                }
                else if(item.GetType() == typeof(ArcSegment))
                {
                    ArcSegment arcSegment = item as ArcSegment;
                    miniLanguage.Append("A " + arcSegment.Size.Width + "," + arcSegment.Size.Height + " " + arcSegment.RotationAngle+" ");
                    miniLanguage.Append((arcSegment.IsLargeArc ? 1 : 0) + " " + (arcSegment.SweepDirection==SweepDirection.Clockwise?1:0)+" ");
                    miniLanguage.Append(arcSegment.Point.X+","+arcSegment.Point.Y+" ");
                }
                else if (item.GetType() == typeof(BezierSegment))
                {
                    BezierSegment bezierSegment = item as BezierSegment;
                    miniLanguage.Append("C "+bezierSegment.Point1.X+","+bezierSegment.Point1.Y+" ");
                    miniLanguage.Append( bezierSegment.Point2.X + "," + bezierSegment.Point2.Y + " ");
                    miniLanguage.Append( bezierSegment.Point3.X + "," + bezierSegment.Point3.Y + " ");
                }
                else if (item.GetType() == typeof(QuadraticBezierSegment))
                {
                    QuadraticBezierSegment QBezierSegment = new QuadraticBezierSegment();
                    miniLanguage.Append("Q " + QBezierSegment.Point1.X + "," + QBezierSegment.Point1.Y + " ");
                    miniLanguage.Append(QBezierSegment.Point2.X + "," + QBezierSegment.Point2.Y + " ");
                }

               
            }
            if (pathFigure.IsClosed)
            {
                miniLanguage.Append("Z");
            }
            if (!miniLanguage.ToString().Contains("Z"))
            {
                miniLanguage.Remove(miniLanguage.Length-1,1);
            }
            return miniLanguage.ToString();
        }
    }
}
