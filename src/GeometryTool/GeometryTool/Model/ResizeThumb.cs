using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     可拖动的选择框的边的外观
/// </summary>
public class ResizeThumb : Thumb
{
    private BorderWithAdorner _borderWithAdorner;

    /// <summary>
    ///     构造函数，用于注册事件
    /// </summary>
    public ResizeThumb()
    {
        DragStarted += ResizeThumb_DragStarted;
        DragDelta += ResizeThumb_DragDelta;
        DragCompleted += ResizeThumb_DragCompleted;
    }

    /// <summary>
    ///     开始缩放
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        _borderWithAdorner = DataContext as BorderWithAdorner;
    }

    /// <summary>
    ///     图形的缩放
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        double maxX = _borderWithAdorner.MaxX, maxY = _borderWithAdorner.MaxY; //记录图形四个角落的值
        double minX = _borderWithAdorner.MinX, minY = _borderWithAdorner.MinY;
        var oldHeight = _borderWithAdorner.ActualHeight; //记录图形未缩放之前的高度
        var oldWidth = _borderWithAdorner.ActualWidth; //记录图形未缩放之前的宽度
        double newWidth = 0; //缩放后图形新的宽度
        double newHeight = 0;
        double deltaVertical, deltaHorizontal = 0; //缩放后图形新的高度
        if (_borderWithAdorner != null)
        {
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom: //如果是拖动下面的边
                    deltaVertical = -e.VerticalChange;
                    newHeight = oldHeight - deltaVertical;
                    break;
                case VerticalAlignment.Top: //如果是拖动上面的边
                    deltaVertical = e.VerticalChange;
                    newHeight = oldHeight - deltaVertical;
                    break;
            }

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left: //如果是拖动左边的边
                    deltaHorizontal = e.HorizontalChange;
                    newWidth = oldWidth - deltaHorizontal;

                    break;
                case HorizontalAlignment.Right: //如果是拖动右边的边
                    deltaHorizontal = -e.HorizontalChange;
                    newWidth = oldWidth - deltaHorizontal;
                    break;
            }
        }

        var multipleX = newWidth / oldWidth;
        var multipleY = newHeight / oldHeight;
        switch (VerticalAlignment) //改变图形的高度
        {
            case VerticalAlignment.Bottom:
                for (var i = 0; i < _borderWithAdorner.EllipseList.Count; ++i)
                {
                    var item = _borderWithAdorner.EllipseList[i].Data as EllipseGeometry;
                    var borderWithDrag = _borderWithAdorner.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWithDrag.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newY = multipleY * (p1.Y - minY) + minY;
                    item.Center = new Point { X = p1.X, Y = newY };
                }

                break;
            case VerticalAlignment.Top:
                for (var i = 0; i < _borderWithAdorner.EllipseList.Count; ++i)
                {
                    var item = _borderWithAdorner.EllipseList[i].Data as EllipseGeometry;
                    var borderWithDrag = _borderWithAdorner.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWithDrag.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newY = multipleY * (p1.Y - maxY) + maxY;
                    item.Center = new Point { X = p1.X, Y = newY };
                }

                break;
        }

        switch (HorizontalAlignment) //改变图形的宽度
        {
            case HorizontalAlignment.Left:
                for (var i = 0; i < _borderWithAdorner.EllipseList.Count; ++i)
                {
                    var item = _borderWithAdorner.EllipseList[i].Data as EllipseGeometry;
                    var borderWithDrag = _borderWithAdorner.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWithDrag.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newX = multipleX * (p1.X - maxX) + maxX;
                    item.Center = new Point { X = newX, Y = p1.Y };
                    if (i < 2)
                    {
                        Console.WriteLine("mouse:  " + e.HorizontalChange + "     p" + i + ":  " + newX + "      ");
                    }
                }

                break;
            case HorizontalAlignment.Right:
                for (var i = 0; i < _borderWithAdorner.EllipseList.Count; ++i)
                {
                    var item = _borderWithAdorner.EllipseList[i].Data as EllipseGeometry;
                    var borderWithDrag = _borderWithAdorner.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWithDrag.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newX = multipleX * (p1.X - minX) + minX;

                    item.Center = new Point { X = newX, Y = p1.Y };
                }

                break;
        }

        _borderWithAdorner.MinX = _borderWithAdorner.MaxX;
        _borderWithAdorner.MaxX = 0;
        _borderWithAdorner.MinY = _borderWithAdorner.MaxY;

        _borderWithAdorner.MaxY = 0;

        foreach (var path in _borderWithAdorner.EllipseList) //重新计算图形的四个边角值
        {
            var item = path.Data as EllipseGeometry;
            var p = item.Center;
            if (_borderWithAdorner.MaxX < p.X)
            {
                _borderWithAdorner.MaxX = p.X;
            }

            if (_borderWithAdorner.MaxY < p.Y)
            {
                _borderWithAdorner.MaxY = p.Y;
            }

            if (_borderWithAdorner.MinX > p.X)
            {
                _borderWithAdorner.MinX = p.X;
            }

            if (_borderWithAdorner.MinY > p.Y)
            {
                _borderWithAdorner.MinY = p.Y;
            }
        }

        GeometryChrome.Arrange(_borderWithAdorner.GeometryAdorner.GeometryChrome);
        e.Handled = true;
    }

    /// <summary>
    ///     缩放后自动吸附
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        for (var i = 0; i < _borderWithAdorner.EllipseList.Count; ++i)
        {
            var item = _borderWithAdorner.EllipseList[i].Data as EllipseGeometry;
            var p1 = item.Center;
            item.Center = new AutoPoints().GetAutoAdsorbPoint(p1);
        }

        e.Handled = true;
    }
}