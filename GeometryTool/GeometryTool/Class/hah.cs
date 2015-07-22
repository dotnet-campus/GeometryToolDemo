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
    public class SimpleCircleAdorner : Adorner
    {

        double x;
        double y;
        double width;
        double height;
        public myBorder myBorder;
        bool isMove;
        public SimpleCircleAdorner(UIElement adornedElement, double x, double y, double width, double height, myBorder vMyBorder)
            : base(adornedElement)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.myBorder = vMyBorder;

            myBorder.rectAdornor = this;
            //this.MouseLeftButtonDown += this.ButtonClicked;
            this.Visibility = Visibility.Hidden;
            
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(x, y, width, height);
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
