using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GeometryTool
{
    /// <summary>
    /// 实现图形拖动的Thumb
    /// </summary>
    public class MoveThumb : Thumb
    {
        private BorderWithAdorner borderWA; //包含该图形的BorderWithAdorner

        /// <summary>
        /// 构造函数，注册拖动事件
        /// </summary>
        public MoveThumb()
        {
            DragStarted +=this.MoveThumb_DragStarted;
            DragDelta += this.MoveThumb_DragDelta;
            DragCompleted+=MoveThumb_DragCompleted;
        }

        /// <summary>
        /// 开始拖动，主要是获取包含着被拖动的图形的BorderWithAdorner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.borderWA = DataContext as BorderWithAdorner;
        }

        /// <summary>
        /// 图形被拖动过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.borderWA != null)
            {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);  //获取移动的距离
                BorderWithAdorner border = this.borderWA;
                for (int i = 0; i < border.EllipseList.Count; ++i)
                {
                    var borderWD = border.EllipseList[i].Parent as BorderWithDrag;
                    if (borderWD.HasOtherPoint == true)                             //如果该点有锁，便跳过移动
                        continue;

                    var item = border.EllipseList[i].Data as EllipseGeometry;       //更新点的坐标
                    item.Center = new Point() { X = item.Center.X + e.HorizontalChange, Y = item.Center.Y + e.VerticalChange };
                }
            }
        }

        /// <summary>
        /// 图形结束拖动，实现自动吸附
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e) 
        {
            for (int i = 0; i < borderWA.EllipseList.Count; ++i)
            {
                EllipseGeometry item = borderWA.EllipseList[i].Data as EllipseGeometry;
                Point p1 = item.Center;
                item.Center = new AutoPoints().GetAutoAdsorbPoint(p1);
            }
        }
    }
}
