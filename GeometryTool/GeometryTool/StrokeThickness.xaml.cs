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
	public partial class StrokeThickness
	{
        public double ThickNess;  //线条的粗细

		public StrokeThickness()
		{
			this.InitializeComponent();		
		}

       /// <summary>
       /// 确定，并返回值
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// 动态改变Thickness的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectThick_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ThickNess=e.NewValue;
            e.Handled = true;
        }
	}
}