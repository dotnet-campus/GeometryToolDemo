using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     实现图形拖动的Thumb
/// </summary>
public class MoveThumb : Thumb
{
    /// <summary>
    /// 包含该图形的BorderWithAdorner
    /// </summary>
    private BorderWithAdorner _borderWithAdorner;

    /// <summary>
    ///     构造函数，注册拖动事件
    /// </summary>
    public MoveThumb()
    {
        DragStarted += MoveThumb_DragStarted;
        DragDelta += MoveThumb_DragDelta;
        DragCompleted += MoveThumb_DragCompleted;
    }

    /// <summary>
    ///     开始拖动，主要是获取包含着被拖动的图形的BorderWithAdorner
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        _borderWithAdorner = DataContext as BorderWithAdorner;
    }

    /// <summary>
    ///     图形被拖动过程
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (_borderWithAdorner != null)
        {
            var dragDelta = new Point(e.HorizontalChange, e.VerticalChange); //获取移动的距离
            var border = _borderWithAdorner;
            for (var i = 0; i < border.EllipseList.Count; ++i)
            {
                var borderWithDrag = border.EllipseList[i].Parent as BorderWithDrag;
                if (borderWithDrag.HasOtherPoint) //如果该点有锁，便跳过移动
                {
                    continue;
                }

                var item = border.EllipseList[i].Data as EllipseGeometry; //更新点的坐标
                item.Center = new Point
                    { X = item.Center.X + e.HorizontalChange, Y = item.Center.Y + e.VerticalChange };
            }
        }
    }

    /// <summary>
    ///     图形结束拖动，实现自动吸附
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        for (var i = 0; i < _borderWithAdorner.EllipseList.Count; ++i)
        {
            var item = _borderWithAdorner.EllipseList[i].Data as EllipseGeometry;
            var p1 = item.Center;
            item.Center = new AutoPoints().GetAutoAdsorbPoint(p1);
        }
    }
}