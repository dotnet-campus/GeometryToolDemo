using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeometryTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isStartPoint;
        Line LastLine;
        GraphAdd graphAdd;
        GraphAppearance graphAppearance;
        public MainWindow()
        {
            InitializeComponent();

            graphAdd = new GraphAdd();
            graphAppearance = new GraphAppearance();
            isStartPoint = true;
            LastLine = null;
        }


        private void Select_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                this.RootCanvas.Tag = radioButton.ToolTip;
            }
            e.Handled = true;
        }

        private void RootCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = Mouse.GetPosition(e.Source as FrameworkElement);
            switch (this.RootCanvas.Tag.ToString())
            {
                case "Line": {
                    HitTestResult result = VisualTreeHelper.HitTest(RootCanvas, p);
                    Path path=null;
                    path=result.VisualHit as Path;
                    
                    while (path == null)
                    {
                        result = VisualTreeHelper.HitTest(RootCanvas, p);
                        path = null;
                        path = result.VisualHit as Path;
                    }
                    break;
                }
                case "Point": { graphAdd.AddPoint(p, graphAppearance, this.RootCanvas); break; }
            }
            
        }
    }
   
}
