using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool
{
    public class SizeAdorner : Adorner
    {
        private SizeChrome chrome;
        private VisualCollection visuals;
        private BorderWithAdorner borderWA;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public SizeAdorner(BorderWithAdorner borderWA)
            : base(borderWA)
        {
            this.SnapsToDevicePixels = true;
            this.borderWA = borderWA;
            this.chrome = new SizeChrome();
            this.chrome.DataContext = borderWA;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(new Point(0.0, 0.0), arrangeBounds));
            return arrangeBounds;
        }
    }
}
