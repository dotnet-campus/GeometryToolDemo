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
    /// <summary>
    /// 选择框的Adorner
    /// </summary>
    public class GeometryAdorner:Adorner
    {
         private VisualCollection visuals;
        public GeometryChrome chrome;       //选择框的真正样式

        /// <summary>
        /// 重写的VisualChildrenCount函数
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        /// <summary>
        /// 构造函数，主要是使chrome是DataContext为BorderWithAdorner
        /// </summary>
        /// <param name="borderWA"></param>
        public GeometryAdorner(BorderWithAdorner borderWA)
            : base(borderWA)
        {
            SnapsToDevicePixels = true;
            this.chrome = new GeometryChrome();
            this.chrome.DataContext = borderWA;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
        }

        /// <summary>
        /// 重写的ArrangeOverride
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            arrangeBounds.Height += 0.5;
            arrangeBounds.Width += 0.5;
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        /// <summary>
        /// 重写GetVisualChild函数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }
}
