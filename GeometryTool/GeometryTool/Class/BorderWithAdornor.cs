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
    /*----------------------------------------------------------------

          // 文件功能描述：定义一个带有装饰器的Border，并且可以响应拖动事件

   ----------------------------------------------------------------*/
    public class BorderWithAdornor : Border
    {
        public RectAdorner rectAdornor;     //表示控件的装饰器
        bool isDragDropInEffect = false;    //表示是否可以拖动
       
        /// <summary>
        /// 无参数的构造函数，主要是为了给Border响应鼠标的事件
        /// </summary>
        public BorderWithAdornor()
        {
            this.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            this.MouseMove += Element_MouseMove;
            this.MouseLeftButtonUp += Element_MouseLeftButtonUp;
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
                FrameworkElement currEle = sender as FrameworkElement;
                System.Windows.Point p = e.GetPosition(Application.Current.MainWindow); //获取目前鼠标的相对位置
                BorderWithAdornor myBorder = currEle as BorderWithAdornor;
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
                if (((currEle as BorderWithAdornor).Child as Path).Data as EllipseGeometry != null)
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
            if (isDragDropInEffect&&MainWindow.ActionMode == "Select")
            {
                FrameworkElement ele = sender as FrameworkElement;
                isDragDropInEffect = false;
                ele.ReleaseMouseCapture();
                e.Handled=true;
            }
        } 
    }
}

namespace BitsOfStuff { }
