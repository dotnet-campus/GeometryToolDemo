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
        public GeometryAdorner GAdorner;
        public double MaxX;
        public double MinX;
        public double MaxY;
        public double MinY;
        public List<Path> EllipseList;
        public BorderWithAdorner()
        {
            this.MouseDown += Element_MouseDown;
            EllipseList = new List<Path>();
        }

        /// <summary>
        /// 当鼠标点击图形的的时候，产生边线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainWindow.ActionMode == "Select")
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.Parent as Canvas);
                if (layer != null)
                {
                    if (EllipseList.Count > 0)
                    {
                        MinX = (EllipseList[0].Data as EllipseGeometry).Center.X;
                        MinY = (EllipseList[0].Data as EllipseGeometry).Center.Y;
                        MaxX = 0;
                        MaxY = 0;
                    }
                    foreach (var path in EllipseList)
                    {
                        EllipseGeometry item = path.Data as EllipseGeometry;
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
                        GAdorner = new GeometryAdorner(this);
                        layer.Add(this.GAdorner);
                    }
                }
            }
        }
    }
}
