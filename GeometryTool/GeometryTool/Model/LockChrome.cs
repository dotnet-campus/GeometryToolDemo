using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeometryTool
{
    public class LockChrome:Image
    {
        public bool isLock;
        public LockChrome()
        {
            this.MouseDown += Element_MouseLeftButtonDown;  //给这个Image添加点击事件，用于解开融合
            this.Source = new BitmapImage(new Uri("Image/lock.png", UriKind.Relative));
            isLock = true;
        }
        
        /// <summary>
        /// 绘制这个锁
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.Width = 1.2;
            this.Height = 1.2;

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;    //放在点的右下角
            this.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            TranslateTransform tt = new TranslateTransform();                       //做一个旋转的变换
            tt.X = 1.2;
            tt.Y = -0.3;
            this.RenderTransform = tt;

            Binding binding = new Binding("HasOtherPoint") { Converter=new ImageVisibilityConverter()};
            this.SetBinding(Image.VisibilityProperty, binding);                     //当没有重合点的时候，隐藏锁
     
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// 用于解开融合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LockChrome Chrome= sender as LockChrome;
            if (Chrome != null)
            {
                if (isLock == true) //融合变不融合
                {
                    BorderWithDrag _border = this.DataContext as BorderWithDrag;
                    BorderLock _borderLock = new BorderLock();
                    _borderLock.unLock(_border);
                }  
                e.Handled = true;
            }
            
        }
    }
}
