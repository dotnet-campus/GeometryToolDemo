using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace GeometryTool
{
    public class RectAdorner:Adorner
    {
        public Controlborder controlBorder;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="vAdornerElement">需要被装饰的控件</param>
        //public RectAdorner(UIElement vAdornerElement)
        //    :base(vAdornerElement)
        //{ }

        //protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);
        //}

        protected override int VisualChildrenCount {
		get { return 1; }
	}

        public RectAdorner(UIElement adornedElement, double x, double y, double width, double height)
		: base(adornedElement) {

            controlBorder = new Controlborder(width,height,x,y);
            controlBorder.DataContext = adornedElement;
            this.AddVisualChild(controlBorder);
	}


	protected override Visual GetVisualChild(int index) {
        return controlBorder;
	}

	protected override Size ArrangeOverride(Size arrangeBounds) {
        controlBorder.Arrange(new Rect(arrangeBounds));
		return arrangeBounds;
	}
            
    }
    
}
