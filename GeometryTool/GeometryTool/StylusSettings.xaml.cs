using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Reflection;                    
using System.Windows.Ink;                   


namespace GeometryTool
{
	public partial class StylusSettings
	{
        public Brush currColor = Brushes.Black;  //当前选择的颜色

		public StylusSettings()
		{
			this.InitializeComponent();
            createGridOfColor();
            
		}

        /// <summary>
        /// 构造颜色
        /// </summary>
        private void createGridOfColor()
        {
            //获取所有的颜色
            PropertyInfo[] props = typeof(Brushes).GetProperties(BindingFlags.Public |
                                                  BindingFlags.Static);
            // 构造颜色
            foreach (PropertyInfo p in props)
            {
                Button b = new Button();
                b.Background = (SolidColorBrush)p.GetValue(null, null);
                b.Foreground = Brushes.Transparent;
                b.BorderBrush=Brushes.Transparent;
                b.Click += new RoutedEventHandler(b_Click); //给按钮注册事件
                this.ugColors.Children.Add(b);
            }
        }

        /// <summary>
        /// 点击颜色按钮，改变选择的颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush sb = (SolidColorBrush)(sender as Button).Background;
            currColor = sb ;
            this.SelectColor.Fill = sb;
        }

        /// <summary>
        /// 保存选择的数据，并返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
          
        }
	}
}