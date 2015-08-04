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

        private int rotationAngle;
        public int RotationAngle
        {
            get { return rotationAngle; }
            set
            {
                rotationAngle = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("RotationAngle"));
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

        private int sweepDirection;
        public int SweepDirection
        {
            get { return sweepDirection; }
            set
            {
                sweepDirection = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SweepDirection"));
                }
            }
        }

        public CurveAppearence()
        {
            this.RadiusX = 2;
            this.RadiusY = 1;
            this.RotationAngle = 0;
            this.isLargeArc = 0;
            this.SweepDirection = 0;
        }
    
    }

}
