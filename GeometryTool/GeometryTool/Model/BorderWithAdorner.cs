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
                ShowAdorner();
            }
        }

        public void ShowAdorner()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.Parent as Canvas);
            if (layer != null)
            {
                GetFourPoint(this);
                if (this.GAdorner == null)
                {
                    Path path1 = this.Child as Path;
                    GAdorner = new GeometryAdorner(this);
                    layer.Add(this.GAdorner);
                }
            }
        }

        /// <summary>
        /// 获取图形四个边角的位置
        /// </summary>
        public void GetFourPoint(BorderWithAdorner vBorderWA)
        {
            if (vBorderWA.EllipseList.Count > 0)
            {
                vBorderWA.MinX = (EllipseList[0].Data as EllipseGeometry).Center.X;
                vBorderWA.MinY = (EllipseList[0].Data as EllipseGeometry).Center.Y;
                vBorderWA.MaxX = 0;
                vBorderWA.MaxY = 0;
            }
            foreach (var path in vBorderWA.EllipseList)
            {
                EllipseGeometry item = path.Data as EllipseGeometry;
                Point point = item.Center;
                if (vBorderWA.MaxX < point.X)
                {
                    vBorderWA.MaxX = point.X;
                }
                if (vBorderWA.MaxY < point.Y)
                {
                    vBorderWA.MaxY = point.Y;
                }
                if (vBorderWA.MinX > point.X)
                {
                    vBorderWA.MinX = point.X;
                }
                if (vBorderWA.MinY > point.Y)
                {
                    vBorderWA.MinY = point.Y;
                }
            }
        }
       
        /// <summary>
        /// 用于复制Border，模仿深度复制
        /// </summary>
        /// <param name="vBorderWA"></param>
        /// <returns></returns>
        public BorderWithAdorner CopyBorder(BorderWithAdorner vBorderWA)
        {
            Path path = vBorderWA.Child as Path;
            GraphAppearance graphAppearance = new GraphAppearance() 
            {
                Stroke = path.Stroke,
                StrokeThickness = path.StrokeThickness,
                StrokeDashArray = path.StrokeDashArray,
                Fill = path.Fill
            };
            GeomertyStringConverter gsc = new GeomertyStringConverter(MainWindow.myRootCanvas, graphAppearance);
            string MiniLanguage = gsc.StringFromGeometry(vBorderWA.Child as Path);  //把该图形转化成为Mini-Language
            BorderWithAdorner newBorderWA = gsc.GeomotryFromString(MiniLanguage);   //把Mini-Language转化成为图形，实现图形的深度复制
            Path newPath = newBorderWA.Child as Path;
            newPath.Stroke = path.Stroke;
            newPath.StrokeThickness = path.StrokeThickness;
            newPath.StrokeDashArray = path.StrokeDashArray;
            newPath.Fill = path.Fill;
            return newBorderWA;
        }
    }
}
