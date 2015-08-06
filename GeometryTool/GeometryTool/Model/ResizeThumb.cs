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
        private Point transformOrigin;
        private BorderWithAdorner borderWA;
        private Canvas canvas;

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted+=ResizeThumb_DragCompleted;
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.borderWA = this.DataContext as BorderWithAdorner;

            if (this.borderWA != null)
            {
                this.canvas = VisualTreeHelper.GetParent(this.borderWA) as Canvas;

                if (this.canvas != null)
                {
                    this.transformOrigin = this.borderWA.RenderTransformOrigin;

                    this.rotateTransform = this.borderWA.RenderTransform as RotateTransform;
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

        /// <summary>
        /// 图形的缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double MaxX = borderWA.MaxX, MaxY = borderWA.MaxY;          //记录图形四个角落的值
            double MinX = borderWA.MinX, MinY =borderWA.MinY;
            double oldHeight = this.borderWA.ActualHeight;              //记录图形未缩放之前的高度
            double oldWidth = this.borderWA.ActualWidth;               //记录图形未缩放之前的宽度
            double newWidth = 0;                                        //缩放后图形新的宽度
            double newHeight = 0;
            double deltaVertical, deltaHorizontal=0;                    //缩放后图形新的高度
            if (this.borderWA != null)
            {
                switch (VerticalAlignment)
                {
                    case System.Windows.VerticalAlignment.Bottom:       //如果是拖动下面的边
                        deltaVertical = -e.VerticalChange;
                        newHeight = oldHeight - deltaVertical;
                        break;
                    case System.Windows.VerticalAlignment.Top:          //如果是拖动上面的边
                        deltaVertical = e.VerticalChange;
                        newHeight = oldHeight - deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case System.Windows.HorizontalAlignment.Left:       //如果是拖动左边的边
                        deltaHorizontal = e.HorizontalChange;
                        newWidth = oldWidth - deltaHorizontal ;
                        
                        break;
                    case System.Windows.HorizontalAlignment.Right:      //如果是拖动右边的边
                        deltaHorizontal = -e.HorizontalChange;
                        newWidth = oldWidth - deltaHorizontal ;
                        break;
                    default:
                        break;
                }
            }
            double MultipleY = newHeight / oldHeight;                   
            double MultipleX = newWidth / oldWidth;
            switch (VerticalAlignment)                                  //改变图形的高度
            {
                case System.Windows.VerticalAlignment.Bottom:      
                    for (int i = 0; i < borderWA.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = borderWA.EllipseList[i].Data as EllipseGeometry;
                        BorderWithDrag borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                        if (borderWD.HasOtherPoint)
                        {
                            continue;
                        }
                        Point p1 = item.Center;
                        double newY = MultipleY * (p1.Y - MinY) + MinY;
                        item.Center = new Point() { X = p1.X, Y = newY };
                    }
                    break;
                case System.Windows.VerticalAlignment.Top:
                    for (int i = 0; i < borderWA.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = borderWA.EllipseList[i].Data as EllipseGeometry;
                        BorderWithDrag borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                        if (borderWD.HasOtherPoint)
                        {
                            continue;
                        }
                        Point p1 = item.Center;
                        double newY = MultipleY * (p1.Y - MaxY) + MaxY;
                        item.Center = new Point() { X = p1.X, Y = newY };
                        
                    }
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment)                        //改变图形的宽度
            {
                case System.Windows.HorizontalAlignment.Left:
                    for (int i = 0; i < borderWA.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = borderWA.EllipseList[i].Data as EllipseGeometry;
                        BorderWithDrag borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                        if (borderWD.HasOtherPoint)
                        {
                            continue;
                        }
                        Point p1 = item.Center;
                        double newX = MultipleX * (p1.X - MaxX) + MaxX;
                        item.Center = new Point() { X = newX, Y = p1.Y };
                        if(i<2)
                        Console.WriteLine("mouse:  " + e.HorizontalChange+"     p"+i+":  "+newX+"      ");
                    }

                    break;
                case System.Windows.HorizontalAlignment.Right:
                    for (int i = 0; i < borderWA.EllipseList.Count; ++i)
                    {
                        EllipseGeometry item = borderWA.EllipseList[i].Data as EllipseGeometry;
                        BorderWithDrag borderWD = borderWA.EllipseList[i].Parent as BorderWithDrag;
                        if (borderWD.HasOtherPoint)
                        {
                            continue;
                        }
                        Point p1 = item.Center;
                        double newX = MultipleX * (p1.X - MinX) + MinX;

                        item.Center = new Point() { X = newX, Y = p1.Y };
                    }
                    break;
                default:
                    break;
            }
            borderWA.MinX = borderWA.MaxX;
            borderWA.MaxX = 0;
            borderWA.MinY = borderWA.MaxY ;
            
            borderWA.MaxY = 0;

            foreach (var path in borderWA.EllipseList)          //重新计算图形的四个边角值
            {
                EllipseGeometry item = path.Data as EllipseGeometry;
                Point p = item.Center;
                if (borderWA.MaxX < p.X)
                {
                    borderWA.MaxX = p.X;
                }
                if (borderWA.MaxY < p.Y)
                {
                    borderWA.MaxY = p.Y;
                }
                if (borderWA.MinX > p.X)
                {
                    borderWA.MinX = p.X;
                }
                if (borderWA.MinY > p.Y)
                {
                    borderWA.MinY = p.Y;
                }
            }
            GeometryChrome.Arrange(borderWA.GAdorner.chrome);
            e.Handled = true;
        }

        /// <summary>
        /// 缩放后自动吸附
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e) 
        {
            for (int i = 0; i < borderWA.EllipseList.Count; ++i)
            {
                EllipseGeometry item = borderWA.EllipseList[i].Data as EllipseGeometry;
                Point p1 = item.Center;
                item.Center = new AutoPoints().GetAutoAdsorbPoint(p1);
            }
            e.Handled = true;
        }
    }
}
