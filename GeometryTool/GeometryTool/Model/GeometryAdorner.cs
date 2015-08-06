using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryTool
{
    public class GeometryAdorner:Adorner
    {
         private VisualCollection visuals;
        public GeometryChrome chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public GeometryAdorner(BorderWithAdorner borderWA)
            : base(borderWA)
        {
            SnapsToDevicePixels = true;
            this.chrome = new GeometryChrome();
            this.chrome.DataContext = borderWA;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            arrangeBounds.Height += 0.5;
            arrangeBounds.Width += 0.5;
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }
}
