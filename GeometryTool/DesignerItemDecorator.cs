using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace GeometryTool
{
    public class DesignerItemDecorator : Control
    {
        private Adorner adorner;

       

        public DesignerItemDecorator()
        {
            BorderWithAdorner boerder = this.DataContext as BorderWithAdorner;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(boerder.Parent as Canvas);

            if (adornerLayer != null)
            {
                ContentControl designerItem = this.DataContext as ContentControl;
                Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
                this.adorner = new ResizeRotateAdorner(designerItem);
                adornerLayer.Add(this.adorner);
            }
        }

    }
}
