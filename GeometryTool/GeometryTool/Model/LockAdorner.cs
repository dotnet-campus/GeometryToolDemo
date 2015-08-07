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
    /// 融合点时锁的Adorner
    /// </summary>
    public class LockAdorner: Adorner
    {
        private VisualCollection visuals;
        public LockChrome chrome;       //锁的样式

        /// <summary>
        /// 重写的VisualChildrenCount
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        /// <summary>
        /// 重写的构造函数
        /// </summary>
        /// <param name="borderWA"></param>
        public LockAdorner(UIElement borderWA)
            : base(borderWA)
        {
            SnapsToDevicePixels = true;
            this.chrome = new LockChrome();
            this.chrome.DataContext = borderWA;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
        }

        /// <summary>
        /// 重写的ArrangeOverride函数
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        /// <summary>
        /// 重写的GetVisualChild的函数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }
}
