using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace GeometryTool
{
    public class myBorder : Border
    {
        public RectAdorner rectAdornor;
       
        public myBorder()
        {
            this.MouseLeftButtonDown += ButtonClicked;
        }
        protected void ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (rectAdornor != null)
            {
                rectAdornor.Visibility = Visibility.Visible;
                e.Handled = true;
            }
        }
    }
}
