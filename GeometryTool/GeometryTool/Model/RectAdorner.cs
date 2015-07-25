using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool
{
    /*----------------------------------------------------------------

          // 文件功能描述：定义一个装饰器

   ----------------------------------------------------------------*/
    public class RectAdorner : Adorner
    {

        double X1=100;
        double X2=100;
        double Y1=100;
        double Y2=100;
        public BorderWithAdornor BorderWithAdornor;
        //public RectAdorner(UIElement adornedElement, double x1, double y1, double width, double height, BorderWithAdornor vBorderWithAdornor)
        //    : base(adornedElement)
        //{
        //    this.x = x;
        //    this.y = y;
        //    this.width = width;
        //    this.height = height;
        //    this.BorderWithAdornor = vBorderWithAdornor;

        //    BorderWithAdornor.rectAdornor = this;
        //    //this.MouseLeftButtonDown += this.ButtonClicked;
        //    this.Visibility = Visibility.Hidden;
            
        //}

        public RectAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(X1, X2, Y1, Y2);
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }

        protected void ButtonClicked(object sender, RoutedEventArgs e)
        {

        }

    }
}
