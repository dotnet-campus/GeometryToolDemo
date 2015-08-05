using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool
{
    public class ResizeRotateAdorner : Adorner
    {
        private VisualCollection visuals;
        private ResizeRotateChrome chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public ResizeRotateAdorner(ContentControl borderWA)
            : base(borderWA)
        {
            SnapsToDevicePixels = true;
            this.chrome = new ResizeRotateChrome();
            this.chrome.DataContext = borderWA;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }
}
