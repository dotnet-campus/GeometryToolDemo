using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.ComponentModel;

namespace GeometryTool
{
    /*----------------------------------------------------------------

           // 文件功能描述：定义图形的外观

    ----------------------------------------------------------------*/
    public class GraphAppearance
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int strokeThickness;  //设置图形的线条的大小
        public int StrokeThickness 
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
        
        public GraphAppearance()
        {
            StrokeThickness = 1;
            Stroke = Brushes.Black;
            Fill = Brushes.White;
        }
    }
}
