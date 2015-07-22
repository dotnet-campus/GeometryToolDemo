using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeometryTool
{
    public class Controlborder : Control
    {

        int x;
        int y;
        int width = 0;
        int height = 0;
        Rect rect;
        public Rect Rect 
        { 
            get { return (Rect)GetValue(RectProperty); }
        }
        public static readonly DependencyProperty RectProperty =
           DependencyProperty.Register("Rect", typeof(ImageSource), typeof(Controlborder), new UIPropertyMetadata(null));
        static Controlborder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Controlborder), new System.Windows.FrameworkPropertyMetadata(typeof(Controlborder)));
        }
        public Controlborder(double vWidth, double vHeight, double x, double y)
        {
            this.width = (int)vWidth;
            this.height = (int)vHeight;
            rect = new Rect();
            rect.Width = this.width;
            rect.Height = this.height;
            rect.X = 0;
            rect.Y = 0;
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {

            this.Width = width + 10;
            this.Height = height + 10;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
