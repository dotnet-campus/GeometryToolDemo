using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryTool
{
    /// <summary>
    /// 主要让border添加了拖动响应，用于点
    /// </summary>
    public class BorderWithDrag : Border
    {
        public RectAdorner rectAdornor;     //表示控件的装饰器
        public Path path;
        bool isDragDropInEffect = false;    //表示是否可以拖动
        System.Windows.Point p;
        /// <summary>
        /// 无参数的构造函数，主要是为了给Border响应鼠标的事件
        /// </summary>
        public BorderWithDrag()
        {
            this.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            this.MouseMove += Element_MouseMove;
            this.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            //path = vPath;
        }

        /// <summary>
        ///拖动鼠标，移动控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseMove(object sender, MouseEventArgs e)
        {
            //判断是否在选择模式下的操作以及是否可以拖动
            if (isDragDropInEffect && MainWindow.ActionMode == "Select")
            {
                Point oldPoint = e.GetPosition(MainWindow.myRootCanvas); //获取目前鼠标的相对位置
                int size = MainWindow.multiple;
                int a = Convert.ToInt32((oldPoint.X));
                Console.WriteLine(a);
                if (oldPoint.X  % size >= size/2)
                { 
                    int b=( (int)( oldPoint.X ))/ size * size;
                    
                    p.X = ((int)(oldPoint.X) / size * size + size);
                    //Console.WriteLine("+ ox:{0} ox:{1} px:{2} px:{3}",oldPoint.X,oldPoint.Y,p.X,p.Y);
                }
                 else
                {
                    p.X = ((int)(oldPoint.X) / size * size); 
                }

                if (oldPoint.Y % size >= size/2)
                { 
                    p.Y = ((int)(oldPoint.Y) / size * size + size);
                    Console.WriteLine("++ size:{0} ox:{1} oy:{2} px:{3} py:{4}",size, oldPoint.X, oldPoint.Y, p.X, p.Y);
                }
                 else
                { 
                    p.Y = ((int)(oldPoint.Y) / size * size);
                    Console.WriteLine("-- size:{0} ox:{1} oy:{2} px:{3} py:{4}", size, oldPoint.X, oldPoint.Y, p.X, p.Y);
                }

                FrameworkElement currEle = sender as FrameworkElement;
                BorderWithDrag myBorder = currEle as BorderWithDrag;
                ((myBorder.Child as Path).Data as EllipseGeometry).Center = new Point() { X = p.X , Y = p.Y  };
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标左击控件，使控件可以被拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //判断是否在选择模式下的操作
            if (MainWindow.ActionMode == "Select")
            {
                FrameworkElement currEle = sender as FrameworkElement;
                
                //判断是否命中的是点
                if (((currEle as BorderWithDrag).Child as Path).Data as EllipseGeometry != null)
                {
                    currEle.CaptureMouse();
                    currEle.Cursor = Cursors.Hand;
                    isDragDropInEffect = true;      //设置可以拖动
                };
                
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标释放，使控件不能被拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       public void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //判断是否在选择模式下的操作以及是否可以拖动
            FrameworkElement currEle = sender as FrameworkElement;
            if (isDragDropInEffect&&MainWindow.ActionMode == "Select")
            {
                isDragDropInEffect = false;
                currEle.ReleaseMouseCapture();
                e.Handled=true;
            }
        } 
    }
}

namespace BitsOfStuff { }
