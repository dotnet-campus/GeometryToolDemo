using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryTool.Class
{
    public class Points:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int x;
        public int X
        {
            get{return x;}
            set 
            {
                x = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this,new PropertyChangedEventArgs("X"));
                }
            }
        }

        private int y;
        public int Y
        {
            get { return Y; }
            set
            {
                Y = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Y"));
                }
            }
        }
    }
}
