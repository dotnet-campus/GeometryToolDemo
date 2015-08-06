using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;

namespace GeometryTool
{
    /*----------------------------------------------------------------

           // 文件功能描述：定义图形的外观

    ----------------------------------------------------------------*/
    //public class GraphAppearance:DependencyObject
    public class GraphAppearance
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double strokeThickness;  //设置图形的线条的大小
        public double StrokeThickness 
        { 
            get{return strokeThickness;}
            set 
            {
                strokeThickness = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StrokeThickness"));
                }
            }
        }
       // public static readonly DependencyProperty 

        private DoubleCollection strokeDashArray;   //设置虚实
        public DoubleCollection StrokeDashArray
        {
            get { return strokeDashArray; }
            set
            {
                strokeDashArray = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StrokeDashArray"));
                }
            }
        }

        private System.Windows.Media.Brush stroke { get; set; } //设置图形的颜色
        public Brush Stroke
        {
            get { return stroke; }
            set
            {
                stroke = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Stroke"));
                }
            }
        }

        private System.Windows.Media.Brush fill { get; set; }   //设置图形填充的颜色
        public Brush Fill
        {
            get { return fill; }
            set
            {
                fill = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Fill"));
                }
            }
        }

        public FillRule fillRule;
        public FillRule FillRule
        {
            get { return fillRule; }
            set
            {
                fillRule = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FillRule"));
                }
            }
        }
       
        public GraphAppearance()
        {
            StrokeThickness = 0.1;
            Stroke = Brushes.Black;
            Fill = Brushes.Transparent;
            FillRule = FillRule.EvenOdd;
        }
    }
}
