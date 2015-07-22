using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace GeometryTool
{
    public class GraphAppearance
    {
        public int StrokeThickness { get;set;}
        public System.Windows.Media.Brush Stroke { get; set; }
        public System.Windows.Media.Brush Fill { get; set; }

        public GraphAppearance()
        {
            StrokeThickness = 1;
            Stroke = Brushes.Black;
            Fill = Brushes.White;
        }
    }
}
