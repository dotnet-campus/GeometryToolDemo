using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryTool
{
    public class BorderWithAdorner : Border
    {
        public MyAdorner GAdorner;
        public double MaxX;
        public double MinX;
        public double MaxY;
        public double MinY;
        public List<EllipseGeometry> EllipseList;
        public BorderWithAdorner()
        {
            this.MouseDown += Element_MouseDown;
            EllipseList = new List<EllipseGeometry>();
            this.LostFocus+=BorderWithAdorner_LostFocus;
        }
        public void Element_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainWindow.ActionMode == "Select")
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.Parent as Canvas);
                if (layer != null)
                {
                    if (EllipseList.Count > 0)
                    {
                        MinX = 0;
                        MinY = EllipseList[0].Center.Y;
                        MaxX = EllipseList[0].Center.X;
                        MaxY = 0;
                    }
                    foreach (EllipseGeometry item in EllipseList)
                    {
                        Point point = item.Center;
                        if (MaxX < point.X)
                        {
                            MaxX = point.X;
                        }
                        if (MaxY < point.Y)
                        {
                            MaxY = point.Y;
                        }
                        if (MinX > point.X)
                        {
                            MinX = point.X;
                        }
                        if (MinY > point.Y)
                        {
                            MinY = point.Y;
                        }
                    }
                    if (GAdorner == null)
                    {
                        Path path1 = this.Child as Path;
                        GAdorner = new MyAdorner(this);
                        layer.Add(this.GAdorner);
                    }
                }
            }
        }

        public void BorderWithAdorner_LostFocus(object sender,RoutedEventArgs e)
        {
            if (GAdorner != null)
                GAdorner = null;
        }
    }
}
