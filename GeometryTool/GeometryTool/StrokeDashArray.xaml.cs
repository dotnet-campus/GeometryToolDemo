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
using System.ComponentModel;
using System.Windows.Shapes;                   


namespace GeometryTool
{
	public partial class StrokeDashArray
	{
        public DoubleCollection strokeDashArray;  //线条的粗细
        public event PropertyChangedEventHandler PropertyChanged;
        //public DoubleCollection SDashArray
        //{

        //    get { return strokeDashArray; }
        //    set
        //    {
        //        strokeDashArray = value;
        //        if (this.PropertyChanged != null)
        //        {
        //            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SDashArray"));
        //        }
        //    }
        //}

        public StrokeDashArray()
		{
            strokeDashArray = new DoubleCollection();
            strokeDashArray.Add(1);
            strokeDashArray.Add(0);
			this.InitializeComponent();
           
            //Binding binding = new Binding() { Source = SDashArray };
            //BindingOperations.SetBinding(this.DashArray, Line.StrokeDashArrayProperty, binding);
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
        /// 动态改变虚线的长度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeDashArray[0] = e.NewValue;
            this.DashArray.StrokeDashArray = strokeDashArray;
            e.Handled = true;
        }

        /// <summary>
        /// 改变间隙的长度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeDashArray[1] = e.NewValue;
            this.DashArray.StrokeDashArray = strokeDashArray;
            e.Handled = true;
        }
	}
}