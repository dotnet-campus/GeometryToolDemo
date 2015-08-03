using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryTool
{
    /// <summary>
    /// 用于设置曲线的外观
    /// </summary>
    public class CurveAppearence
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int radiusX;
        public int RadiusX
        {
            get { return radiusX; }
            set 
            {
                radiusX = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("RadiusX"));
                }
            }
        }

        private int radiusY;
        public int RadiusY
        {
            get { return radiusY; }
            set
            {
                radiusY = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("RadiusY"));
                }
            }
        }

        private int degrees;
        public int Degrees
        {
            get { return degrees; }
            set
            {
                degrees = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Degrees"));
                }
            }
        }

        private int isLargeArc;
        public int IsLargeArc
        {
            get { return isLargeArc; }
            set
            {
                isLargeArc = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsLargeArc"));
                }
            }
        }

        private int isClockwise;
        public int IsClockwise
        {
            get { return isClockwise; }
            set
            {
                isClockwise = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsClockwise"));
                }
            }
        }

        public CurveAppearence()
        {
            this.RadiusX = 2;
            this.RadiusY = 1;
            this.degrees = 0;
            this.isLargeArc = 0;
            this.IsClockwise = 0;
        }
    
    }

}
