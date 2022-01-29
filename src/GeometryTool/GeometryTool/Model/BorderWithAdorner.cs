using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryTool;

/// <summary>
///     包含着图形的border，主要是显示选择框
/// </summary>
public class BorderWithAdorner : Border
{
    public List<Path> EllipseList { get; }

    /// <summary>
    /// 图形的选择框
    /// </summary>
    public GeometryAdorner GeometryAdorner { set; get; }

    /// <summary>
    /// 最大的X位置
    /// </summary>
    public double MaxX { set; get; }

    /// <summary>
    /// 最大的Y位置
    /// </summary>
    public double MaxY { set; get; }

    /// <summary>
    /// 最小的X位置
    /// </summary>
    public double MinX { set; get; }

    /// <summary>
    /// 最小的Y位置
    /// </summary>
    public double MinY { set; get; }

    public BorderWithAdorner()
    {
        MouseDown += Element_MouseDown;
        EllipseList = new List<Path>();
    }

    /// <summary>
    ///     当鼠标点击图形的的时候，产生边线
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Element_MouseDown(object sender, MouseEventArgs e)
    {
        if (MainWindow.ActionMode == "Select")
        {
            ShowAdorner();
        }
    }

    /// <summary>
    ///     展示Adorner
    /// </summary>
    public void ShowAdorner()
    {
        var layer = AdornerLayer.GetAdornerLayer(Parent as Canvas);
        if (layer != null)
        {
            GetFourPoint(this);
            if (GeometryAdorner == null)
            {
                var path = Child as Path;
                GeometryAdorner = new GeometryAdorner(this);
                layer.Add(GeometryAdorner);
            }
        }
    }

    /// <summary>
    ///     获取图形四个边角的位置
    /// </summary>
    public void GetFourPoint(BorderWithAdorner borderWithAdorner)
    {
        if (borderWithAdorner.EllipseList.Count > 0)
        {
            borderWithAdorner.MinX = (EllipseList[0].Data as EllipseGeometry).Center.X;
            borderWithAdorner.MinY = (EllipseList[0].Data as EllipseGeometry).Center.Y;
            borderWithAdorner.MaxX = 0;
            borderWithAdorner.MaxY = 0;
        }

        foreach (var path in borderWithAdorner.EllipseList)
        {
            var item = path.Data as EllipseGeometry;
            var point = item.Center;
            if (borderWithAdorner.MaxX < point.X)
            {
                borderWithAdorner.MaxX = point.X;
            }

            if (borderWithAdorner.MaxY < point.Y)
            {
                borderWithAdorner.MaxY = point.Y;
            }

            if (borderWithAdorner.MinX > point.X)
            {
                borderWithAdorner.MinX = point.X;
            }

            if (borderWithAdorner.MinY > point.Y)
            {
                borderWithAdorner.MinY = point.Y;
            }
        }
    }

    /// <summary>
    ///     用于复制Border，模仿深度复制
    /// </summary>
    /// <param name="borderWithAdorner"></param>
    /// <returns></returns>
    public BorderWithAdorner CopyBorder(BorderWithAdorner borderWithAdorner)
    {
        var path = borderWithAdorner.Child as Path;
        var graphAppearance = new GraphAppearance
        {
            Stroke = path.Stroke,
            StrokeThickness = path.StrokeThickness,
            StrokeDashArray = path.StrokeDashArray,
            Fill = path.Fill
        };
        var geometryStringConverter = new GeometryStringConverter(MainWindow.MyRootCanvas, graphAppearance);
        var miniLanguage =
            geometryStringConverter.StringFromGeometry(borderWithAdorner.Child as Path); //把该图形转化成为Mini-Language
        var newBorderWithAdorner =
            geometryStringConverter.GeometryFromString(miniLanguage); //把Mini-Language转化成为图形，实现图形的深度复制
        var newPath = newBorderWithAdorner.Child as Path;
        newPath.Stroke = path.Stroke;
        newPath.StrokeThickness = path.StrokeThickness;
        newPath.StrokeDashArray = path.StrokeDashArray;
        newPath.Fill = path.Fill;
        return newBorderWithAdorner;
    }
}