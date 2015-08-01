using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GeometryTool
{
    public class MoveThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private BorderWithAdorner designerItem;

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as BorderWithAdorner;

            if (this.designerItem != null)
            {
                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                if (this.rotateTransform != null)
                {
                    dragDelta = this.rotateTransform.Transform(dragDelta);
                }

                BorderWithAdorner border = this.designerItem;
                foreach (var item in border.EllipseList)
                {
                    item.Center = new Point() { X = item.Center.X + e.HorizontalChange, Y = item.Center.Y + e.VerticalChange };
                }
            }
        }
    }
}
