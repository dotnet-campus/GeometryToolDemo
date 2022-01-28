using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryTool;

/// <summary>
///     字符串转图形，图形转字符串
/// </summary>
public class GeomertyStringConverter
{
    public GraphAppearance vGraphAppearance;
    public Canvas vRootCanvas;

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="vRootCanvas"></param>
    /// <param name="vGraphAppearance"></param>
    public GeomertyStringConverter(Canvas vRootCanvas, GraphAppearance vGraphAppearance)
    {
        this.vRootCanvas = vRootCanvas;
        this.vGraphAppearance = vGraphAppearance;
    }

    /// <summary>
    ///     从字符串中读取图形，并绘制出来
    /// </summary>
    /// <param name="vMiniLanguege"></param>
    public BorderWithAdorner GeomotryFromString(string vMiniLanguege)
    {
        var grapAdd = new GraphAdd();
        var path = new Path
        {
            Stroke = vGraphAppearance.Stroke,
            StrokeThickness = vGraphAppearance.StrokeThickness,
            StrokeDashArray = vGraphAppearance.StrokeDashArray
        };

        if (vGraphAppearance.Fill != null)
        {
            path.Fill = vGraphAppearance.Fill;
        }

        var regex = new Regex(@"([a-zA-Z])\s*([^a-zA-Z]*)");
        var matchList = regex.Matches(vMiniLanguege);

        //定义直线
        var pathGeometry = new PathGeometry();
        var borderWA = new BorderWithAdorner();
        borderWA.Child = path;
        path.Data = pathGeometry;
        pathGeometry.Figures = new PathFigureCollection();
        var mPathFigure = new PathFigure();
        ;
        var EllipsePoint = new Point();
        var EllipseStartPath = new Path();
        var EllipseList = new List<Path>();
        var number = 0;
        var border = new BorderWithDrag();
        for (var i = 0; i < matchList.Count; ++i)
        {
            //产生点
            number++;
            var match = matchList[i];
            if (i == 0) //处理起点的问题
            {
                if (match.Groups[1].ToString() != "M") //起点包含M
                {
                    mPathFigure.StartPoint = new Point { X = 0, Y = 0 };
                    grapAdd.AddPointWithNoBorder(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipseStartPath);
                    borderWA.EllipseList.Add(EllipseStartPath);
                    pathGeometry.Figures.Add(mPathFigure);
                }
                else //起点不包含M
                {
                    //创建PathFigure，并绑定StartPoint
                    GetEllipsePointWithNoBorder(match, out EllipseStartPath, out EllipsePoint,
                        @"([\+\-\d\.]*),([\+\-\d\.]*)"); //构造EllipsePoint          
                    borderWA.EllipseList.Add(EllipseStartPath);
                    if (Regex.IsMatch(vMiniLanguege, "[HLV]"))
                    {
                        EllipseList.Add(EllipseStartPath);
                        border = new BorderWithDrag(path, 1, EllipseList);
                        border.Child = EllipseStartPath;
                    }
                    else
                    {
                        border = new BorderWithDrag();
                        border.Child = EllipseStartPath;
                    }
                }

                var StartE = EllipseStartPath.Data as EllipseGeometry;
                pathGeometry.Figures.Clear();
                mPathFigure = new PathFigure();
                if (Regex.IsMatch(vMiniLanguege, @"\sZ"))
                {
                    mPathFigure.IsClosed = true;
                }

                var binding = new Binding("Center") { Source = StartE }; //绑定起点
                binding.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(mPathFigure, PathFigure.StartPointProperty, binding);
                pathGeometry.Figures.Add(mPathFigure);
                var segmentCollection = new PathSegmentCollection();
                mPathFigure.Segments = segmentCollection;
            }

            Path EllipsePath;

            switch (match.Groups[1].ToString())
            {
                case "L":
                {
                    //创建PathFigure，并绑定StartPoint

                    GetEllipsePointWithNoBorder(match, out EllipsePath, out EllipsePoint,
                        @"([\+\-\d\.]*),([\+\-\d\.]*)"); //构造EllipsePoint
                    borderWA.EllipseList.Add(EllipsePath);
                    //创建LineSegment，并绑定Point
                    grapAdd.AddLineSegment(mPathFigure, EllipsePath);
                    EllipseList.Add(EllipsePath);
                    border = new BorderWithDrag(path, number, EllipseList);
                    border.Child = EllipsePath;

                    break;
                }

                case "H":
                {
                    EllipsePoint.X = Convert.ToDouble(match.Groups[2].ToString());

                    grapAdd.AddPointWithNoBorder(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
                    borderWA.EllipseList.Add(EllipsePath);
                    grapAdd.AddLineSegment(mPathFigure, EllipsePath);
                    EllipseList.Add(EllipsePath);
                    border = new BorderWithDrag(path, number, EllipseList);
                    border.Child = EllipsePath;

                    break;
                }

                case "V":
                {
                    EllipsePoint.Y = Convert.ToDouble(match.Groups[2].ToString());

                    grapAdd.AddPointWithNoBorder(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
                    borderWA.EllipseList.Add(EllipsePath);
                    grapAdd.AddLineSegment(mPathFigure, EllipsePath);
                    EllipseList.Add(EllipsePath);
                    border = new BorderWithDrag(path, number, EllipseList);
                    border.Child = EllipsePath;

                    break;
                }

                case "A":
                {
                    //创建LineSegment，并绑定Point
                    var size = new Size
                    {
                        Width = Convert.ToDouble(Regex
                            .Match(match.Groups[0].ToString(), @"A ([\+\-\d\.]*),([\+\-\d\.]*)").Groups[1].ToString()),
                        Height = Convert.ToDouble(Regex
                            .Match(match.Groups[0].ToString(), @"A ([\+\-\d\.]*),([\+\-\d\.]*)").Groups[2].ToString())
                    };

                    SweepDirection vSweepDirection;
                    var vIsLargeArc = false;
                    var imatches = Regex.Matches(match.Groups[0].ToString(),
                        @"A\s*[\+\-\d\.]+,[\+\-\d\.]+\s*([\+\-\.\d]+)\s*([\+\-\.\d]+)\s*([\+\-\.\d]+)");
                    if (Convert.ToInt32(imatches[0].Groups[2].ToString()) == 1) //设置SweepDirection
                    {
                        vSweepDirection = SweepDirection.Clockwise;
                    }
                    else
                    {
                        vSweepDirection = SweepDirection.Counterclockwise;
                    }

                    if (Convert.ToInt32(imatches[0].Groups[3].ToString()) == 1) //设置IsLargeArc
                    {
                        vIsLargeArc = true;
                    }

                    //创建PathFigure，并绑定StartPoint
                    GetEllipsePointWithNoBorder(match, out EllipsePath, out EllipsePoint,
                        @"\d ([\+\-\d\.]*),([\+\-\d\.]*)"); //构造EllipsePoint
                    grapAdd.AddArcSegment(mPathFigure, EllipsePath, size,
                        Convert.ToDouble(imatches[0].Groups[1].ToString()), vSweepDirection, vIsLargeArc);
                    borderWA.EllipseList.Add(EllipsePath);
                    border = new BorderWithDrag();
                    border.Child = EllipsePath;
                    break;
                }

                case "C":
                {
                    //创建PathFigure，并绑定StartPoint
                    Path EllipsePath2;
                    GetEllipsePointWithNoBorder(match, out EllipsePath2, out EllipsePoint,
                        @"([\+\-\d\.]*),([\+\-\d\.]*)"); //构造EllipsePoint
                    borderWA.EllipseList.Add(EllipsePath2);
                    border = new BorderWithDrag();
                    border.Child = EllipsePath2;

                    Path EllipsePath1;
                    GetEllipsePointWithNoBorder(match, out EllipsePath1, out EllipsePoint,
                        @"C\s+[\+\-\d\.]+,[\+\-\d\.]+\s*([\+\-\d\.]+),([\+\-\d\.]+)"); //构造EllipsePoint
                    borderWA.EllipseList.Add(EllipsePath1);
                    border = new BorderWithDrag();
                    border.Child = EllipsePath1;

                    GetEllipsePointWithNoBorder(match, out EllipsePath, out EllipsePoint,
                        @"([\+\-\d\.]+),([\+\-\d\.]+)\s*$"); //构造EllipsePoint
                    borderWA.EllipseList.Add(EllipsePath);
                    grapAdd.AddBezierSegment(mPathFigure, EllipsePath2, EllipsePath1, EllipsePath);
                    border = new BorderWithDrag();
                    border.Child = EllipsePath;

                    break;
                }

                case "Q":
                {
                    //创建PathFigure，并绑定StartPoint
                    Path EllipsePath1;
                    GetEllipsePointWithNoBorder(match, out EllipsePath1, out EllipsePoint,
                        @"([\+\-\d\.]*),([\+\-\d\.]*)"); //构造EllipsePoint
                    borderWA.EllipseList.Add(EllipsePath1);
                    border = new BorderWithDrag();
                    border.Child = EllipsePath1;

                    GetEllipsePointWithNoBorder(match, out EllipsePath, out EllipsePoint,
                        @"\d\s+([\+\-\d\.]*),([\+\-\d\.]*)"); //构造EllipsePoint
                    borderWA.EllipseList.Add(EllipsePath);
                    grapAdd.AddQuadraticSegment(mPathFigure, EllipsePath1, EllipsePath);
                    border = new BorderWithDrag();
                    border.Child = EllipsePath;

                    break;
                }
            }
        }

        return borderWA;
    }

    /// <summary>
    ///     把图形转换成为字符串
    /// </summary>
    /// <param name="vPath"></param>
    /// <returns></returns>
    public string StringFromGeometry(Path vPath)
    {
        var miniLanguage = new StringBuilder();
        var pathGemetry = vPath.Data as PathGeometry;
        var pathFigure = pathGemetry.Figures[0];
        var segmentCol = pathFigure.Segments;
        miniLanguage.Append("M " + pathFigure.StartPoint.X + "," + pathFigure.StartPoint.Y + " ");

        foreach (var item in segmentCol)
        {
            if (item.GetType() == typeof(LineSegment))
            {
                miniLanguage.Append("L " + (item as LineSegment).Point.X + "," + (item as LineSegment).Point.Y + " ");
            }
            else if (item.GetType() == typeof(ArcSegment))
            {
                var arcSegment = item as ArcSegment;
                miniLanguage.Append("A " + arcSegment.Size.Width + "," + arcSegment.Size.Height + " " +
                                    arcSegment.RotationAngle + " ");
                miniLanguage.Append((arcSegment.IsLargeArc ? 1 : 0) + " " +
                                    (arcSegment.SweepDirection == SweepDirection.Clockwise ? 1 : 0) + " ");
                miniLanguage.Append(arcSegment.Point.X + "," + arcSegment.Point.Y + " ");
            }
            else if (item.GetType() == typeof(BezierSegment))
            {
                var bezierSegment = item as BezierSegment;
                miniLanguage.Append("C " + bezierSegment.Point1.X + "," + bezierSegment.Point1.Y + " ");
                miniLanguage.Append(bezierSegment.Point2.X + "," + bezierSegment.Point2.Y + " ");
                miniLanguage.Append(bezierSegment.Point3.X + "," + bezierSegment.Point3.Y + " ");
            }
            else if (item.GetType() == typeof(QuadraticBezierSegment))
            {
                var QBezierSegment = item as QuadraticBezierSegment;
                miniLanguage.Append("Q " + QBezierSegment.Point1.X + "," + QBezierSegment.Point1.Y + " ");
                miniLanguage.Append(QBezierSegment.Point2.X + "," + QBezierSegment.Point2.Y + " ");
            }
        }

        if (pathFigure.IsClosed)
        {
            miniLanguage.Append("Z ");
        }

        return miniLanguage.ToString();
    }

    /// <summary>
    ///     产生一个EllipsePoint
    /// </summary>
    /// <param name="match"></param>
    private void GetEllipsePoint(Match match, out Path EllipsePath, out Point EllipsePoint, string vPattern)
    {
        var grapAdd = new GraphAdd();
        EllipsePoint = new Point();
        EllipsePoint.X = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), vPattern).Groups[1].ToString());
        EllipsePoint.Y = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), vPattern).Groups[2].ToString());
        EllipsePath = new Path();
        grapAdd.AddPoint(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
    }

    /// <summary>
    ///     产生一个带BorderWithAdorner的Ellipse
    /// </summary>
    /// <param name="match"></param>
    /// <param name="EllipsePath"></param>
    /// <param name="EllipsePoint"></param>
    /// <param name="vPattern"></param>
    private void GetEllipsePointWithNoBorder(Match match, out Path EllipsePath, out Point EllipsePoint, string vPattern)
    {
        var grapAdd = new GraphAdd();
        EllipsePoint = new Point();
        EllipsePoint.X = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), vPattern).Groups[1].ToString());
        EllipsePoint.Y = Convert.ToDouble(Regex.Match(match.Groups[0].ToString(), vPattern).Groups[2].ToString());
        EllipsePath = new Path();
        grapAdd.AddPointWithNoBorder(EllipsePoint, vGraphAppearance, vRootCanvas, out EllipsePath);
    }

    /// <summary>
    ///     构造PathGeometry
    /// </summary>
    /// <param name="vPath"></param>
    /// <returns></returns>
    public string StringFromPathGeometry(Path vPath)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("          <GeometryDrawing Brush=\"" + vPath.Fill + "\">");
        stringBuilder.AppendLine("              <GeometryDrawing.Geometry>");
        stringBuilder.AppendLine("                  <PathGeometry Figures=\"" + vPath.Data +
                                 (Regex.IsMatch(vPath.Data.ToString(), "[Zz]") ? " Z" : "") + "\"  />");
        stringBuilder.AppendLine("              </GeometryDrawing.Geometry>");
        stringBuilder.AppendLine("              <GeometryDrawing.Pen>");
        stringBuilder.AppendLine("                  <Pen Thickness=\"" + vPath.StrokeThickness + "\" Brush=\"" +
                                 vPath.Stroke + "\" />");
        stringBuilder.AppendLine("              </GeometryDrawing.Pen>");
        stringBuilder.Append("          </GeometryDrawing>");
        return stringBuilder.ToString();
    }

    /// <summary>
    ///     从XML中读取图形
    /// </summary>
    /// <param name="vXMLString"></param>
    /// <returns></returns>
    public List<BorderWithAdorner> PathGeometryFromString(string vXMLString)
    {
        var pattern =
            @"<GeometryDrawing Brush=""([#\dA-Fa-f]*)"">\s*<GeometryDrawing.Geometry>\s*<PathGeometry Figures=""([\s\,\.\+\-\dA-Za-z]*)""  />\s*</GeometryDrawing.Geometry>\s*<GeometryDrawing.Pen>\s*<Pen Thickness=""([\d\.\-\+]*)"" Brush=""([#\dA-Fa-f]*)"" />";
        var matchList = Regex.Matches(vXMLString, pattern);
        var borderWAList = new List<BorderWithAdorner>();
        foreach (Match item in matchList)
        {
            var BackgroundColor = item.Groups[1].ToString();
            var StrokeColor = item.Groups[4].ToString();
            var MiniLanguage = item.Groups[2].ToString();
            var StrokeThickness = item.Groups[3].ToString();
            var graphAppearance = new GraphAppearance();
            graphAppearance.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColor));
            graphAppearance.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeColor));
            graphAppearance.StrokeThickness = Convert.ToDouble(StrokeThickness);
            var gsc = new GeomertyStringConverter(MainWindow.MyRootCanvas, graphAppearance);
            var newBorderWA = gsc.GeomotryFromString(MiniLanguage); //把Mini-Language转化成为图形，实现图形的深度复制
            var newPath = newBorderWA.Child as Path;
            newPath.Stroke = graphAppearance.Stroke;
            newPath.StrokeThickness = graphAppearance.StrokeThickness;
            newPath.Fill = graphAppearance.Fill;
            borderWAList.Add(newBorderWA);
        }

        return borderWAList;
    }
}