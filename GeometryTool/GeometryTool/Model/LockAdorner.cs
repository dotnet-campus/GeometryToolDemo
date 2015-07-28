using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool
{
    /// <summary>
    /// 融合的Adorner
    /// </summary>
    public class LockAdorner: Adorner
    {
        private VisualCollection visuals;
        private LockChrome chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public LockAdorner(UIElement designerItem)
            : base(designerItem)
        {
            SnapsToDevicePixels = true;
            this.chrome = new LockChrome();
            this.chrome.DataContext = designerItem;
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
