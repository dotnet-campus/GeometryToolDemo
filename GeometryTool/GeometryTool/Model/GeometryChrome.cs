using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeometryTool
{
    public class GeometryChrome : ContentControl
    {

        public bool isLock;

       
        /// <summary>
        /// 绘制这个锁
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            BorderWithAdorner border = this.DataContext as BorderWithAdorner;
            border.MaxX =0;
            border.MinX = (border.EllipseList[0].Data as EllipseGeometry).Center.X;
            border.MaxY = 0;
            border.MinY = (border.EllipseList[0].Data as EllipseGeometry).Center.Y;
            foreach(var path  in border.EllipseList)
            {
                EllipseGeometry item = path.Data as EllipseGeometry;
                Point p = item.Center;
                if (border.MaxX < p.X)
                {
                    border.MaxX = p.X;
                }
                if (border.MaxY < p.Y)
                {
                    border.MaxY = p.Y;
                }
                if (border.MinX > p.X)
                {
                    border.MinX = p.X;
                }
                if (border.MinY > p.Y)
                {
                    border.MinY = p.Y;
                }
           }

            PathGeometry pg = (border.Child as Path).Data as PathGeometry;
            if (pg.Figures[0].Segments.Count>0)
            {
                var geometry = pg.GetFlattenedPathGeometry();
                var bound = geometry.Bounds;
                this.Width = bound.Width + 1;
                this.Height =bound.Height+1;
                this.Margin = new Thickness(border.ActualWidth - bound.Width - 0.65, border.ActualHeight - bound.Height - 0.65, 0, 0);
            }
            else
            {
                this.Width = border.MaxX - border.MinX + 1;
                this.Height = border.MaxY - border.MinY + 1;

                this.Margin = new Thickness(border.MinX - 0.5, border.MinY - 0.5, 0, 0);
            }
            return base.ArrangeOverride(arrangeBounds);
        }

      
    }
}
