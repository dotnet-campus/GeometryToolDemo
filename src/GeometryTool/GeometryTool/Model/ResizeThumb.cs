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
    private BorderWithAdorner borderWA;

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
        borderWA = DataContext as BorderWithAdorner;
    }

    /// <summary>
    ///     图形的缩放
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        double MaxX = borderWA.MaxX, MaxY = borderWA.MaxY; //记录图形四个角落的值
        double MinX = borderWA.MinX, MinY = borderWA.MinY;
        var oldHeight = borderWA.ActualHeight; //记录图形未缩放之前的高度
        var oldWidth = borderWA.ActualWidth; //记录图形未缩放之前的宽度
        double newWidth = 0; //缩放后图形新的宽度
        double newHeight = 0;
        double deltaVertical, deltaHorizontal = 0; //缩放后图形新的高度
        if (borderWA != null)
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

        var MultipleY = newHeight / oldHeight;
        var MultipleX = newWidth / oldWidth;
        switch (VerticalAlignment) //改变图形的高度
        {
            case VerticalAlignment.Bottom:
                for (var i = 0; i < borderWA.EllipseList.Count; ++i)
                {
                    var item = borderWA.EllipseList[i].Data as EllipseGeometry;
                    var borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWD.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newY = MultipleY * (p1.Y - MinY) + MinY;
                    item.Center = new Point { X = p1.X, Y = newY };
                }

                break;
            case VerticalAlignment.Top:
                for (var i = 0; i < borderWA.EllipseList.Count; ++i)
                {
                    var item = borderWA.EllipseList[i].Data as EllipseGeometry;
                    var borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWD.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newY = MultipleY * (p1.Y - MaxY) + MaxY;
                    item.Center = new Point { X = p1.X, Y = newY };
                }

                break;
        }

        switch (HorizontalAlignment) //改变图形的宽度
        {
            case HorizontalAlignment.Left:
                for (var i = 0; i < borderWA.EllipseList.Count; ++i)
                {
                    var item = borderWA.EllipseList[i].Data as EllipseGeometry;
                    var borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWD.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newX = MultipleX * (p1.X - MaxX) + MaxX;
                    item.Center = new Point { X = newX, Y = p1.Y };
                    if (i < 2)
                    {
                        Console.WriteLine("mouse:  " + e.HorizontalChange + "     p" + i + ":  " + newX + "      ");
                    }
                }

                break;
            case HorizontalAlignment.Right:
                for (var i = 0; i < borderWA.EllipseList.Count; ++i)
                {
                    var item = borderWA.EllipseList[i].Data as EllipseGeometry;
                    var borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWD.HasOtherPoint)
                    {
                        continue;
                    }

                    var p1 = item.Center;
                    var newX = MultipleX * (p1.X - MinX) + MinX;

                    item.Center = new Point { X = newX, Y = p1.Y };
                }

                break;
        }

        borderWA.MinX = borderWA.MaxX;
        borderWA.MaxX = 0;
        borderWA.MinY = borderWA.MaxY;

        borderWA.MaxY = 0;

        foreach (var path in borderWA.EllipseList) //重新计算图形的四个边角值
        {
            var item = path.Data as EllipseGeometry;
            var p = item.Center;
            if (borderWA.MaxX < p.X)
            {
                borderWA.MaxX = p.X;
            }

            if (borderWA.MaxY < p.Y)
            {
                borderWA.MaxY = p.Y;
            }

            if (borderWA.MinX > p.X)
            {
                borderWA.MinX = p.X;
            }

            if (borderWA.MinY > p.Y)
            {
                borderWA.MinY = p.Y;
            }
        }

        GeometryChrome.Arrange(borderWA.GAdorner.chrome);
        e.Handled = true;
    }

    /// <summary>
    ///     缩放后自动吸附
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        for (var i = 0; i < borderWA.EllipseList.Count; ++i)
        {
            var item = borderWA.EllipseList[i].Data as EllipseGeometry;
            var p1 = item.Center;
            item.Center = new AutoPoints().GetAutoAdsorbPoint(p1);
        }

        e.Handled = true;
    }
}