using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     图形旋转的Thumb
/// </summary>
public class RotateThumb : Thumb
{
    private BorderWithAdorner borderWA; //图形的Border
    private Canvas canvas; //图形所在的画布
    private Point centerPoint; //图形的重点
    private Vector startVector; //开始的坐标向量

    public RotateThumb()
    {
        DragDelta += RotateThumb_DragDelta;
        DragStarted += RotateThumb_DragStarted;
        DragCompleted += RotateThumb_DragCompleted;
    }


    /// <summary>
    ///     开始旋转
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        borderWA = DataContext as BorderWithAdorner;

        if (borderWA != null)
        {
            canvas = VisualTreeHelper.GetParent(borderWA) as Canvas;
            if (canvas != null)
            {
                centerPoint = borderWA.TranslatePoint(
                    new Point { X = (borderWA.MaxX + borderWA.MinX) / 2.0, Y = (borderWA.MaxY + borderWA.MinY) / 2.0 },
                    canvas);

                var startPoint = Mouse.GetPosition(canvas);
                startVector = Point.Subtract(startPoint, centerPoint); //计算开始的向量
            }
        }
    }

    /// <summary>
    ///     旋转的过程
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (borderWA != null && canvas != null)
        {
            var currentPoint = Mouse.GetPosition(canvas);
            var deltaVector = Point.Subtract(currentPoint, centerPoint);
            var angle = Vector.AngleBetween(startVector, deltaVector) / 360 * 2 * Math.PI;
            var centerX = (borderWA.MaxX + borderWA.MinX) / 2.0;
            var centerY = (borderWA.MaxY + borderWA.MinY) / 2.0; //计算旋转的角度，中点坐标

            foreach (var item in borderWA.EllipseList) //根据公式来计算算旋转后的位置
            {
                if ((item.Parent as BorderWithDrag).HasOtherPoint)
                {
                    continue;
                }

                var ellipse = item.Data as EllipseGeometry;
                var oldPoint = ellipse.Center;
                var newX = (oldPoint.X - centerX) * Math.Cos(angle) - (oldPoint.Y - centerY) * Math.Sin(angle) +
                           centerX;
                var newY = (oldPoint.X - centerX) * Math.Sin(angle) + (oldPoint.Y - centerY) * Math.Cos(angle) +
                           centerY;
                ellipse.Center = new Point { X = newX, Y = newY };
            }

            var startPoint = currentPoint;
            startVector = Point.Subtract(startPoint, centerPoint); //重新赋值开始的向量
        }
    }

    /// <summary>
    ///     结束旋转，并自动吸附到最近的点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        foreach (var item in borderWA.EllipseList)
        {
            var ellipse = item.Data as EllipseGeometry;
            var oldPoint = ellipse.Center;
            var p = new AutoPoints().GetAutoAdsorbPoint(oldPoint);
            ellipse.Center = new Point { X = p.X, Y = p.Y };
        }
    }
}