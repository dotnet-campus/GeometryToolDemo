using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GeometryTool
{
    /// <summary>
    /// 图形旋转的Thumb
    /// </summary>
    public class RotateThumb : Thumb
    {
        private Vector startVector;             //开始的坐标向量
        private Point centerPoint;              //图形的重点
        private BorderWithAdorner borderWA;     //图形的Border
        private Canvas canvas;                  //图形所在的画布

        public RotateThumb()
        {
            DragDelta += this.RotateThumb_DragDelta;
            DragStarted += this.RotateThumb_DragStarted;
            DragCompleted+=RotateThumb_DragCompleted;
        }


        /// <summary>
        /// 开始旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.borderWA = DataContext as BorderWithAdorner;

            if (this.borderWA != null)
            {
                this.canvas = VisualTreeHelper.GetParent(this.borderWA) as Canvas;
                if (this.canvas != null)
                {
                    this.centerPoint = this.borderWA.TranslatePoint(
                        new Point() { X = (borderWA.MaxX + borderWA.MinX) / 2.0, Y = (borderWA.MaxY + borderWA.MinY) / 2.0 },
                                  this.canvas);

                    Point startPoint = Mouse.GetPosition(this.canvas);
                    this.startVector = Point.Subtract(startPoint, this.centerPoint);    //计算开始的向量

                }
            }
        }

        /// <summary>
        /// 旋转的过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.borderWA != null && this.canvas != null)
            {
                Point currentPoint = Mouse.GetPosition(this.canvas);
                Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);
                double angle = Vector.AngleBetween(this.startVector, deltaVector)/360*2*Math.PI;
                double centerX = (borderWA.MaxX + borderWA.MinX) / 2.0;
                double centerY = (borderWA.MaxY + borderWA.MinY) / 2.0;     //计算旋转的角度，中点坐标

                foreach (var item in borderWA.EllipseList)                  //根据公式来计算算旋转后的位置
                {
                    EllipseGeometry ellipse = item.Data as EllipseGeometry;
                    Point oldPoint = ellipse.Center;
                    double newX = (oldPoint.X - centerX) * Math.Cos(angle) - (oldPoint.Y - centerY) * Math.Sin(angle) + centerX;
                    double newY = (oldPoint.X - centerX) * Math.Sin(angle) + (oldPoint.Y - centerY) * Math.Cos(angle) + centerY;
                    ellipse.Center = new Point() { X=newX,Y=newY};
                }

                Point startPoint = currentPoint;        
                this.startVector = Point.Subtract(startPoint, this.centerPoint);    //重新赋值开始的向量
            }
        }

        /// <summary>
        /// 结束旋转，并自动吸附到最近的点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            foreach (var item in borderWA.EllipseList)
            {
                EllipseGeometry ellipse = item.Data as EllipseGeometry;
                Point oldPoint = ellipse.Center;
                Point p = new AutoPoints().GetAutoAdsorbPoint(oldPoint);
                ellipse.Center = new Point() { X=p.X,Y=p.Y };
            }
        }
        
    }
}
