using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeometryTool
{
    public class MyChrome : ContentControl
    {

        public bool isLock;

        public void ReArrange(Size arrangeBounds)
        {
            //ArrangeOverride(arrangeBounds);
        }
        /// <summary>
        /// 绘制这个锁
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            BorderWithAdorner border = this.DataContext as BorderWithAdorner;
            border.MaxX =0;
            border.MinX = border.EllipseList[0].Center.X;
            border.MaxY = 0;
            border.MinY = border.EllipseList[0].Center.Y;
            foreach(EllipseGeometry item in border.EllipseList)
            {
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
        

            this.Width = border.MaxX-border.MinX+1;
            this.Height = border.MaxY - border.MinY+1;
            this.Margin = new Thickness(border.MinX-0.5, border.MinY-0.5,0,0);
            return base.ArrangeOverride(arrangeBounds);
        }
        }
    
}
