using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace GeometryTool
{
    public class ResizeThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private double angle;
        private Adorner adorner;
        private Point transformOrigin;
        private BorderWithAdorner designerItem;
        private Canvas canvas;

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = this.DataContext as BorderWithAdorner;

            if (this.designerItem != null)
            {
                this.canvas = VisualTreeHelper.GetParent(this.designerItem) as Canvas;

                if (this.canvas != null)
                {
                    this.transformOrigin = this.designerItem.RenderTransformOrigin;

                    this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                    if (this.rotateTransform != null)
                    {
                        this.angle = this.rotateTransform.Angle * Math.PI / 180.0;
                    }
                    else
                    {
                        this.angle = 0.0d;
                    }

                }
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double MaxX = designerItem.MaxX, MaxY = designerItem.MaxY;
            double MinX = designerItem.MinX, MinY =designerItem.MinY;
            double oldHeight = this.designerItem.ActualHeight;
            double oldWidth = this.designerItem.ActualHeight;
            double newWidth = 0;
            double newHeight = 0;
            if (this.designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case System.Windows.VerticalAlignment.Bottom:
                        deltaVertical = -e.VerticalChange;
                        newHeight = oldHeight - deltaVertical;
                        break;
                    case System.Windows.VerticalAlignment.Top:
                        deltaVertical = e.VerticalChange;
                        newHeight = oldHeight - deltaVertical;
                        break;
                    default:
                        break;
                }



                switch (HorizontalAlignment)
                {
                    case System.Windows.HorizontalAlignment.Left:
                        deltaHorizontal = e.HorizontalChange;
                        newWidth = oldWidth - deltaHorizontal ;
                        break;
                    case System.Windows.HorizontalAlignment.Right:
                        deltaHorizontal = -e.HorizontalChange;
                        newWidth = oldWidth - deltaHorizontal ;
                        break;
                    default:
                        break;
                }
            }
            double MultipleY = newHeight / oldHeight;
            double MultipleX = newWidth / oldWidth;

            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    for (int i = 0; i < designerItem.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = designerItem.EllipseList[i];
                        Point p1 = item.Center;
                        item.Center = new Point() { X = p1.X, Y = MultipleY * (p1.Y - MinY) + MinY };
                    }
                    break;
                case System.Windows.VerticalAlignment.Top:
                    for (int i = 0; i < designerItem.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = designerItem.EllipseList[i];
                        Point p1 = item.Center;
                        item.Center = new Point() { X = p1.X, Y = MultipleY * (p1.Y - MaxY) + MaxY };
                    }
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    for (int i = 0; i < designerItem.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = designerItem.EllipseList[i];
                        Point p1 = item.Center;
                        item.Center = new Point() { X = MultipleX * (p1.X - MaxX) + MaxX, Y = p1.Y };
                    }

                    break;
                case System.Windows.HorizontalAlignment.Right:
                    for (int i = 0; i < designerItem.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = designerItem.EllipseList[i];
                        Point p1 = item.Center;
                        item.Center = new Point() { X = MultipleX * (p1.X - MinX) + MinX, Y = p1.Y };
                    }
                    
                    break;
                default:
                    break;
            }
            designerItem.MaxX = 0;
            designerItem.MinX = designerItem.EllipseList[0].Center.X;
            designerItem.MaxY = 0;
            designerItem.MinY = designerItem.EllipseList[0].Center.X;

            foreach (EllipseGeometry item in designerItem.EllipseList)
            {
                Point p = item.Center;
                if (designerItem.MaxX < p.X)
                {
                    designerItem.MaxX = p.X;
                }
                if (designerItem.MaxY < p.Y)
                {
                    designerItem.MaxY = p.Y;
                }
                if (designerItem.MinX > p.X)
                {
                    designerItem.MinX = p.X;
                }
                if (designerItem.MinY > p.Y)
                {
                    designerItem.MinY = p.Y;
                }
            }

            //designerItem.GAdorner.chrome.ReArrange(new Size() { Width = designerItem.ActualWidth+5, Height = designerItem.ActualHeight+5 });
            e.Handled = true;
        }

        
    }
}
