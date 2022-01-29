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
    /// <summary>
    /// 图形的Border
    /// </summary>
    private BorderWithAdorner _borderWithAdorner;

    /// <summary>
    /// 图形所在的画布
    /// </summary>
    private Canvas _canvas;

    /// <summary>
    /// 图形的重点
    /// </summary>
    private Point _centerPoint;

    /// <summary>
    /// 开始的坐标向量
    /// </summary>
    private Vector _startVector;

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
        _borderWithAdorner = DataContext as BorderWithAdorner;

        if (_borderWithAdorner != null)
        {
            _canvas = VisualTreeHelper.GetParent(_borderWithAdorner) as Canvas;
            if (_canvas != null)
            {
                _centerPoint = _borderWithAdorner.TranslatePoint(
                    new Point
                    {
                        X = (_borderWithAdorner.MaxX + _borderWithAdorner.MinX) / 2.0,
                        Y = (_borderWithAdorner.MaxY + _borderWithAdorner.MinY) / 2.0
                    },
                    _canvas);

                var startPoint = Mouse.GetPosition(_canvas);
                _startVector = Point.Subtract(startPoint, _centerPoint); //计算开始的向量
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
        if (_borderWithAdorner != null && _canvas != null)
        {
            var currentPoint = Mouse.GetPosition(_canvas);
            var deltaVector = Point.Subtract(currentPoint, _centerPoint);
            var angle = Vector.AngleBetween(_startVector, deltaVector) / 360 * 2 * Math.PI;
            var centerX = (_borderWithAdorner.MaxX + _borderWithAdorner.MinX) / 2.0;
            var centerY = (_borderWithAdorner.MaxY + _borderWithAdorner.MinY) / 2.0; //计算旋转的角度，中点坐标

            foreach (var item in _borderWithAdorner.EllipseList) //根据公式来计算算旋转后的位置
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
            _startVector = Point.Subtract(startPoint, _centerPoint); //重新赋值开始的向量
        }
    }

    /// <summary>
    ///     结束旋转，并自动吸附到最近的点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        foreach (var item in _borderWithAdorner.EllipseList)
        {
            var ellipse = item.Data as EllipseGeometry;
            var oldPoint = ellipse.Center;
            var p = new AutoPoints().GetAutoAdsorbPoint(oldPoint);
            ellipse.Center = new Point { X = p.X, Y = p.Y };
        }
    }
}