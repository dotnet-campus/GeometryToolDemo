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
    public List<Path> EllipseList;
    public GeometryAdorner GAdorner; //图形的选择框
    public double MaxX; //最大的X位置
    public double MaxY; //最大的Y位置
    public double MinX; //最小的X位置
    public double MinY; //最小的Y位置

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
            if (GAdorner == null)
            {
                var path1 = Child as Path;
                GAdorner = new GeometryAdorner(this);
                layer.Add(GAdorner);
            }
        }
    }

    /// <summary>
    ///     获取图形四个边角的位置
    /// </summary>
    public void GetFourPoint(BorderWithAdorner vBorderWA)
    {
        if (vBorderWA.EllipseList.Count > 0)
        {
            vBorderWA.MinX = (EllipseList[0].Data as EllipseGeometry).Center.X;
            vBorderWA.MinY = (EllipseList[0].Data as EllipseGeometry).Center.Y;
            vBorderWA.MaxX = 0;
            vBorderWA.MaxY = 0;
        }

        foreach (var path in vBorderWA.EllipseList)
        {
            var item = path.Data as EllipseGeometry;
            var point = item.Center;
            if (vBorderWA.MaxX < point.X)
            {
                vBorderWA.MaxX = point.X;
            }

            if (vBorderWA.MaxY < point.Y)
            {
                vBorderWA.MaxY = point.Y;
            }

            if (vBorderWA.MinX > point.X)
            {
                vBorderWA.MinX = point.X;
            }

            if (vBorderWA.MinY > point.Y)
            {
                vBorderWA.MinY = point.Y;
            }
        }
    }

    /// <summary>
    ///     用于复制Border，模仿深度复制
    /// </summary>
    /// <param name="vBorderWA"></param>
    /// <returns></returns>
    public BorderWithAdorner CopyBorder(BorderWithAdorner vBorderWA)
    {
        var path = vBorderWA.Child as Path;
        var graphAppearance = new GraphAppearance
        {
            Stroke = path.Stroke,
            StrokeThickness = path.StrokeThickness,
            StrokeDashArray = path.StrokeDashArray,
            Fill = path.Fill
        };
        var gsc = new GeometryStringConverter(MainWindow.MyRootCanvas, graphAppearance);
        var MiniLanguage = gsc.StringFromGeometry(vBorderWA.Child as Path); //把该图形转化成为Mini-Language
        var newBorderWA = gsc.GeomotryFromString(MiniLanguage); //把Mini-Language转化成为图形，实现图形的深度复制
        var newPath = newBorderWA.Child as Path;
        newPath.Stroke = path.Stroke;
        newPath.StrokeThickness = path.StrokeThickness;
        newPath.StrokeDashArray = path.StrokeDashArray;
        newPath.Fill = path.Fill;
        return newBorderWA;
    }
}